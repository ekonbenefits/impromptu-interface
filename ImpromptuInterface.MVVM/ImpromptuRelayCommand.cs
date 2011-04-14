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
using System.Linq;
using System.Text;
using System.Windows.Input;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Command that relays to a target and method name.
    /// </summary>
    [Serializable]
    public class ImpromptuRelayCommand : ICommand
    {
        private readonly object _executeTarget;
        private readonly Invocation _executeInvoke;
        private readonly object _canExecuteTarget;
        private readonly Invocation _canExecuteInvoke;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuRelayCommand"/> class.
        /// </summary>
        /// <param name="executeTarget">The execute target.</param>
        /// <param name="executeName">Name of the execute.</param>
        public ImpromptuRelayCommand(object executeTarget, String_OR_InvokeMemberName executeName)
        {
            _executeTarget = executeTarget;
            _executeInvoke = new Invocation(InvocationKind.InvokeMemberAction, executeName);
        }
       

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuRelayCommand"/> class.
        /// </summary>
        /// <param name="executeTarget">The execute target.</param>
        /// <param name="executeName">Name of the execute method.</param>
        /// <param name="canExecuteTarget">The can execute target.</param>
        /// <param name="canExecuteName">Name of the can execute method.</param>
        public ImpromptuRelayCommand(object executeTarget, string executeName, object canExecuteTarget, string canExecuteName)
            :this(executeTarget, executeName)
        {
            _canExecuteTarget = canExecuteTarget;
            _canExecuteInvoke = new Invocation(InvocationKind.InvokeMember, canExecuteName);
        }

        public void Execute(object parameter)
        {
            _executeInvoke.InvokeWithArgs(_executeTarget, parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteTarget == null)
                return true;
            return (bool)_executeInvoke.InvokeWithArgs(_canExecuteTarget, parameter);
        }


        /// <summary>
        /// Raises the can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }


        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

    }
}
