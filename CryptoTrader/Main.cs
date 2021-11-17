using System.Threading.Tasks;
using CryptoTrader.Commands;
using Spectre.Console.Cli;

namespace CryptoTrader
{
    internal static class CryptoTrader
    {
        internal static ICommandApp App;
        
        public static async Task<int> Main(string[] args)
        {
            App = new CommandApp();
            
            App.Configure(config =>
            {
                config.AddCommand<GuiCommand>("gui");
                
                config.AddCommand<HyperoptCommand>("hyperopt");
            });
            
            return await App.RunAsync(args);
        }
    }
}
