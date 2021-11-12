using System;

namespace CryptoTrader.Utils
{
    public static class MathUtils
    {
        public static T Map<T>(T input, T inMin, T inMax, T outMin, T outMax)
            where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable {
            
            return ((dynamic)input - (dynamic)inMin) * ((dynamic)outMax - (dynamic)outMin) / ((dynamic)inMax - (dynamic)inMin) + (dynamic)outMin;
        }
        
        public static bool IsIntegral(this Type type)
        {
            var typeCode = (int) Type.GetTypeCode(type);
            return typeCode > 4 && typeCode < 13;
        }
    }
}
