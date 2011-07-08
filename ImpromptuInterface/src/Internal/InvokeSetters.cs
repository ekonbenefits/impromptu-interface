using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using ImpromptuInterface.Optimization;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Internal
{
    public class InvokeSetters : DynamicObject
    {
        internal InvokeSetters()
        {

        }
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
