using System.Threading.Tasks;
using CryptoTrader.Commands;
using Spectre.Console.Cli;

namespace CryptoTrader
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var app = new CommandApp();
            
            app.Configure(config =>
            {
                config.AddCommand<HyperoptCommand>("hyperopt");
            });
            
            return await app.RunAsync(args);
        }
    }
}
