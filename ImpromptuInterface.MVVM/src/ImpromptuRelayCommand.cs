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
using System.Reflection;
using System.Text;
using System.Windows.Input;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Command that relays to a target and method name.
    /// </summary>
    [Serializable]
    public class ImpromptuRelayCommand : ICommand
    {
        private readonly object _executeTarget;
        private readonly ISetupViewModel _setup;
        private readonly CacheableInvocation _executeInvoke;
        private readonly CacheableInvocation _executeInvokeNoArg;
        private readonly object _canExecuteTarget;
        private readonly CacheableInvocation _canExecuteInvoke;
        private readonly CacheableInvocation _canExecuteInvokeNoArg;
        private readonly CacheableInvocation _canExecuteInvokeGet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuRelayCommand"/> class.
        /// </summary>
        /// <param name="executeTarget">The execute target.</param>
        /// <param name="executeName">Name of the execute.</param>
        /// <param name="setup">The setup which has the on error event</param>
        public ImpromptuRelayCommand(object executeTarget, String_OR_InvokeMemberName executeName, ISetupViewModel setup =null)
        {
            _executeTarget = executeTarget;
            _setup = setup;
            _executeInvoke = new CacheableInvocation(InvocationKind.InvokeMemberAction, executeName,1);
            _executeInvokeNoArg = new CacheableInvocation(InvocationKind.InvokeMemberAction, executeName, 0); 
        }
       

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuRelayCommand"/> class.
        /// </summary>
        /// <param name="executeTarget">The execute target.</param>
        /// <param name="executeName">Name of the execute method.</param>
        /// <param name="canExecuteTarget">The can execute target.</param>
        /// <param name="canExecuteName">Name of the can execute method.</param>
        public ImpromptuRelayCommand(object executeTarget, String_OR_InvokeMemberName executeName, object canExecuteTarget, String_OR_InvokeMemberName canExecuteName, ISetupViewModel setup = null)
            :this(executeTarget, executeName, setup)
        {
            _canExecuteTarget = canExecuteTarget;
            _canExecuteInvoke = new CacheableInvocation(InvocationKind.InvokeMember, canExecuteName ,1);
            _canExecuteInvokeGet = new CacheableInvocation(InvocationKind.Get,canExecuteName);
            _canExecuteInvokeNoArg = new CacheableInvocation(InvocationKind.InvokeMember, canExecuteName);
        }

        public void Execute(object parameter)
        {
            try
            {
                try
                {
                    if (parameter == null)
                    {
                        //if parameter is null try invoking without argument
                        _executeInvokeNoArg.Invoke(_executeTarget);
                        return;
                    }

                }
                catch (RuntimeBinderException) { /* if it doesn't bind continue */ }
                catch (TargetParameterCountException){}
                _executeInvoke.Invoke(_executeTarget, parameter);
            }
            catch (Exception ex)
            {
                if (_setup == null || !_setup.RaiseCommandErrorHandler(ex))
                {
                    throw;
                }
            }
        }


        public bool CanExecute(object parameter)
        {
            try
            {
                if (_canExecuteTarget == null)
                    return true;


                try //First check if there is a property that matches name
                {
                    dynamic tResult =_canExecuteInvokeGet.Invoke(_canExecuteTarget);
                    if (tResult is bool) //If it is a bool return, if not InvokeMember will handle it
                    {
                        return tResult;
                    }
                }
                catch (RuntimeBinderException){ /* if it doesn't bind continue */}
                try
                {
                    if (parameter == null) //if parameter is null try invoking without argument
                        return (bool)_canExecuteInvokeNoArg.Invoke(_canExecuteTarget);
                }
                catch (RuntimeBinderException){ /* if it doesn't bind continue */ }
                catch (TargetParameterCountException) { }

                return (bool)_canExecuteInvoke.Invoke(_canExecuteTarget, parameter);
                
            }catch (Exception ex)
            {
                if (_setup ==null || !_setup.RaiseCommandErrorHandler(ex))
                {
                    throw;
                }
                return true;
            }
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
