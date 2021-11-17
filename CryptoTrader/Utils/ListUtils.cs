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
    }
}
