using System;
using Faforever.Qai.Irc;
using IrcDotNet;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai {
	public static class Program {
		public static int Main(string[] args) {
			CommandLineApplication app = new CommandLineApplication();

			app.HelpOption("-h|--help");

			app.OnExecuteAsync(async cancellationToken => {
				ServiceCollection services = new ServiceCollection();
				services.AddLogging(options => options.AddConsole());

				await using var serviceProvider = services.BuildServiceProvider();

				using QaIrc ircBot = new QaIrc("irc.faforever.com", new IrcUserRegistrationInfo {
					NickName = "Balleby", RealName = "Balleby", Password = "balleby", UserName = "balleby"
				}, serviceProvider.GetService<ILogger<QaIrc>>());
				ircBot.Run();

				//TODO Make sure the Discord bot is started aswell.
				Console.ReadLine();
			});

			return app.Execute(args);
		}
	}
}