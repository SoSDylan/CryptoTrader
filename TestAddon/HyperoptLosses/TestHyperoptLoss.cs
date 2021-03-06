using System;
using CryptoTrader.Backtesting;
using CryptoTrader.Hyperopts.Loss;

namespace TestAddon.HyperoptLosses
{
    public class TestHyperoptLoss : IHyperoptLoss
    {
        public double GetLoss(BacktestResults results, DateTime startDate, DateTime endDate)
        {
            return results.ProfitPercentage;
        }
    }
}
