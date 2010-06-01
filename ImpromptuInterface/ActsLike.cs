

using System.Linq;

namespace ImpromptuInterface
{
    using System;

    /// <summary>
    /// This interface can be used on your custom dynamic objects if you want impromptu interfaces without casting to object or using the static method syntax of ActsLike
    /// </summary>
    public interface IActsLike 
    {
        TInterface ActsLike<TInterface>(params Type[] otherInterfaces) where TInterface : class;
    }


    public static class Impromptu
    {
        /// <summary>
        /// Extension Method that Wraps an existing object with an Explicit interface definition
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="originalDynamic">The original object can be annoymous type, System.DynamicObject as well as any others.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        public static TInterface ActsLike<TInterface>(this Object originalDynamic, params Type[]otherInterfaces)where TInterface:class
        {
            var tType = originalDynamic.GetType();

            var tProxy = BuildProxy.BuildType(tType,typeof(TInterface), otherInterfaces);

            return (TInterface)Activator.CreateInstance(tProxy, originalDynamic, new[] { typeof(TInterface) }.Concat(otherInterfaces).ToArray());
        }
    }

}
