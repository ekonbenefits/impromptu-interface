// 
//  Copyright 2011 Ekon Benefits
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace ImpromptuInterface.Optimization
{
    /// <summary>
    /// `new T(args)` with the constructor chosen by the arguments' runtime types, as C#'s
    /// `new T(dynamicArg)` does — rather than by <see cref="Activator"/>, whose overload
    /// resolution is not the same.
    /// </summary>
    /// <remarks>
    /// The call site is built once per (type, argument count) and cached: the shape is what a
    /// site is compiled for, and the arguments' runtime types are what it dispatches on.
    /// </remarks>
    internal static class DynamicConstructor
    {
        private static readonly ConcurrentDictionary<Tuple<Type, int>, Func<object[], object>> _sites =
            new ConcurrentDictionary<Tuple<Type, int>, Func<object[], object>>();

        /// <summary>Constructs <paramref name="type"/> with <paramref name="args"/>.</summary>
        internal static object Invoke(Type type, params object[] args)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            args = args ?? new object[0];

            // A dynamic invocation does not see a value type's parameterless constructor.
            if (args.Length == 0 && type.IsValueType)
                return Activator.CreateInstance(type);

            var site = _sites.GetOrAdd(Tuple.Create(type, args.Length), key => Compile(key.Item1, key.Item2));
            return site(args);
        }

        private static Func<object[], object> Compile(Type type, int count)
        {
            var argsParameter = Expression.Parameter(typeof(object[]), "args");

            // The type is the site's first argument, as a static type; each real argument
            // arrives as object, so the binder dispatches on its runtime type.
            var infos = new List<CSharpArgumentInfo>
            {
                CSharpArgumentInfo.Create(
                    CSharpArgumentInfoFlags.IsStaticType | CSharpArgumentInfoFlags.UseCompileTimeType, null)
            };
            var arguments = new List<Expression> { Expression.Constant(type, typeof(Type)) };

            for (var i = 0; i < count; i++)
            {
                infos.Add(CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null));
                arguments.Add(Expression.ArrayIndex(argsParameter, Expression.Constant(i)));
            }

            // The site's result type has to be one the binder can produce. For a value type the
            // binder produces the value type itself, and asking for object instead fails with
            // "The result type ... is not compatible with the result type ... expected by the
            // call site" - so ask for the type and box afterwards.
            var resultType = type.IsValueType ? type : typeof(object);

            Expression call = Expression.Dynamic(
                Binder.InvokeConstructor(CSharpBinderFlags.None, type, infos),
                resultType,
                arguments);

            if (resultType != typeof(object))
                call = Expression.Convert(call, typeof(object));

            return Expression.Lambda<Func<object[], object>>(call, argsParameter).Compile();
        }
    }
}
