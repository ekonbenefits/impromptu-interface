// 
//  Copyright 2010  Ekon Benefits
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

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using ImpromptuInterface.Optimization;
using CSharp = Microsoft.CSharp.RuntimeBinder;
namespace ImpromptuInterface
{
    using System;

    /// <summary>
    /// This interface can be used on your custom dynamic objects if you want impromptu interfaces without casting to object or using the static method syntax of ActLike.
    /// Also if you want to change the behavior for slightly for specific types as this will take precident when using the dynamic keyword or your specific type is known staticly.
    /// </summary>
    public interface IActLike 
    {
        /// <summary>
        /// This interface can be used on your custom dynamic objects if you want impromptu interfaces without casting to object or using the static method syntax of ActLike.
        /// Also if you want to change the behavior for slightly for specific types as this will take precident when using the dynamic keyword or your specific type is known staticly.
        /// </summary>
        ///<param name="otherInterfaces"></param>
        ///<typeparam name="TInterface"></typeparam>
        ///<returns></returns>
        TInterface ActLike<TInterface>(params Type[] otherInterfaces) where TInterface : class;
    }


    /// <summary>
    /// Main API
    /// </summary>
    public static class Impromptu
    {
        private static readonly IDictionary<Tuple<Type, string, Type>, CallSite> _binderCache = new Dictionary<Tuple<Type, string, Type>, CallSite>();
        private static readonly object _binderCacheLock = new object();


        /// <summary>
        /// Creates a cached call site at runtime. 
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="binder">The CallSite binder.</param>
        ///<param name="name">Member Name</param>
        ///<param name="context"> Permissions Context type</param>
        /// <returns>The CallSite</returns>
        /// <remarks>
        ///  Advanced usage only for serious custom dynamic invocation.
        /// </remarks>  
        /// <seealso cref="CreateCallSite{T}"/>
        public static CallSite CreateCallSite(Type delegateType, CallSiteBinder binder, string name, Type context)
        {

            var tHash = Tuple.Create(delegateType, name, context);
            lock (_binderCacheLock)
            {
                CallSite tOut = null;
                if (!_binderCache.TryGetValue(tHash, out tOut))
                {
                    tOut = CallSite.Create(delegateType, binder);
                    _binderCache[tHash] = tOut;
                }
                return tOut;
            }
        }


        /// <summary>
        /// Creates a cached call site at runtime.
        /// </summary>
        /// <typeparam name="T">Delegate Type</typeparam>
        /// <param name="binder">The binder.</param>
        ///<param name="name">Member Name</param>
        ///<param name="context"> Permissions Context type</param>
        ///<returns>The CallSite</returns>
        /// <remarks>
        ///  Advanced usage only for serious custom dynamic invocation.
        /// </remarks>
        /// <example>
        /// Unit test that exhibits usage
        ///<code><![CDATA[
        /// 
        ///    string tResult = String.Empty;
        ///    var tPoco = new MethOutPoco();
        ///    var tBinder =
        ///        Binder.InvokeMember(BinderFlags.None, "Func", null, GetType(),
        ///                                    new[]
        ///                                        {
        ///                                            Info.Create(
        ///                                                InfoFlags.None, null),
        ///                                            Info.Create(
        ///                                                InfoFlags.IsOut |
        ///                                                InfoFlags.UseCompileTimeType, null)
        ///                                        });
        ///
        ///    var tSite = Impromptu.CreateCallSite<DynamicTryString>(tBinder);
        /// 
        ///    tSite.Target.Invoke(tSite, tPoco, out tResult);
        ///
        ///    Assert.AreEqual("success", tResult);
        ///  ]]></code>
        /// </example>
        /// <seealso cref="CreateCallSite"/>
        public static CallSite<T> CreateCallSite<T>(CallSiteBinder binder, string name, Type context) where T: class
        {
            var tHash = Tuple.Create(typeof(T), name, context);
            lock (_binderCacheLock)
            {
                CallSite tOut = null;
                if (!_binderCache.TryGetValue(tHash, out tOut))
                {
                    tOut = CallSite<T>.Create(binder);
                   _binderCache[tHash] = tOut;
                }
                return (CallSite<T>)tOut;
            }
        }



        /// <summary>
        /// Dynamically Invokes a member method using the DLR
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="args">The args.</param>
        /// <returns> The result</returns>
        /// <example>   
        /// Unit test that exhibits usage:
        /// <code>
        /// <![CDATA[
        ///    dynamic tExpando = new ExpandoObject();
        ///    tExpando.Func = new Func<int, string>(it => it.ToString());
        ///
        ///    var tValue = 1;
        ///    var tOut = Impromptu.InvokeMember(tExpando, "Func", tValue);
        ///
        ///    Assert.AreEqual(tValue.ToString(), tOut);
        /// ]]>
        /// </code>
        /// </example>
        public static dynamic InvokeMember(object target, string name, params object[] args)
        {
            
            var tContext = target.GetType();
            var tList = new List<CSharp.CSharpArgumentInfo>()
                            {
                                CSharp.CSharpArgumentInfo.Create(CSharp.CSharpArgumentInfoFlags.None, null)
                            };
            tList.AddRange(args.Select(
                                           it =>
                                           CSharp.CSharpArgumentInfo.Create(CSharp.CSharpArgumentInfoFlags.UseCompileTimeType, null)));
            var tBinder =CSharp.Binder.InvokeMember(CSharp.CSharpBinderFlags.None, name, null,
                                       tContext,tList);


            return InvokeHelper.InvokeMember(tBinder,name,tContext,target,args);
        }



        /// <summary>
        /// Dynamically Invokes a member method which returns void using the DLR
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="args">The args.</param>
        /// <example>
        /// Unit test that exhibits usage:
        /// <code>
        /// <![CDATA[
        ///    var tTest = "Wrong";
        ///    var tValue = "Correct";
        ///
        ///    dynamic tExpando = new ExpandoObject();
        ///    tExpando.Action = new Action<string>(it => tTest = it);
        ///
        ///    Impromptu.InvokeMemberAction(tExpando, "Action", tValue);
        ///
        ///    Assert.AreEqual(tValue, tTest);
        /// ]]>
        /// </code>
        /// </example>
        public static void InvokeMemberAction(object target, string name, params object[] args)
        {


            var tArgs = new List<CSharp.CSharpArgumentInfo>()
                            {
                                CSharp.CSharpArgumentInfo.Create(CSharp.CSharpArgumentInfoFlags.None, null)
                            };
            tArgs.AddRange(args.Select(
                it =>
                CSharp.CSharpArgumentInfo.Create(CSharp.CSharpArgumentInfoFlags.UseCompileTimeType, null)));
            var tContext = target.GetType();
            var tBinder = CSharp.Binder.InvokeMember(CSharp.CSharpBinderFlags.ResultDiscarded, name, null,
                                       tContext,tArgs);

			
   			InvokeHelper.InvokeMemberAction(tBinder, name, tContext,target, args);

        
        }
	

        /// <summary>
        /// Dynamically Invokes a set member using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <example>
        /// Unit test that exhibits usage:
        /// <code>
        /// <![CDATA[
        ///    dynamic tExpando = new ExpandoObject();
        ///
        ///    var tSetValue = "1";
        ///
        ///    Impromptu.InvokeSet(tExpando, "Test", tSetValue);
        ///
        ///    Assert.AreEqual(tSetValue, tExpando.Test);
        /// ]]>
        /// </code>
        /// </example>
        public static void InvokeSet(object target, string name, object value)
        {
            InvokeSetHelper(target, name, (dynamic) value);
        }

        private static void InvokeSetHelper<T>(object target, string name, T  value)
        {
            var tContext = target.GetType();
            var tBinder = CSharp.Binder.SetMember(CSharp.CSharpBinderFlags.ResultDiscarded, name,
                                                  tContext,
                                                  new List<CSharp.CSharpArgumentInfo>()
                                                      {
                                                          CSharp.CSharpArgumentInfo.Create(
                                                              CSharp.CSharpArgumentInfoFlags.None, null),
															CSharp.CSharpArgumentInfo.Create(
				                                 			CSharp.CSharpArgumentInfoFlags.UseCompileTimeType, null)
				
                                                      });

            var tCallSite = CreateCallSite<Action<CallSite, object, T>>(tBinder, name, tContext);
            tCallSite.Target.Invoke(tCallSite, target, value);
        }


        /// <summary>
        /// Dynamically Invokes a get member using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <returns>The result.</returns>
        /// <example>
        /// Unit Test that describes usage
        /// <code>
        /// <![CDATA[
        ///    var tSetValue = "1";
        ///    var tAnon = new { Test = tSetValue };
        ///
        ///    var tOut =Impromptu.InvokeGet(tAnon, "Test");
        ///
        ///    Assert.AreEqual(tSetValue, tOut);
        /// ]]>
        /// </code>
        /// </example>
        public static dynamic InvokeGet(object target, string name)
        {
            var tContext = target.GetType();
            var tBinder = CSharp.Binder.GetMember(CSharp.CSharpBinderFlags.None, name,
                                                  tContext,
                                                  new List<CSharp.CSharpArgumentInfo>()
                                                      {
                                                          CSharp.CSharpArgumentInfo.Create(
                                                              CSharp.CSharpArgumentInfoFlags.None, null)
                                                      });

            var tCallSite = CreateCallSite<Func<CallSite,object,object>>(tBinder, name, tContext);
            return tCallSite.Target.Invoke(tCallSite, target);
        }

	

		public static object FastDynamicInvoke(this Delegate del, params object[] args)
		{
		    if(del.Method.ReturnType == typeof(void)){
				
				InvokeHelper.FastDynamicInvokeAction(del, args);
				return null;
			}
		    return InvokeHelper.FastDynamicInvokeReturn(del, args);
		}

        public static Type GenericDelegateType(int paramCount, bool returnVoid = false)
        {
            var tParamCount = returnVoid ? paramCount : paramCount - 1;
            if (tParamCount > 16)
                throw new ArgumentException(String.Format("{0} only handle at most {1} parameters", returnVoid ? "Action" : "Func", returnVoid ? 16 : 17), "paramCount");
            if(tParamCount < 0)
                throw new ArgumentException(String.Format("{0} must have at least {1} parameter(s)", returnVoid ? "Action" : "Func", returnVoid ? 0 : 1), "paramCount");


            return returnVoid
                ? InvokeHelper.ActionKinds[tParamCount]
                : InvokeHelper.FuncKinds[tParamCount];
        }

        /// <summary>
        /// Dynamically invokes a method determined by the CallSite binder and be given an appropriate delegate type
        /// </summary>
        /// <param name="callSite">The Callsite</param>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        /// <remarks>
        /// Advanced use only. Use this method for serious custom invocation, otherwise there are other convenience methods such as
        /// <see cref="InvokeMember"></see>, <see cref="InvokeGet"></see>, <see cref="InvokeSet"></see> and <see cref="InvokeMemberAction"></see>
        /// </remarks>
        public static dynamic Invoke(CallSite callSite, object target, params object[] args)
        {
         
            
            var tParameters = new List<object>();
            tParameters.Add(callSite);
            tParameters.Add(target);
            tParameters.AddRange(args);

            MulticastDelegate tDelegate = ((dynamic)callSite).Target;

            return tDelegate.FastDynamicInvoke(tParameters.ToArray());
        }


        /// <summary>
        /// Extension Method that Wraps an existing object with an Explicit interface definition
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="originalDynamic">The original object can be annoymous type, System.DynamicObject as well as any others.</param>
        /// <param name="otherInterfaces">Optional other interfaces.</param>
        /// <returns></returns>
        public static TInterface ActLike<TInterface>(this object originalDynamic, params Type[] otherInterfaces) where TInterface : class
        {
            var tType = originalDynamic.GetType();

            var tProxy = BuildProxy.BuildType(tType,typeof(TInterface), otherInterfaces);



            return
                (TInterface)
                InitializeProxy(tProxy, originalDynamic, new[] {typeof (TInterface)}.Concat(otherInterfaces));
        }



        /// <summary>
        /// Makes static methods for the passed in property spec, designed to be used with old api's that reflect properties.
        /// </summary>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="propertySpec">The property spec.</param>
        /// <returns></returns>
        public static dynamic ActLikeProperties(this object originalDynamic, IDictionary<string, Type> propertySpec)
        {
            var tType = originalDynamic.GetType();

            var tProxy = BuildProxy.BuildType(tType, propertySpec);



            return
                InitializeProxy(tProxy, originalDynamic, propertySpec: propertySpec);
        }

        private static object InitializeProxy(Type proxytype, object original, IEnumerable<Type> interfaces =null, IDictionary<string, Type> propertySpec =null)
        {
            var tProxy = (IActLikeProxyInitialize)Activator.CreateInstance(proxytype);
            tProxy.Initialize(original, interfaces, propertySpec);
            return tProxy;
        }

       

        /// <summary>
        /// This Extension method is called off the calling context to perserve permissions with the object wrapped with an explicit interface definition.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="caller">The caller.</param>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        /// <example>
        /// UnitTest That describes usage
        /// <code>
        /// <![CDATA[
        ///     var tTest = new TestWithPrivateMethod();
        ///     var tNonExposed = this.CallActLike<IExposePrivateMethod>(tTest);
        ///     Assert.Throws<RuntimeBinderException>(() => tNonExposed.Test());
        /// ]]>
        /// </code>
        /// </example>
        public static TInterface CallActLike<TInterface>(this object caller, object originalDynamic, params Type[] otherInterfaces) where TInterface : class
        {
            var tType = caller.GetType();

            var tProxy = BuildProxy.BuildType(tType, typeof(TInterface), otherInterfaces);

            return
               (TInterface)
               InitializeProxy(tProxy, originalDynamic, new[] { typeof(TInterface) }.Concat(otherInterfaces));
        }

        /// <summary>
        /// Chainable Linq to Objects Method, allows you to wrap a list of objects with an Explict interface defintion
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        public static IEnumerable<TInterface> AllActLike<TInterface>(this IEnumerable<object> originalDynamic, params Type[] otherInterfaces) where TInterface : class
        {
            return originalDynamic.Select(it => it.ActLike<TInterface>(otherInterfaces));
        }

        /// <summary>
        /// Static Method that wraps an existing dyanmic object with a explicit interface type
        /// </summary>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        public static dynamic DynamicActLike(object originalDynamic, params Type[] otherInterfaces)
        {
            var tType = originalDynamic.GetType();

            var tProxy = BuildProxy.BuildType(tType, otherInterfaces.First(), otherInterfaces.Skip(1).ToArray());

            return InitializeProxy(tProxy, originalDynamic, otherInterfaces);

        }

        /// <summary>
        /// This Extension method is called off the calling context to perserve permissions with the object wrapped with an explicit interface definition.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        public static dynamic CallDynamicActLike(this object caller, object originalDynamic, params Type[] otherInterfaces)
        {
            var tType = caller.GetType();

            var tProxy = BuildProxy.BuildType(tType, otherInterfaces.First(), otherInterfaces.Skip(1).ToArray());

            return InitializeProxy(tProxy, originalDynamic, otherInterfaces);
        }


        /// <summary>
        /// Chainable Linq to Objects Method, allows you to wrap a list of objects, and preserve method permissions with a caller, with an Explict interface defintion
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="caller">The caller.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        public static IEnumerable<TInterface> AllCallActLike<TInterface>(this IEnumerable<object> originalDynamic, object caller, params Type[] otherInterfaces) where TInterface : class
        {
            return originalDynamic.Select(it => caller.CallActLike<TInterface>(it,otherInterfaces));
        }


    }

}
