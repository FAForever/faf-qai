using System;
using McMaster.Extensions.CommandLineUtils;

namespace Faforever.Qai
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication();

            app.HelpOption("-h|--help");
            
            app.OnExecuteAsync(async cancellationToken => {
                Console.WriteLine("Hello World!");
            });

            return app.Execute(args);
        }
    }
}