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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using ImpromptuInterface.Internal.Support;
using ImpromptuInterface.Optimization;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Expando-Type List for dynamic objects
    /// </summary>
    [Serializable]
    public class ImpromptuList : ImpromptuDictionaryBase, IList<object>, IDictionary<string, object>, INotifyCollectionChanged, ITypedList, IList

    {

        /// <summary>
        /// Wrapped list
        /// </summary>
        protected IList<object> _list;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuDictionary"/> class.
        /// </summary>
        private static readonly object ListLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuList"/> class.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="members">The members.</param>
        public ImpromptuList(
            IEnumerable<object> contents =null,
            IEnumerable<KeyValuePair<string, object>> members =null):base(members)
        {
            if (contents == null)
            {
                _list = new List<object>();
                return;
            }
            if (contents is IList<object>)
            {
                _list = contents as IList<object>;
            }
            else
            {
                _list = contents.ToList();
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuList"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ImpromptuList(SerializationInfo info, 
           StreamingContext context):base(info,context)
        {
            _list = info.GetValue<IList<object>>("_list");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (_list.OfType<Delegate>().Any())
            {
                throw new SerializationException("Won't serialize protoType objects containing delegates");
            }
            base.GetObjectData(info,context);
            info.AddValue("_list", _list);
        }
#endif

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<dynamic> GetEnumerator()
        {
            return _list.GetEnumerator();
        }



        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(dynamic item)
        {
            InsertHelper(item);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            lock (ListLock)
            {
                _list.Clear();

            } 
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(dynamic item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(object[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }



        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _list.Count; }
        }



        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int IndexOf(dynamic item)
        {
            lock (ListLock)
            {
                return _list.IndexOf(item);
            }
        }

        public void Insert(int index, dynamic item)
        {
            InsertHelper(item,index);
        }

        private void InsertHelper(object item, int? index = null)
        {
            lock (ListLock)
            {
                if (!index.HasValue)
                {
                    index = _list.Count;
                    _list.Add(item);
                   
                }
                else
                {
                    _list.Insert(index.Value, item);
                }
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Add, newItem: item, newIndex: index);
        }

        public void RemoveAt(int index)
        {
            RemoveHelper(index: index);
        }

        public bool Remove(dynamic item)
        {
            return RemoveHelper(item);
        }

        private bool RemoveHelper(object item = null, int? index = null)
        {
      
            lock (ListLock)
            {
                if (item != null)
                {
                    index = _list.IndexOf(item);
                    if (index < 0)
                        return false;
                }

                item  = item ?? _list[index.GetValueOrDefault()];
                _list.RemoveAt(index.GetValueOrDefault());
            } 
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, oldItem: item, oldIndex: index);

            return true;
        }

        public dynamic this[int index]
        {
            get { return _list[index]; }
            set
            {
                object tOld;
                lock (ListLock)
                {
                    tOld = _list[index];
                    _list[index] = value;
                }

                OnCollectionChanged(NotifyCollectionChangedAction.Replace, tOld, value, index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem = null, object newItem = null, int? oldIndex = null, int? newIndex = null)

        {
            if (CollectionChanged != null)
            {
                switch (action)
                {
                    case NotifyCollectionChangedAction.Add:
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, newIndex.GetValueOrDefault()));
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, oldItem, oldIndex.GetValueOrDefault()));
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, oldItem, newItem, oldIndex.GetValueOrDefault()));
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        CollectionChanged(this,new NotifyCollectionChangedEventArgs(action));
                        break;
                }
            }

            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnPropertyChanged("Count");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnPropertyChanged("Count");
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnPropertyChanged("Count");
                    break;
            }
        }

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        dynamic IDictionary<string, object>.this[string key]
        {
         
            get { return _dictionary[key]; }
            set
            {
                SetProperty(key, value);
            }
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(ImpromptuList other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other._list, _list);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ImpromptuList);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ _list.GetHashCode();
            }
        }


        /// <summary>
        /// Gets or sets the override getting item method names. USED for GetItemProperties
        /// </summary>
        /// <value>The override getting item method names.</value>
        public Func<IEnumerable<object>, IEnumerable<string>> OverrideGettingItemMethodNames { get; set; }



        /// <summary>
        /// Gets the represented item. USED fOR GetItemProperties
        /// </summary>
        /// <returns></returns>
        protected virtual dynamic GetRepresentedItem()
        {
            var tItem = ((IEnumerable<object>)this).FirstOrDefault();
            return tItem;
        }

    
      
#region Implementation of ITypedList

#if !SILVERLIGHT

        /// <summary>
        /// Returns the name of the list.
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor"/> objects, for which the list name is returned. This can be null.</param>
        /// <returns>The name of the list.</returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return null;
        }


        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            IEnumerable<string> tList = new string[] { };
            if (OverrideGettingItemMethodNames != null)
            {
                tList = OverrideGettingItemMethodNames(this);
            }
            else
            {

                tList = Impromptu.GetMemberNames(GetRepresentedItem(), dynamicOnly: true);
            }

            return new PropertyDescriptorCollection(tList.Select(it => new ImpromptuPropertyDescriptor(it)).ToArray());
        }

#endif

#endregion


        #region Implementation of ICollection

        public void CopyTo(Array array, int index)
        {
            ((IList)_list).CopyTo(array, index);
        }
        private readonly object _syncRoot = new object();
        public object SyncRoot
        {
            get { return _syncRoot; }
        }


        public bool IsSynchronized
        {
            get { return false; }
        }

        #endregion

        #region Implementation of IList


        int IList.Add(object value)
        {
            Add(value);
            return Count - 1;
        }

        void IList.Remove(object value)
        {
            Remove(value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        #endregion
    }
}
