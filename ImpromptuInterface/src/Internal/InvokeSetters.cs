using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ImpromptuInterface.Optimization;
using Microsoft.CSharp.RuntimeBinder;
using ImpromptuInterface.Internal.Support;
using System.Reflection;

namespace ImpromptuInterface.Internal
{
    /// <summary>
    /// Internal class implmenation for <see cref="Impromptu.InvokeSetAll"/>
    /// </summary>
    public class InvokeSetters : DynamicObject,ICustomTypeProvider
    {
        internal InvokeSetters()
        {

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

        /// <summary>
        /// Provides the implementation for operations that invoke an object. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as invoking an object or a delegate.
        /// </summary>
        /// <param name="binder">Provides information about the invoke operation.</param>
        /// <param name="args">The arguments that are passed to the object during the invoke operation. For example, for the sampleObject(100) operation, where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="args"/>[0] is equal to 100.</param>
        /// <param name="result">The result of the object invocation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
        /// </returns>
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            IEnumerable<KeyValuePair<string, object>> tDict = null;
            object target = null;
            result = null;

            //Setup Properties as dictionary
            if (binder.CallInfo.ArgumentNames.Any())
            {
                
                if (binder.CallInfo.ArgumentNames.Count + 1 == binder.CallInfo.ArgumentCount)
                {
                    target = args.First();
                    tDict = binder.CallInfo.ArgumentNames
                        .Zip(args.Skip(1), (key, value) => new { key, value })
                        .ToDictionary(k => k.key, v => v.value);

                }else
                {
                    throw new RuntimeBinderException("InvokeSetAll requires first parameter to be target unamed, and all other parameters to be named.");
                }
            }
            else if (args.Length == 2)
            {
                target = args[0];
                if (args[1] is IEnumerable<KeyValuePair<string, object>>)
                {
                    tDict = (IEnumerable<KeyValuePair<string, object>>)args[1];
                }
                else if (args[1] is IEnumerable
                        && args[1].GetType().IsGenericType
                    )
                {
                    var tEnumerableArg = (IEnumerable)args[1];

                    var tInterface = tEnumerableArg.GetType().GetInterface("IEnumerable`1", false);
                    if(tInterface !=null)
                    {
                        var tParamTypes = tInterface.GetGenericArguments();
                        if(tParamTypes.Length ==1 
                            && tParamTypes[0].GetGenericTypeDefinition() == typeof(Tuple<,>))
                        {
                           tDict= tEnumerableArg.Cast<dynamic>().ToDictionary(k => (string) k.Item1, v => (object) v.Item2);
                        }
                    }
                }
                else if (Util.IsAnonymousType(args[1]))
                {
                    var keyDict = new Dictionary<string, object>();
                    foreach (var tProp in args[1].GetType().GetProperties())
                    {
                        keyDict[tProp.Name] = Impromptu.InvokeGet(args[1], tProp.Name);
                    }
                    tDict = keyDict;
                }
            }
            //Invoke all properties
            if (target != null && tDict != null)
            {
                foreach (var tPair in tDict)
                {
                    Impromptu.InvokeSetChain(target, tPair.Key, tPair.Value);
                }
                result = target;
                return true;
            }
            return false;
        }
    }
}
