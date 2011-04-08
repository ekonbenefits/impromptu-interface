using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ImpromptuInterface.Optimization
{
    /// <summary>
    /// Utility Class
    /// </summary>
    public static class Util
    {
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
