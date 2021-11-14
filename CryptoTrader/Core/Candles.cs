using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoTrader.Core
{
    public class Candles
    {
        public readonly int Interval;
        public readonly int MaxCandles;
        
        private List<Candle> _candles = new();
        /// <summary>
        /// Start of list is oldest candle
        /// End of list is newest candle
        /// </summary>
        public IList<Candle> List => _candles.AsReadOnly();
        
        public Candles(int interval, int maxCandles)
        {
            Interval = interval;
            MaxCandles = maxCandles;
        }
        
        public Candles(List<Candle> candles, int interval, int maxCandles)
        {
            _candles = candles;
            Interval = interval;
            MaxCandles = maxCandles;
            
            RemoveExcessCandles();
        }
        
        public void AddCandle(Candle candle)    
        {
            _candles.Add(candle);
            
            RemoveExcessCandles();
        }

        public void RemoveExcessCandles()
        {
            if (_candles.Count > MaxCandles)
            {
                _candles.RemoveRange(0, _candles.Count - MaxCandles);
            }
        }
        
        public Candle? this[int offset]
        {
            get
            {
                if (_candles.Count <= offset)
                {
                    return null;
                }
            
                return List[^(offset + 1)];
            }
        }

        public Candle GetCurrentCandle()
        {
            return List.Last();
        }
    }
    
    public class Candle
    {
        public DateTime OpenTime { get; }
        public double High { get; }
        public double Low { get; }
        public double Open { get; }
        public double Close { get; }
        public double Volume { get; }
        public DateTime CloseTime { get; }
        public double QuoteAssetVolume { get; }
        public int NumberOfTrades { get; }
        public double TakerBuyBaseAssetVolume { get; }
        public double TakerBuyQuoteAssetVolume { get; }

        public Candle(DateTime openTime,
            double open,
            double high,
            double low,
            double close,
            double volume,
            DateTime closeTime,
            double quoteAssetVolume,
            int numberOfTrades,
            double takerBuyBaseAssetVolume,
            double takerBuyQuoteAssetVolume)
        {
            OpenTime = openTime;
            High = high;
            Low = low;
            Open = open;
            Close = close;
            Volume = volume;
            CloseTime = closeTime;
            QuoteAssetVolume = quoteAssetVolume;
            NumberOfTrades = numberOfTrades;
            TakerBuyBaseAssetVolume = takerBuyBaseAssetVolume;
            TakerBuyQuoteAssetVolume = takerBuyQuoteAssetVolume;
        }

        public Candle(long openTime,
            double open,
            double high,
            double low,
            double close,
            double volume,
            long closeTime,
            double quoteAssetVolume,
            int numberOfTrades,
            double takerBuyBaseAssetVolume,
            double takerBuyQuoteAssetVolume)
        {
            OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(openTime).DateTime;
            High = high;
            Low = low;
            Open = open;
            Close = close;
            Volume = volume;
            CloseTime = DateTimeOffset.FromUnixTimeMilliseconds(closeTime).DateTime;
            QuoteAssetVolume = quoteAssetVolume;
            NumberOfTrades = numberOfTrades;
            TakerBuyBaseAssetVolume = takerBuyBaseAssetVolume;
            TakerBuyQuoteAssetVolume = takerBuyQuoteAssetVolume;
        }
    }
}
