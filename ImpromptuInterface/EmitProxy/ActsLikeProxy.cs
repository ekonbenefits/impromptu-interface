using System;
using System.Collections.Generic;

namespace ImpromptuInterface
{
    /// <summary>
    /// This interface can be used to access the original content of your emitted type;
    /// </summary>
    public interface IActsLikeProxy
    {
        dynamic Original { get; }
    }

    /// <summary>
    /// This interface can be used on your custom dynamic objects if you want to know the interface you are implementing
    /// </summary>
    public interface IDynamicKnowsLike
    {
        IEnumerable<Type> KnownInterfaces { set; }
    }


    /// <summary>
    /// Base class of Emited Proxies
    /// </summary>
    public abstract class ActsLikeProxy : IActsLikeProxy
    {
        public dynamic Original{ get; private set;}

        protected ActsLikeProxy(dynamic original, Type[] interfaces)
        {
            Original = original;
            var tKnowOriginal = Original as IDynamicKnowsLike;
            if (tKnowOriginal != null)
                tKnowOriginal.KnownInterfaces =interfaces;
            
        }
    }
}
