using System;
using CryptoTrader.Backtesting;

namespace CryptoTrader.Hyperopts.Loss
{
    internal class OnlyProfitHyperoptLoss : IHyperoptLoss
    {
        public double GetLoss(BacktestResults results, DateTime startDate, DateTime endDate)
        {
            return results.ProfitPercentage;
        }
    }
}
