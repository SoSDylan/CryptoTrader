using CryptoTrader.Core;
using CryptoTrader.Hyperopts;
using CryptoTrader.Indicators;
using CryptoTrader.Strategy;

namespace TestAddon.Strategies
{
    public class BollingerBandStrategy : BaseStrategy
    {
        private double _buyMultiplier = 1;
        private double _sellMultiplier = 1;
        
        public override HyperoptContext HyperoptContext()
        {
            return new HyperoptContext()
                .Optimize(nameof(_buyMultiplier), () => _buyMultiplier, val => _buyMultiplier = val, 0.75, 1.25)
                .Optimize(nameof(_sellMultiplier), () => _sellMultiplier, val => _sellMultiplier = val, 0.75, 1.25);
        }

        public override double? BuySignal(Candles candles)
        {
            var bb = new BollingerBands(candles, 10);
            var vbb = bb[0];
            
            if (!vbb.HasValue) return null;

            if (candles.GetCurrentCandle().Close <= _buyMultiplier * vbb.Value.Lower)
                return candles.GetCurrentCandle().Close;

            return null;
        }

        public override double? SellSignal(Candles candles)
        {
            var bb = new BollingerBands(candles, 10);
            var vbb = bb[0];
            
            if (!vbb.HasValue) return null;

            if (candles.GetCurrentCandle().Close >= _sellMultiplier * vbb.Value.Main)
                return candles.GetCurrentCandle().Close;

            return null;
        }
    }
}
