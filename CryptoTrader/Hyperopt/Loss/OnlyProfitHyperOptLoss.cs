using CryptoTrader.Backtest;

namespace CryptoTrader.Hyperopt.Loss
{
    public class OnlyProfitHyperoptLoss : IHyperoptLoss
    {
        public double GetLoss(BacktestResults results)
        {
            return results.Profit;
        }
    }
}
