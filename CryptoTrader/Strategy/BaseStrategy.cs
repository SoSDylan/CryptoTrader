using CryptoTrader.Core;
using CryptoTrader.Hyperopts;

namespace CryptoTrader.Strategy
{
    public abstract class BaseStrategy
    {
        public abstract HyperoptContext HyperoptContext();
        
        /// <summary>Decide whether to initiate a buy trade</summary>
        /// <param name="candles">The previous candles</param>
        /// <returns>A <c>double</c> value to buy or <c>null</c> to not buy</returns>
        public abstract double? BuySignal(Candles candles);
        
        /// <summary>Decide whether to initiate a sell trade</summary>
        /// <param name="candles">The previous candles</param>
        /// <returns>A <c>double</c> value to sell or <c>null</c> to not sell</returns>
        public abstract double? SellSignal(Candles candles);

        public (double? buy, double? sell) Tick(Candles candles)
        {
            var buy = BuySignal(candles);
            var sell = SellSignal(candles);

            return (buy, sell);
        }
        
        public BaseStrategy DeepCopy()
        {
            return (BaseStrategy) this.MemberwiseClone();
        }
    }
}
