using CryptoTrader.Core;
using CryptoTrader.Hyperopts;
using CryptoTrader.Strategy;
using TestAddon.Indicators;

namespace TestAddon.Strategies
{
    public class MoonPhaseStrategy : BaseStrategy
    {
        private int _buy = 0;
        private int _sell = 0;
        private double _stopLossMultiplier = 0.98; // 2%
    
        protected override HyperoptContext HyperoptContext()
        {
            return new HyperoptContext()
                .Optimize(nameof(_buy), () => _buy, val => _buy = val, 0, 30)
                .Optimize(nameof(_sell), () => _sell, val => _sell = val, 0, 30);
        }

        protected override BuyRequest? BuySignal(Candles candles)
        {
            var moon = new MoonPhases(candles);
            var vmoon = moon[0];
        
            if (!vmoon.HasValue) return null;

            if (_buy >= vmoon.Value)
                return new BuyRequest(candles.GetCurrentCandle().Close, candles.GetCurrentCandle().Close * _stopLossMultiplier);

            return null;
        }

        protected override SellRequest? SellSignal(Candles candles)
        {
            var moon = new MoonPhases(candles);
            var vmoon = moon[0];
        
            if (!vmoon.HasValue) return null;

            if (_sell <= vmoon.Value)
                return new SellRequest(candles.GetCurrentCandle().Close);

            return null;
        }
    }
}
