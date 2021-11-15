using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTrader.Backtesting;
using CryptoTrader.Core;
using CryptoTrader.Hyperopts.Loss;
using CryptoTrader.Strategy;

namespace CryptoTrader.Hyperopts
{
    public class Hyperopt
    {
        private readonly BaseStrategy _strategy;
        private readonly Candles _candles;
        private readonly IHyperoptLoss _hyperoptLoss;
        private readonly HyperoptContext _hyperoptContext;
        private readonly int _epochs;
        private readonly int? _buyTimeout;
        private readonly int? _sellTimeout;

        // The higher the number, the better the algorithm
        private double _bestLossResult = double.MinValue;
        private Dictionary<string, dynamic> _bestOptimizableValues = new();
        private BacktestResults _bestBacktestResult;

        public Hyperopt(BaseStrategy strategy, IHyperoptLoss hyperoptLoss, Candles candles, int epochs,
            int? buyTimeout = null, int? sellTimeout = null)
        {
            _strategy = strategy;
            _candles = candles;
            _hyperoptLoss = hyperoptLoss;
            _epochs = epochs;
            _buyTimeout = buyTimeout;
            _sellTimeout = sellTimeout;

            _hyperoptContext = _strategy.HyperoptContext();
        }

        public void Optimize()
        {
            for (var epoch = 0; epoch < _epochs; epoch++)
            {
                // Run backtesting
                var backtestResult = new Backtest(_strategy, _candles, _buyTimeout, _sellTimeout).RunBacktest();
                // Print test results
                backtestResult.PrintBasicResults();
                
                // Get loss from backtest results
                var lossResult = _hyperoptLoss.GetLoss(backtestResult);
                
                // check if this is the best loss result
                if (lossResult > _bestLossResult)
                {
                    _bestLossResult = lossResult;
                    _bestBacktestResult = backtestResult;
                    _bestOptimizableValues = new Dictionary<string, dynamic>();
                    foreach (var optimizable in _hyperoptContext.Optimizables)
                    {
                        _bestOptimizableValues.Add(optimizable.GetName(), optimizable.GetValue());
                    }
                }
                
                // Optimize values
                OptimizeValues(epoch, _epochs);
            }

            PrintHyperoptResults();
        }

        private void PrintHyperoptResults()
        {
            Console.WriteLine();
            _bestBacktestResult.PrintResults();
            
            Console.WriteLine();
            Console.WriteLine("------------ Best Values ------------");
            
            foreach (var (name, value) in _bestOptimizableValues)
            {
                Console.Write(name);
                Console.Write(": ");
                Console.WriteLine(value);
            }
        }

        private void RandomizeValues()
        {
            foreach (var optimizable in _hyperoptContext.Optimizables)
            {
                optimizable.RandomizeValue();
            }
        }

        private void OptimizeValues(int epoch, int maxEpochs)
        {
            for (var i = 0; i < _hyperoptContext.Optimizables.Count; i++)
            {
                var lastOptimization = _hyperoptContext.Optimizables[i];
                var bestOptimization = _bestOptimizableValues.ElementAt(i).Value;
                
                lastOptimization.OptimizeValue(epoch,
                    maxEpochs,
                    lastOptimization.GetValue(),
                    bestOptimization);
            }
        }
    }
}
