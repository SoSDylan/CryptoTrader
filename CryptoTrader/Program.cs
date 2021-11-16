using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CryptoTrader.Commands;
using CryptoTrader.Core;
using CryptoTrader.Hyperopts;
using CryptoTrader.Hyperopts.Loss;
using CryptoTrader.Strategy;
using Spectre.Console;
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
