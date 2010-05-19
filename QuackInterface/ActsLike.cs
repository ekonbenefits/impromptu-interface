using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QuackInterface
{
    public interface IActsLike
    {
        TInterface ActsLike<TInterface>(params Type[] otherInterfaces);
    }

    public static class ObjectExtensions
    {
        public static TInterface ActsLike<TInterface>(this Object originalDynamic, params Type[]otherInterfaces)where TInterface:class
        {
            var tType = originalDynamic.GetType();

            var tProxy = BuildProxy.BuildType(tType,typeof(TInterface), otherInterfaces);

              return (TInterface)Activator.CreateInstance(tProxy, originalDynamic);
        }
    }

    public static class Dynamic
    {
        public static TInterface ActsLike<TInterface>(dynamic originalDynamic, params Type[] otherInterfaces) where TInterface : class
        {
            var tType = originalDynamic.GetType();

            var tProxy = BuildProxy.BuildType(tType, typeof(TInterface), otherInterfaces);

            return (TInterface)Activator.CreateInstance(tProxy, (object)originalDynamic);
        }
    }
}
