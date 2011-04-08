using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ImpromptuInterface.Build
{
#if !SILVERLIGHT



    /// <summary>
    /// Support Deserializing the proxy since on separate runs of an executable
    /// </summary>
    [Serializable]
    public class ActLikeProxySerializationHelper : IObjectReference 
    {
        private object Original;
        private Type[] Interfaces;
        private Type Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActLikeProxySerializationHelper"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public ActLikeProxySerializationHelper(SerializationInfo info, 
           StreamingContext context)
        {
            Original = info.GetValue("Original", typeof (Object));
            Interfaces = (Type[]) info.GetValue("Interfaces", typeof (Type).MakeArrayType());
            Context = (Type) info.GetValue("Context", typeof (Type));
        }


        public object GetRealObject(StreamingContext context)
        {
            var tType =BuildProxy.BuildType(Context, Interfaces.First(), Interfaces.Skip(1).ToArray());
           return Impromptu.InitializeProxy(tType, Original, Interfaces);
        }

    }
#endif
}
