using Faforever.Qai.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Faforever.Qai.Startup
{
    public static partial class StartupExtensions
    {
        public static BotConfig InitializeBotConfig(this WebApplicationBuilder builder)
        {
            var botConfig = new BotConfig();
            builder.Configuration.GetSection("Config").Bind(botConfig);

            SetLegacyEnvironmentVariables(botConfig);

            if (botConfig.Discord.Roles == null)
                botConfig.Discord.Roles = new BotConfig.RolesSettings { FafStaff = new ulong[] { 133735496479145984 } };

            var validator = new BotConfigValidator();
            var validationResult = validator.Validate(botConfig);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                var errorMessage = string.Join("\n", errors);
                throw new InvalidOperationException($"Invalid configuration:\n{errorMessage}");
            }

            builder.Services.AddSingleton(botConfig);

            return botConfig;
        }

        private static void SetLegacyEnvironmentVariables(BotConfig botConfig)
        {
            var environmentVariableMap = new Dictionary<string, Action<string>> {
                ["FAF_CLIENT_SECRET"] = value => botConfig.Faf.ClientSecret = value,
                ["FAFAPI_CLIENT_ID"] = value => botConfig.FafApi.ClientId = value,
                ["FAFAPI_CLIENT_SECRET"] = value => botConfig.FafApi.ClientSecret = value,
                ["DISCORD_CLIENT_SECRET"] = value => botConfig.Discord.ClientSecret = value,
                ["DISCORD_TOKEN"] = value => botConfig.Discord.Token = value,
                ["IRC_SERVER"] = value => botConfig.Irc.Connection = value,
                ["IRC_CHANNELS"] = value => botConfig.Irc.Channels = value,
                ["IRC_USER"] = value => botConfig.Irc.User = value,
                ["IRC_PASS"] = value => botConfig.Irc.Password = value,
                ["TWITCH_CLIENT_SECRET"] = value => botConfig.Twitch.ClientSecret = value
            };

            foreach (var item in environmentVariableMap)
            {
                var value = Environment.GetEnvironmentVariable(item.Key);
                if (value != null)
                    item.Value(value);
            }
        }
    }
}
