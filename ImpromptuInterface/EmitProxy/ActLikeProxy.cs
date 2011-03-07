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
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;

namespace ImpromptuInterface.Build
{
    /// <summary>
    /// This interface can be used to define your own custom proxy if you preload it.
    /// </summary>
    /// <remarks>
    /// Advanced usage only! This is required as well as <see cref="ActLikeProxyAttribute"></see>
    /// </remarks>
    public interface IActLikeProxyInitialize : IActLikeProxy
    {
        ///<summary>
        /// Method used to Initialize Proxy
        ///</summary>
        ///<param name="original"></param>
        ///<param name="interfaces"></param>
        ///<param name="informalInterface"></param>
        void Initialize(dynamic original, IEnumerable<Type> interfaces =null, IDictionary<string, Type> informalInterface = null);
    }


    /// <summary>
    /// Base class of Emited Proxies
    /// </summary>
    public abstract class ActLikeProxy : IActLikeProxyInitialize
    {
        /// <summary>
        /// Returns the proxied object
        /// </summary>
        /// <value></value>
        public dynamic Original{ get; private set;}

        private bool _init = false;

        /// <summary>
        /// Method used to Initialize Proxy
        /// </summary>
        /// <param name="original"></param>
        /// <param name="interfaces"></param>
        /// <param name="informalInterface"></param>
        public virtual void Initialize(dynamic original, IEnumerable<Type> interfaces=null, IDictionary<string, Type> informalInterface =null)
        {
            if(original == null)
                throw new ArgumentNullException("original", "Can't proxy a Null value");

            if (_init)
                throw new MethodAccessException("Initialize should not be called twice!");
            _init = true;
            Original = original;
            var tKnowOriginal = Original as IDynamicKnowLike;
            if (tKnowOriginal != null)
            {
                if(interfaces !=null)
                    tKnowOriginal.KnownInterfaces = interfaces;
                if (informalInterface != null)
                    tKnowOriginal.KnownPropertySpec = informalInterface;
            }

        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(Original, obj)) return true;
            if (!(obj is ActLikeProxy)) return Original.Equals(obj);
            return Equals((ActLikeProxy) obj);
        }

        /// <summary>
        /// Actlike proxy should be equivalent to the objects they proxy
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(ActLikeProxy other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(Original, other.Original)) return true;
            return Equals(other.Original, Original);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Original.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Original.ToString();
        }
    }
}
