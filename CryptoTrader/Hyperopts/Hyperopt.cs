using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTrader.Backtesting;
using CryptoTrader.Core;
using CryptoTrader.Hyperopts.Loss;
using CryptoTrader.Strategy;
using CryptoTrader.Utils;
using Spectre.Console;

namespace CryptoTrader.Hyperopts
{
    internal class Hyperopt
    {
        private readonly BaseStrategy _strategy;
        private readonly Candles _candles;
        private readonly IHyperoptLoss _hyperoptLoss;
        private readonly HyperoptContext _hyperoptContext;
        private readonly int _epochs;
        private readonly int? _buyTimeout;
        private readonly int? _sellTimeout;
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        // The higher the number, the better the algorithm
        private double _bestLossResult = double.MinValue;
        private Dictionary<string, dynamic> _bestOptimizableValues = new();
        private BacktestResults _bestBacktestResult;

        internal Hyperopt(BaseStrategy strategy, IHyperoptLoss hyperoptLoss, Candles candles, int epochs,
            int? buyTimeout = null, int? sellTimeout = null)
        {
            _strategy = strategy;
            _candles = candles;
            _hyperoptLoss = hyperoptLoss;
            _epochs = epochs;
            _buyTimeout = buyTimeout;
            _sellTimeout = sellTimeout;

            _startDate = candles.List.First().OpenTime;
            _endDate = candles.List.Last().CloseTime;

            _hyperoptContext = _strategy.HyperoptContext();
        }

        internal void Optimize()
        {
            var table = new Table().Centered();

            table.Title("Hyperopts".AsTableTitle());
            table.SimpleHeavyBorder();
            table.BorderColor(Color.Yellow);

            // Add columns
            table.AddColumn(new TableColumn("\n[yellow bold]Backtest[/]").Footer("[yellow bold]Backtest[/]").RightAligned());
            table.AddColumn(new TableColumn("\n[red bold]Profit[/]").Footer("[red bold]Profit[/]").RightAligned());
            table.AddColumn(new TableColumn("\n[green]Total[/]").Footer("[green]Total[/]").RightAligned());
            table.AddColumn(new TableColumn("[green bold]Trades[/]  \n[green]Successful[/]").Footer("[green]Successful[/]\n[green bold]Trades[/]  ").RightAligned());
            table.AddColumn(new TableColumn("\n[green]Failed[/]").Footer("[green]Failed[/]").RightAligned());
            table.AddColumn(new TableColumn("\n[purple bold]Wins[/]").Footer("[purple bold]Wins[/]").RightAligned());
            table.AddColumn(new TableColumn("\n[purple bold]Losses[/]").Footer("[purple bold]Losses[/]").RightAligned());
            table.AddColumn(new TableColumn("\n[purple bold]W/L[/]").Footer("[purple bold]W/L[/]").RightAligned());
            
            AnsiConsole.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Crop)
                .Cropping(VerticalOverflowCropping.Top)
                .Start(ctx =>
                {
                    for (var epoch = 1; epoch <= _epochs; epoch++)
                    {
                        // Run backtesting
                        var backtestResult = new Backtest(epoch, _strategy, _candles, _buyTimeout, _sellTimeout).RunBacktest();

                        // Add a row to the table
                        table.AddRow($"[grey]#{epoch}[/]",
                                     $"[blue]{backtestResult.ProfitPercentage:0.00} %[/]",
                                     $"[blue]{backtestResult.TotalTradesCount}[/]",
                                     $"[blue]{backtestResult.SuccessfulTradesCount}[/]",
                                     $"[blue]{backtestResult.FailedTradesCount}[/]",
                                     $"[blue]{backtestResult.WinTradesCount}[/]",
                                     $"[blue]{backtestResult.LossTradesCount}[/]",
                                     $"[blue]{backtestResult.WinLossRatio:0.00}[/]");
                        ctx.Refresh();
                        
                        // Get loss from backtest results
                        var lossResult = _hyperoptLoss.GetLoss(backtestResult, _startDate, _endDate);
                        
                        // check if this is the best loss result
                        if (lossResult > _bestLossResult)
                        {
                            _bestLossResult = lossResult;
                            _bestBacktestResult = backtestResult;
                            _bestOptimizableValues = new Dictionary<string, dynamic>();
                            foreach (var optimizable in _hyperoptContext.Optimizables)
                            {
                                _bestOptimizableValues.Add(optimizable.GetName(), optimizable.GetValue());
                            }
                        }
                        
                        // Optimize values
                        OptimizeValues(epoch, _epochs);
                    }
                });

            PrintHyperoptResults();
        }

        private void PrintHyperoptResults()
        {
            Console.WriteLine();
            Console.WriteLine();
            AnsiConsole.Write(new FigletText("Best Hyperopt")
                .Centered()
                .Color(Color.LightSlateBlue));
            
            
            Console.WriteLine();
            _bestBacktestResult.PrintResults();
            Console.WriteLine();
            Console.WriteLine();
            
            // Create table
            var table = new Table().Centered();

            table.Title("Best Values".AsTableTitle());
            table.SimpleHeavyBorder();
            table.BorderColor(Color.Yellow);

            // Add columns
            table.AddColumn(new TableColumn("[red bold]Variable Name[/]").LeftAligned());
            table.AddColumn(new TableColumn("[green bold]Value[/]").RightAligned());

            foreach (var (name, value) in _bestOptimizableValues)
            {
                // Add row
                table.AddRow($"[blue bold]{name}[/]", $"[blue]{value.ToString() as string}[/]");
            }

            // Render the table to the console
            AnsiConsole.Write(table);
        }

        private void RandomizeValues()
        {
            foreach (var optimizable in _hyperoptContext.Optimizables)
            {
                optimizable.RandomizeValue();
            }
        }

        private void OptimizeValues(int epoch, int maxEpochs)
        {
            for (var i = 0; i < _hyperoptContext.Optimizables.Count; i++)
            {
                var lastOptimization = _hyperoptContext.Optimizables[i];
                var bestOptimization = _bestOptimizableValues.ElementAt(i).Value;
                
                lastOptimization.OptimizeValue(epoch,
                    maxEpochs,
                    lastOptimization.GetValue(),
                    bestOptimization);
            }
        }
    }
}
