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
        private double _stopLossMultiplier = 0.98; // 2%
        private double _forceStopLossMultiplier = 0.9; // 10%

        protected override HyperoptContext HyperoptContext()
        {
            return new HyperoptContext()
                .Optimize(nameof(_buyMultiplier), () => _buyMultiplier, val => _buyMultiplier = val, 0.75, 1.25)
                .Optimize(nameof(_sellMultiplier), () => _sellMultiplier, val => _sellMultiplier = val, 0.75, 1.25)
                .Optimize(nameof(_stopLossMultiplier), () => _stopLossMultiplier, val => _stopLossMultiplier = val, 0.9, 0.99)
                .Optimize(nameof(_forceStopLossMultiplier), () => _forceStopLossMultiplier, val => _forceStopLossMultiplier = val, 0.8, 0.9);
        }

        protected override BuyRequest? BuySignal(Candles candles)
        {
            var bb = new BollingerBands(candles, 10);
            var vbb = bb[0];
            
            if (!vbb.HasValue) return null;

            if (candles.GetCurrentCandle().Close <= _buyMultiplier * vbb.Value.Lower)
                return new BuyRequest(candles.GetCurrentCandle().Close,
                                      candles.GetCurrentCandle().Close * _stopLossMultiplier,
                                      candles.GetCurrentCandle().Close * _forceStopLossMultiplier);

            return null;
        }

        protected override SellRequest? SellSignal(Candles candles)
        {
            var bb = new BollingerBands(candles, 10);
            var vbb = bb[0];
            
            if (!vbb.HasValue) return null;

            if (candles.GetCurrentCandle().Close >= _sellMultiplier * vbb.Value.Main)
                return new SellRequest(candles.GetCurrentCandle().Close);

            return null;
        }
    }
}
