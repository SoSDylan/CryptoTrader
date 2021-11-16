using System;
using System.Threading.Tasks;
using CryptoTrader.Core;
using CryptoTrader.Hyperopts;
using CryptoTrader.Hyperopts.Loss;
using CryptoTrader.Strategy;
using Spectre.Console;

namespace CryptoTrader
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            PrintTitle();
            
            var candles = await DownloadManager.DownloadAndParseCandles("BTCUSDT",
                                                                        720,
                                                                        DateTime.Now.AddDays(-1200),
                                                                        DateTime.Now.AddDays(0));
            
            // var candlesList = new List<Candle>
            // {
            //     new Candle(10, 0, 7, 6),
            //     new Candle(11, 2, 9, 4),
            //     new Candle(11, 1, 6, 8),
            //     new Candle(12, 2, 8, 9),
            // };
            // var candles = new Candles(candlesList, 5, 100);
            
            var hyperopt = new Hyperopt(new MoonPhaseStrategy(), new OnlyProfitHyperoptLoss(), candles, 100);
            
            hyperopt.Optimize();
        }

        private static void PrintTitle()
        {
            var cryptoText = new FigletText("Crypto  ")
                .Centered()
                .Color(Color.LightSlateBlue);
            var traderText = new FigletText("  Trader")
                .Centered()
                .Color(Color.LightSlateBlue);
            
            AnsiConsole.Write(cryptoText);
            AnsiConsole.Write(traderText);
        }
    }
}
