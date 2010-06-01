using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface
{
    /// <summary>
    /// Dynamic Object that knows about the Impromtu Interface return types;
    /// </summary>
    public abstract class ImpromptuObject : DynamicObject, IDynamicKnowsLike, IActsLike
    {
        private static readonly IDictionary<TypeHash, IDictionary<string, Type>> _returnTypHash =
        new Dictionary<TypeHash, IDictionary<string, Type>>();

        private TypeHash _hash;

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _returnTypHash[_hash].Select(it => it.Key);
        }


        protected virtual Type TypeForName(string name)
        {
            return _returnTypHash[_hash][name];
        }

        public virtual void SetKnownInterfaces(IEnumerable<Type> interfaces)
        {
            lock ("com.ImpromptuInterface.DynamicReturnTypeHash")
            {
                _hash = new TypeHash(interfaces);
                if (_returnTypHash.ContainsKey(_hash)) return;
                var tDict = interfaces.SelectMany(@interface => @interface.GetProperties())
                    .ToDictionary(info => info.Name, info => info.GetGetMethod().ReturnType);
                _returnTypHash.Add(_hash, tDict);
            }
        }

        public virtual TInterface ActsLike<TInterface>(params Type[] otherInterfaces) where TInterface:class
        {
            return Impromptu.ActsLike<TInterface>(this, otherInterfaces);
        }
    }
}
