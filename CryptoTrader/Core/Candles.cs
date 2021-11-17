using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTrader.Utils;

namespace CryptoTrader.Core
{
    public class Candles
    {
        public readonly int Interval;
        public readonly int MaxCandles;
        
        private AtomicList<Candle> _list = new();
        /// <summary>
        /// Start of list is oldest candle
        /// End of list is newest candle
        /// </summary>
        public AtomicList<Candle> List => _list;
        
        public Candles(int interval, int maxCandles)
        {
            Interval = interval;
            MaxCandles = maxCandles;
        }
        
        public Candles(AtomicList<Candle> list, int interval, int maxCandles)
        {
            _list = list;
            Interval = interval;
            MaxCandles = maxCandles;
            
            RemoveExcessCandles();
        }
        
        public void AddCandle(Candle candle)    
        {
            _list.Add(candle);
            
            RemoveExcessCandles();
        }

        public void RemoveExcessCandles()
        {
            if (_list.Count > MaxCandles)
            {
                _list.RemoveRange(0, _list.Count - MaxCandles);
            }
        }
        
        public Candle? this[int offset]
        {
            get
            {
                if (_list.Count <= offset)
                {
                    return null;
                }
            
                return _list[^(offset + 1)];
            }
        }

        public Candle GetCurrentCandle()
        {
            return _list[^1];
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
