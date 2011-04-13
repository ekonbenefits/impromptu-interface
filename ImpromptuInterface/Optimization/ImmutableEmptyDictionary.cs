using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ImpromptuInterface.Optimization
{


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class ImmutableEmptyDictionary<TKey,TValue>:IDictionary<TKey,TValue>
    {
        private static IDictionary<TKey, TValue> _instance;

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static IDictionary<TKey,TValue> Instance
        {
           get
           {
               return _instance ?? (_instance = new ImmutableEmptyDictionary<TKey, TValue>());
           }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return (new List<KeyValuePair<TKey, TValue>>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public int Count
        {
            get { return 0; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool ContainsKey(TKey key)
        {
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        public bool Remove(TKey key)
        {
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        public TValue this[TKey key]
        {
            get { throw new KeyNotFoundException(); }
            set { throw new NotSupportedException(); }
        }

        public ICollection<TKey> Keys
        {
            get { return new TKey[] { }; }
        }

        public ICollection<TValue> Values
        {
            get { return new TValue[] {}; }
        }
    }
}
