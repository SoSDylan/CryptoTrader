using CryptoTrader.Core;

namespace CryptoTrader.Indicators
{
    public abstract class Indicator
    {
        protected readonly Candles Candles;

        protected Indicator(Candles candles)
        {
            Candles = candles;
        }
    }
}
