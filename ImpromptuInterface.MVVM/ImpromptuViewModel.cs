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

namespace ImpromptuInterface.MVVM
{
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
        
        }

#if !SILVERLIGHT
        protected ImpromptuViewModel(SerializationInfo info, 
           StreamingContext context):base(info,context)
        {
          

        }
#endif


        private ImpropmtuCommands _trampoline;

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

    
    }
}
