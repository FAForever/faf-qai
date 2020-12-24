using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Structures.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Newtonsoft.Json;

namespace Faforever.Qai.Core.Database
{
	public class QAIDatabaseFactory : IDesignTimeDbContextFactory<QAIDatabaseModel>
	{
		public QAIDatabaseModel CreateDbContext(string[] args)
		{
			DatabaseConfiguration dbConfig;
			using (FileStream fs = new(Path.Join("Config", "database_config.json"), FileMode.Open))
			{
				using StreamReader sr = new(fs);
				var json = sr.ReadToEnd();
				dbConfig = JsonConvert.DeserializeObject<DatabaseConfiguration>(json);
			}

			var optionsBuilder = new DbContextOptionsBuilder<QAIDatabaseModel>();
			optionsBuilder.UseSqlite(dbConfig.DataSource);

			return new QAIDatabaseModel(optionsBuilder.Options);
		}

		private class InvalidConfigException : Exception
		{
			public InvalidConfigException() : base() { }
			public InvalidConfigException(string? message) : base(message) { }
		}
	}
}
