using System.Linq;
using CryptoTrader.Core;

namespace CryptoTrader.Indicators
{
    public class SimpleMovingAverage : Indicator
    {
        private readonly int _periods;

        public SimpleMovingAverage(Candles candles, int periods) : base(candles)
        {
            _periods = periods;
        }
        
        /// <summary>
        /// Gets the SMA value for the specified candle
        /// </summary>
        /// <param name="index">What candle should we get, 1 would mean 1 period before</param>
        public double? this[int index]
        {
            get
            {
                var rangeOfInterest = Candles.List.SkipLast(index).TakeLast(_periods).ToList();
                if (rangeOfInterest.Any())
                    return rangeOfInterest.Average(candle => candle.Close);
                
                return null;
            }
        }
    }
}
