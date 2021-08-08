using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;

using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Database.Entities;
using Faforever.Qai.Core.Structures.Webhooks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Core.Services
{
    // Test Webhook URL
    // https://discord.com/api/webhooks/773214715328725012/Kj0P_CMUooloZTyfIOP37QD1GLuXp_LVRS3ax2FqxT5mP2dOvLoP_xMZiJ8L6Sf0jYgq

    public sealed class RelayService
    {
        public delegate Task RelayDiscordMessage(string ircChannel, string author, string message);
        public event RelayDiscordMessage? DiscordMessageReceived;

        private readonly ILogger _logger;
        private readonly DiscordRestClient _rest;
        private readonly QAIDatabaseModel _db;
        private bool initialized;

        private ConcurrentDictionary<string, HashSet<(ulong, string)>> IRCToWebhookRelations { get; set; }
        private ConcurrentDictionary<ulong, string> DiscordToIRCWebhookRelations { get; set; }

        public RelayService(QAIDatabaseModel db, ILogger<RelayService> logger, DiscordRestClient rest)
        {
            this._db = db;
            this.initialized = false;
            this._logger = logger;
            this._rest = rest;

            IRCToWebhookRelations = new();
            DiscordToIRCWebhookRelations = new();
        }

        public async Task InitializeAsync()
        {
            if (initialized)
                return;

            var relays = _db.RelayConfigurations.AsNoTracking().ToList();

            foreach (var r in relays)
            {
                foreach (var hook in r.Webhooks)
                {
                    AddToWebhookDict(hook.Key, hook.Value.Id, hook.Value.Token);
                }

                foreach (var links in r.DiscordToIRCLinks)
                {
                    var live = await _rest.GetWebhookAsync(links.Key);
                    DiscordToIRCWebhookRelations[live.ChannelId] = links.Value;
                }
            }

            this.initialized = true;
        }

        private void AddToWebhookDict(string key, ulong id, string token)
        {
            if (IRCToWebhookRelations.ContainsKey(key))
                IRCToWebhookRelations[key].Add((id, token));
            else
                IRCToWebhookRelations[key] = new HashSet<(ulong, string)>() { (id, token) };
        }

        public async Task<bool> AddRelayAsync(ulong discordGuild, DiscordWebhook hook, string ircChannel)
        {
            try
            {
                AddToWebhookDict(ircChannel, hook.Id, hook.Token);

                DiscordToIRCWebhookRelations[hook.ChannelId] = ircChannel;

                // This method should not be passed values that dont have a configuration value created for them.
                var cfg = await _db.FindAsync<RelayConfiguration>(discordGuild);
                if (cfg is null)
                    throw new Exception("Failed to get valid relay configuration.");

                _db.Update(cfg);

                var hookData = new DiscordWebhookData()
                {
                    Id = hook.Id,
                    Token = hook.Token
                };

                cfg.Webhooks[ircChannel] = hookData;
                cfg.DiscordToIRCLinks[hook.Id] = ircChannel;

                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add new Relay.");
                return false;
            }
        }

        public async Task<bool> RemoveRelayAsync(ulong discordGuild, ulong webhookId)
        {
            try
            {
                // This method should not be passed values that dont have a configuration vlaue created for them.
                var cfg = await _db.FindAsync<RelayConfiguration>(discordGuild);

                if (cfg is null)
                    throw new Exception("Failed to get valid relay configuration.");

                DiscordWebhookData? hook = null;
                if (cfg.DiscordToIRCLinks.TryRemove(webhookId, out string? ircChannel))
                {
                    // At least one thing changed, so tell the database to save changes.
                    _db.Update(cfg);

                    if (cfg.Webhooks.TryRemove(ircChannel, out hook))
                    {
                        IRCToWebhookRelations[ircChannel]?.Remove((hook.Id, hook.Token));

                        var realHook = await _rest.GetWebhookWithTokenAsync(hook.Id, hook.Token);

                        _ = DiscordToIRCWebhookRelations.TryRemove(realHook.ChannelId, out _);

                        await realHook.DeleteAsync();
                    }
                }

                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove Relay.");
                return false;
            }
        }

        public async Task Discord_MessageReceived(ulong discordChannel, string author, string message)
        {
            if (!DiscordToIRCWebhookRelations.TryGetValue(discordChannel, out var ircChannel))
                return;

            try
            {
                // send to IRC
                if (DiscordMessageReceived != null)
                    await DiscordMessageReceived(ircChannel, author, message);

                // bounce to other discord channels listening
                await IRC_MessageReceived(ircChannel, author, message, discordChannel);
            }
            catch
            {
                _logger.LogWarning("No IRC receivers registered.");
            }
        }

        public async Task IRC_MessageReceived(string ircChannel, string author, string message, ulong channelToIgnore = 0)
        {
            if (!IRCToWebhookRelations.TryGetValue(ircChannel, out var hooks))
                return;

            // /me action from IRC is encoded \0001ACTION the action\0001
            if(message.StartsWith("\u0001ACTION"))
                message = $"_{message[8..].TrimEnd('\u0001')}_";

            foreach (var h in hooks)
            {
                var msg = new DiscordWebhookBuilder()
                        .WithUsername(author)
                        .WithAvatarUrl("https://www.faforever.com/images/faf-logo.png")
                        .WithContent(message);

                if (channelToIgnore != 0)
                {
                    var hook = await _rest.GetWebhookWithTokenAsync(h.Item1, h.Item2);

                    if (hook.ChannelId == channelToIgnore)
                        continue;

                    await hook.ExecuteAsync(msg);
                }
                else
                {
                    await _rest.ExecuteWebhookAsync(h.Item1, h.Item2, msg);
                }
            }
        }
    }
}
