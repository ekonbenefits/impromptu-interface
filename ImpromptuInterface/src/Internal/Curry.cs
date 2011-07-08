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
            private int? _totalArgCount;
           

            internal Curry(object target, int? totalArgCount=null)
             {
                 _target = target;
                _totalArgCount = totalArgCount;
             }

           public override bool  TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
           {
               result = new Curried(_target, binder.Name, Util.NameArgsIfNecessary(binder.CallInfo,args));
               return true;
           }
            public override bool  TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
                var tCurrying = _target as Curried;

                
                result = tCurrying != null
                             //If already currying append
                             ? new Curried(tCurrying.Target, tCurrying.MemberName,
                                            tCurrying.Args.Concat(Util.NameArgsIfNecessary(binder.CallInfo, args)).
                                                ToArray(), _totalArgCount)
                             : new Curried(_target, String.Empty, Util.NameArgsIfNecessary(binder.CallInfo, args), _totalArgCount);
                return true;
           }
        }

        

        public class Curried:DynamicObject
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



                result = tBaseDelegate;

                return true;
            }

            internal Curried(object target, string memberName, object[] args, int? totalCount=null)
            {
                _target = target;
                _memberName = memberName;
                _totalArgCount = totalCount;
                _args = args;
            }

            private readonly int? _totalArgCount;
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

            public int? TotalArgCount
            {
                get { return _totalArgCount; }
            }


            public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
                var tNamedArgs =Util.NameArgsIfNecessary(binder.CallInfo, args);
                var tNewArgs = _args.Concat(tNamedArgs).ToArray();

                if (_totalArgCount.HasValue && (_totalArgCount - Args.Length - args.Length > 0))
                {
                    result= new Curried(Target, MemberName, tNewArgs,
                                       TotalArgCount);

                    return true;
                }
                var tInvokeDirect = String.IsNullOrWhiteSpace(_memberName);
                var tDel = _target as Delegate;

               
                if (tInvokeDirect && _totalArgCount.HasValue && _target is Delegate)
                {
                   result= tDel.FastDynamicInvoke(tNewArgs);
                    return true;
                }

                var tInvocationKind = String.IsNullOrWhiteSpace(_memberName)
                                          ? InvocationKind.InvokeUnknown
                                          : InvocationKind.InvokeMemberUnknown;

                var tInvocation = new Invocation(tInvocationKind, _memberName);

                

                result =tInvocation.Invoke(_target, tNewArgs);


                return true;
            }
        }

}
