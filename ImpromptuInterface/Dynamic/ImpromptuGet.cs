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

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            result =Impromptu.InvokeGet(Target, binder.Name);
            return true;
        }
    }
}
