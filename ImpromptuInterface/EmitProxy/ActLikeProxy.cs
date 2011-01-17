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
        dynamic Original { get; }
    }

    /// <summary>
    /// This interface can be used on your custom dynamic objects if you want to know the interface you are implementing
    /// </summary>
    public interface IDynamicKnowLike
    {
        IEnumerable<Type> KnownInterfaces { set; }
    }


    /// <summary>
    /// Base class of Emited Proxies
    /// </summary>
    public abstract class ActLikeProxy : IActLikeProxy
    {
        public dynamic Original{ get; private set;}

        protected ActLikeProxy(dynamic original, IEnumerable<Type> interfaces)
        {
            Original = original;
            var tKnowOriginal = Original as IDynamicKnowLike;
            if (tKnowOriginal != null)
                tKnowOriginal.KnownInterfaces =interfaces;
            
        }
    }
}
