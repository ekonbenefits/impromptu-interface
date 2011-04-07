using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ImpromptuInterface.Build
{
#if SILVERLIGHT
    public class SerializableAttribute:Attribute{
    }
    public interface IObjectReference {}
    public interface ISerializable {}
#else



    [Serializable]
    public class ActLikeProxySerializationHelper : IObjectReference 
    {
        private object Original;
        private Type[] Interfaces;
        private Type Context;

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
           return Impromptu.InitializeProxy(tType, Original);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
#endif
}
