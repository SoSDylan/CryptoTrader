using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoTrader.Utils
{
    public static class ListUtils
    {
        public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> list)
        {
            return list.Select((item, index) => (item, index));
        }
        
        public static double StandardDeviation(this IEnumerable<double> values)
        {   
            double standardDeviation = 0;

            var enumerable = values.ToList();
            if (enumerable.Any()) 
            {      
                // Compute the average.     
                double avg = enumerable.Average();

                // Perform the Sum of (value-avg)_2_2.      
                double sum = enumerable.Sum(d => Math.Pow(d - avg, 2));

                // Put it all together.      
                standardDeviation = Math.Sqrt((sum) / (enumerable.Count()-1));   
            }  

            return standardDeviation;
        }
    }
}
