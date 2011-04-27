using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ImpromptuInterface.MVVM
{
    public partial class ImpromptuViewModel
    {
        /// <summary>
        /// Trampoline object to add dependency
        /// </summary>
        public class DependsOn : DynamicObject
        {
            private readonly ImpromptuViewModel _parent;
            private readonly string _property;

            internal DependsOn(ImpromptuViewModel parent, string property)
            {
                _parent = parent;
                _property = property;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = new LinkFinal(_parent, _property, binder.Name);

                return true;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return _parent.LinkedProperties.Where(it => it.Value.Contains(_property)).Select(it=>it.Key);
            }
        }

        /// <summary>
        /// Trampoline object to finish dependency link
        /// </summary>
        public class LinkFinal
        {
            private readonly ImpromptuViewModel _parent;
            private readonly string _property;
            private readonly string _dependency;

            internal LinkFinal(ImpromptuViewModel parent, string property, string dependency)
            {
                _parent = parent;
                _property = property;
                _dependency = dependency;
            }

            /// <summary>
            /// Links the property with the dependency.
            /// </summary>
            public void Link()
            {
                _parent.DependencyLink(_property,_dependency);
            }

            /// <summary>
            /// Unlinks the property with the dependency.
            /// </summary>
            public void Unlink()
            {
                _parent.DependencyUnlink(_property, _dependency);
            }
        }


        /// <summary>
        /// Dependency aware version of FireOnPropertyChangedDependencyAware
        /// </summary>
        public class FireOnPropertyChangedDependencyAware : FireOnPropertyChanged
        {
            private readonly ImpromptuViewModel _parent;
            private readonly Dictionary<PropertyChangedEventHandler, string> _uniqueEvents = new Dictionary<PropertyChangedEventHandler, string>();


            /// <summary>
            /// Proxy to track add and remove of delegates
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public class DelegateAddRemove<T>
            {
                /// <summary>
                /// Implements the operator +.
                /// </summary>
                /// <param name="left">The left.</param>
                /// <param name="right">The right.</param>
                /// <returns>The result of the operator.</returns>
                public static  DelegateAddRemove<T> operator +(DelegateAddRemove<T> left, T right)
                {
                    left.Delegate = right;
                    left.IsAdding = true;

                    return left;
                }

                /// <summary>
                /// Implements the operator -.
                /// </summary>
                /// <param name="left">The left.</param>
                /// <param name="right">The right.</param>
                /// <returns>The result of the operator.</returns>
                public static DelegateAddRemove<T> operator -(DelegateAddRemove<T> left, T right)
                {
                    left.Delegate = right;
                    left.IsAdding = false;

                    return left;
                }

                /// <summary>
                /// Gets or sets the delegate.
                /// </summary>
                /// <value>The delegate.</value>
                public T Delegate { get; protected set; }

                /// <summary>
                /// Gets or sets a value indicating whether this instance is adding.
                /// </summary>
                /// <value><c>true</c> if this instance is adding; otherwise, <c>false</c>.</value>
                public bool IsAdding { get; protected set; }

                
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="FireOnPropertyChangedDependencyAware"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public FireOnPropertyChangedDependencyAware(ImpromptuViewModel parent)
                : base(parent)
            {
                _parent = parent;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = new DelegateAddRemove<PropertyChangedEventHandler>();
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                var tValue = (DelegateAddRemove<PropertyChangedEventHandler>)value;
               
                string tGuid;
                if (tValue.IsAdding)
                {
                    if (!_uniqueEvents.TryGetValue(tValue.Delegate, out tGuid))
                    {
                        tGuid = Guid.NewGuid().ToString();
                        _uniqueEvents.Add(tValue.Delegate, tGuid);
                        EventStore.Add(tGuid, tValue.Delegate);
                    } 
                    _parent.DependencyLink(tGuid, binder.Name);
                }
                else
                {
                    if (_uniqueEvents.TryGetValue(tValue.Delegate, out tGuid))
                    {
                        _parent.DependencyUnlink(tGuid, binder.Name);
                    }
                }
                return true;
            }
        } 
    }
}
