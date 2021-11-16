using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTrader.Hyperopts.Loss;
using CryptoTrader.Strategy;

namespace CryptoTrader.Utils
{
    internal static class ReflectionUtils
    {
        internal static Type GetStrategyFromClassName(string className)
        {
            var strategy = GetStrategies().FirstOrDefault(x => x.Name == className);
            
            if (strategy == null)
                throw new Exception($"Strategy \"{className}\" not found\n" +
                                    $"       Strategies: {string.Join(", ", GetStrategies().Select(x => x.Name))}");

            return strategy;
        }
        
        internal static Type GetHyperoptLossFromClassName(string className)
        {
            var hyperoptLoss = GetHyperoptLosses().FirstOrDefault(x => x.Name == className);
            
            if (hyperoptLoss == null)
                throw new Exception($"Hyperopt Loss \"{className}\" not found\n" +
                                    $"       Hyperopt Losses: {string.Join(", ", GetHyperoptLosses().Select(x => x.Name))}");

            return hyperoptLoss;
        }
        
        internal static List<Type> GetStrategies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(BaseStrategy).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToList();
        }
        
        internal static List<Type> GetHyperoptLosses()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IHyperoptLoss).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToList();
        }
    }
}
