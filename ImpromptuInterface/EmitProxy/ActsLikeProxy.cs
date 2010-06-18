using System;
using System.Collections.Generic;

namespace ImpromptuInterface
{
    /// <summary>
    /// This interface can be used to access the original content of your emitted type;
    /// </summary>
    public interface IActLikeProxy
    {
        dynamic Original { get; }
    }

    /// <summary>
    /// This interface can be used on your custom dynamic objects if you want to know the interface you are implementing
    /// </summary>
    public interface IDynamicKnowLike
    {
        IEnumerable<Type> KnownInterfaces { set; }
    }


    /// <summary>
    /// Base class of Emited Proxies
    /// </summary>
    public abstract class ActLikeProxy : IActLikeProxy
    {
        public dynamic Original{ get; private set;}

        protected ActLikeProxy(dynamic original, IEnumerable<Type> interfaces)
        {
            Original = original;
            var tKnowOriginal = Original as IDynamicKnowLike;
            if (tKnowOriginal != null)
                tKnowOriginal.KnownInterfaces =interfaces;
            
        }
    }
}
