using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Spectre.Console;

namespace CryptoTrader.Core
{
    public static class DownloadManager
    {
        public static async Task<Candles> DownloadAndParseCandles(string symbol, int interval, DateTime startTime, DateTime endTime)
        {
            Candles result = null!;
            
            await AnsiConsole.Progress()
                .Columns(
                    new TaskDescriptionColumn(),    // Task description
                    new SpinnerColumn()             // Spinner
                )
                .StartAsync(async ctx =>
                {
                    // Define tasks
                    var task = ctx.AddTask("[green]Downloading candles[/]");
                    task.MaxValue = 1;

                    result = await InternalDownloadAndParseCandles(symbol, interval, startTime, endTime);
                    
                    task.Increment(100);
                });

            return result;
        }
        
        private static async Task<Candles> InternalDownloadAndParseCandles(string symbol, int interval, DateTime startTime, DateTime endTime)
        {
            string intervalString = interval switch
            {
                < 60 => $"{interval:D}m",
                < 1440 => $"{interval/60:D}h",
                _ => $"{interval/1440:D}d"
            };
            
            var candleList = new List<Candle>();
            
            var uri = new Uri($"https://api.binance.com/api/v1/klines?symbol={symbol}&interval={intervalString}&limit=1000");

            while (candleList.Count == 0 || candleList.Last().OpenTime > startTime)
            {
                using var client = new HttpClient();
                
                var binanceEndTime = candleList.LastOrDefault()?.OpenTime ?? endTime;
                var binanceEndTimeUnix = (long) (binanceEndTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
                var response = await client.GetAsync(uri + $"&endTime={binanceEndTimeUnix}");

                var json = await response.Content.ReadFromJsonAsync<List<List<dynamic>>>();
                json.Reverse();
                
                foreach (var jsonCandle in json)
                {
                    long   openTime                 = long.Parse(jsonCandle[0].ToString()); // Open time
                    double open                     = double.Parse(jsonCandle[1].ToString()); // Open
                    double high                     = double.Parse(jsonCandle[2].ToString()); // High
                    double low                      = double.Parse(jsonCandle[3].ToString()); // Low
                    double close                    = double.Parse(jsonCandle[4].ToString()); // Close
                    double volume                   = double.Parse(jsonCandle[5].ToString()); // Volume
                    long   closeTime                = long.Parse(jsonCandle[6].ToString()); // Close time
                    double quoteAssetVolume         = double.Parse(jsonCandle[7].ToString()); // Quote asset volume
                    int    numberOfTrades           = int.Parse(jsonCandle[8].ToString()); // Number of trades
                    double takerBuyBaseAssetVolume  =  double.Parse(jsonCandle[9].ToString()); // Taker buy base asset volume
                    double takerBuyQuoteAssetVolume = double.Parse(jsonCandle[10].ToString()); // Taker buy quote asset volume

                    var candle = new Candle(openTime,
                                            open,
                                            high,
                                            low,
                                            close,
                                            volume,
                                            closeTime,
                                            quoteAssetVolume,
                                            numberOfTrades,
                                            takerBuyBaseAssetVolume,
                                            takerBuyQuoteAssetVolume);
                    
                    // When joining different requests, make sure the last candles are not duplicated
                    if (candleList.LastOrDefault()?.OpenTime == candle.OpenTime)
                        continue;

                    candleList.Add(candle);
                    
                    // Don't add more than the requested amount of candles
                    if (candle.OpenTime < startTime)
                        break;
                }
            }

            candleList.Reverse();

            return new Candles(candleList, interval, candleList.Count);
        }
    }
}
