using System;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// This interface can be used on your custom dynamic objects if you want impromptu interfaces without casting to object or using the static method syntax of ActLike.
    /// Also if you want to change the behavior for slightly for specific types as this will take precident when using the dynamic keyword or your specific type is known staticly.
    /// </summary>
    public interface IActLike
    {
        /// <summary>
        /// This interface can be used on your custom dynamic objects if you want impromptu interfaces without casting to object or using the static method syntax of ActLike.
        /// Also if you want to change the behavior for slightly for specific types as this will take precident when using the dynamic keyword or your specific type is known staticly.
        /// </summary>
        ///<param name="otherInterfaces"></param>
        ///<typeparam name="TInterface"></typeparam>
        ///<returns></returns>
        TInterface ActLike<TInterface>(params Type[] otherInterfaces) where TInterface : class;
    }
}