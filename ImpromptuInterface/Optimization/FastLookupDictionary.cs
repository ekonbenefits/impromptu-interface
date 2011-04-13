// 
//  Copyright 2011 Ekon Benefits
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ImpromptuInterface.Optimization
{
#if!SILVERLIGHT


    internal class FastLookupDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        internal  Hashtable _hashtable = new Hashtable();


        internal class HashtableEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly IDictionaryEnumerator _enumerator;

            public HashtableEnumerator(IDictionaryEnumerator enumerator)
            {
                _enumerator = enumerator;
            }

            public void Dispose()
            {
               
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get { return new KeyValuePair<TKey, TValue>((TKey)_enumerator.Key, (TValue) _enumerator.Value); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new HashtableEnumerator(_hashtable.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _hashtable.Add(item.Key,item.Value);
        }

        public void Clear()
        {
            _hashtable.Clear();
            
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _hashtable.ContainsKey(item.Key) && (item.Value.Equals(_hashtable[item.Key]));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var tArray =_hashtable.Keys.OfType<TKey>().Select(it => new KeyValuePair<TKey,TValue>(it, (TValue) _hashtable[it])).ToArray();
            Array.Copy(tArray, arrayIndex, array, 0, array.Length);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                Remove(item.Key);
                return true;
            }
            return false;
        }

        public int Count
        {
            get { return _hashtable.Count; }
        }

        public bool IsReadOnly
        {
            get { return _hashtable.IsReadOnly; }
        }

        public bool ContainsKey(TKey key)
        {
            return _hashtable.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _hashtable.Add(key,value);
        }

        public bool Remove(TKey key)
        {
            if (_hashtable.ContainsKey(key))
            {
                _hashtable.Remove(key);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var tValue = _hashtable[key];

            if (tValue != null)
            {
                value = (TValue) tValue;
                return true;
            }
            if (ContainsKey(key))
            {
                value = (TValue)tValue;
                return true;
            }
            value = default(TValue);
            return false;
        }

        public TValue this[TKey key]
        {
            get { return (TValue) _hashtable[key]; }
            set { _hashtable[key] = value; }
        }

        public ICollection<TKey> Keys
        {
            get { return _hashtable.Keys.Cast<TKey>().ToList(); }
        }

        public ICollection<TValue> Values
        {
            get { return _hashtable.Keys.Cast<TValue>().ToList(); }
        }
    }
#endif
}