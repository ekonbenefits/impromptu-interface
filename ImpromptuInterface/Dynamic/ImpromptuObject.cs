using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface
{
    /// <summary>
    /// Dynamic Object that knows about the Impromtu Interface return types;
    /// Override Typical Dynamic Object methods, and use TypeForName to get the return type of an interface member.
    /// </summary>
    public abstract class ImpromptuObject : DynamicObject, IDynamicKnowsLike, IActsLike
    {
        protected static readonly IDictionary<TypeHash, IDictionary<string, Type>> _returnTypHash =
        new Dictionary<TypeHash, IDictionary<string, Type>>();

        protected TypeHash _hash;

        /// <summary>
        /// Gets or sets the known interfaces.
        /// Set should only be called be the factory methood
        /// </summary>
        /// <value>The known interfaces.</value>
        public virtual IEnumerable<Type> KnownInterfaces
        {
            get
            {
                return _hash.Types;
            }
            set
            {
                lock ("com.ImpromptuInterface.DynamicReturnTypeHash")
                {
                    _hash = new TypeHash(value);
                    if (_returnTypHash.ContainsKey(_hash)) return;
                    var tDict = value.SelectMany(@interface => @interface.GetProperties())
                        .ToDictionary(info => info.Name, info => info.GetGetMethod().ReturnType);
                    _returnTypHash.Add(_hash, tDict);
                }
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _returnTypHash[_hash].Select(it => it.Key);
        }

        protected virtual Type TypeForName(string name)
        {
            return _returnTypHash[_hash][name];
        }

        public virtual TInterface ActsLike<TInterface>(params Type[] otherInterfaces) where TInterface:class
        {
            return Impromptu.ActsLike<TInterface>(this, otherInterfaces);
        }
    }
}
