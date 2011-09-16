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
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ImpromptuInterface.Dynamic;
using ImpromptuInterface.Optimization;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// View Model that uses a Dynamic Implementation to remove boilerplate for Two-Way bound properties and commands to methods. 
    /// If you specific a TInterface it provides a guide to the dynamic properties
    /// </summary>
    /// <typeparam name="TInterfaceContract">The type of the interface.</typeparam>
    [Serializable]
    public class ImpromptuViewModel<TInterfaceContract> : ImpromptuViewModel where TInterfaceContract : class 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuViewModel&lt;TInterface&gt;"/> class.
        /// </summary>
        public ImpromptuViewModel()
        {
            _contract = Impromptu.ActLike<TInterfaceContract>(this, typeof(INotifyPropertyChanged));
     
        }

#if !SILVERLIGHT
        protected ImpromptuViewModel(SerializationInfo info,
           StreamingContext context)
            : base(info, context)
        {
            _contract = Impromptu.ActLike<TInterfaceContract>(this);
        }
#endif
        private readonly TInterfaceContract _contract;

        /// <summary>
        /// Convenient access to Dynamic Properties but represented by a Static Interface.
        ///  When subclassing you can use Static.PropertyName = x, etc
        /// </summary>
        /// <value>The static.</value>
        [Obsolete("Use Contract Property instead")]
        public TInterfaceContract Static
        {
            get { return _contract; }
        }

        /// <summary>
        /// Convenient access to Dynamic Properties but represented by a Static Interface.
        ///  When subclassing you can use Contract.PropertyName = x, etc
        /// </summary>
        /// <value>The contract interface.</value>
        public TInterfaceContract Contract
        {
            get { return _contract; }
        }
    }


    /// <summary>
    /// View Model that uses a Dynamic Implementation to remove boilerplate for Two-Way bound properties and commands to methods
    /// </summary>
    [Serializable]
    public partial class ImpromptuViewModel:ImpromptuDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuViewModel"/> class.
        /// </summary>
        public ImpromptuViewModel()
        {
            LinkedProperties = new Dictionary<string, List<string>>();
        }

#if !SILVERLIGHT
        protected ImpromptuViewModel(SerializationInfo info, 
           StreamingContext context):base(info,context)
        {
            LinkedProperties = info.GetValue <IDictionary<string, List<string>>> ("_linkedProperties");
        }


        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("LinkedProperties", LinkedProperties);
        }
#endif


        private ImpromptuCommandBinder _commandTrampoline;
        private PropertyDepends _dependencyTrampoline;
        private FireOnPropertyChanged _onChangedTrampoline;

        protected readonly IDictionary<string, List<string>> LinkedProperties;
        private object _dependTrampoline;

        /// <summary>
        /// Convenient access to Dynamic Properties. When subclassing you can use Dynamic.PropertyName = x, etc.
        /// </summary>
        /// <value>The command.</value>
        protected dynamic Dynamic
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the command for binding. usage: {Binding Command.MethodName} for <code>void MethodName(object parmeter)</code> and optionally <code>bool CanMethodName(object parameter)</code>.
        /// </summary>
        /// <value>The command.</value>
        public virtual dynamic Command
        {
            get { return _commandTrampoline ?? (_commandTrampoline = new ImpromptuCommandBinder(this)); }
        }

        /// <summary>
        /// Gets the EventBinder to bind events to this model.
        /// </summary>
        /// <value>The events.</value>
        public virtual dynamic Events
        {
            get { return new EventBinder(this); }
        }

        /// <summary>
        /// Sets up dependency relations amoung dependenant properties
        /// </summary>
        /// <value>The dependencies.</value>
        [Obsolete("Use Depend instead")]
        public dynamic Dependencies
        {
            get {
                return _dependencyTrampoline ?? (_dependencyTrampoline = new PropertyDepends(this));
            }
        }

        public dynamic Depend
        {
            get { return _dependTrampoline ?? (_dependTrampoline = new PropertyDepend(this)); }
        }


        /// <summary>
        /// Properties the changed.
        /// </summary>
        /// <param name="delegate">The @delegate.</param>
        /// <returns></returns>
        public static PropertyChangedEventHandler ChangedHandler(PropertyChangedEventHandler @delegate)
        {
            return @delegate;
        }

        /// <summary>
        /// Subscribe to OnProeprtyChanged notififcations of specific properties
        /// </summary>
        /// <value>The on changed.</value>
        public dynamic OnChanged
        {
            get
            {
                return _onChangedTrampoline ?? (_onChangedTrampoline = new FireOnPropertyChangedDependencyAware(this));
            }
        }

        /// <summary>
        /// Links a property to a dependency.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="dependency">To.</param>
        public void DependencyLink(string property, string dependency)
        {
            List<string> tList;
            if(!LinkedProperties.TryGetValue(dependency,out tList))
            {
                tList = new List<string>();
                LinkedProperties[dependency] = tList;
            }
            if(!tList.Contains(property))
                tList.Add(property);
        }

        /// <summary>
        /// Unlinks a dependencies.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="dependency">The dependency.</param>
        public void DependencyUnlink(string property, string dependency)
        {
            List<string> tList;
            if (LinkedProperties.TryGetValue(dependency, out tList))
            {
                tList.Remove(property);
            }
        }


        protected virtual void OnPropertyChanged(string key, HashSet<string> alreadyRaised)
        {
            if (!alreadyRaised.Contains(key))
            {
                base.OnPropertyChanged(key);

                alreadyRaised.Add(key);

                List<string> tList;
                if (!LinkedProperties.TryGetValue(key, out tList)) return;
                foreach (var tKey in tList.Distinct())
                {
                    OnPropertyChanged(tKey, alreadyRaised);
                }
            }
        }


   

    

        protected override void OnPropertyChanged(string key)
        {
            OnPropertyChanged(key, new HashSet<string>());
        }

        #region Trampoline Classes

      


        #endregion
    }
}
