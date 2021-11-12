using CryptoTrader.Backtest;

namespace CryptoTrader.Hyperopt.Loss
{
    public interface IHyperoptLoss
    {
        // Bigger number for better results
        public double GetLoss(BacktestResults results);
    }
}
