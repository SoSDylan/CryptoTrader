using System.Linq;
using CryptoTrader.Core;
using CryptoTrader.Strategy;

namespace CryptoTrader.Backtest
{
    public class Backtest
    {
        private readonly BaseStrategy _strategy;
        private readonly Candles _candles;

        public Backtest(BaseStrategy strategy, Candles candles)
        {
            _strategy = strategy.DeepCopy();
            _candles = candles;
        }

        public BacktestResults RunBacktest()
        {
            for (int i = 1; i <= _candles.List.Count; i++)
            {
                var candles = new Candles(_candles.List.Take(i).ToList(), _candles.Interval, _candles.MaxCandles);
                _strategy.Tick(candles);
            }
            
            // TODO: need to run actual backtest
            return new BacktestResults(_strategy.Profit);
        }
    }
}
