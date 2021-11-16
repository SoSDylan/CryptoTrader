using CryptoTrader.Backtesting;

namespace CryptoTrader.Hyperopts.Loss
{
    internal class OnlyProfitHyperoptLoss : IHyperoptLoss
    {
        public double GetLoss(BacktestResults results)
        {
            return results.ProfitPercentage;
        }
    }
}
