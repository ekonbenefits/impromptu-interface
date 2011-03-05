using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImpromptuInterface.Optimization
{
    internal static class InvokeHelper
    {

        internal static void InvokeMemberAction(CallSiteBinder binder,
                                                    string name,
                                                    Type context,
                                                    object target, params dynamic[] args)
        {

            var tSwitch = args.Length;
            var tArgTypes = args.Select(it => (Type)it.GetType());
            if (tArgTypes.Any(it => it.IsNotPublic))
            {
                tSwitch = int.MaxValue;
            }
            switch (tSwitch)
            {
                case 0:
                    InvokeHelper.InvokeMemberActionHelper(binder, name, context, target);
                    break;
                case 1:
                    InvokeHelper.InvokeMemberActionHelper(binder, name, context, target, args[0]);
                    break;
                case 2:
                    InvokeHelper.InvokeMemberActionHelper(binder, name, context, target, args[0], args[1]);
                    break;
                case 3:
                    InvokeHelper.InvokeMemberActionHelper(binder, name, context, target, args[0], args[1], args[2]);
                    break;
                default:

                    var tDelagateType = BuildProxy.GenerateCallSiteFuncType(tArgTypes, typeof(void));
                    Impromptu.Invoke(Impromptu.CreateCallSite(tDelagateType, binder, name, context), target, args);
                    break;

            }
        }

        internal static object InvokeMember(CallSiteBinder binder,
                                            string name,
                                          Type context,
                                          object target, params dynamic[] args)
        {

            var tSwitch = args.Length;
            var tArgTypes = args.Select(it => (Type)it.GetType());
            if (tArgTypes.Any(it => it.IsNotPublic))
            {
                tSwitch = int.MaxValue;
            }
            switch (tSwitch)
            {
                case 0:
                    return InvokeHelper.InvokeMemberHelper(binder, name, context, target);
                case 1:
                    return InvokeHelper.InvokeMemberHelper(binder, name, context, target, args[0]);
                case 2:
                    return InvokeHelper.InvokeMemberHelper(binder, name, context, target, args[0], args[1]);
                case 3:
                    return InvokeHelper.InvokeMemberHelper(binder, name, context, target, args[0], args[1], args[2]);
                default:

                    var tDelagateType = BuildProxy.GenerateCallSiteFuncType(tArgTypes, typeof(object));
                    return Impromptu.Invoke(Impromptu.CreateCallSite(tDelagateType, binder, name, context), target, args);

            }
        }


        private static void InvokeMemberActionHelper(
                                                    CallSiteBinder binder,
                                                    string name,
                                                    Type context,
                                                    object target)
        {

            var tCallSite = Impromptu.CreateCallSite<Action<CallSite, object>>(binder, name, context);
            tCallSite.Target.Invoke(tCallSite, target);
        }
        private static void InvokeMemberActionHelper<T1>(
                                                    CallSiteBinder binder,
                                                    string name,
                                                    Type context,
                                                    object target, T1 arg1)
        {

            var tCallSite = Impromptu.CreateCallSite<Action<CallSite, object, T1>>(binder, name, context);
            tCallSite.Target.Invoke(tCallSite, target, arg1);
        }
        private static void InvokeMemberActionHelper<T1, T2>(
                                                    CallSiteBinder binder,
                                                    string name,
                                                    Type context,
                                                    object target, T1 arg1, T2 arg2)
        {

            var tCallSite = Impromptu.CreateCallSite<Action<CallSite, object, T1, T2>>(binder, name, context);
            tCallSite.Target.Invoke(tCallSite, target, arg1, arg2);
        }
        private static void InvokeMemberActionHelper<T1, T2, T3>(
                                                    CallSiteBinder binder,
                                                    string name,
                                                    Type context,
                                                    object target, T1 arg1, T2 arg2, T3 arg3)
        {

            var tCallSite = Impromptu.CreateCallSite<Action<CallSite, object, T1, T2, T3>>(binder, name, context);
            tCallSite.Target.Invoke(tCallSite, target, arg1, arg2, arg3);
        }


        private static object InvokeMemberHelper(
                                                    CallSiteBinder binder,
                                                    string name,
                                                    Type context,
                                                    object target)
        {

            var tCallSite = Impromptu.CreateCallSite<Func<CallSite, object, object>>(binder, name, context);
            return tCallSite.Target.Invoke(tCallSite, target);
        }
        private static object InvokeMemberHelper<T1>(
                                                    CallSiteBinder binder,
                                                    string name,
                                                    Type context,
                                                    object target, T1 arg1)
        {

            var tCallSite = Impromptu.CreateCallSite<Func<CallSite, object, T1, object>>(binder, name, context);
            return tCallSite.Target.Invoke(tCallSite, target, arg1);
        }
        private static object InvokeMemberHelper<T1, T2>(
                                                    CallSiteBinder binder,
                                                    string name,
                                                    Type context,
                                                    object target, T1 arg1, T2 arg2)
        {

            var tCallSite = Impromptu.CreateCallSite<Func<CallSite, object, T1, T2, object>>(binder, name, context);
            return tCallSite.Target.Invoke(tCallSite, target, arg1, arg2);
        }
        private static object InvokeMemberHelper<T1, T2, T3>(
                                                    CallSiteBinder binder,
                                                    string name,
                                                    Type context,
                                                    object target, T1 arg1, T2 arg2, T3 arg3)
        {

            var tCallSite = Impromptu.CreateCallSite<Func<CallSite, object, T1, T2, T3, object>>(binder, name, context);
            return tCallSite.Target.Invoke(tCallSite, target, arg1, arg2, arg3);
        }
    }
}
