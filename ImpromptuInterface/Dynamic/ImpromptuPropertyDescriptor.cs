using System;
using System.ComponentModel;
using ImpromptuInterface;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Property Descriptor for Dynamic Objects
    /// </summary>
    public class ImpromptuPropertyDescriptor:PropertyDescriptor
    {

        private readonly CacheableInvocation _invokeGet;
        private readonly CacheableInvocation _invokeSet;
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ImpromptuPropertyDescriptor(string name) : base(name, null)
        {
            _invokeGet = new CacheableInvocation(InvocationKind.Get, name);
            _invokeSet = new CacheableInvocation(InvocationKind.Set, name);

        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            try
            {
                return _invokeGet.Invoke(component);
            }
            catch (RuntimeBinderException)
            {
                
                return null;
            }
          
        }

        public override void ResetValue(object component)
        {
            
        }

        public override void SetValue(object component, object value)
        {   
            try
            {
                _invokeSet.Invoke(component, value);
            }
            catch (RuntimeBinderException)
            {
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof(object); }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get
            {
                return typeof (object);
            }
        }
    }
}
