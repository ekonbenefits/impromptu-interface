using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ImpromptuInterface.Internal.Support;

namespace ImpromptuInterface.Dynamic
{

    /// <summary>
    /// Abstract base for the Generic class <see cref="ImpromptuLazy{T}"/> with <see cref="Create{T}(System.Func{T})"/> fatory methods
    /// </summary>
    [Serializable]
    public abstract class ImpromptuLazy:ImpromptuForwarder
    {
        /// <summary>
        /// Creates ImpromptuLazy based on the specified valuefactory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valuefactory">The valuefactory.</param>
        /// <returns></returns>
        public static dynamic Create<T>(Func<T> valueFactory)
        {
            return new ImpromptuLazy<T>(valueFactory);
        }
        /// <summary>
        /// Creates ImpromptuLazy based on the specified target.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static dynamic Create<T>(Lazy<T> target)
        {
            return new ImpromptuLazy<T>(target);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuLazy"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        protected ImpromptuLazy(object target) : base(target)
        {
        }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuLazy"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ImpromptuLazy(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif

    }

    /// <summary>
    /// Wraps a Lazy Type evalutaes on first method call
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ImpromptuLazy<T> : ImpromptuLazy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuLazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public ImpromptuLazy(Lazy<T> target) : base(target)
        {
        }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuForwarder"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ImpromptuLazy(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuLazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        public ImpromptuLazy(Func<T> valueFactory ):base(new Lazy<T>(valueFactory))
        {
            
        }
     

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return ((Lazy<T>)Target).IsValueCreated 
                ? base.GetDynamicMemberNames() 
                : Enumerable.Empty<string>();
        }

        protected override object CallTarget
        {
            get
            {
                return ((Lazy<T>) Target).Value;
            }
        }

       
    }
}
