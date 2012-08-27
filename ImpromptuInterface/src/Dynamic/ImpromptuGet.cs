// 
//  Copyright 2011 Ekon Benefits
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
using System.Reflection;
using System.Runtime.Serialization;
using ImpromptuInterface.Internal.Support;
using ImpromptuInterface.Optimization;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Dynamic Proxy that exposes any properties of objects, and can massage results based on interface
    /// </summary>
    [Serializable]
    public class ImpromptuGet:ImpromptuForwarder
    {
     

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuGet"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public ImpromptuGet(object target):base(target)
        {
            
        }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuGet"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ImpromptuGet(SerializationInfo info, 
           StreamingContext context):base(info,context)
        {


        }

       
#endif


        /// <summary>
        /// Creates the proxy over the specified target.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static T Create<T>(object target) where T : class
        {
            return new ImpromptuGet(target).ActLike<T>();
        }
        /// <summary>
        /// Creates the proxy over the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static dynamic Create(object target)
        {
            return new ImpromptuGet(target);
        }
        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            if (base.TryGetMember(binder, out result))
            {
                return this.MassageResultBasedOnInterface(binder.Name, true, ref result);
            }
            return false;
        }


        /// <summary>
        /// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as calling a method.
        /// </summary>
        /// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="args">The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="args"/> is equal to 100.</param>
        /// <param name="result">The result of the member invocation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {

            if (!base.TryInvokeMember(binder, args, out result))
            {
                result = Impromptu.InvokeGet(CallTarget, binder.Name);
                if (result == null)
                    return false;
                var tDel = result as Delegate;
                if (!binder.CallInfo.ArgumentNames.Any() && tDel != null)
                {
                    try
                    {
                        result = this.InvokeMethodDelegate(tDel, args);
                    }
                    catch (RuntimeBinderException)
                        //If it has out parmaters etc it can't be invoked dynamically like this.
                        //if we return false it will be handle by the GetProperty and then handled by the original dynamic invocation 
                    {
                        return false;
                    }
                }
                try
                {
                    result = Impromptu.Invoke(result, Util.NameArgsIfNecessary(binder.CallInfo, args));
                }
                catch (RuntimeBinderException)//If it has out parmaters etc it can't be invoked dynamically like this.
                //if we return false it will be handle by the GetProperty and then handled by the original dynamic invocation 
                {
                    return false;
                } 
            }

            return this.MassageResultBasedOnInterface(binder.Name, true, ref result);
        }


        /// <summary>
        /// Tries the index of the get.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="indexes">The indexes.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public override bool TryGetIndex(System.Dynamic.GetIndexBinder binder, object[] indexes, out object result)
        {
            if (base.TryGetIndex(binder, indexes, out result))
            {
                return this.MassageResultBasedOnInterface(Invocation.IndexBinderName, true, ref result);
            }
            return false;
        }
    }

}
