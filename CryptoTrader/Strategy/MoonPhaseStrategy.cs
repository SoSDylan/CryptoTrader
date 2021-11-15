using System;
using CryptoTrader.Core;
using CryptoTrader.Hyperopts;
using CryptoTrader.Indicators;

namespace CryptoTrader.Strategy
{
    public class MoonPhaseStrategy : BaseStrategy
    {
        private int _buy = 0;
        private int _sell = 0;

        private int _buyBool = 0;
        private int _sellBool = 0;
        
        public override HyperoptContext HyperoptContext()
        {
            return new HyperoptContext()
                .Optimize(nameof(_buy), () => _buy, val => _buy = val, 0, 30)
                .Optimize(nameof(_sell), () => _sell, val => _sell = val, 0, 30)
                .Optimize(nameof(_buyBool), () => _buyBool, val => _buyBool = val, 0, 1)
                .Optimize(nameof(_sellBool), () => _sellBool, val => _sellBool = val, 0, 1);
        }

        public override double? BuySignal(Candles candles)
        {
            var moon = new MoonPhases(candles);
            var vmoon = moon[0];
            
            if (!vmoon.HasValue) return null;

            if (_buyBool == 0 && _buy <= vmoon.Value)
                return candles.GetCurrentCandle().Close;
            else if (_buyBool != 0 && _buy >= vmoon.Value)
                return candles.GetCurrentCandle().Close;

            return null;
        }

        public override double? SellSignal(Candles candles)
        {
            var moon = new MoonPhases(candles);
            var vmoon = moon[0];
            
            if (!vmoon.HasValue) return null;

            if (_sellBool == 0 && _sell <= vmoon.Value)
                return candles.GetCurrentCandle().Close;
            else if (_sellBool != 0 && _sell >= vmoon.Value)
                return candles.GetCurrentCandle().Close;

            return null;
        }
    }
}
