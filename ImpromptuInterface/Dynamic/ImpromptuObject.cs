// 
//  Copyright 2010  Ekon Benefits
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

                return _hash.Types.Cast<Type>();
            }
            set
            {
                lock (TypeHashLock)
                {
                    _hash = new TypeHash(value);
                    if (_returnTypHash.ContainsKey(_hash)) return;

                    var tPropReturType = value.SelectMany(@interface => @interface.GetProperties())
                        .Where(property=>property.GetGetMethod() !=null)
                        .Select(property=>new{property.Name, property.GetGetMethod().ReturnType});

                    var tMethodReturnType = value.SelectMany(@interface => @interface.GetMethods())
                      .Select(property => new { property.Name, property.ReturnType });

                    var tDict = tPropReturType.Concat(tMethodReturnType)
                        .ToDictionary(info => info.Name, info => info.ReturnType);
                    
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

        /// <summary>
        /// Tries to get the type for the property name from the interface.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="returnType">The return Type.</param>
        /// <returns></returns>
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


        /// <summary>
        /// Allows ActLike to be called via dyanmic invocation
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        public virtual TInterface ActLike<TInterface>(params Type[] otherInterfaces) where TInterface:class
        {
            return Impromptu.ActLike<TInterface>(this, otherInterfaces);
        }
    }
}
