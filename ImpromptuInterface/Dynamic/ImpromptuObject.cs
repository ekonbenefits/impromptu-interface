using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ImpromptuInterface
{
    /// <summary>
    /// Dynamic Object that knows about the Impromtu Interface return types;
    /// Override Typical Dynamic Object methods, and use TypeForName to get the return type of an interface member.
    /// </summary>
    public abstract class ImpromptuObject : DynamicObject, IDynamicKnowLike, IActLike
    {
        protected static readonly IDictionary<TypeHash, IDictionary<string, Type>> _returnTypHash =
        new Dictionary<TypeHash, IDictionary<string, Type>>();
        private static readonly object TypeHashLock = new object();
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
                lock (TypeHashLock)
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
            return HashForThisType().Select(it => it.Key);
        }

        private IDictionary<string, Type> HashForThisType()
        {
            return _hash == null || !_returnTypHash.ContainsKey(_hash)
                ? new Dictionary<string, Type>() 
                : _returnTypHash[_hash];
        }

        protected virtual bool TryTypeForName(string name, out Type returnType)
        {
            if (!HashForThisType().ContainsKey(name))
            {
                returnType = null;
                return false;
            }

            returnType = HashForThisType()[name];
            return true;
        }


        public virtual TInterface ActLike<TInterface>(params Type[] otherInterfaces) where TInterface:class
        {
            return Impromptu.ActLike<TInterface>(this, otherInterfaces);
        }
    }
}
