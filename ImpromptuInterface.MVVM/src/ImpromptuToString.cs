using System;
using System.Collections.Generic;
using ImpromptuInterface.Dynamic;
using System.Linq;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Extension methods to create ImpromptuToString proxies;
    /// </summary>
    public static class ImpromptuToStringExtensions
    {

  

        /// <summary>
        /// Proxies everything to replace the ToString Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <param name="toStringDelegate">To string delegate.</param>
        /// <returns></returns>
        public static ImpromptuToString<T> ProxyToString<T>(this T target, Func<T,string> toStringDelegate)
        {
            return new ImpromptuToString<T>(target, toStringDelegate);
        }

        /// <summary>
        /// Proxies all items to replace the ToString Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targets">The targets.</param>
        /// <param name="toStringDelegate">To string delegate.</param>
        /// <returns></returns>
        public static IEnumerable<ImpromptuToString<T>> ProxyAllToString<T>(this IEnumerable<T> targets, Func<T,string> toStringDelegate)
        {
            return targets.Select(it=> new ImpromptuToString<T>(it, toStringDelegate));
        }
    }


    /// <summary>
    /// Proxy to override to string
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    public class ImpromptuToString<TTarget>: ImpromptuForwarder
    {

        /// <summary>
        /// Null value representative object.
        /// </summary>
        /// <returns></returns>
        public static ImpromptuToString<TTarget>  NullValue(string toStringValue)
        {
            return NullValue(it => toStringValue);
        }

        /// <summary>
        /// Null  value representative object.
        /// </summary>
        /// <returns></returns>
        public static ImpromptuToString<TTarget> NullValue(Func<TTarget, string> toStringDelegate)
        {
            return new ImpromptuToString<TTarget>(default(TTarget), toStringDelegate);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ImpromptuInterface.MVVM.ImpromptuToString&lt;TTarget&gt;"/> to <see cref="TTarget"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TTarget(ImpromptuToString<TTarget> proxy)
        {
            return (TTarget)proxy.Target;
        }

        private readonly Func<TTarget, string> _toStringDelegate;



        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuToString&lt;TTarget&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="toStringDelegate">To string delegate.</param>
        public ImpromptuToString(TTarget target, Func<TTarget,string> toStringDelegate) : base(target)
        {
            _toStringDelegate = toStringDelegate;
        }

        public override string ToString()
        {
            return _toStringDelegate((TTarget)Target);
        }
    }
}
