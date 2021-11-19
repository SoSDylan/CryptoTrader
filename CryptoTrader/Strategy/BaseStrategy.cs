using CryptoTrader.Core;
using CryptoTrader.Hyperopts;

namespace CryptoTrader.Strategy
{
    public abstract class BaseStrategy
    {
        protected internal abstract HyperoptContext HyperoptContext();
        
        /// <summary>Decide whether to initiate a buy trade</summary>
        /// <param name="candles">The previous candles</param>
        /// <returns>The price to buy or <c>null</c> to not buy</returns>
        protected abstract BuyRequest? BuySignal(Candles candles);

        /// <summary>Decide whether to initiate a sell trade</summary>
        /// <param name="candles">The previous candles</param>
        /// <returns>The price to sell or <c>null</c> to not sell</returns>
        protected abstract SellRequest? SellSignal(Candles candles);

        internal (BuyRequest? buy, SellRequest? sell) Tick(Candles candles)
        {
            var buy = BuySignal(candles);
            var sell = SellSignal(candles);

            return (buy, sell);
        }
        
        internal BaseStrategy DeepCopy()
        {
            return (BaseStrategy) this.MemberwiseClone();
        }

        protected internal struct BuyRequest
        {
            public double Price;
            public double StopLoss;
            public double? ForceStopLoss;
            
            public BuyRequest(double price, double stopLoss, double? forceStopLoss = null)
            {
                Price = price;
                StopLoss = stopLoss;
                ForceStopLoss = stopLoss;
            }
        }

        protected internal struct SellRequest
        {
            public double Price;
            
            public SellRequest(double price)
            {
                Price = price;
            }
        }
    }
}
