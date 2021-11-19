using System;
using CryptoTrader.Backtesting;

namespace CryptoTrader.Hyperopts.Loss
{
    internal class SharpeHyperoptLoss : IHyperoptLoss
    {
        // Taken from 
        public double GetLoss(BacktestResults results, DateTime startDate, DateTime endDate)
        {
            var totalProfit = results.ProfitPercentage / 100;
            var daysPeriod = (endDate - startDate).TotalDays;

            // adding slippage of 0.1% per trade
            totalProfit = totalProfit - 0.0005;
            var expectedReturnsMean = totalProfit / daysPeriod;
            var profitStandardDeviation = results.ProfitStandardDeviation;

            double sharpRatio;
            if (profitStandardDeviation != 0)
                sharpRatio = expectedReturnsMean / profitStandardDeviation * Math.Sqrt(365);
            else
                // Define low (negative) sharpe ratio to be clear that this is NOT optimal.
                sharpRatio = double.MinValue;

            return sharpRatio;
        }
    }
}
