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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using ImpromptuInterface.InvokeExt;
namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Attached Property Class
    /// </summary>
    public static class Event
    {
        internal class EventHandlerHash
        {
            
        
            private readonly string _name;
            private readonly Type _type;

            public EventHandlerHash(string name, Type type)
            {
                _name = name;
                _type = type;
            }

            public bool Equals(EventHandlerHash other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other._name, _name) && Equals(other._type, _type);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (EventHandlerHash)) return false;
                return Equals((EventHandlerHash) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((_name != null ? _name.GetHashCode() : 0)*397) ^ (_type != null ? _type.GetHashCode() : 0);
                }
            }
        }


       

        /// <summary>
        /// Property to Describe binding Events in DataContext
        /// </summary>
        public static readonly DependencyProperty BindProperty = DependencyProperty.RegisterAttached("Bind",
                                                                                                     typeof (EventBinder
                                                                                                         ),
                                                                                                     typeof (Event),
                                                                                                     new PropertyMetadata
                                                                                                         (null,
                                                                                                          OnBindChange));


        private static void OnBindChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tOldBinder = e.OldValue as EventBinder;
            if (tOldBinder != null)
            {
                tOldBinder.UnRegister(d);
            }
            var tBinder = e.NewValue as EventBinder;
            if (tBinder != null)
            {
                tBinder.Register(d);
            }
        }

        /// <summary>
        /// Sets the bind.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetBind(DependencyObject element, EventBinder value)
        {
            element.SetValue(BindProperty, value);
        }

        /// <summary>
        /// Gets the bind.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static EventBinder GetBind(DependencyObject element)
        {
            return element.GetValue(BindProperty) as EventBinder;
        }

        private static readonly IDictionary<EventHandlerHash, object> _eventHandlerStore = new Dictionary<EventHandlerHash, object>();
        private static readonly object _eventHandlerStoreLock = new object();


        internal static EventHandler FixEventHandler(EventHandler<EventArgs> func)
        {
            return (sender, e) => func(sender, e);
        }

        /// <summary>
        /// Object with method to create latebound eventhandler delegates
        /// </summary>
        public class BinderEventHandlerMemberName
        {

            /// <summary>
            /// The Invoke Method's Info
            /// </summary>
            public static readonly MethodInfo InvokeMethodInfo =
                typeof (BinderEventHandlerMemberName).GetMethod("Invoke");

            private readonly CacheableInvocation _invocation;

            /// <summary>
            /// Initializes a new instance of the <see cref="BinderEventHandlerMemberName"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            public BinderEventHandlerMemberName(string name)
            {
                _invocation = new CacheableInvocation(InvocationKind.InvokeMemberAction, name, 2);
            }

            /// <summary>
            /// Invokes the eventhandler
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The e.</param>
            public void Invoke(object sender, object e)
            {
                 var tSender = sender as DependencyObject;
                    if (tSender != null)
                    {
                        EventBinder tBinder = GetBind(tSender);
                        if (tBinder != null)
                        {
                            try
                            {
                                _invocation.Invoke(tBinder.Target, tSender, e);
                            }
                            catch (RuntimeBinderException)
                            {

                            }

                        }
                    }
            }
        }

        /// <summary>
        /// Generates the event handler.
        /// </summary>
        /// <param name="delType">Type of the del.</param>
        /// <param name="membername">The membername.</param>
        /// <returns></returns>
        public static object GenerateEventHandler(Type delType, string membername)
        {
            var tHash = new EventHandlerHash(membername, delType);
            

            lock (_eventHandlerStoreLock)
            {
                object tReturn;
                if (!_eventHandlerStore.TryGetValue(tHash, out tReturn))
                {
                    tReturn = Delegate.CreateDelegate(delType, new BinderEventHandlerMemberName(membername), 
                                                      BinderEventHandlerMemberName.InvokeMethodInfo);
                }
                return tReturn;
            }
        }

     
    }

    /// <summary>
    /// Helper Object to create event bindings.
    /// </summary>
    public class EventBinder:DynamicObject
    {
        public object Target { get; protected set; }
        private Lazy<EventBinderTo> Child;
        private IDictionary<string,string> List= new Dictionary<string, string>();
        private string _lastKey;

        public EventBinder(object target)
        {
            Target = target;
            Child = new Lazy<EventBinderTo>(()=>new EventBinderTo(target, this));
        }


        /// <summary>
        /// Registers the events on the  specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        public void Register(Object source)
        {
            foreach (var tPair in List)
            {
                RegisterUnRegister(false, source, tPair.Key, tPair.Value);
            }
        }

        /// <summary>
        /// Unregisters the events on the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        public void UnRegister(Object source)
        {
            foreach (var tPair in List)
            {
                RegisterUnRegister(true, source, tPair.Key, tPair.Value);
            }
        }



        /// <summary>
        /// Registers or unregister.
        /// </summary>
        /// <param name="un">if set to <c>true</c> [un].</param>
        /// <param name="source">The source.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="targetName">Name of the target.</param>
        private void RegisterUnRegister(bool un,object source, string eventName, string targetName)
        {
            if (Impromptu.InvokeIsEvent(source, eventName))
            {
               var tEvent = source.GetType().GetEvent(eventName);
               

                var tEventHandler = Event.GenerateEventHandler(tEvent.EventHandlerType, targetName);

                  if (un)
                    Impromptu.InvokeSubtractAssign(source, eventName, tEventHandler);
                  else
                    Impromptu.InvokeAddAssign(source,eventName, tEventHandler);
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this[binder.Name];
            return true;
        }

        /// <summary>
        /// To is used to specify what the event is going to bind to
        /// </summary>
        /// <value>To.</value>
        public virtual EventBinderTo To
        {
            get { return Child.Value; }
        }

        protected void UpdateLastKey(string value)
        {
            List[_lastKey] = value;
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <value></value>
        public virtual object this[string index]
        {
            get
            {
                List[index] = null;
                _lastKey = index;
                return this;
            }
        }

        /// <summary>
        /// Helper Object to create event bindings.
        /// </summary>
        public class EventBinderTo : EventBinder
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EventBinder.EventBinderTo"/> class.
            /// </summary>
            /// <param name="target">The target.</param>
            /// <param name="parent">The parent.</param>
            public EventBinderTo(object target, EventBinder parent) : base(target)
            {
                Parent = parent;
            }

            /// <summary>
            /// Gets or sets the parent.
            /// </summary>
            /// <value>The parent.</value>
            public EventBinder Parent { get; protected set; }

            /// <summary>
            /// Gets the <see cref="System.Object"/> at the specified index.
            /// </summary>
            /// <value></value>
            public override object this[string index]
            {
                get
                {
                    Parent.UpdateLastKey(index);

                    return Parent;
                }
            }
        }
    }

   
}
