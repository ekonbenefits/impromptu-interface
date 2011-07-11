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
using System.Linq.Expressions;
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

            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                return CurryConverter.TryConvert(this, binder, out result);
            }

           public override bool  TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
           {
               result = new Currying(_target, binder.Name, Util.NameArgsIfNecessary(binder.CallInfo,args), _totalArgCount);
               return true;
           }
            public override bool  TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
                var tCurrying = _target as Currying;

                
                result = tCurrying != null
                             //If already currying append
                             ? new Currying(tCurrying.Target, tCurrying.MemberName,
                                            tCurrying.Args.Concat(Util.NameArgsIfNecessary(binder.CallInfo, args)).
                                                ToArray(), tCurrying.TotalArgCount, tCurrying.InvocationKind)
                             : new Currying(_target, String.Empty, Util.NameArgsIfNecessary(binder.CallInfo, args), _totalArgCount);
                return true;
           }
        }

        internal static class CurryConverter
        {
            internal static IDictionary<Type, Delegate> CompiledExpressions = new Dictionary<Type, Delegate>();

            internal static bool TryConvert(object target, ConvertBinder binder, out object result)
            {
                result = null;
                if (!typeof(Delegate).IsAssignableFrom(binder.Type.BaseType))
                {
                    return false;
                }
                var tDelMethodInfo = binder.Type.GetMethod("Invoke");
                var tReturnType = tDelMethodInfo.ReturnType;
                var tAction = tReturnType == typeof(void);
                var tParams = tDelMethodInfo.GetParameters();
                var tLength = tDelMethodInfo.GetParameters().Length;
                Delegate tBaseDelegate = tAction
                                             ? InvokeHelper.WrapAction(target, tLength)
                                             : InvokeHelper.WrapFunc(tReturnType, target, tLength);


                if (!InvokeHelper.IsActionOrFunc(binder.Type) || tParams.Any(it => it.ParameterType.IsValueType))
                //Conditions that aren't contravariant;
                {
                    Delegate tGetResult;

                    if (!CompiledExpressions.TryGetValue(binder.Type, out tGetResult))
                    {
                        var tParamTypes = tParams.Select(it => it.ParameterType).ToArray();
                        var tDelParam = Expression.Parameter(tBaseDelegate.GetType());
                        var tInnerParams = tParamTypes.Select(Expression.Parameter).ToArray();

                        var tI = Expression.Invoke(tDelParam,
                                                   tInnerParams.Select(it => (Expression)Expression.Convert(it, typeof(object))));
                        var tL = Expression.Lambda(binder.Type, tI, tInnerParams);

                        tGetResult =
                            Expression.Lambda(Expression.GetFuncType(tBaseDelegate.GetType(), binder.Type), tL,
                                              tDelParam).Compile();
                        CompiledExpressions[binder.Type] = tGetResult;
                    }

                    result = tGetResult.DynamicInvoke(tBaseDelegate);

                    return true;
                }
                result = tBaseDelegate;

                return true;
            }
        }

        public class Currying:DynamicObject
        {
           

            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                return CurryConverter.TryConvert(this, binder, out result);
            }

          

            internal Currying(object target, string memberName, object[] args, int? totalCount=null, InvocationKind? invocationKind=null)
            {
                _target = target;
                _memberName = memberName;
                _invocationKind = invocationKind ?? (String.IsNullOrWhiteSpace(_memberName)
                                         ? InvocationKind.InvokeUnknown
                                         : InvocationKind.InvokeMemberUnknown);
                _totalArgCount = totalCount;
                _args = args;
            }

            private readonly int? _totalArgCount;
            private readonly object _target;
            private readonly string _memberName;
            private readonly object[] _args;
            private readonly InvocationKind _invocationKind;

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

            public InvocationKind InvocationKind
            {
                get { return _invocationKind; }
            }

            private CacheableInvocation _cacheableInvocation;

            public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
                var tNamedArgs =Util.NameArgsIfNecessary(binder.CallInfo, args);
                var tNewArgs = _args.Concat(tNamedArgs).ToArray();

                if (_totalArgCount.HasValue && (_totalArgCount - Args.Length - args.Length > 0))
                    //Not Done currying
                {
                    result= new Currying(Target, MemberName, tNewArgs,
                                       TotalArgCount, InvocationKind);

                    return true;
                }
                var tInvokeDirect = String.IsNullOrWhiteSpace(_memberName);
                var tDel = _target as Delegate;

               
                if (tInvokeDirect &&  binder.CallInfo.ArgumentNames.Count ==0 && _target is Delegate)
                    //Optimization for direct delegate calls
                {
                   result= tDel.FastDynamicInvoke(tNewArgs);
                    return true;
                }

               
                Invocation tInvocation;
                if (binder.CallInfo.ArgumentNames.Count == 0) //If no argument names we can cache the callsite
                {
                    if (_cacheableInvocation == null)
                    {
                        

                        _cacheableInvocation = new CacheableInvocation(InvocationKind,_memberName,argCount:tNewArgs.Length,context:_target);
                    }
                    tInvocation = _cacheableInvocation;
                }
                else
                {
                    tInvocation = new Invocation(InvocationKind, _memberName);
                }

                result =tInvocation.Invoke(_target, tNewArgs);


                return true;
            }
        }

}
