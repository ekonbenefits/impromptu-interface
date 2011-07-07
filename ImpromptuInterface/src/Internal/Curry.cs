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
using System.Text;
using ImpromptuInterface.Dynamic;
using ImpromptuInterface.Optimization;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Internal
{
    public class Curry:DynamicObject
        {
            private readonly object _target;

            internal Curry(object target)
             {
                 _target = target;
             }

           public override bool  TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
           {
               result = new Currying(_target, binder.Name, Util.NameArgsIfNecessary(binder.CallInfo,args));
               return true;
           }
            public override bool  TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
                var tCurrying = _target as Currying;
                result = tCurrying != null
                             //If already currying append
                             ? new Currying(tCurrying.Target, tCurrying.MemberName,
                                            tCurrying.Args.Concat(Util.NameArgsIfNecessary(binder.CallInfo, args)).
                                                ToArray())
                             : new Currying(_target, String.Empty, Util.NameArgsIfNecessary(binder.CallInfo, args));
                return true;
           }
        }

        

        public class Currying:DynamicObject
        {
            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                 result = null;
 	             if(!binder.Explicit || !typeof (Delegate).IsAssignableFrom(binder.Type.BaseType))
 	             {
 	                 return false;
 	             }
                var tDelMethodInfo = binder.Type.GetMethod("Invoke");
                var tReturnType = tDelMethodInfo.ReturnType;
                var tAction = tReturnType == typeof (void);
                var tLength =tDelMethodInfo.GetParameters().Length;
                Delegate tBaseDelegate = tAction
                    ? InvokeHelper.WrapAction(this, tLength)
                    : InvokeHelper.WrapFunc(tReturnType, this, tLength);
                
                result =Delegate.CreateDelegate(binder.Type, tBaseDelegate.Method);
                return true;
            }


            private readonly object _target;
            private readonly string _memberName;
            private readonly object[] _args;

            public object Target
            {
                get { return _target; }
            }

            public string MemberName
            {
                get { return _memberName; }
            }

            public object[] Args
            {
                get { return _args; }
            }

            internal Currying(object target,string memberName, params object[] args)
            {
                _target = target;
                _memberName = memberName;
                _args = args;
            }

            public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
            
                var tInvocationKind = String.IsNullOrWhiteSpace(_memberName)
                                          ? InvocationKind.InvokeUnknown
                                          : InvocationKind.InvokeMemberUnknown;

                var tInvocation = new Invocation(tInvocationKind, _memberName);

                var tNamedArgs =Util.NameArgsIfNecessary(binder.CallInfo, args);

                result =tInvocation.Invoke(_target, _args.Concat(tNamedArgs).ToArray());


                return true;
            }
        }

}
