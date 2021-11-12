using System;
using CryptoTrader.Core;
using CryptoTrader.Hyperopt;

namespace CryptoTrader.Strategy
{
    public class TestStrategy : BaseStrategy
    {
        private int _buyLessThan = 0;
        private int _sellGreaterThan = 0;
        
        public override HyperoptContext HyperoptContext()
        {
            return new HyperoptContext()
                .Optimize(nameof(_buyLessThan), () => _buyLessThan, val => _buyLessThan = val, 0, 10)
                .Optimize(nameof(_sellGreaterThan), () => _sellGreaterThan, val => _sellGreaterThan = val, 0, 10);
        }

        public override bool BuySignal(Candles candles)
        {
            return candles.GetCurrentCandle().Close <= _buyLessThan;
        }

        public override bool SellSignal(Candles candles)
        {
            return candles.GetCurrentCandle().Close > _sellGreaterThan;
        }
    }
}
