using CryptoTrader.Backtesting;

namespace CryptoTrader.Hyperopts.Loss
{
    public class OnlyProfitHyperoptLoss : IHyperoptLoss
    {
        public double GetLoss(BacktestResults results)
        {
            return results.ProfitPercentage;
        }
    }
}
