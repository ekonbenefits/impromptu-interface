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

namespace ImpromptuInterface
{
    /// <summary>
    /// This interface can be used to access the original content of your emitted type;
    /// </summary>
    public interface IActLikeProxy
    {
        ///<summary>
        /// Returns the proxied object
        ///</summary>
        dynamic Original { get; }
        
    }


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
    /// This interface can be used on your custom dynamic objects if you want to know the interface you are impromptu-ly implementing.
    /// </summary>
    public interface IDynamicKnowLike
    {
        ///<summary>
        /// Property used to pass interface information to proxied object
        ///</summary>
        IEnumerable<Type> KnownInterfaces { set; }

        /// <summary>
        /// Sets the known property spec.
        /// </summary>
        /// <value>The known property spec.</value>
        IDictionary<string, Type> KnownPropertySpec { set; }
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(Original, obj)) return true;
            if (obj.GetType() != typeof (ActLikeProxy)) return Original.Equals(obj);
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

        public override int GetHashCode()
        {
            return Original.GetHashCode();
        }

        public override string ToString()
        {
            return Original.ToString();
        }
    }
}
