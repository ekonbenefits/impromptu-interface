using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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


        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
          
            result = Impromptu.InvokeGet(Target, binder.Name);

            return true;

        }

        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {
            Type tType;
            if (TryTypeForName(binder.Name, out tType))
            {
               return InvokeMember(tType == typeof(void), binder, args, out result);
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


            return InvokeMember(tVoidBool, binder, args, out result);
        }

        private bool InvokeMember(bool isVoid,System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {
            object[] tArgs;
            if(binder.CallInfo.ArgumentNames.Count == 0)
                 tArgs =args;
            else
            {
                var tStop = binder.CallInfo.ArgumentCount - binder.CallInfo.ArgumentNames.Count;
                tArgs =  Enumerable.Repeat(default(string), tStop).Concat(binder.CallInfo.ArgumentNames).Zip(args, (n, v) => n == null ? v : new InvokeArg(n, v)).ToArray();
            }

            if (isVoid)
            {
                Impromptu.InvokeMemberAction(Target,binder.Name, tArgs);
                result = null;
            }
            else
            {
                result = Impromptu.InvokeMember(Target, binder.Name, tArgs);
            }
            return true;
        }

        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {

            Impromptu.InvokeSet(Target, binder.Name, value);

            return true;
        }

        
    }
}
