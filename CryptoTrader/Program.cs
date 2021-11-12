using System.Collections.Generic;
using CryptoTrader.Core;
using CryptoTrader.Hyperopt.Loss;
using CryptoTrader.Strategy;

namespace CryptoTrader
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var candlesList = new List<Candle>
            {
                new Candle(10, 0, 7, 6),
                new Candle(11, 2, 9, 4),
                new Candle(11, 1, 6, 8),
                new Candle(12, 2, 8, 9),
            };
            var candles = new Candles(candlesList, 5, 100);
            
            var hyperopt = new Hyperopt.Hyperopt(new TestStrategy(), candles, new OnlyProfitHyperoptLoss(), 10);
            
            hyperopt.Optimize();
        }
    }
}
