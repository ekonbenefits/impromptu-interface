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
    public class ImpromptuFactory:ImpromptuObject
    {

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            result = GetInstanceForDynamicMember(binder.Name);
            return result != null;
        }

        protected virtual object CreateType(Type type)
        {
            return Activator.CreateInstance(type);
        }

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
