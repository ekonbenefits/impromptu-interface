using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
        internal abstract class EventHandlerHash
        {

        }

        internal class EventHandlerHash<T>:EventHandlerHash
        {
            private readonly string _name;

            public EventHandlerHash(string name)
                {
                    _name = name;
                }

            public bool Equals(EventHandlerHash<T> other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other._name, _name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (EventHandlerHash<T>)) return false;
                return Equals((EventHandlerHash<T>) obj);
            }

            public override int GetHashCode()
            {
                return (_name != null ? _name.GetHashCode() : 0);
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

        public static EventHandler<T> GenerateEventHandler<T>(string memberName) where T: EventArgs
        {
            var tHash = new EventHandlerHash<T>(memberName);

            object tReturn;

            lock (_eventHandlerStoreLock)
            {
                if (!_eventHandlerStore.TryGetValue(tHash, out tReturn))
                {
                    tReturn = new EventHandler<T>((sender, e) =>
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
                                                                                                   memberName, tSender, e);
                                                                  }
                                                                  catch (RuntimeBinderException)
                                                                  {

                                                                  }

                                                              }
                                                          }
                                                      });
                }
            }

            return (EventHandler<T>)tReturn;
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
                Type[] tArgType = null;
                var tPlainEventHandler = tEvent.EventHandlerType == typeof (EventHandler);
                if (tPlainEventHandler)
               {
                   tArgType = new[]{typeof(EventArgs)};
               }
               else if (tEvent.EventHandlerType.IsGenericType && tEvent.EventHandlerType.GetGenericTypeDefinition() == typeof(EventHandler<>))
               {
                   tArgType = tEvent.EventHandlerType.GetGenericArguments();
               }

                if(tArgType !=null){
         

                  object tEventHandler =  Impromptu.InvokeMember(
                       typeof (Event).WithStaticContext(),
                       "GenerateEventHandler".WithGenericArgs(tArgType),
                       targetName ?? string.Format("On{0}", eventName));

                  if (tPlainEventHandler)
                  {
                      tEventHandler = Event.FixEventHandler((EventHandler<EventArgs>) tEventHandler);
                  }
                  if (un)
                    Impromptu.InvokeSubtractAssign(source, eventName, tEventHandler);
                  else
                    Impromptu.InvokeAddAssign(source,eventName, tEventHandler);
               }

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
