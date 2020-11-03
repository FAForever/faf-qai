using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

using Faforever.Qai.Core.Structures;
using Faforever.Qai.Core.Structures.Configurations;
using Faforever.Qai.Core.Structures.Webhooks;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Faforever.Qai.Core.Database
{
	public class QAIDatabaseModel : DbContext
	{
		public DbSet<DiscordGuildConfiguration> DiscordConfigs { get; set; }
		public DbSet<RelayConfiguration> RelayConfigurations { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			// TODO: Setup SQLite DB and connection string.
			
			options.UseSqlite("Data Source=test.db")
				.EnableDetailedErrors();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<DiscordGuildConfiguration>()
				.Property(b => b.FafLinks)
				.HasConversion(
				v => JsonConvert.SerializeObject(v),
				v => JsonConvert.DeserializeObject<ConcurrentDictionary<ulong, string>>(v) ?? new ConcurrentDictionary<ulong, string>());

			modelBuilder.Entity<DiscordGuildConfiguration>()
				.Property(b => b.Records)
				.HasConversion(
				v => JsonConvert.SerializeObject(v),
				v => JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(v) ?? new ConcurrentDictionary<string, string>());

			modelBuilder.Entity<DiscordGuildConfiguration>()
				.Property(b => b.UserBlacklist)
				.HasConversion(
				v => JsonConvert.SerializeObject(v),
				v => JsonConvert.DeserializeObject<HashSet<ulong>>(v) ?? new HashSet<ulong>());

			modelBuilder.Entity<RelayConfiguration>()
				.Property(b => b.DiscordToIRCLinks)
				.HasConversion(
				v => JsonConvert.SerializeObject(v),
				v => JsonConvert.DeserializeObject<ConcurrentDictionary<ulong, string>>(v) ?? new ConcurrentDictionary<ulong, string>());

			modelBuilder.Entity<RelayConfiguration>()
				.Property(b => b.Webhooks)
				.HasConversion(
				v => JsonConvert.SerializeObject(v),
				v => JsonConvert.DeserializeObject<ConcurrentDictionary<string, DiscordWebhook>>(v) ?? new ConcurrentDictionary<string, DiscordWebhook>());
		}
	}
}
