using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTrader.Core;
using CryptoTrader.Strategy;

namespace CryptoTrader.Backtesting
{
    public class Backtest
    {
        private readonly BaseStrategy _strategy;
        private readonly Candles _candles;
        private readonly int? _buyTimeout;
        private readonly int? _sellTimeout;

        private List<Trade> _trades = new();

        private Trade? CurrentTrade => _trades.LastOrDefault();

        public Backtest(BaseStrategy strategy, Candles candles, int? buyTimeout = null, int? sellTimeout = null)
        {
            _strategy = strategy.DeepCopy();
            _candles = candles;
            
            _buyTimeout = buyTimeout;
            _sellTimeout = sellTimeout;
        }

        public BacktestResults RunBacktest()
        {
            for (int i = 1; i <= _candles.List.Count; i++)
            {
                var currentCandles = new Candles(_candles.List.Take(i).ToList(), _candles.Interval, _candles.MaxCandles);
                var (buy, sell) = _strategy.Tick(currentCandles);
                DoBuyAndSell(buy, sell, currentCandles);
            }
            
            var profitPercentage = _trades.Sum(trade => trade.ProfitPercentage);
            var totalTradesCount = _trades.Count;
            var successfulTradesCount = _trades.Count(trade => trade.Successful);
            var failedTradesCount = _trades.Count(trade => !trade.Successful);
            
            return new BacktestResults(profitPercentage, totalTradesCount, successfulTradesCount, failedTradesCount);
        }

        private void DoBuyAndSell(double? buy, double? sell, Candles candles)
        {
            // Emulates buying and selling on an exchange
            // Checks if the price has changed to a price where the order can be fulfilled
            
            if (CurrentTrade == null || CurrentTrade.IsFinalized)
            {
                _trades.Add(new Trade());
            }

            var tradeState = CurrentTrade!.TradeState;
            
            switch (tradeState)
            {
                case TradeState.Purchased when sell.HasValue:
                    // Try Sell
                    CurrentTrade.TrySell(sell.Value, candles.GetCurrentCandle().CloseTime);
                    break;
                
                case TradeState.Waiting when buy.HasValue:
                    // Try Buy
                    CurrentTrade.TryBuy(buy.Value, candles.GetCurrentCandle().CloseTime);
                    break;
                
                case TradeState.BuyPending when candles.GetCurrentCandle().CloseTime > CurrentTrade.BuyAtTime:
                    if (_buyTimeout != null &&
                        candles.GetCurrentCandle().CloseTime > CurrentTrade.BuyAtTime.Value.AddMinutes(_buyTimeout!.Value))
                        CurrentTrade.RejectBuy();
                    
                    if (candles.GetCurrentCandle().Low < CurrentTrade.BuyAtPrice)
                        CurrentTrade.FinalizeBuy();
                    
                    break;
                
                case TradeState.SellPending when candles.GetCurrentCandle().CloseTime > CurrentTrade.SellAtTime:
                    if (_sellTimeout != null &&
                        candles.GetCurrentCandle().CloseTime > CurrentTrade.SellAtTime.Value.AddMinutes(_sellTimeout!.Value))
                        CurrentTrade.RejectSell();
                    
                    if (candles.GetCurrentCandle().High > CurrentTrade.SellAtPrice)
                        CurrentTrade.FinalizeSell();
                    
                    break;
            }
        }
    }

    public class Trade
    {
        public TradeState TradeState = TradeState.Waiting;
        
        public double? BuyAtPrice;
        public DateTime? BuyAtTime;
        public double? SellAtPrice;
        public DateTime? SellAtTime;
        
        public bool IsFinalized => TradeState is TradeState.Finalized or TradeState.BuyRejected or TradeState.SellRejected;

        public bool Successful => TradeState is TradeState.Finalized;

        public double ProfitPercentage
        {
            get
            {
                if (TradeState == TradeState.Finalized)
                {
                    var multiplier = (SellAtPrice - BuyAtPrice) / BuyAtPrice ?? 0;
                    return multiplier * 100;
                }

                return 0;
            }
        }

        public void TryBuy(double buyAtPrice, DateTime buyAtTime)
        {
            TradeState = TradeState.BuyPending;
            
            BuyAtPrice = buyAtPrice;
            BuyAtTime = buyAtTime;
        }

        public void TrySell(double sellAtPrice, DateTime sellAtTime)
        {
            TradeState = TradeState.SellPending;
            
            SellAtPrice = sellAtPrice;
            SellAtTime = sellAtTime;
        }

        public void FinalizeBuy()
        {
            TradeState = TradeState.Purchased;
        }

        public void FinalizeSell()
        {
            TradeState = TradeState.Finalized;
        }

        public void RejectBuy()
        {
            TradeState = TradeState.BuyRejected;
        }

        public void RejectSell()
        {
            TradeState = TradeState.SellRejected;
        }
    }

    public enum TradeState
    {
        Waiting,
        BuyPending,
        BuyRejected,
        Purchased,
        SellPending,
        SellRejected,
        Finalized,
    }
}