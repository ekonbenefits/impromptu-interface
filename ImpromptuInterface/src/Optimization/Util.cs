// 
//  Copyright 2011  Ekon Benefits
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
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using Dynamitey;
using Microsoft.CSharp.RuntimeBinder;



namespace ImpromptuInterface.Optimization
{
    /// <summary>
    /// Utility Class
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Determines whether [is anonymous type] [the specified target].
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        /// 	<c>true</c> if [is anonymous type] [the specified target]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAnonymousType(object target)
        {
            if(target ==null)
                return false;

            var type = target as Type ?? target.GetType();

            return type.IsNotPublic
                   && Attribute.IsDefined(
                       type,
                       typeof (CompilerGeneratedAttribute),
                       false);
        }

        static Util()
        {
            IsMono = Type.GetType("Mono.Runtime") != null;

        }

  

        /// <summary>
        /// Gets the target context.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="context">The context.</param>
        /// <param name="staticContext">if set to <c>true</c> [static context].</param>
        /// <returns></returns>
        public static object GetTargetContext(this object target, out Type context, out bool staticContext)
        {
            var tInvokeContext = target as InvokeContext;
            staticContext = false;
            if (tInvokeContext != null)
            {
                staticContext = tInvokeContext.StaticContext;
                context = tInvokeContext.Context;
                context = context.FixContext();
                return tInvokeContext.Target;
            }

            context = target as Type ?? target.GetType();
            context = context.FixContext();
            return target;
        }


        /// <summary>
        /// Fixes the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static Type FixContext(this Type context)
        {
            if (context.IsArray)
            {
                return typeof (object);
            }
            return context;
        }
        


#if !SILVERLIGHT
        /// <summary>
        /// Gets the value. Conveinence Ext method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info">The info.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T GetValue<T>(this SerializationInfo info, string name)
        {
            return (T) info.GetValue(name, typeof (T));
        }
#endif
		
	
		
		/// <summary>
		/// Is Current Runtime Mono?
		/// </summary>
		public static readonly bool IsMono;

 
    }
}
