using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using ImpromptuInterface.Internal.Support;
using ImpromptuInterface.Optimization;
using System.Reflection;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Internal method for subsequent invocations of <see cref="Impromptu.Curry(object,System.Nullable{int})"/>
    /// </summary>
    public class PartialApplyInvocation : DynamicObject, ICustomTypeProvider
    {


        /// <summary>
        /// Provides implementation for type conversion operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations that convert an object from one type to another.
        /// </summary>
        /// <param name="binder">Provides information about the conversion operation. The binder.Type property provides the type to which the object must be converted. For example, for the statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Type returns the <see cref="T:System.String"/> type. The binder.Explicit property provides information about the kind of conversion that occurs. It returns true for explicit conversion and false for implicit conversion.</param>
        /// <param name="result">The result of the type conversion operation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = Impromptu.CoerceToDelegate(this, binder.Type);

            return result != null;
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

        public PartialApplyInvocation(object target, object[] args, string memberName = null, int? totalCount = null, InvocationKind? invocationKind = null)
        {
            _target = target;
            _memberName = memberName;
            _invocationKind = invocationKind ?? (String.IsNullOrWhiteSpace(_memberName)
                                     ? InvocationKind.InvokeUnknown
                                     : InvocationKind.InvokeMemberUnknown);
            _totalArgCount = totalCount;
            _args = args;
        }

        private readonly int? _totalArgCount;
        private readonly object _target;
        private readonly string _memberName;
        private readonly object[] _args;
        private readonly InvocationKind _invocationKind;

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public object Target
        {
            get { return _target; }
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        public string MemberName
        {
            get { return _memberName; }
        }

        /// <summary>
        /// Gets the args.
        /// </summary>
        /// <value>The args.</value>
        public object[] Args
        {
            get { return _args; }
        }

        /// <summary>
        /// Gets the total arg count.
        /// </summary>
        /// <value>The total arg count.</value>
        public int? TotalArgCount
        {
            get { return _totalArgCount; }
        }

        /// <summary>
        /// Gets the kind of the invocation.
        /// </summary>
        /// <value>The kind of the invocation.</value>
        public InvocationKind InvocationKind
        {
            get { return _invocationKind; }
        }

        private IDictionary<int, CacheableInvocation> _cacheableInvocation = new Dictionary<int, CacheableInvocation>();

        /// <summary>
        /// Provides the implementation for operations that invoke an object. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as invoking an object or a delegate.
        /// </summary>
        /// <param name="binder">Provides information about the invoke operation.</param>
        /// <param name="args">The arguments that are passed to the object during the invoke operation. For example, for the sampleObject(100) operation, where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="args[0]"/> is equal to 100.</param>
        /// <param name="result">The result of the object invocation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
        /// </returns>
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            var tNamedArgs = Util.NameArgsIfNecessary(binder.CallInfo, args);
            var tNewArgs = _args.Concat(tNamedArgs).ToArray();

            if (_totalArgCount.HasValue && (_totalArgCount - Args.Length - args.Length > 0))
            //Not Done currying
            {
                result = new PartialApplyInvocation(Target, tNewArgs, MemberName,
                                   TotalArgCount, InvocationKind);

                return true;
            }
            var tInvokeDirect = String.IsNullOrWhiteSpace(_memberName);
            var tDel = _target as Delegate;


            if (tInvokeDirect && binder.CallInfo.ArgumentNames.Count == 0 && _target is Delegate)
            //Optimization for direct delegate calls
            {
                result = tDel.FastDynamicInvoke(tNewArgs);
                return true;
            }


            Invocation tInvocation;
            if (binder.CallInfo.ArgumentNames.Count == 0) //If no argument names we can cache the callsite
            {
                CacheableInvocation tCacheableInvocation;
                if (!_cacheableInvocation.TryGetValue(tNewArgs.Length, out tCacheableInvocation))
                {
                    tCacheableInvocation = new CacheableInvocation(InvocationKind, _memberName, argCount: tNewArgs.Length, context: _target);
                    _cacheableInvocation[tNewArgs.Length] = tCacheableInvocation;

                }
                tInvocation = tCacheableInvocation;
            }
            else
            {
                tInvocation = new Invocation(InvocationKind, _memberName);
            }

            result = tInvocation.Invoke(_target, tNewArgs);


            return true;
        }
    }
}
