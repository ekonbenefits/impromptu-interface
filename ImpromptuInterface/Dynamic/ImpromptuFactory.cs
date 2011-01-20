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
using System.Linq;
using System.Text;

namespace ImpromptuInterface
{
    /// <summary>
    /// Base Class for making a fluent factory using an Impromptu Interface return type.
    /// </summary>
    public class ImpromptuFactory:ImpromptuObject
    {

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            result = GetInstanceForDynamicMember(binder.Name);
            return result != null;
        }

        /// <summary>
        /// Constructs the type. Override for changing type intialization property changes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected virtual object CreateType(Type type)
        {
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Gets the instance for a dynamic member. Override for type constrcution behavoir changes based on property name.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns></returns>
        public virtual object GetInstanceForDynamicMember(string memberName)
        {
            Type type;
            if (TryTypeForName(memberName, out type))
            {
                return CreateType(type);
            }
            return null;
        }
    }


    /// <summary>
    /// Base Class for making a singleton fluent factory using an Impromptu Interface return type.
    /// </summary>
    public class ImpromptuSingleInstancesFactory : ImpromptuFactory
    {
        protected readonly Dictionary<string, dynamic> _hashFactoryTypes= new Dictionary<string, dynamic>();
        protected readonly object _lockTable = new object();


        public override object GetInstanceForDynamicMember(string memberName)
        {
            lock (_lockTable)
            {
                if (!_hashFactoryTypes.ContainsKey(memberName))
                {
                    Type type;
                    if (TryTypeForName(memberName, out type))
                    {
                        _hashFactoryTypes.Add(memberName, CreateType(type));
                    }
                    else
                    {
                        return null;
                    }

                }

                return _hashFactoryTypes[memberName];
            }
        }
    }
}
