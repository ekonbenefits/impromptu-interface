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
using ImpromptuInterface.Dynamic;
using ImpromptuInterface.Optimization;

namespace ImpromptuInterface.Internal
{
    /// <summary>
    /// Internal Implementation of <see cref="Impromptu.Curry(object,System.Nullable{int})"/>
    /// </summary>
    public class Curry:DynamicObject
        {
            private readonly object _target;
            private readonly int? _totalArgCount;
           

            internal Curry(object target, int? totalArgCount=null)
             {
                 _target = target;
                _totalArgCount = totalArgCount;
             }

            /// <summary>
            /// Provides implementation for type conversion operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations that convert an object from one type to another.
            /// </summary>
            /// <param name="binder">Provides information about the conversion operation. The binder.Type property provides the type to which the object must be converted. For example, for the statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Type returns the <see cref="T:System.String"/> type. The binder.Explicit property provides information about the kind of conversion that occurs. It returns true for explicit conversion and false for implicit conversion.</param>
            /// <param name="result">The result of the type conversion operation.</param>
            /// <returns>
            /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
            /// </returns>
            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                return CurryConverter.TryConvert(this, binder, out result);
            }

            /// <summary>
            /// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as calling a method.
            /// </summary>
            /// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
            /// <param name="args">The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="args"/>[0] is equal to 100.</param>
            /// <param name="result">The result of the member invocation.</param>
            /// <returns>
            /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
            /// </returns>
           public override bool  TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
           {
               result = new Currying(_target, binder.Name, Util.NameArgsIfNecessary(binder.CallInfo,args), _totalArgCount);
               return true;
           }
           /// <summary>
           /// Provides the implementation for operations that invoke an object. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as invoking an object or a delegate.
           /// </summary>
           /// <param name="binder">Provides information about the invoke operation.</param>
           /// <param name="args">The arguments that are passed to the object during the invoke operation. For example, for the sampleObject(100) operation, where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="args"/>[0] is equal to 100.</param>
           /// <param name="result">The result of the object invocation.</param>
           /// <returns>
           /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
           /// </returns>
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
            internal static readonly IDictionary<Type, Delegate> CompiledExpressions = new Dictionary<Type, Delegate>();

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

        /// <summary>
        /// Internal method for subsequent invocations of <see cref="Impromptu.Curry(object,System.Nullable{int})"/>
        /// </summary>
        public class Currying:DynamicObject
        {


            /// <summary>
            /// Provides implementation for type conversion operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations that convert an object from one type to another.
            /// </summary>
            /// <param name="binder">Provides information about the conversion operation. The binder.Type property provides the type to which the object must be converted. For example, for the statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Type returns the <see cref="T:System.String"/> type. The binder.Explicit property provides information about the kind of conversion that occurs. It returns true for explicit conversion and false for implicit conversion.</param>
            /// <param name="result">The result of the type conversion operation.</param>
            /// <returns>
            /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
            /// </returns>
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

            /// <summary>
            /// Gets the target.
            /// </summary>
            /// <value>The target.</value>
            public object Target
            {
                get { return _target; }
            }

            /// <summary>
            /// Gets the name of the member.
            /// </summary>
            /// <value>The name of the member.</value>
            public string MemberName
            {
                get { return _memberName; }
            }

            /// <summary>
            /// Gets the args.
            /// </summary>
            /// <value>The args.</value>
            public object[] Args
            {
                get { return _args; }
            }

            /// <summary>
            /// Gets the total arg count.
            /// </summary>
            /// <value>The total arg count.</value>
            public int? TotalArgCount
            {
                get { return _totalArgCount; }
            }

            /// <summary>
            /// Gets the kind of the invocation.
            /// </summary>
            /// <value>The kind of the invocation.</value>
            public InvocationKind InvocationKind
            {
                get { return _invocationKind; }
            }

            private CacheableInvocation _cacheableInvocation;

            /// <summary>
            /// Provides the implementation for operations that invoke an object. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as invoking an object or a delegate.
            /// </summary>
            /// <param name="binder">Provides information about the invoke operation.</param>
            /// <param name="args">The arguments that are passed to the object during the invoke operation. For example, for the sampleObject(100) operation, where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="args[0]"/> is equal to 100.</param>
            /// <param name="result">The result of the object invocation.</param>
            /// <returns>
            /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
            /// </returns>
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
