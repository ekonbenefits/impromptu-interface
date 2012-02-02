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
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ImpromptuInterface.Build;
using ImpromptuInterface.Internal;
using ImpromptuInterface.InvokeExt;
using ImpromptuInterface.Optimization;

namespace ImpromptuInterface
{
    using System;


    /// <summary>
    /// Main API
    /// </summary>
    public static class Impromptu
    {


        private static readonly Type ComObjectType;

        private static readonly dynamic ComBinder;

        static Impromptu()
        {
            try
            {
                ComObjectType = typeof(object).Assembly.GetType("System.__ComObject");
                ComBinder = new Dynamic.ImpromptuLateLibraryType(
                "System.Dynamic.ComBinder, System.Dynamic, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            }
            catch
            {
                
            }
        }
        /// <summary>
        /// Creates a cached call site at runtime.
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="binder">The CallSite binder.</param>
        /// <param name="name">Member Name</param>
        /// <param name="context">Permissions Context type</param>
        /// <param name="argNames">The arg names.</param>
        /// <param name="staticContext">if set to <c>true</c> [static context].</param>
        /// <param name="isEvent">if set to <c>true</c> [is event].</param>
        /// <returns>The CallSite</returns>
        /// <remarks>
        /// Advanced usage only for serious custom dynamic invocation.
        /// </remarks>
        /// <seealso cref="CreateCallSite{T}"/>
        public static CallSite CreateCallSite(Type delegateType, CallSiteBinder binder, String_OR_InvokeMemberName name, Type context, string[] argNames = null, bool staticContext = false, bool isEvent =false)
        {

            return InvokeHelper.CreateCallSite(delegateType, binder.GetType(), () => binder, name, context, argNames, staticContext,
                                                  isEvent);
        }

        /// <summary>
        /// Creates the call site.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binder">The binder.</param>
        /// <param name="name">The name.</param>
        /// <param name="context">The context.</param>
        /// <param name="argNames">The arg names.</param>
        /// <param name="staticContext">if set to <c>true</c> [static context].</param>
        /// <param name="isEvent">if set to <c>true</c> [is event].</param>
        /// <returns></returns>
        /// /// 
        /// <example>
        /// Unit test that exhibits usage
        /// <code><![CDATA[
        /// string tResult = String.Empty;
        /// var tPoco = new MethOutPoco();
        /// var tBinder =
        /// Binder.InvokeMember(BinderFlags.None, "Func", null, GetType(),
        /// new[]
        /// {
        /// Info.Create(
        /// InfoFlags.None, null),
        /// Info.Create(
        /// InfoFlags.IsOut |
        /// InfoFlags.UseCompileTimeType, null)
        /// });
        /// var tSite = Impromptu.CreateCallSite<DynamicTryString>(tBinder);
        /// tSite.Target.Invoke(tSite, tPoco, out tResult);
        /// Assert.AreEqual("success", tResult);
        /// ]]></code>
        /// </example>
        /// <seealso cref="CreateCallSite"/>
        public static CallSite<T> CreateCallSite<T>(CallSiteBinder binder, String_OR_InvokeMemberName name, Type context, string[] argNames = null, bool staticContext = false, bool isEvent =false) where T : class
        {
            return InvokeHelper.CreateCallSite<T>(binder.GetType(), () => binder, name, context, argNames, staticContext,
                                                 isEvent);
        }

        /// <summary>
        /// Dynamically Invokes a member method using the DLR
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name. Can be a string it will be implicitly converted</param>
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
        public static dynamic InvokeMember(object target, String_OR_InvokeMemberName name, params object[] args)
        {
            string[] tArgNames;
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);
            args = Util.GetArgsAndNames(args, out tArgNames);
            CallSite tCallSite = null;

            return InvokeHelper.InvokeMemberCallSite(target, name, args, tArgNames, tContext, tStaticContext, ref tCallSite);
        }

        /// <summary>
        /// Invokes the specified target using the DLR;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static dynamic Invoke(object target, params object[] args)
        {
            string[] tArgNames;
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);
            args = Util.GetArgsAndNames(args, out tArgNames);
            CallSite tCallSite = null;

            return InvokeHelper.InvokeDirectCallSite(target, args, tArgNames, tContext, tStaticContext, ref tCallSite);
        }


        /// <summary>
        /// Dynamically Invokes indexer using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="indexes">The indexes.</param>
        /// <returns></returns>
        public static dynamic InvokeGetIndex(object target, params object[] indexes)
        {     
                        string[] tArgNames;
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);
            indexes = Util.GetArgsAndNames( indexes, out tArgNames);
            CallSite tCallSite = null;

            return InvokeHelper.InvokeGetIndexCallSite(target, indexes, tArgNames, tContext, tStaticContext,ref tCallSite);
        }


        /// <summary>
        /// Invokes setindex.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="indexesThenValue">The indexes then value.</param>
        public static void InvokeSetIndex(object target, params object[] indexesThenValue)
        {
            string[] tArgNames;
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);
            indexesThenValue = Util.GetArgsAndNames(indexesThenValue, out tArgNames);

            CallSite tCallSite = null;
            InvokeHelper.InvokeSetIndexCallSite(target, indexesThenValue, tArgNames, tContext, tStaticContext, ref tCallSite);
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
        public static void InvokeMemberAction(object target, String_OR_InvokeMemberName name, params object[] args)
        {
            string[] tArgNames;
            Type tContext;
            bool tStaticContext;

            target = target.GetTargetContext(out tContext, out tStaticContext);
            args = Util.GetArgsAndNames(args, out tArgNames);

            CallSite tCallSite = null;
            InvokeHelper.InvokeMemberActionCallSite(target, name, args, tArgNames, tContext, tStaticContext, ref tCallSite);
        }

        /// <summary>
        /// Invokes the action using the DLR
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        public static void InvokeAction(object target, params object[] args)
        {
            string[] tArgNames;
            Type tContext;
            bool tStaticContext;

            target = target.GetTargetContext(out tContext, out tStaticContext);
            args = Util.GetArgsAndNames(args, out tArgNames);

            CallSite tCallSite = null;
            InvokeHelper.InvokeDirectActionCallSite(target, args, tArgNames, tContext, tStaticContext, ref tCallSite);
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
        /// <remarks>
        /// if you call a static property off a type with a static context the csharp dlr binder won't do it, so this method reverts to reflection
        /// </remarks>
        public static object InvokeSet(object target, string name, object value)
        {
            Type tContext;
            bool tStaticContext;
            target =target.GetTargetContext(out tContext, out tStaticContext);
            tContext = tContext.FixContext();


            CallSite tCallSite =null;
            return InvokeHelper.InvokeSetCallSite(target, name, value, tContext, tStaticContext, ref tCallSite);
        }

        /// <summary>
        /// Invokes the set on the end of a property chain.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyChain">The property chain.</param>
        /// <param name="value">The value.</param>
        public static object InvokeSetChain(object target, string propertyChain, object value)
        {
            var tProperties = propertyChain.Split('.');
            var tGetProperties = tProperties.Take(tProperties.Length - 1);
            var tSetProperty = tProperties.Last();
            var tSetTarget = tGetProperties.Aggregate(target, InvokeGet);
            return InvokeSet(tSetTarget, tSetProperty, value);
        }


      

        private static readonly dynamic _invokeSetAll = new InvokeSetters();
        /// <summary>
        /// Call Like method invokes set on target and a list of property/value. Invoke with dictionary, anonymous type or named arguments.
        /// </summary>
        /// <value>The invoke set all.</value>
        public static dynamic InvokeSetAll
        {
            get { return _invokeSetAll; }
        }

        /// <summary>
        /// Wraps a target to partial apply a method (or target if you can invoke target directly eg delegate).
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="totalArgCount">The total arg count.</param>
        /// <returns></returns>
        public static dynamic Curry(object target, int? totalArgCount=null)
        {
            if (target is Delegate && !totalArgCount.HasValue)
                return Curry((Delegate) target);
            return new Curry(target, totalArgCount);
        }

        /// <summary>
        /// Wraps a delegate to partially apply it.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static dynamic Curry(Delegate target)
        {
            return new Curry(target, target.Method.GetParameters().Length);
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
            Type tContext;
            bool tStaticContext;
            target =target.GetTargetContext(out tContext, out tStaticContext);
            tContext = tContext.FixContext();
            CallSite tSite = null;
            return InvokeHelper.InvokeGetCallSite(target, name, tContext, tStaticContext, ref tSite);
        }

        /// <summary>
        /// Invokes the getter property chain.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyChain">The property chain.</param>
        /// <returns></returns>
        public static dynamic InvokeGetChain(object target, string propertyChain)
        {
            var tProperties =propertyChain.Split('.');
            return tProperties.Aggregate(target, InvokeGet);
        }

        /// <summary>
        /// Determines whether the specified name on target is event. This allows you to know whether to InvokeMemberAction
        ///  add_{name} or a combo of {invokeget, +=, invokeset} and the corresponding remove_{name} 
        /// or a combon of {invokeget, -=, invokeset}
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified target is event; otherwise, <c>false</c>.
        /// </returns>
        public static bool InvokeIsEvent(object target, string name)
        {
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);
            tContext = tContext.FixContext();
            CallSite tCallSite = null;
            return InvokeHelper.InvokeIsEventCallSite(target, name, tContext, ref tCallSite);
        }


        /// <summary>
        /// Invokes add assign with correct behavior for events.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void InvokeAddAssign(object target, string name, object value)
        {
            CallSite callSiteAdd =null;
            CallSite callSiteGet =null;
            CallSite callSiteSet =null;
            CallSite callSiteIsEvent = null;
            Type context;
            bool staticContext;
            target = target.GetTargetContext(out context, out staticContext);

            var args = new[] { value };
            string[] argNames;
            args = Util.GetArgsAndNames(args, out argNames);

            InvokeHelper.InvokeAddAssignCallSite(target, name, args, argNames, context, staticContext, ref callSiteIsEvent, ref callSiteAdd, ref callSiteGet, ref callSiteSet);
        }


        /// <summary>
        /// Invokes subtract assign with correct behavior for events.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void InvokeSubtractAssign(object target, string name, object value)
        {
            Type context;
            bool staticContext;
            target = target.GetTargetContext(out context, out staticContext);

            var args = new[] { value };
            string[] argNames;

            args = Util.GetArgsAndNames(args, out argNames);


            CallSite callSiteIsEvent = null;
            CallSite callSiteRemove = null;
            CallSite callSiteGet = null;
            CallSite callSiteSet = null;


            InvokeHelper.InvokeSubtractAssignCallSite(target, name, args, argNames, context, staticContext, ref callSiteIsEvent, ref callSiteRemove, ref callSiteGet,ref  callSiteSet);
        }

        /// <summary>
        /// Invokes  convert using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="type">The type.</param>
        /// <param name="explict">if set to <c>true</c> [explict].</param>
        /// <returns></returns>
        public static dynamic InvokeConvert(object target, Type type, bool explict =false)
        {
            Type tContext;
            bool tDummy;
            target = target.GetTargetContext(out tContext, out tDummy);

            CallSite tCallSite =null;
            return InvokeHelper.InvokeConvertCallSite(target, explict, type, tContext, ref tCallSite);

        }

        /// <summary>
        /// (Obsolete)Invokes the constructor. misspelling
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        [Obsolete("use InvokeConstructor, this was a spelling mistake")]
                public static dynamic InvokeConstuctor(Type type, params object[] args)
                {
                    return InvokeConstructor(type, args);
                }


        /// <summary>
        /// Invokes the constuctor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static dynamic InvokeConstructor(Type type, params object[] args)
        {
            string[] tArgNames;
            bool tValue = type.IsValueType;
            if (tValue && args.Length == 0)  //dynamic invocation doesn't see constructors of value types
            {
                return Activator.CreateInstance(type);
            }

           args = Util.GetArgsAndNames( args, out tArgNames);
           CallSite tCallSite = null;

           var tContext = type.FixContext();


            return InvokeHelper.InvokeConstructorCallSite(type, tValue, args, tArgNames,tContext, ref tCallSite);
        }


        /// <summary>
        /// FastDynamicInvoke extension method. Runs up to runs up to 20x faster than <see cref="System.Delegate.DynamicInvoke"/> .
        /// </summary>
        /// <param name="del">The del.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
		public static object FastDynamicInvoke(this Delegate del, params object[] args)
		{
		    if(del.Method.ReturnType == typeof(void)){
				
				InvokeHelper.FastDynamicInvokeAction(del, args);
				return null;
			}
		    return InvokeHelper.FastDynamicInvokeReturn(del, args);
		}

        /// <summary>
        /// Given a generic parameter count and whether it returns void or not gives type of Action or Func
        /// </summary>
        /// <param name="paramCount">The param count.</param>
        /// <param name="returnVoid">if set to <c>true</c> [return void].</param>
        /// <returns>Type of Action or Func</returns>
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
        /// Gets the member names of properties. Not all IDynamicMetaObjectProvider have support for this.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="dynamicOnly">if set to <c>true</c> [dynamic only]. Won't add reflected properties</param>
        /// <returns></returns>
        public static IEnumerable<string> GetMemberNames(object target, bool dynamicOnly = false)
        {
            var tList = new List<string>();
            if (!dynamicOnly)
            {
               tList.AddRange(target.GetType().GetProperties().Select(it => it.Name));
            }

            var tTarget = target as IDynamicMetaObjectProvider;
            if (tTarget !=null)
            {
                tList.AddRange(tTarget.GetMetaObject(Expression.Constant(tTarget)).GetDynamicMemberNames());
            }else
            {
               
                if(ComObjectType !=null && ComObjectType.IsInstanceOfType(target))
                {
                    tList.AddRange(ComBinder.GetDynamicDataMemberNames(target));
                }
            }
            return tList;
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
        public static dynamic InvokeCallSite(CallSite callSite, object target, params object[] args)
        {
         
            
            var tParameters = new List<object> {callSite, target};
            tParameters.AddRange(args);

            MulticastDelegate tDelegate = ((dynamic)callSite).Target;

            return tDelegate.FastDynamicInvoke(tParameters.ToArray());
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
        [Obsolete("Use InvokeCallSite instead;")]
        public static dynamic Invoke(CallSite callSite, object target, params object[] args)
        {


            var tParameters = new List<object> { callSite, target };
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
            Type tContext;
            bool tDummy;
            originalDynamic = originalDynamic.GetTargetContext(out tContext, out tDummy);
            tContext = tContext.FixContext();


            var tProxy = BuildProxy.BuildType(tContext, typeof(TInterface), otherInterfaces);



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
            Type tContext;
            bool tDummy;
            originalDynamic = originalDynamic.GetTargetContext(out tContext, out tDummy);
            tContext = tContext.FixContext();

            var tProxy = BuildProxy.BuildType(tContext, propertySpec);



            return
                InitializeProxy(tProxy, originalDynamic, propertySpec: propertySpec);
        }



        /// <summary>
        /// Private helper method that initializes the proxy.
        /// </summary>
        /// <param name="proxytype">The proxytype.</param>
        /// <param name="original">The original.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <param name="propertySpec">The property spec.</param>
        /// <returns></returns>
        internal static object InitializeProxy(Type proxytype, object original, IEnumerable<Type> interfaces =null, IDictionary<string, Type> propertySpec =null)
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
        [Obsolete("Using InvokeContext wrapper to change permission context from target")]
        public static TInterface CallActLike<TInterface>(this object caller, object originalDynamic, params Type[] otherInterfaces) where TInterface : class
        {
            return originalDynamic.WithContext(caller).ActLike<TInterface>(otherInterfaces);
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
            Type tContext;
            bool tDummy;
            originalDynamic = originalDynamic.GetTargetContext(out tContext, out tDummy);
            tContext = tContext.FixContext();

            var tProxy = BuildProxy.BuildType(tContext, otherInterfaces.First(), otherInterfaces.Skip(1).ToArray());

            return InitializeProxy(tProxy, originalDynamic, otherInterfaces);

        }

        /// <summary>
        /// This Extension method is called off the calling context to perserve permissions with the object wrapped with an explicit interface definition.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
       [Obsolete("Using WithContext() wrapper to change permission context from target")]
        public static dynamic CallDynamicActLike(this object caller, object originalDynamic, params Type[] otherInterfaces)
        {

            return DynamicActLike(originalDynamic.WithContext(caller), otherInterfaces);

        }


        /// <summary>
        /// Chainable Linq to Objects Method, allows you to wrap a list of objects, and preserve method permissions with a caller, with an Explict interface defintion
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="caller">The caller.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        [Obsolete("Using WithContext() wrapper to change permission context from target")] 
        public static IEnumerable<TInterface> AllCallActLike<TInterface>(this IEnumerable<object> originalDynamic, object caller, params Type[] otherInterfaces) where TInterface : class
        {
            return originalDynamic.Select(it => it.WithContext(caller).ActLike<TInterface>(otherInterfaces));
        }


    }

}
