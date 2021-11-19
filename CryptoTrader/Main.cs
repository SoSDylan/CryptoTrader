using System;
using System.IO;
using System.Threading.Tasks;
using CryptoTrader.Commands;
using DarkIce.Toolkit.Core.Extensions;
using Spectre.Console.Cli;
using YamlDotNet.Serialization.NamingConventions;

namespace CryptoTrader
{
    internal static class CryptoTrader
    {
        public static AppConfig Config { get; private set; }
        internal static ICommandApp App;
        
        public static async Task<int> Main(string[] args)
        {
            LoadConfig();

            return await StartApp(args);
        }

        private static async Task<int> StartApp(string[] args)
        {
            App = new CommandApp();
            
            App.Configure(config =>
            {
                config.AddCommand<GuiCommand>("gui");
                
                config.AddCommand<HyperoptCommand>("hyperopt");
            });
            
            return await App.RunAsync(args);
        }

        private static void LoadConfig()
        {
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            Config = deserializer.Deserialize<AppConfig>(File.ReadAllText("config.yaml"));

            if (Config.IsNullOrDefault())
            {
                throw new Exception("Config could not be loaded.");
            }
        }
        
        public sealed class AppConfig
        { 
            public ExchangeConfig Exchange { get; private set; }

            public class ExchangeConfig
            {
                public double Fee { get; private set; }
                public double FeeMultiplier => (100 - Fee) / 100;
            }
        }
    }
}
