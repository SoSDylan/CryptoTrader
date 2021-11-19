using System;
using CryptoTrader.Backtesting;

namespace CryptoTrader.Hyperopts.Loss
{
    public interface IHyperoptLoss
    {
        // Bigger number for better results
        public double GetLoss(BacktestResults results, DateTime startDate, DateTime endDate);
    }
}
