using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using DSharpPlus.Entities;

namespace Faforever.Qai.Core.Database.Entities
{
    public class DiscordGuildConfiguration(ulong guildId, string prefix)
    {
        [Key]
        public ulong GuildId { get; set; } = guildId;
        public string Prefix { get; set; } = prefix;

        public ulong? RoleWhenLinked { get; set; }

        // Ignore the value for UserBlacklist, we dont need it.
        public HashSet<ulong> UserBlacklist { get; set; } = new();

        public ConcurrentDictionary<ulong, string> FafLinks { get; set; } = new();
        public ConcurrentDictionary<string, string> Records { get; set; } = new();

        /// <summary>
        /// Roles that are registered to be able to be subscribed to.
        /// </summary>
        private HashSet<ulong> RegisteredRoles { get; set; } = new();

        public DiscordGuildConfiguration() : this(0, "") { } // used by EFcore or simillar processes or creating blank templates.
        #region Registered Roles
        public bool IsRoleSubscribable(DiscordRole role)
            => IsRoleSubscribable(role.Id);

        public bool IsRoleSubscribable(ulong roleId)
        {
            lock (RegisteredRoles)
            {
                return RegisteredRoles.Contains(roleId);
            }
        }

        public bool RegisterRole(DiscordRole role)
            => RegisterRole(role.Id);

        public bool RegisterRole(ulong roleId)
        {
            lock (RegisteredRoles)
            {
                return RegisteredRoles.Add(roleId);
            }
        }

        public bool UnregisterRole(DiscordRole role)
            => UnregisterRole(role.Id);

        public bool UnregisterRole(ulong roleId)
        {
            lock (RegisteredRoles)
            {
                return RegisteredRoles.Remove(roleId);
            }
        }
        #endregion
    }
}
