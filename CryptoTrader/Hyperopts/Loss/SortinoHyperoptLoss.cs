using System;
using System.Linq;
using CryptoTrader.Backtesting;
using CryptoTrader.Utils;

namespace CryptoTrader.Hyperopts.Loss
{
    internal class SortinoHyperoptLoss : IHyperoptLoss
    {
        public double GetLoss(BacktestResults results, DateTime startDate, DateTime endDate)
        {
            var totalProfit = results.ProfitPercentage / 100;
            var daysPeriod = (endDate - startDate).TotalDays;
            
            // adding slippage of 0.1% per trade
            totalProfit = totalProfit - 0.0005;
            var expectedReturnsMean = totalProfit / daysPeriod;

            var downsideReturns = results.Trades.Where(x => x.Profit < 0);
            var downStandardDeviation = downsideReturns.Select(x => x.Profit).StandardDeviation();

            double sortinoRatio;
            if (downStandardDeviation != 0)
                sortinoRatio = expectedReturnsMean / downStandardDeviation * Math.Sqrt(365);
            else
                // Define low (negative) sharpe ratio to be clear that this is NOT optimal.
                sortinoRatio = double.MinValue;

            return sortinoRatio;
        }
    }
}
