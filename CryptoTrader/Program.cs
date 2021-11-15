using System;
using System.Threading.Tasks;
using CryptoTrader.Core;
using CryptoTrader.Hyperopts;
using CryptoTrader.Hyperopts.Loss;
using CryptoTrader.Strategy;
using Spectre.Console;

namespace CryptoTrader
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            PrintTitle();
            
            var candles = await DownloadManager.DownloadAndParseCandles("BTCUSDT",
                                                                        1440,
                                                                        DateTime.Now.AddDays(-100),
                                                                        DateTime.Now.AddDays(0));
            
            // var candlesList = new List<Candle>
            // {
            //     new Candle(10, 0, 7, 6),
            //     new Candle(11, 2, 9, 4),
            //     new Candle(11, 1, 6, 8),
            //     new Candle(12, 2, 8, 9),
            // };
            // var candles = new Candles(candlesList, 5, 100);
            
            var hyperopt = new Hyperopt(new MoonPhaseStrategy(), new OnlyProfitHyperoptLoss(), candles, 1000);
            
            hyperopt.Optimize();
        }

        private static void PrintTitle()
        {
            var cryptoText = new FigletText("Crypto")
                .LeftAligned()
                .Color(new Color(38, 176, 215));
            var traderText = new FigletText("    Trader")
                .LeftAligned()
                .Color(new Color(38, 176, 215));
            
            AnsiConsole.Write(cryptoText);
            AnsiConsole.Write(traderText);
        }
    }
}
