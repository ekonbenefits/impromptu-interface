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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using ImpromptuInterface.Dynamic;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Optimization
{
    /// <summary>
    /// Utility Class
    /// </summary>
    public static class Util
    {


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


        public static Type FixContext(this Type context)
        {
            if (context.IsArray)
            {
                return typeof (object);
            }
            return context;
        }

        internal static bool MassageResultBasedOnInterface(this ImpromptuObject target, string binderName, bool resultFound, ref object result)
        {
            if (result is ImpromptuForwarderAddRemove) //Don't massage AddRemove Proxies
                return true;

            Type tType;
            var tTryType = target.TryTypeForName(binderName, out tType);
            if (tTryType && tType == typeof(void))
            {
                return true;
            }

            if(resultFound){
              if (result is IDictionary<string, object>
                    && !(result is ImpromptuDictionaryBase)
                    && (!tTryType || tType == typeof(object)))
                {
                    result = new ImpromptuDictionary((IDictionary<string, object>)result);
                }
                else if (tTryType)
                {
                    if (result != null && !tType.IsAssignableFrom(result.GetType()))
                    {

                        if (tType.IsInterface)
                        {
                            result = Impromptu.DynamicActLike(result, tType);
                        }
                        else
                        {
                          
                            try
                            {
                                object tResult;

                                tResult = Impromptu.InvokeConvert(target, tType, explict: true);

                                result = tResult;
                            }catch(RuntimeBinderException)
                            {
                                Type tReducedType = tType;
                                if (tType.IsGenericType && tType.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))
                                {
                                    tReducedType = tType.GetGenericArguments().First();
                                }


                                if (result is IConvertible && typeof(IConvertible).IsAssignableFrom(tReducedType))
                                {

                                    result = Convert.ChangeType(result, tReducedType, Thread.CurrentThread.CurrentCulture);

                                }else
                                {  //finally check type converter since it's the slowest.

#if !SILVERLIGHT
                                    var tConverter = TypeDescriptor.GetConverter(tType);
#else
                                    TypeConverter tConverter = null;
                                    var tAttributes = tType.GetCustomAttributes(typeof(TypeConverterAttribute), false);
                                    var tAttribute  =tAttributes.OfType<TypeConverterAttribute>().FirstOrDefault();
                                    if(tAttribute !=null)
                                    {
                                        tConverter =
                                            Impromptu.InvokeConstructor(Type.GetType(tAttribute.ConverterTypeName));
                                    }
#endif
                                    if (tConverter !=null && tConverter.CanConvertFrom(result.GetType()))
                                    {
                                        result = tConverter.ConvertFrom(result);
                                    }
                                }
                            }
                        }
                    }
                    else if (result == null && tType.IsValueType)
                    {
                        result = Impromptu.InvokeConstructor(tType);
                    }
                }
            }
            else
            {
                result = null;
                if (!tTryType)
                {

                    return false;
                }
                if (tType.IsValueType)
                {
                    result = Impromptu.InvokeConstructor(tType);
                }
            }
            return true;
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
		
		static Util(){
			IsMono = Type.GetType ("Mono.Runtime") != null;
		}
		
		public static readonly bool IsMono;
    }
}
