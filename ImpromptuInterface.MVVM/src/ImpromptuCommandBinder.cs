using System;
using System.Collections.Generic;
using System.Dynamic;
using ImpromptuInterface.Dynamic;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Trampoline object to give access to methods as Commands of original viewmodal
    /// </summary>
    public class ImpromptuCommandBinder : DynamicObject
    {
        private readonly object _parent;

        private readonly Dictionary<string, ImpromptuRelayCommand> _commands =
            new Dictionary<string, ImpromptuRelayCommand>();


        internal ImpromptuCommandBinder(object viewModel)
        {
            _parent = viewModel;
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

                    var tCanExecute = string.Format("Can{0}", key);

                    var tDictParent = _parent as IDictionary<string, object>;
                    if ((tDictParent != null && tDictParent.ContainsKey(tCanExecute))
                        || _parent.GetType().GetMethod(tCanExecute) != null)
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