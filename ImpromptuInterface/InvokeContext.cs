using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface
{

    public static class InvokeContextExtension
    {
        public static InvokeContext WithContext(this object target, Type context)
        {
            return new InvokeContext(target, context);
        }

        public static InvokeContext WithContext<TContext>(this object target)
        {
            return new InvokeContext(target, typeof(TContext));
        }

        public static InvokeContext WithContext(this object target, object context)
        {
            return new InvokeContext(target, context);
        }
    }

    public class InvokeContext
    {
        public object Target { get; set; }
        public Type Context { get; set; }

        public InvokeContext(object Target, Type context)
        {
            this.Target = Target;
            Context = context;
        }

        public InvokeContext(object Target, object context)
        {
            this.Target = Target;
            Context = context.GetType();
        }
    }
}
