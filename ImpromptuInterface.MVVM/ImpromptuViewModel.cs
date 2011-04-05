using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using ImpromptuInterface.Dynamic;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// View Model that uses a Dynamic Implementation to remove boilerplate for Two-Way bound properties and commands to methods
    /// </summary>
    public class ImpromptuViewModel:ImpromptuDictionary
    {
        private Trampoline _trampoline;

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
            get { return _trampoline ?? (_trampoline = new Trampoline(this)); }
        }

        protected class Trampoline : ImpromptuDictionary
        {
            private readonly ImpromptuDictionary _parent;

            public Trampoline(ImpromptuDictionary parent)
            {
                _parent = parent;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (!base.TryGetMember(binder, out result))
                {
                    var tName = binder.Name;
                    var tCanExecute = "Can" + tName;
                    if (_parent.ContainsKey(tCanExecute) || _parent.GetType().GetMethod(tCanExecute) != null)
                    {
                        result = new ImpromptuRelayCommand(_parent, tName, _parent, tCanExecute);
                    }
                    else
                    {
                        result = new ImpromptuRelayCommand(_parent, tName);
                    }
                    this[tName] = result;
                }
                return true;
            }
        }

    
    }
}
