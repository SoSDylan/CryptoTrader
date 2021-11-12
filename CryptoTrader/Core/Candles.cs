using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoTrader.Core
{
    public class Candles
    {
        public readonly int Timestep;
        public readonly int MaxCandles;
        
        private List<Candle> _candles = new();
        public IList<Candle> List => _candles.AsReadOnly();
        
        public Candles(int timestep, int maxCandles)
        {
            Timestep = timestep;
            MaxCandles = maxCandles;
        }
        
        public Candles(List<Candle> candles, int timestep, int maxCandles)
        {
            _candles = candles;
            Timestep = timestep;
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

        public Candle GetCurrentCandle()
        {
            return List.Last();
        }

        public Candle? GetCandle(int offset)
        {
            if (_candles.Count < offset)
            {
                return null;
            }
            
            return List[^(offset + 1)];
        }
    }
    
    public class Candle
    {
        public double High { get; }
        public double Low { get; }
        public double Open { get; }
        public double Close { get; }

        public Candle(double high, double low, double open, double close)
        {
            High = high;
            Low = low;
            Open = open;
            Close = close;
        }
    }
}
