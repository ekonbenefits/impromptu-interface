using System;
using System.Collections.Generic;
using System.Dynamic;
using ImpromptuInterface.Dynamic;
using ImpromptuInterface.Internal.Support;
using System.Reflection;
namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Trampoline object to give access to methods as Commands of original viewmodal
    /// </summary>
    public class ImpromptuCommandBinder : DynamicObject,ICustomTypeProvider
    {
        private readonly object _parent;

        private readonly Dictionary<string, ImpromptuRelayCommand> _commands =
            new Dictionary<string, ImpromptuRelayCommand>();

        private ISetupViewModel _setup;

        internal ImpromptuCommandBinder(object viewModel, ISetupViewModel setup = null)
        {
            _parent = viewModel;
            _setup = setup;
        }


#if SILVERLIGHT5

        /// <summary>
        /// Gets the custom Type.
        /// </summary>
        /// <returns></returns>
        public Type GetCustomType()
        {
            return this.GetDynamicCustomType();
        }
#endif

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
                        result = new ImpromptuRelayCommand(_parent, key, _parent, tCanExecute,_setup);
                    }
                    else
                    {
                        result = new ImpromptuRelayCommand(_parent, key,_setup);
                    }
                    _commands[key] = result;
                }
                return result;
            }
        }
    }
}