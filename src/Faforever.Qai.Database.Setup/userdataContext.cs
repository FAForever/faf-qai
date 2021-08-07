using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Faforever.Qai.Database.Setup
{
    public partial class userdataContext : DbContext
    {
        public userdataContext()
        {
        }

        public userdataContext(DbContextOptions<userdataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountLink> AccountLinks { get; set; }
        public virtual DbSet<WatchedMap> WatchedMaps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=Database/userdata.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountLink>(entity =>
            {
                entity.ToTable("account_links");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("TIMESTAMP")
                    .HasColumnName("create_time")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DiscordId)
                    .IsRequired()
                    .HasColumnName("discord_id");

                entity.Property(e => e.FafId)
                    .HasColumnType("MEDIUMINT UNSIGNED")
                    .HasColumnName("faf_id");
            });

            modelBuilder.Entity<WatchedMap>(entity =>
            {
                entity.ToTable("watched_maps");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MapVersionId)
                    .HasColumnType("INTEGER UNSIGNED")
                    .HasColumnName("map_version_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
