using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace Benchmarks
{
    /// <summary>
    /// What Castle DynamicProxy needs in order to do what <c>ActLike</c> does: an interceptor
    /// that forwards each interface member to the same-named member of an object that does not
    /// implement the interface.
    /// </summary>
    /// <remarks>
    /// Castle emits the proxy type; it does not bind to a target that lacks the interface, so
    /// the forwarding is the caller's to write. This one caches the resolved <see cref="MethodInfo"/>
    /// per interface method, which is the fast honest version — resolving by name on every call
    /// would be measuring a straw man.
    /// </remarks>
    public class DuckInterceptor : IInterceptor
    {
        private readonly object _target;
        private readonly ConcurrentDictionary<MethodInfo, MethodInfo> _map = new ConcurrentDictionary<MethodInfo, MethodInfo>();

        public DuckInterceptor(object target) => _target = target;

        public void Intercept(IInvocation invocation)
        {
            var target = _map.GetOrAdd(
                invocation.Method,
                m => _target.GetType().GetMethod(m.Name, m.GetParameters().Select(p => p.ParameterType).ToArray()));

            if (target == null)
                throw new MissingMethodException(_target.GetType().Name, invocation.Method.Name);

            invocation.ReturnValue = target.Invoke(_target, invocation.Arguments);
        }
    }
}
