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
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ImpromptuInterface.Dynamic;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Supports Providing Property info to Binding things like DataGrids that refresh with bindings
    /// </summary>
    [Serializable]
    public class ImpromptuBindingList: ImpromptuList, IBindingList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuBindingList"/> class.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="members">The members.</param>
        public ImpromptuBindingList(IEnumerable<object> contents=null, 
            IEnumerable<KeyValuePair<string, object>> members =null) : base(contents, members)
        {
        }

#if !SILVERLIGHT
        protected ImpromptuBindingList(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

  

        #region Implementation of IBindingList
          
        [Obsolete("Not Supported")]
        public object AddNew()
        {
            throw new NotSupportedException();
        }

        [Obsolete("Not Supported")]
        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }
           
        [Obsolete("Not Supported")]
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotSupportedException();
        }

         
        [Obsolete("Not Supported")]
        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotSupportedException();
        }

        [Obsolete("Not Supported")]
        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }

        [Obsolete("Not Supported")]
        public void RemoveSort()
        {
            throw new NotSupportedException();
        }

        public bool AllowNew
        {
            get { return false; }
        }

        public bool AllowEdit
        {
            get { return false; }
        }

        public bool AllowRemove
        {
            get { return false; }
        }

        public bool SupportsChangeNotification
        {
            get { return false; }
        }

        public bool SupportsSearching
        {
            get { return false; }
        }

        public bool SupportsSorting
        {
            get { return false; }
        }

        public bool IsSorted
        {
            get { return false; }
        }
                
        [Obsolete("Not Used")]
        public PropertyDescriptor SortProperty
        {
            get { return null; }
        }

        [Obsolete("Not Used")]
        public ListSortDirection SortDirection
        {
            get { return default(ListSortDirection); }
        }


        /// <summary>
        /// Occurs when the list changes or an item in the list changes.
        /// </summary>
        [Obsolete("Not Used")]
        public event ListChangedEventHandler ListChanged;
       

        #endregion 
#endif
    }
}
