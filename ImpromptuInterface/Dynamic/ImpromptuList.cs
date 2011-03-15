using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Expando-Type List for dynamic objects
    /// </summary>
    public class ImpromptuList : ImpromptuDictionaryBase, IList<object>, IDictionary<string, object>, INotifyCollectionChanged

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

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public IEnumerator<object> GetEnumerator()
        {
            return _list.GetEnumerator();
        }



        public void Add(object item)
        {
            InsertHelper(item);
        }

        public void Clear()
        {
            lock (ListLock)
            {
                _list.Clear();

            } 
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Contains(object item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

     

        public int Count
        {
            get { return _list.Count; }
        }



        public int IndexOf(object item)
        {
            lock (ListLock)
            {
                return _list.IndexOf(item);
            }
        }

        public void Insert(int index, object item)
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
                } 
                _list.Insert(index.Value, item);
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Add, newItem: item, newIndex: index);
        }

        public void RemoveAt(int index)
        {
            RemoveHelper(index: index);
        }

        public bool Remove(object item)
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

        public object this[int index]
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
    
    }
}
