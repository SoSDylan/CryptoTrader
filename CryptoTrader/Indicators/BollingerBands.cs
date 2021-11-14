using CryptoTrader.Core;

namespace CryptoTrader.Indicators
{
    public class BollingerBands : Indicator
    {
        private readonly SimpleMovingAverage _sma;
        private readonly StandardDeviation _sd;

        public readonly int K;

        public BollingerBands(Candles candles, int periods, int k = 2) : base(candles)
        {
            _sma = new SimpleMovingAverage(candles, periods);
            _sd = new StandardDeviation(candles, periods);

            K = k;
        }

        public BollingerBand? this[int index]
        {
            get
            {
                var main = _sma[index];
                var upper = _sma[index] + K * _sd[index];
                var lower = _sma[index] - K * _sd[index];
                
                if (!main.HasValue || !upper.HasValue || !lower.HasValue)
                    return null;
                
                return new BollingerBand(main.Value, upper.Value, lower.Value);
            }
        }

        public struct BollingerBand
        {
            public BollingerBand(double main, double upper, double lower)
            {
                Main = main;
                Upper = upper;
                Lower = lower;
            }

            public double Main { get; private set; }
            public double Upper { get; private set; }
            public double Lower { get; private set; }
        }
    }
}
