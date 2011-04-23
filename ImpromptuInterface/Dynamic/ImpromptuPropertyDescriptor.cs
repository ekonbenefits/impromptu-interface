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
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ImpromptuPropertyDescriptor(string name) : base(name, null)
        {
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            try
            {
                return Impromptu.InvokeGet(component, Name);
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
            Impromptu.InvokeSet(component, Name, value);
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
