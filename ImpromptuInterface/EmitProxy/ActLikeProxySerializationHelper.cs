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
        public object Original;
        public Type[] Interfaces;
        public Type Context;

        public object GetRealObject(StreamingContext context)
        {
           var tType =BuildProxy.BuildType(Context, Interfaces.First(), Interfaces.Skip(1).ToArray());
           return Impromptu.InitializeProxy(tType, Original, Interfaces);
        }

    }
#endif
}
