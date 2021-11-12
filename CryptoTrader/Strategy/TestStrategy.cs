using System;
using CryptoTrader.Core;
using CryptoTrader.Hyperopt;

namespace CryptoTrader.Strategy
{
    public class TestStrategy : BaseStrategy
    {
        private double _buyLessThan = 0;
        private double _sellGreaterThan = 0;
        
        public override HyperoptContext HyperoptContext()
        {
            return new HyperoptContext()
            // .Optimize(nameof(_buyLessThan), () => _buyLessThan, val => _buyLessThan = val, 0, 10)
            // .Optimize(nameof(_sellGreaterThan), () => _sellGreaterThan, val => _sellGreaterThan = val, 0, 10);
            .Optimize(nameof(_buyLessThan), () => _buyLessThan, val => _buyLessThan = val, -100, 100)
            .Optimize(nameof(_sellGreaterThan), () => _sellGreaterThan, val => _sellGreaterThan = val, -100, 100);
        }

        public override double? BuySignal(Candles candles)
        {
            var lastCandle = candles.GetOlderCandle(1);
            if (lastCandle == null) return null;

            if (candles.GetCurrentCandle().Close - lastCandle.Close <= _buyLessThan)
                return candles.GetCurrentCandle().Close;
            
            return null;
        }

        public override double? SellSignal(Candles candles)
        {
            var lastCandle = candles.GetOlderCandle(1);
            if (lastCandle == null) return null;

            if (candles.GetCurrentCandle().Close - lastCandle.Close > _sellGreaterThan)
                return candles.GetCurrentCandle().Close;

            return null;
        }
    }
}
