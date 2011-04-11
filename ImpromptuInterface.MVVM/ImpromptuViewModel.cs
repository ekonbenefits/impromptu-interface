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
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    public class ImpromptuViewModel<TInterface>:ImpromptuViewModel where TInterface:class 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuViewModel&lt;TInterface&gt;"/> class.
        /// </summary>
        public ImpromptuViewModel()
        {
            _static = Impromptu.ActLike<TInterface>(this);
        }

#if !SILVERLIGHT
        protected ImpromptuViewModel(SerializationInfo info,
           StreamingContext context)
            : base(info, context)
        {
            _static = Impromptu.ActLike<TInterface>(this);
        }
#endif
        private readonly TInterface _static;

        /// <summary>
        /// Convenient access to Dynamic Properties but represented by a Static Interface.
        ///  When subclassing you can use Static.PropertyName = x, etc
        /// </summary>
        /// <value>The static.</value>
        public TInterface Static
        {
            get { return _static; }
        }
    }


    /// <summary>
    /// View Model that uses a Dynamic Implementation to remove boilerplate for Two-Way bound properties and commands to methods
    /// </summary>
    public class ImpromptuViewModel:ImpromptuDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuViewModel"/> class.
        /// </summary>
        public ImpromptuViewModel()
        {
            _linkedProperties = new Dictionary<string, List<string>>();
        }

#if !SILVERLIGHT
        protected ImpromptuViewModel(SerializationInfo info, 
           StreamingContext context):base(info,context)
        {
            _linkedProperties = info.GetValue <IDictionary<string, List<string>>> ("_linkedProperties");
        }


        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_linkedProperties", _linkedProperties);
        }
#endif


        private ImpropmtuCommands _trampoline;
        private PropertyDepends _depTrampoline;
        protected readonly IDictionary<string, List<string>> _linkedProperties;

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
            get { return _trampoline ?? (_trampoline = new ImpropmtuCommands(this)); }
        }


        /// <summary>
        /// Sets up dependency relations amoung dependenant properties
        /// </summary>
        /// <value>The dependencies.</value>
        public dynamic Dependencies
        {
            get {
                return _depTrampoline ?? (_depTrampoline = new PropertyDepends(this));
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
            if(!_linkedProperties.TryGetValue(dependency,out tList))
            {
                tList = new List<string>();
                _linkedProperties[dependency] = tList;
            }
            tList.Add(property);
        }


        protected virtual void OnPropertyChanged(string key, HashSet<string> alreadyRaised)
        {
            if (!alreadyRaised.Contains(key))
            {
                base.OnPropertyChanged(key);

                alreadyRaised.Add(key);

                List<string> tList;
                if (!_linkedProperties.TryGetValue(key, out tList)) return;
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

        /// <summary>
        /// Trampoline object to choose property
        /// </summary>
        public class PropertyDepends : DynamicObject
        {
            private readonly ImpromptuViewModel _parent;

            internal PropertyDepends(ImpromptuViewModel parent)
            {
                _parent = parent;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return _parent._linkedProperties.SelectMany(it => it.Value).Distinct();
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = new DependsOn(_parent, binder.Name);

                return true;
            }
        }

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
                return _parent._linkedProperties.Where(it => it.Value.Contains(_property)).Select(it=>it.Key);
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
        }

        /// <summary>
        /// Trampoline object to give access to methods as Commands of original viewmodal
        /// </summary>
        public class ImpropmtuCommands : DynamicObject
        {
            private readonly ImpromptuDictionary _parent;

            private readonly Dictionary<string, ImpromptuRelayCommand> _commands =
                new Dictionary<string, ImpromptuRelayCommand>();


            internal ImpropmtuCommands(ImpromptuDictionary parent)
            {
                _parent = parent;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = this[binder.Name];
                return true;
            }

            /// <summary>
            /// Gets the <see cref="ImpromptuInterface.MVVM.ImpromptuRelayCommand"/> with the specified key.
            /// </summary>
            /// <value></value>
            public ImpromptuRelayCommand this[String key]
            {
                get
                {
                    ImpromptuRelayCommand result;

                    if (!_commands.TryGetValue(key, out result))
                    {

                        var tCanExecute = "Can" + key;
                        if (_parent.ContainsKey(tCanExecute) || _parent.GetType().GetMethod(tCanExecute) != null)
                        {
                            result = new ImpromptuRelayCommand(_parent, key, _parent, tCanExecute);
                        }
                        else
                        {
                            result = new ImpromptuRelayCommand(_parent, key);
                        }
                        _commands[key] = result;
                    }
                    return result;
                }
            }
        }

        #endregion
    }
}
