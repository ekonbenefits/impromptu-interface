// 
//  Copyright 2010  Ekon Benefits
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
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ImpromptuInterface.Dynamic
{
    public class ImpromptuDictionary:ImpromptuObject,IDictionary<string,object>,INotifyPropertyChanged
    {
        protected readonly IDictionary<string,object> _dictionary = new Dictionary<string, object>();


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_dictionary.ContainsKey(binder.Name))
                result = _dictionary[binder.Name];
            else
            {
                result = null;
                Type tType;
                if (!TryTypeForName(binder.Name, out tType))
                {
                    
                    return false;
                }
                if (tType.IsValueType)
                {
                    result = Activator.CreateInstance(tType);
                }
            }
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            if (_dictionary.ContainsKey(binder.Name))
            {
                var tFunc = _dictionary[binder.Name] as Delegate;
                if (tFunc !=null)
                {
                    try
                    {
                        result = tFunc.DynamicInvoke(args);
                    }
                    catch (TargetInvocationException ex)
                    {
                        if(ex.InnerException !=null)
                            throw ex.InnerException;
                        throw ex;
                    }
                }
                return false;
            }
            Type tType;
            if (!TryTypeForName(binder.Name, out tType))
            {

                return false;
            }
            if (tType.IsValueType)
            {
                result = Activator.CreateInstance(tType);
            }
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;
            return true;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
           return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
           this[item.Key]=item.Value;
        }

        public void Clear()
        {
            var tKeys = Keys;
           

            _dictionary.Clear();

            foreach (var tKey in tKeys)
            {
                OnPropertyChanged(tKey);
            }
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array,arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            object tValue;
            if (TryGetValue(item.Key, out tValue))
            {
                if (item.Value == tValue)
                {
                    Remove(item.Key);
                }
            }
            return false;
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            this[key]=value;
        }

        public bool Remove(string key)
        {
           var tReturn = _dictionary.Remove(key);
           OnPropertyChanged(key);
           return tReturn;
        }

        public bool TryGetValue(string key, out object value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public object this[string key]
        {
            get { return _dictionary[key]; }
            set
            {
                _dictionary[key] = value;
                OnPropertyChanged(key);
            }
        }

        protected virtual void OnPropertyChanged(string key)
        {
            if(PropertyChanged !=null)
             PropertyChanged(this,new PropertyChangedEventArgs(key));
        }

        public ICollection<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<object> Values
        {
            get { return _dictionary.Values; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
