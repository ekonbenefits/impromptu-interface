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
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Dynamic
{
    ///<summary>
    /// Proxies Calls allows subclasser to override do extra actions before or after base invocation
    ///</summary>
    /// <remarks>
    /// This may not be as efficient as other proxies that can work on just static objects or just dynamic objects
    /// Consider this class a work in progress
    /// </remarks>
    public abstract class ImpromptuForwarder:ImpromptuObject
    {
        protected ImpromptuForwarder(object target)
        {
            Target = target;
        }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public object Target {  get; protected set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
          
            result = Impromptu.InvokeGet(Target, binder.Name);

            return true;

        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            object[] tArgs = NameArgsIfNecessary(binder.CallInfo, args);

            Type tType;
            if (TryTypeForName(binder.Name, out tType))
            {
                return InvokeMember(tType == typeof(void), binder, tArgs, out result);
            }

            bool tVoidBool;
            try
            {
                var tProp = Target.GetType().GetMethod(binder.Name);
                tVoidBool = tProp.ReturnType == typeof (void);
            }
            catch (AmbiguousMatchException)
            {

                tVoidBool =false;
            }

            try
            {
                return InvokeMember(tVoidBool, binder, tArgs, out result);
            }
            catch (RuntimeBinderException)
            {
                return InvokeMember(!tVoidBool, binder, tArgs, out result);
            }
        }

        private bool InvokeMember(bool isVoid,InvokeMemberBinder binder, object[] args, out object result)
        {

            if (isVoid)
            {
                Impromptu.InvokeMemberAction(Target, binder.Name, args);
                result = null;
            }
            else
            {
                result = Impromptu.InvokeMember(Target, binder.Name, args);
            }
            return true;
        }

    

        private object[] NameArgsIfNecessary(CallInfo callInfo, object[] args)
        {
            object[] tArgs;
            if (callInfo.ArgumentNames.Count == 0)
                tArgs =args;
            else
            {
                var tStop = callInfo.ArgumentCount - callInfo.ArgumentNames.Count;
                tArgs = Enumerable.Repeat(default(string), tStop).Concat(callInfo.ArgumentNames).Zip(args, (n, v) => n == null ? v : new InvokeArg(n, v)).ToArray();
            }
            return tArgs;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {

            Impromptu.InvokeSet(Target, binder.Name, value);

            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {

            object[] tArgs = NameArgsIfNecessary(binder.CallInfo, indexes);

            result = Impromptu.InvokeGetIndex(Target, tArgs);
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var tCombinedArgs = indexes.Concat(new[] { value }).ToArray();
            object[] tArgs = NameArgsIfNecessary(binder.CallInfo, tCombinedArgs);

            Impromptu.InvokeSetIndex(tArgs);
            return true;
        }
        
    }
}
