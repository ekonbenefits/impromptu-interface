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
using System.Linq;
using System.Runtime.CompilerServices;
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
        TInterface ActLike<TInterface>(params Type[] otherInterfaces) where TInterface : class;
    }


    public static class Impromptu
    {


        public static dynamic InvokeMember(object target, string name, params object[] args)
        {
            var tArgTypes = args.Select(it => it.GetType()).ToArray();

            var tBinder =CSharp.Binder.InvokeMember(CSharp.CSharpBinderFlags.None, name, null,
                                       target.GetType(),
                                       new[]{CSharp.CSharpArgumentInfo.Create(CSharp.CSharpArgumentInfoFlags.None,null)}.Concat(args.Select(
                                           it =>
                                           CSharp.CSharpArgumentInfo.Create(CSharp.CSharpArgumentInfoFlags.UseCompileTimeType, null))));


            var tList = new List<Type>();
            tList.Add(typeof(CallSite));
            tList.Add(typeof(object));
            tList.AddRange(tArgTypes);
            tList.Add(typeof(object));

            var tDelagateType = BuildProxy.GenericDelegateType(tList.Count).MakeGenericType(tList.ToArray());

            return Invoke(tBinder, tDelagateType, target, args);
        }

        public static void InvokeMemberAction(object target, string name, params object[] args)
        {
            var tArgTypes = args.Select(it => it.GetType()).ToArray();

            var tBinder = CSharp.Binder.InvokeMember(CSharp.CSharpBinderFlags.ResultDiscarded, name, null,
                                       target.GetType(),
                                       new[] { CSharp.CSharpArgumentInfo.Create(CSharp.CSharpArgumentInfoFlags.None, null) }.Concat(args.Select(
                                           it =>
                                           CSharp.CSharpArgumentInfo.Create(CSharp.CSharpArgumentInfoFlags.UseCompileTimeType, null))));


            var tList = new List<Type>();
            tList.Add(typeof(CallSite));
            tList.Add(typeof(object));
            tList.AddRange(tArgTypes);

            var tDelagateType = BuildProxy.GenericDelegateType(tList.Count, action:true).MakeGenericType(tList.ToArray());

            Invoke(tBinder, tDelagateType, target, args);
        }

        public static void InvokeSet(object target, string name, object value)
        {
       

            var tBinder = CSharp.Binder.SetMember(CSharp.CSharpBinderFlags.None, name,
                                                  target.GetType(),
                                                  new[]
                                                      {
                                                          CSharp.CSharpArgumentInfo.Create(
                                                              CSharp.CSharpArgumentInfoFlags.None, null)
                                                      });


            var tList = new [] {typeof (CallSite), typeof (object), value.GetType()};

            var tDelagateType = BuildProxy.GenericDelegateType(tList.Length,action:true).MakeGenericType(tList);

            Invoke(tBinder, tDelagateType, target, value);
        }

        public static dynamic InvokeGet(object target, string name)
        {


            var tBinder = CSharp.Binder.GetMember(CSharp.CSharpBinderFlags.None, name,
                                                  target.GetType(),
                                                  new[]
                                                      {
                                                          CSharp.CSharpArgumentInfo.Create(
                                                              CSharp.CSharpArgumentInfoFlags.None, null)
                                                      });


            var tFuncGenParameters = new [] {typeof (CallSite), typeof (object), typeof (object)};

            var tDelagateType = BuildProxy.GenericDelegateType(tFuncGenParameters.Length).MakeGenericType(tFuncGenParameters);

            return Invoke(tBinder, tDelagateType, target);
        }

        public static CallSite<T> CallSiteCreateCached<T>(CallSiteBinder binder) where T:class 
        {
            return (CallSite<T>)CallSite.Create(typeof(T), binder);
        }

        public static dynamic Invoke(CallSiteBinder binder, Type delegateType, object target, params object[] args)
        {
            dynamic callSite = CallSite.Create(delegateType, binder);

            var tParameters = new List<object>();
            tParameters.Add(callSite);
            tParameters.Add(target);
            tParameters.AddRange(args);

            return ((Delegate)callSite.Target).DynamicInvoke(tParameters.ToArray());
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

            return (TInterface)Activator.CreateInstance(tProxy, originalDynamic, new[] { typeof(TInterface) }.Concat(otherInterfaces).ToArray());
        }

        /// <summary>
        /// This Extension method is called off the calling context to perserve permissions with the object wrapped with an explicit interface definition.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="caller">The caller.</param>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        public static TInterface CallActLike<TInterface>(this object caller, object originalDynamic, params Type[] otherInterfaces) where TInterface : class
        {
            var tType = caller.GetType();

            var tProxy = BuildProxy.BuildType(tType, typeof(TInterface), otherInterfaces);

            return (TInterface)Activator.CreateInstance(tProxy, originalDynamic, new[] { typeof(TInterface) }.Concat(otherInterfaces).ToArray());
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

            return Activator.CreateInstance(tProxy, originalDynamic, otherInterfaces);
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

            return Activator.CreateInstance(tProxy, originalDynamic, otherInterfaces);
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
