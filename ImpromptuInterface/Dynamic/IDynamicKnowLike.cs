using System;
using System.Collections.Generic;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// This interface can be used on your custom dynamic objects if you want to know the interface you are impromptu-ly implementing.
    /// </summary>
    public interface IDynamicKnowLike
    {
        ///<summary>
        /// Property used to pass interface information to proxied object
        ///</summary>
        IEnumerable<Type> KnownInterfaces { set; }

        /// <summary>
        /// Sets the known property spec.
        /// </summary>
        /// <value>The known property spec.</value>
        IDictionary<string, Type> KnownPropertySpec { set; }
    }
}