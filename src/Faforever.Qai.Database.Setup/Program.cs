using Faforever.Qai.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Faforever.Qai.Database.Setup
{
    class Program
    {
        static void Main(string[] args)
        {
			StartAsync().GetAwaiter().GetResult();
        }

		public static async Task StartAsync()
		{
			ServiceCollection services = new ServiceCollection();
			services.AddDbContext<QAIDatabaseModel>(options =>
			{
				options.UseSqlite("Data Source=Database/qai-dostya.db");
			}, ServiceLifetime.Singleton, ServiceLifetime.Singleton)
				.AddDbContext<userdataContext>(options =>
				{
					options.UseSqlite("Data Source=Database/userdata.db");
				}, ServiceLifetime.Singleton, ServiceLifetime.Singleton);

			var proivder = services.BuildServiceProvider();
			var scope = proivder.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<QAIDatabaseModel>();
			var data = scope.ServiceProvider.GetRequiredService<userdataContext>();

			ApplyDatabaseMigrations(db);

			Console.WriteLine("Starting.");

			await data.AccountLinks.ForEachAsync(async (x) =>
			{
				var did = Convert.ToUInt64(x.DiscordId);
				var link = await db.FindAsync<Core.Structures.Link.AccountLink>(did);
				if(link is null)
				{
					try
					{
						await db.AddAsync(new Core.Structures.Link.AccountLink()
						{
							DiscordId = did,
							FafId = Convert.ToInt32(x.FafId)
						});
					}
					catch { }
				}
			});

			await db.SaveChangesAsync();

			Console.WriteLine("Done.");
		}

		private static void ApplyDatabaseMigrations(DbContext database)
		{
			if (!(database.Database.GetPendingMigrations()).Any())
			{
				return;
			}

			database.Database.Migrate();
			database.SaveChanges();
		}
	}
}
