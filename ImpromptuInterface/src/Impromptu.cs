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
using System.Reflection.Emit;
using ImpromptuInterface.Build;

namespace ImpromptuInterface
{
    using System;

    
    /// <summary>
    /// Main API
    /// </summary>
    public static class Impromptu
    {

        /// <summary>
        /// Extension Method that Wraps an existing object with an Explicit interface definition
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="originalDynamic">The original object can be annoymous type, System.DynamicObject as well as any others.</param>
        /// <param name="otherInterfaces">Optional other interfaces.</param>
        /// <returns></returns>
        public static TInterface ActLike<TInterface>(this object originalDynamic, params Type[] otherInterfaces) where TInterface : class
         => BuildProxy.DefaultProxyMaker.ActLike<TInterface>(originalDynamic, otherInterfaces);


        /// <summary>
        /// Unwraps the act like proxy (if wrapped).
        /// </summary>
        /// <param name="proxiedObject">The proxied object.</param>
        /// <returns></returns>
        public static dynamic UndoActLike(this object proxiedObject)
        {

            var actLikeProxy = proxiedObject as IActLikeProxy;
            if (actLikeProxy != null)
            {
                return actLikeProxy.Original;
            }
            return proxiedObject;
        }


        /// <summary>
        /// Extension Method that Wraps an existing object with an Interface of what it is implicitly assigned to.
        /// </summary>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        public static dynamic ActLike(this object originalDynamic, params Type[] otherInterfaces)
            => BuildProxy.DefaultProxyMaker.ActLike(originalDynamic, otherInterfaces);


        public static TInterface Create<TTarget, TInterface>() where TTarget : new() where TInterface : class 
            => BuildProxy.DefaultProxyMaker.Create<TTarget, TInterface>();

        public static TInterface Create<TTarget, TInterface>(params object[] args) where TInterface : class 
            => BuildProxy.DefaultProxyMaker.Create<TTarget, TInterface>(args);


        /// <summary>
        /// Makes static methods for the passed in property spec, designed to be used with old api's that reflect properties.
        /// </summary>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="propertySpec">The property spec.</param>
        /// <returns></returns>
        public static dynamic ActLikeProperties(this object originalDynamic, IDictionary<string, Type> propertySpec)
         => BuildProxy.DefaultProxyMaker.ActLikeProperties(originalDynamic, propertySpec);


        /// <summary>
        /// Chainable Linq to Objects Method, allows you to wrap a list of objects with an Explict interface defintion
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        public static IEnumerable<TInterface> AllActLike<TInterface>(this IEnumerable<object> originalDynamic, params Type[] otherInterfaces) where TInterface : class 
            => BuildProxy.DefaultProxyMaker.AllActLike<TInterface>(originalDynamic, otherInterfaces);

        /// <summary>
        /// 
        /// Static Method that wraps an existing dynamic object with a explicit interface type
        /// </summary>
        /// <param name="originalDynamic">The original dynamic.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        public static dynamic DynamicActLike(object originalDynamic, params Type[] otherInterfaces)
            => BuildProxy.DefaultProxyMaker.DynamicActLike(originalDynamic, otherInterfaces);



    }

}
