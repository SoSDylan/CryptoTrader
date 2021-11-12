using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTrader.Core;
using CryptoTrader.Hyperopt.Loss;
using CryptoTrader.Strategy;

namespace CryptoTrader
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var candles = await DownloadManager.DownloadAndParseCandles("BTCUSDT",
                                                                        5,
                                                                        DateTime.Now.AddDays(-10),
                                                                        DateTime.Now.AddDays(0));
            
            // var candlesList = new List<Candle>
            // {
            //     new Candle(10, 0, 7, 6),
            //     new Candle(11, 2, 9, 4),
            //     new Candle(11, 1, 6, 8),
            //     new Candle(12, 2, 8, 9),
            // };
            // var candles = new Candles(candlesList, 5, 100);
            
            var hyperopt = new Hyperopt.Hyperopt(new TestStrategy(), candles, new OnlyProfitHyperoptLoss(), 100);
            
            hyperopt.Optimize();
        }
    }
}
