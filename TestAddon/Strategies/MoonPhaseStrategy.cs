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
    
        public override HyperoptContext HyperoptContext()
        {
            return new HyperoptContext()
                .Optimize(nameof(_buy), () => _buy, val => _buy = val, 0, 30)
                .Optimize(nameof(_sell), () => _sell, val => _sell = val, 0, 30);
        }

        public override double? BuySignal(Candles candles)
        {
            var moon = new MoonPhases(candles);
            var vmoon = moon[0];
        
            if (!vmoon.HasValue) return null;

            if (_buy >= vmoon.Value)
                return candles.GetCurrentCandle().Close;

            return null;
        }

        public override double? SellSignal(Candles candles)
        {
            var moon = new MoonPhases(candles);
            var vmoon = moon[0];
        
            if (!vmoon.HasValue) return null;

            if (_sell <= vmoon.Value)
                return candles.GetCurrentCandle().Close;

            return null;
        }
    }
}
