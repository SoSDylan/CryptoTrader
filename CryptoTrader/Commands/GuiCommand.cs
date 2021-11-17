using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CryptoTrader.Core;
using CryptoTrader.Hyperopts;
using CryptoTrader.Hyperopts.Loss;
using CryptoTrader.Strategy;
using CryptoTrader.Utils;
using DarkIce.Toolkit.Core.Extensions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CryptoTrader.Commands
{
    internal class GuiCommand : AsyncCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            // var commandString = AnsiConsole.Prompt(
            //     new SelectionPrompt<string>()
            //         .Title("Please choose a [green]command[/]?")
            //         .PageSize(10)
            //         .MoreChoicesText("[grey](Move up and down to reveal more commands)[/]")
            //         .AddChoices("hyperopt"));

            var commandString = "hyperopt";
            
            var strategyString = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please choose a [green]strategy[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more strategies)[/]")
                    .AddChoices(ReflectionUtils.GetStrategies().Select(x => x.Name)));
            
            var hyperoptLossString = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please choose a [green]hyperopt loss[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more hyperopt losses)[/]")
                    .AddChoices(ReflectionUtils.GetHyperoptLosses().Select(x => x.Name)));
            
            var symbolString = AnsiConsole.Prompt(
                new TextPrompt<string>("[grey][[Optional]][/] [green]Symbol[/]:")
                    .DefaultValue("BTCUSDT").AllowEmpty());
            
            var epochs = AnsiConsole.Prompt(
                new TextPrompt<int>("[grey][[Optional]][/] [green]Epochs[/]:")
                    .DefaultValue(100).AllowEmpty());
            
            var interval = AnsiConsole.Prompt(
                new TextPrompt<int>("[grey][[Optional]][/] [green]Interval in minutes[/]:")
                    .DefaultValue(5).AllowEmpty());
            
            var days = AnsiConsole.Prompt(
                new TextPrompt<int>("[grey][[Optional]][/] [green]Backtest days[/]:")
                    .DefaultValue(10).AllowEmpty());

            await CryptoTrader.App.RunAsync(new[] {
                commandString,
                strategyString,
                hyperoptLossString,
                "-s", symbolString,
                "-e", epochs.ToString(),
                "-i", interval.ToString(),
                "-d", days.ToString(),
            });
            
            return 0;
        }
    }
}
