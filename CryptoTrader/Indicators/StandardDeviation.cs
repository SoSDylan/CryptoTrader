using System;
using CryptoTrader.Core;

namespace CryptoTrader.Indicators
{
    public class StandardDeviation : Indicator
    {
        private readonly int _periods;
        private readonly SimpleMovingAverage _sma;

        public StandardDeviation(Candles candles, int periods) : base(candles)
        {
            _periods = periods;
            _sma = new SimpleMovingAverage(candles, periods);
        }

        public double? this[int index]
        {
            get
            {
                var average = _sma[index];

                if (!average.HasValue)
                    return null;
                
                double sum = 0;
                int count = 0;
                
                for (var p = 0; p < _periods; p++)
                {
                    var candle = Candles[index + p];
                    
                    if (candle == null) continue;
                    
                    sum += Math.Pow(candle.Close - average.Value, 2.0);
                    count++;
                }
                
                return Math.Sqrt(sum / count);
            }
        }
    }
}
