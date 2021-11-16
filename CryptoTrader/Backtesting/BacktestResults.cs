using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTrader.Utils;
using Spectre.Console;

namespace CryptoTrader.Backtesting
{
    public class BacktestResults
    {
        internal readonly List<Trade> Trades;
        
        internal readonly double ProfitPercentage;
        
        internal readonly int TotalTradesCount;
        internal readonly int SuccessfulTradesCount;
        internal readonly int FailedTradesCount;
        
        internal readonly int WinTradesCount;
        internal readonly int LossTradesCount;

        internal double WinLossRatio
        {
            get
            {
                if (LossTradesCount == 0)
                    return 1;
                
                double ratio = (double) WinTradesCount / (double) LossTradesCount;

                if (double.IsNaN(ratio))
                    return 1;
                
                return ratio;
            }
        }

        internal BacktestResults(List<Trade> trades)
        {
            Trades = trades;
            
            ProfitPercentage = trades.Sum(trade => trade.ProfitPercentage);
            
            TotalTradesCount = trades.Count;
            SuccessfulTradesCount = trades.Count(trade => trade.Successful);
            FailedTradesCount = trades.Count(trade => !trade.Successful);
            
            WinTradesCount = trades.Count(trade => trade.ProfitPercentage > 0);
            LossTradesCount = trades.Count(trade => trade.ProfitPercentage <= 0);
        }

        internal void PrintBasicResults()
        {
            Console.Write("Profit (%): ");
            Console.WriteLine(ProfitPercentage);
        }

        internal void PrintResults()
        {
            PrintTotals();
            Console.WriteLine();
            Console.WriteLine();
            PrintDetails();
        }

        private void PrintTotals()
        {
            // Create table
            var table = new Table().Centered();

            table.Title("[[ [yellow bold]Backtest Results[/] ]]");
            table.SimpleHeavyBorder();
            table.BorderColor(Color.Yellow);

            // Add columns
            table.AddColumn(new TableColumn("\n[red bold]Profit[/]").RightAligned());
            table.AddColumn(new TableColumn("\n[green]Total[/]").RightAligned());
            table.AddColumn(new TableColumn("[green bold]Trades[/]  \n[green]Successful[/]").RightAligned());
            table.AddColumn(new TableColumn("\n[green]Failed[/]").RightAligned());
            table.AddColumn(new TableColumn("\n[purple bold]Wins[/]").RightAligned());
            table.AddColumn(new TableColumn("\n[purple bold]Losses[/]").RightAligned());
            table.AddColumn(new TableColumn("\n[purple bold]W/L[/]").RightAligned());

            // Add row
            table.AddRow($"[blue]{ProfitPercentage:0.00} %[/]",
                         $"[blue]{TotalTradesCount}[/]",
                         $"[blue]{SuccessfulTradesCount}[/]",
                         $"[blue]{FailedTradesCount}[/]",
                         $"[blue]{WinTradesCount}[/]",
                         $"[blue]{LossTradesCount}[/]",
                         $"[blue]{WinLossRatio:0.00}[/]");

            // Render the table to the console
            AnsiConsole.Write(table);
        }

        private void PrintDetails()
        {
            // Create table
            var table = new Table().Centered();

            table.Title("[[ [yellow bold]Trade Details[/] ]]");
            table.SimpleHeavyBorder();
            table.BorderColor(Color.Yellow);

            // Add columns
            table.AddColumn(new TableColumn("[red bold]Trade[/]").RightAligned());
            table.AddColumn(new TableColumn("[green bold]Buy Price[/]").RightAligned());
            table.AddColumn(new TableColumn("[green bold]Sell Price[/]").RightAligned());
            table.AddColumn(new TableColumn("[purple bold]Profit[/]").RightAligned());

            foreach (var (trade, i) in Trades.WithIndex())
            {
                // Add row
                table.AddRow($"[white]#{i}[/]", $"[blue]{trade.BuyAtPrice}[/]", $"[blue]{trade.SellAtPrice}[/]", $"[blue]{trade.ProfitPercentage:0.00}[/]");
            }

            // Render the table to the console
            AnsiConsole.Write(table);
        }
    }
}
