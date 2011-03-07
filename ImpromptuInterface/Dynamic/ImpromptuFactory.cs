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

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Base Class for making a fluent factory using an Impromptu Interface return type.
    /// </summary>
    public class ImpromptuFactory:ImpromptuObject
    {

        /// <summary>
        /// Provides the default implementation for operations that get instance as defined by <see cref="GetInstanceForDynamicMember"/>. Classes derived from the <see cref="T:ImpromptuInterface.ImpromptuObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
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
        /// <summary>
        /// Store Singletons
        /// </summary>
        protected readonly Dictionary<string, dynamic> _hashFactoryTypes= new Dictionary<string, dynamic>();

        /// <summary>
        /// Lock for accessing singletons
        /// </summary>
        protected readonly object _lockTable = new object();


        /// <summary>
        /// Gets the instance for a dynamic member. Override for type constrcution behavoir changes based on property name.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns></returns>
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
