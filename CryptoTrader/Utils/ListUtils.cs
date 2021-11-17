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
    }
    
    public class AtomicList<T> : List<T>
    {
        private int? _maxSize;

        /// <summary>
        /// Only allow access to <c>maxSize</c> amount of elements in this list.
        /// The underlying list will not be modified.
        /// </summary>
        /// <param name="maxSize">The amount of elements to allow access to</param>
        internal void SetMaxSize(int? maxSize)
        {
            lock (this)
            {
                _maxSize = maxSize;
            }
        }
        
        public new int Count
        {
            get
            {
                lock (this)
                {
                    if (_maxSize.HasValue)
                        return base.Count > _maxSize.Value ? _maxSize.Value : base.Count;
                    else
                        return base.Count;
                }
            }
        }
        
        public new T this[int index] {
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException();
                
                return base[index];
            }
            set
            {
                throw new NotSupportedException();
                // if (index >= Count)
                //     throw new IndexOutOfRangeException();
                //
                // base[index] = value;
            }
        }
    }
}
