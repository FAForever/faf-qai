using System.Collections.Generic;
using Faforever.Qai.Core.Database.Entities;
using Faforever.Qai.Core.Extensions;

using Microsoft.EntityFrameworkCore;

namespace Faforever.Qai.Core.Database
{
	public class QAIDatabaseModel : DbContext
	{
		public DbSet<DiscordGuildConfiguration> DiscordConfigs => Set<DiscordGuildConfiguration>();
		public DbSet<RelayConfiguration> RelayConfigurations => Set<RelayConfiguration>();
		public DbSet<AccountLink> AccountLinks => Set<AccountLink>();

		public QAIDatabaseModel(DbContextOptions<QAIDatabaseModel> options) : base(options)
		{

		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			// Futher configuration goes here.
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<DiscordGuildConfiguration>().Property(b => b.FafLinks).Json();
			modelBuilder.Entity<DiscordGuildConfiguration>().Property(b => b.Records).Json();
			modelBuilder.Entity<DiscordGuildConfiguration>().Property(b => b.UserBlacklist).Json();
			modelBuilder.Entity<DiscordGuildConfiguration>().Property<HashSet<ulong>>("RegisteredRoles").Json();

			modelBuilder.Entity<RelayConfiguration>().Property(b => b.DiscordToIRCLinks).Json();
			modelBuilder.Entity<RelayConfiguration>().Property(b => b.Webhooks).Json();
		}
	}
}
