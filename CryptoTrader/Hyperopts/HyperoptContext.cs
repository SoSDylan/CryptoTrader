using System;
using System.Collections.Generic;
using CryptoTrader.Utils;

namespace CryptoTrader.Hyperopts
{
    public class HyperoptContext
    {
        private List<IOptimizable> _optimizables = new();
        public IList<IOptimizable> Optimizables => _optimizables.AsReadOnly();

        public HyperoptContext Optimize<T>(string name, Func<T> get, Action<T> set, T min, T max)
            where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            _optimizables.Add(new Optimizable<T>(name, get, set, min, max));

            return this;
        }

        public interface IOptimizable
        {
            public string GetName();
            public dynamic GetValue();
            
            public void RandomizeValue();
            public void OptimizeValue(int epoch, int maxEpochs, dynamic lastValue, dynamic bestValue);
        }

        private class Optimizable<T> : IOptimizable
        {
            private Func<T> _get;
            private Action<T> _set;
            private T _min;
            private T _max;
            // private T _step;
            
            private int _optimizedCount = 0;

            private string _name { get; set; }
            private T _value { get; set; }
            
            public Optimizable(string name, Func<T> get, Action<T> set, T min, T max)
            {
                _get = get;
                _set = set;
                _min = min;
                _max = max;

                _name = name;
                _value = _get()!;
            }

            public string GetName()
            {
                return _name;
            }

            public dynamic GetValue()
            {
                return _value!;
            }

            public void RandomizeValue()
            {
                _optimizedCount++;

                dynamic value = _get()!;
                dynamic min = _min!;
                dynamic max = _max!;
                
                // Get random value between min and max
                dynamic randomValue = min + (dynamic) new Random().NextDouble() * (max - min);

                _set((T) randomValue);
                _value = (T) randomValue;
            }

            public void OptimizeValue(int epoch, int maxEpochs, dynamic lastValue, dynamic bestValue)
            {
                _optimizedCount++;

                dynamic min = _min!;
                dynamic max = _max!;

                // Start bringing in random min and random max once we are 25% of the way through the epochs
                var percentage = Math.Min(maxEpochs, (maxEpochs - epoch) * 1.25);
                
                dynamic randomMin = MathUtils.Map(percentage, maxEpochs, 0, min, bestValue);
                dynamic randomMax = MathUtils.Map(percentage, maxEpochs, 0, max, bestValue);
                
                // Get random value between min and max
                dynamic randomValue = randomMin + (dynamic) new Random().NextDouble() * (randomMax - randomMin);

                if (typeof(T).IsIntegral())
                    randomValue = Math.Round(randomValue);
                
                _set((T) randomValue);
                _value = (T) randomValue;
            }
        }
    }
}
