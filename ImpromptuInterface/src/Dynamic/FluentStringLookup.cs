using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ImpromptuInterface.Internal.Support;
using ImpromptuInterface.Optimization;
using System.Reflection;
namespace ImpromptuInterface.Dynamic
{


    /// <summary>
    /// Building block to use Method calls as dynamic lookups
    /// </summary>
    public class FluentStringLookup:DynamicObject,ICustomTypeProvider
    {
        private readonly Func<string, dynamic> _lookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentStringLookup"/> class.
        /// </summary>
        /// <param name="lookup">The lookup.</param>
        public FluentStringLookup(Func<string,dynamic> lookup)
        {
            _lookup = lookup;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = _lookup(binder.Name);
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            result = null;
            if (args.Length == 1 && args.First() is String)
            {
                result = _lookup(args[0] as String);
                return true;
            }
            return false;
        }

#if SILVERLIGHT5

        /// <summary>
        /// Gets the custom Type.
        /// </summary>
        /// <returns></returns>
        public Type GetCustomType()
        {
            return this.GetDynamicCustomType();
        }
#endif

    }
}
