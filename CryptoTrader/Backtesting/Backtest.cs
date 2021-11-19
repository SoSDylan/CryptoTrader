using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CryptoTrader.Core;
using CryptoTrader.Strategy;
using CryptoTrader.Utils;

namespace CryptoTrader.Backtesting
{
    internal class Backtest
    {
        private readonly int _epoch;
        private readonly BaseStrategy _strategy;
        private readonly Candles _candles;
        private readonly int? _buyTimeout;
        private readonly int? _sellTimeout;

        private List<Trade> _trades = new();

        private Trade? CurrentTrade => _trades.LastOrDefault();

        internal Backtest(int epoch, BaseStrategy strategy, Candles candles, int? buyTimeout = null, int? sellTimeout = null)
        {
            _epoch = epoch;
            _strategy = strategy.DeepCopy();
            _candles = candles;
            
            _buyTimeout = buyTimeout;
            _sellTimeout = sellTimeout;
        }

        internal BacktestResults RunBacktest()
        {
            for (int i = 1; i <= _candles.List.RealCount; i++)
            {
                _candles.List.SetMaxSize(i);
                var (buy, sell) = _strategy.Tick(_candles);
                DoBuyAndSell(buy, sell, _candles);
            }

            var finalizedTrades = _trades.Where(trade => trade.IsFinalized).ToList();
            
            return new BacktestResults(_epoch, finalizedTrades);
        }

        private void DoBuyAndSell(BaseStrategy.BuyRequest? buy, BaseStrategy.SellRequest? sell, Candles candles)
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
                case TradeState.Purchased:
                    if (sell.HasValue)
                    {
                        // Try Sell
                        var sellRequest = sell.Value;

                        CurrentTrade.TrySell(sellRequest.Price, candles.GetCurrentCandle().CloseTime);
                    }

                    if (candles.GetCurrentCandle().Low < CurrentTrade.StopLoss)
                    {
                        // Try stop loss
                        CurrentTrade.TrySell(CurrentTrade.StopLoss.Value, candles.GetCurrentCandle().CloseTime);
                    }
                    
                    if (CurrentTrade.ForceStopLoss.HasValue && candles.GetCurrentCandle().Low < CurrentTrade.ForceStopLoss)
                    {
                        // Try force stop loss
                        CurrentTrade.TrySell(CurrentTrade.ForceStopLoss.Value, candles.GetCurrentCandle().CloseTime);
                        CurrentTrade.FinalizeSell();
                    }
                    
                    break;
                
                case TradeState.Waiting when buy.HasValue:
                    // Try Buy
                    var buyRequest = buy.Value;
                    
                    CurrentTrade.TryBuy(buyRequest.Price,
                                        candles.GetCurrentCandle().CloseTime,
                                        buyRequest.StopLoss,
                                        buyRequest.ForceStopLoss);
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
        
        private double? BuyAtPriceFee => BuyAtPrice.HasValue ? BuyAtPrice.Value * (1 - CryptoTrader.Config.Exchange.FeeMultiplier) : null;
        private double? SellAtPriceFee => BuyAtPrice.HasValue ? BuyAtPrice.Value * (1 - CryptoTrader.Config.Exchange.FeeMultiplier) : null;
        
        private double? BuyAtPriceFeeTaken => BuyAtPrice.HasValue ? BuyAtPrice.Value - BuyAtPriceFee : null;
        private double? SellAtPriceFeeTaken => SellAtPrice.HasValue ? SellAtPrice.Value - SellAtPriceFee : null;
        
        public double? StopLoss;
        public double? ForceStopLoss;
        
        public bool IsFinalized => TradeState is TradeState.Finalized or TradeState.BuyRejected or TradeState.SellRejected;

        public bool Successful => TradeState is TradeState.Finalized;

        public double ProfitPercentage
        {
            get
            {
                if (TradeState == TradeState.Finalized)
                {
                    var multiplier = ((SellAtPrice - BuyAtPrice) - (SellAtPriceFee + BuyAtPriceFee)) / SellAtPrice ?? 0;
                    return multiplier * 100;
                }

                return 0;
            }
        }

        public void TryBuy(double buyAtPrice, DateTime buyAtTime, double stopLoss, double? forceStopLoss)
        {
            TradeState = TradeState.BuyPending;
            
            BuyAtPrice = buyAtPrice;
            BuyAtTime = buyAtTime;

            StopLoss = stopLoss;
            ForceStopLoss = forceStopLoss;
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
