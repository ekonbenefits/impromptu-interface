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

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Similar to Expando Objects but handles null values when the property is defined with an impromptu interface
    /// </summary>
     public class ImpromptuDictionary:ImpromptuDictionaryBase,IDictionary<string,object>
    {

        /// <summary>
        /// Convenience create method to make an Impromptu Dictionary instance acting like interface type parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict">The dict.</param>
        /// <returns></returns>
        public static T Create<T>(IEnumerable<KeyValuePair<string, object>> dict = null) where T : class
        {
            return dict == null
                       ? new ImpromptuDictionary().ActLike<T>()
                       : new ImpromptuDictionary(dict).ActLike<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuDictionary"/> class.
        /// </summary>
        public ImpromptuDictionary() 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuDictionary"/> class.
        /// </summary>
        /// <param name="dict">The dict.</param>
        public ImpromptuDictionary(IEnumerable<KeyValuePair<string, object>> dict) : base(dict)
        {
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _dictionary.Count; }
        }


        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
           return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            var tKeys = Keys;
           

            _dictionary.Clear();

            foreach (var tKey in tKeys)
            {
                OnPropertyChanged(tKey);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key]
        {
            get { return _dictionary[key]; }
            set
            {
                SetProperty(key, value);
            }
        }
    }
	
		public class ImpromptuChainableDictionary:ImpromptuDictionary{
			public override bool TryInvokeMember (InvokeMemberBinder binder, object[] args, out object result)
			{
				if(base.TryInvokeMember (binder, args, out result)){
					return true;
				}
				if(binder.CallInfo.ArgumentCount ==1){
					_dictionary[binder.Name] = args.FirstOrDefault();
					result = this;
					return true;
				}
                if (binder.CallInfo.ArgumentCount > 1)
                {
                    _dictionary[binder.Name] = new ImpromptuList(args);
                    result = this;
                    return true;
                }
				
				return false;
			}
		}
}
