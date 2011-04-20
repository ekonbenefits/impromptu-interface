using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using ImpromptuInterface;
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


       
#if!SILVERLIGHT

        /// <summary>
        /// Property to Describe binding Events in DataContext
        /// </summary>
        public static readonly DependencyProperty BindProperty = DependencyProperty.RegisterAttached(
          "Bind",
          typeof(EventBinder),
          typeof(Event),
          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender, OnBindChange)
        );
#else
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
#endif


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

        public static void SetBind(DependencyObject element, EventBinder value)
        {
            element.SetValue(BindProperty, value);
        }

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

        public class BinderEventHandlerMemberName
        {

            public static readonly MethodInfo InvokeMethodInfo =
                typeof (BinderEventHandlerMemberName).GetMethod("Invoke");

            private readonly string _name;

            public BinderEventHandlerMemberName(string name)
            {
                _name = name;
            }

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
                                Impromptu.InvokeMemberAction(tBinder.Target,
                                                            _name, tSender, e);
                            }
                            catch (RuntimeBinderException)
                            {

                            }

                        }
                    }
            }
        }

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


        public void Register(Object source)
        {
            foreach (var tPair in List)
            {
                RegisterUnRegister(false, source, tPair.Key, tPair.Value);
            }
        }

        public void UnRegister(Object source)
        {
            foreach (var tPair in List)
            {
                RegisterUnRegister(true, source, tPair.Key, tPair.Value);
            }
        }

        

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

        public virtual EventBinderTo To
        {
            get { return Child.Value; }
        }

        protected void UpdateLastKey(string value)
        {
            List[_lastKey] = value;
        }

        public virtual object this[string index]
        {
            get
            {
                List[index] = null;
                _lastKey = index;
                return this;
            }
        }

        public class EventBinderTo : EventBinder
        {
            public EventBinderTo(object target, EventBinder parent) : base(target)
            {
                Parent = parent;
            }

            public EventBinder Parent { get; protected set; }

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
