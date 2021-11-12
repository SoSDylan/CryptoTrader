using CryptoTrader.Core;
using CryptoTrader.Hyperopt;

namespace CryptoTrader.Strategy
{
    public abstract class BaseStrategy
    {
        public bool HasPurchased { get; private set; }
        public double? BoughtAt { get; private set; }
        
        public double Profit { get; private set; }
        
        public abstract HyperoptContext HyperoptContext();
        
        public abstract bool BuySignal(Candles candles);
        public abstract bool SellSignal(Candles candles);

        public void Tick(Candles candles)
        {
            var buy = BuySignal(candles);
            var sell = SellSignal(candles);

            if (HasPurchased && BoughtAt.HasValue && sell)
            {
                // Sell
                Profit += candles.GetCurrentCandle().Close - BoughtAt.Value;
                BoughtAt = null;
                HasPurchased = false;
            }
            else if (!HasPurchased && buy)
            {
                // Buy
                BoughtAt = candles.GetCurrentCandle().Close;
                HasPurchased = true;
            }
        }
        
        public BaseStrategy DeepCopy()
        {
            return (BaseStrategy) this.MemberwiseClone();
        }
    }
}
