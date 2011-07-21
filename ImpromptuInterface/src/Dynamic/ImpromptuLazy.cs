using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ImpromptuInterface.Dynamic;

namespace ImpromptuInterface.src.Dynamic
{
    /// <summary>
    /// Wraps a Lazy Type evalutaes on first method call
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ImpromptuLazy<T>:ImpromptuForwarder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuLazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public ImpromptuLazy(Lazy<T> target) : base(target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuLazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        public ImpromptuLazy(Func<T> valueFactory ):base(new Lazy<T>(valueFactory))
        {
            
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (((Lazy<T>)Target).IsValueCreated)
            {
                return base.GetDynamicMemberNames();
            }
            return Enumerable.Empty<string>();
        }

        protected override object CallTarget
        {
            get
            {
                return ((Lazy<T>) Target).Value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuLazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public ImpromptuLazy(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
