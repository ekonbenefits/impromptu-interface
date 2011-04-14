// 
//  Copyright 2011  Ekon Benefits
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface
{

    /// <summary>
    /// Extension Methods for Adding context to invocation
    /// </summary>
    public static class InvokeContextExtension
    {
        /// <summary>
        /// Combines target with context.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static InvokeContext WithContext(this object target, Type context)
        {
            return new InvokeContext(target, context);
        }

        /// <summary>
        /// Combines target with context.
        /// </summary>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static InvokeContext WithContext<TContext>(this object target)
        {
            return new InvokeContext(target, typeof(TContext));
        }

        /// <summary>
        /// Combines target with context.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static InvokeContext WithContext(this object target, object context)
        {
            return new InvokeContext(target, context);
        }

        /// <summary>
        /// Withes the static context.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static InvokeContext WithStaticContext(this Type target, object context =null)
        {

            return new InvokeContext(target,true,context);
        }
    }

    /// <summary>
    /// Object that stores a context with a target for dynamic invocation
    /// </summary>
    [Serializable]
    public class InvokeContext
    {
        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public object Target { get; protected set; }
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public Type Context { get; protected set; }

        public bool StaticContext { get; protected set; }

        public InvokeContext(Type target, bool staticContext, object context)
        {
            if (context != null && !(context is Type))
            {
                context = context.GetType();
            }
            Target = target;
            Context = ((Type)context) ?? target;
            StaticContext = staticContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeContext"/> class.
        /// </summary>
        /// <param name="Target">The target.</param>
        /// <param name="context">The context.</param>
        public InvokeContext(object Target, object context)
        {
            this.Target = Target;

            if (context != null && !(context is Type))
            {
                context = context.GetType();
            }

            Context = (Type)context;
        }
    }
}
