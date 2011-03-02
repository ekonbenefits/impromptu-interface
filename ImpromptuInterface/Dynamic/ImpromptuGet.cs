using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Dynamic Proxy that exposes any (and only) getter properties of wrapped objects including Anonymous objects
    /// </summary>
    public class ImpromptuGet:ImpromptuObject
    {
        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public object Target { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuGet"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public ImpromptuGet(object target)
        {
            Target = target;
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            result =Impromptu.InvokeGet(Target, binder.Name);
            return true;
        }
    }
}
