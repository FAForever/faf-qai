using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Faforever.Qai.Discord;

using NUnit.Framework;

namespace Faforever.Qai.Tests.Discord
{
    public class CommandTests
    {
		protected DiscordBot Bot { get; set; }
		protected DiscordShardedClient Client { get; set; }
		protected int TestingShardId { get; set; } = 1;
		protected DiscordGuild TestingGuild { get; set; }
		protected DiscordChannel TestingChannel { get; set; }
		protected List<DiscordMember> Actors { get; set; }
		protected CommandsNextExtension CNext { get; set; }
		protected IReadOnlyDictionary<string, Command> Commands { get; set; }

		[OneTimeSetUp]
		public async Task SetUp()
		{

		}

		[OneTimeTearDown]
		public async Task TearDown()
		{

		}
    }
}
