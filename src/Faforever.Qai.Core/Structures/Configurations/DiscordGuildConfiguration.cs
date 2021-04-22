using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using DSharpPlus.Entities;

namespace Faforever.Qai.Core.Structures.Configurations
{
	public class DiscordGuildConfiguration
	{
		[Key]
		public ulong GuildId { get; set; }
		public string Prefix { get; set; }

		public ulong? RoleWhenLinked { get; set; }

		// Ignore the value for UserBlacklist, we dont need it.
		public HashSet<ulong> UserBlacklist { get; set; }

		public ConcurrentDictionary<ulong, string> FafLinks { get; set; }
		public ConcurrentDictionary<string, string> Records { get; set; }

		/// <summary>
		/// Roles that are registered to be able to be subscribed to.
		/// </summary>
		private HashSet<ulong> RegisteredRoles { get; set; }

		public DiscordGuildConfiguration() : this(0, "") { } // used by EFcore or simillar processes or creating blank templates.

		public DiscordGuildConfiguration(ulong guildId, string prefix)
		{
			this.GuildId = guildId;
			this.Prefix = prefix;
			this.UserBlacklist =new();
			this.FafLinks = new();
			this.Records = new();
			this.RegisteredRoles = new();
		}
		#region Registered Roles
		public bool IsRoleSubscribable(DiscordRole role)
			=> IsRoleSubscribable(role.Id);

		public bool IsRoleSubscribable(ulong roleId)
		{
			lock(RegisteredRoles)
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
