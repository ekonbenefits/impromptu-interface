using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using ImpromptuInterface.Dynamic;

namespace ImpromptuInterface.Optimization
{
    /// <summary>
    /// Utility Class
    /// </summary>
    public static class Util
    {


        internal static bool MassageResultBasedOnInterface(this ImpromptuObject target, string binderName, bool resultFound, ref object result)
        {
            Type tType;
            var tTryType = target.TryTypeForName(binderName, out tType);
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
                       if (tType.IsGenericType && tType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                       {
                           tType = tType.GetGenericArguments().First();
                       }


                       result = Convert.ChangeType(result, tType, Thread.CurrentThread.CurrentCulture);
                    }
                    else if (result == null && tType.IsValueType)
                    {
                        result = Activator.CreateInstance(tType);
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
                    result = Activator.CreateInstance(tType);
                }
            }
            return true;
        }

        internal static object InvokeMethodDelegate(this object target, Delegate tFunc, object[] args)
        {
            object result;
            try
            {
                result = tFunc.FastDynamicInvoke(
                    tFunc.IsSpecialThisDelegate()
                        ? new[] { target }.Concat(args).ToArray()
                        : args
                    );
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw ex;
            }
            return result;
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
		
		public static bool IsMono{
			get{
				return Type.GetType ("Mono.Runtime") != null;
			}
		}
    }
}
