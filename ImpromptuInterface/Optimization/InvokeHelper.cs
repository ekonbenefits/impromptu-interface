using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Optimization
{
    internal static class InvokeHelper
    {
        internal static readonly Type[] FuncKinds;
        internal static readonly Type[] ActionKinds;
        static InvokeHelper()
        {
            FuncKinds = new []
                            {
                                typeof(Func<>), //0
                                typeof(Func<,>),
                                typeof(Func<,,>),
                                typeof(Func<,,,>),
                                typeof(Func<,,,,>),
                                typeof(Func<,,,,,>),
                                typeof(Func<,,,,,,>),
                                typeof(Func<,,,,,,,>),
                                typeof(Func<,,,,,,,,>),
                                typeof(Func<,,,,,,,,,>),
                                typeof(Func<,,,,,,,,,,>),
                                typeof(Func<,,,,,,,,,,,>),
                                typeof(Func<,,,,,,,,,,,,>),
                                typeof(Func<,,,,,,,,,,,,,>),
                                typeof(Func<,,,,,,,,,,,,,,>),
                                typeof(Func<,,,,,,,,,,,,,,,>),
                                typeof(Func<,,,,,,,,,,,,,,,,>), //16
                            };
            ActionKinds = new []
                            {
                                typeof(Action), //0
                                typeof(Action<>),
                                typeof(Action<,>),
                                typeof(Action<,,>),
                                typeof(Action<,,,>),
                                typeof(Action<,,,,>),
                                typeof(Action<,,,,,>),
                                typeof(Action<,,,,,,>),
                                typeof(Action<,,,,,,,>),
                                typeof(Action<,,,,,,,,>),
                                typeof(Action<,,,,,,,,,>),
                                typeof(Action<,,,,,,,,,,>),
                                typeof(Action<,,,,,,,,,,,>),
                                typeof(Action<,,,,,,,,,,,,>),
                                typeof(Action<,,,,,,,,,,,,,>),
                                typeof(Action<,,,,,,,,,,,,,,>),
                                typeof(Action<,,,,,,,,,,,,,,,>), //16
                            };
        }


        internal static void InvokeMemberAction(CallSiteBinder binder,
                                                    string name,
                                                    Type context,
                                                    object target, params dynamic[] args)
        {

            var tSwitch = args.Length;
            var tArgTypes = args.Select(it => (Type)it.GetType());
            if (tArgTypes.Any(it => it.IsNotPublic))
            {
                tSwitch = Int32.MaxValue;
            }
            switch (tSwitch)
            {
                case 0:
                    InvokeMemberActionHelper(binder, name, context, target);
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
                tSwitch = Int32.MaxValue;
            }
            switch (tSwitch)
            {
                case 0:
                    return InvokeMemberHelper(binder, name, context, target);
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

        private static void InvokeSetHelper<T>(object target, string name, T  value)
        {
            var tContext = target.GetType();
            var tBinder = Binder.SetMember(CSharpBinderFlags.ResultDiscarded, name,
                                                  tContext,
                                                  new List<CSharpArgumentInfo>()
                                                      {
                                                          CSharpArgumentInfo.Create(
                                                              CSharpArgumentInfoFlags.None, null),
                                                          CSharpArgumentInfo.Create(
                                                              CSharpArgumentInfoFlags.UseCompileTimeType, null)
				
                                                      });

            var tCallSite = Impromptu.CreateCallSite<Action<CallSite, object, T>>(tBinder, name, tContext);
            tCallSite.Target.Invoke(tCallSite, target, value);
        }

        internal static object FastDynamicInvokeReturn(Delegate del, object[] args)
        {
            dynamic tDel =del;
            switch(args.Length){
                default:
                    return del.DynamicInvoke(args);
                    #region Optimization
                case 1:
                    return tDel((dynamic)args[0]);
                case 2:
                    return tDel((dynamic)args[0],(dynamic)args[1]);
                case 3:
                    return tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2]);
                case 4:
                    return tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3]);
                case 5:
                    return tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4]);
                case 6:
                    return tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5]);
                case 7:
                    return tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6]);
                case 8:
                    return tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6],(dynamic)args[7]);
                case 9:
                    return tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6],(dynamic)args[7],(dynamic)args[8]);
                case 10:
                    return tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6],(dynamic)args[7],(dynamic)args[8],(dynamic)args[9]);
                case 11:
                    return tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6],(dynamic)args[7],(dynamic)args[8],(dynamic)args[9],(dynamic)args[10]);
                case 12:
                    return tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6],(dynamic)args[7],(dynamic)args[8],(dynamic)args[9],(dynamic)args[10],(dynamic)args[11]);
                    #endregion
            }
        }

        internal static void FastDynamicInvokeAction(Delegate del, params object[] args)
        {
            dynamic tDel =del;
            switch(args.Length){
                default:
                    del.DynamicInvoke(args);
                    return;
                    #region Optimization
                case 1:
                    tDel((dynamic)args[0]);
                    return;
                case 2:
                    tDel((dynamic)args[0],(dynamic)args[1]);
                    return;
                case 3:
                    tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2]);
                    return;
                case 4:
                    tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3]);
                    return;
                case 5:
                    tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4]);
                    return;
                case 6:
                    tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5]);
                    return;
                case 7:
                    tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6]);
                    return;
                case 8:
                    tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6],(dynamic)args[7]);
                    return;
                case 9:
                    tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6],(dynamic)args[7],(dynamic)args[8]);
                    return;
                case 10:
                    tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6],(dynamic)args[7],(dynamic)args[8],(dynamic)args[9]);
                    return;
                case 11:
                    tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6],(dynamic)args[7],(dynamic)args[8],(dynamic)args[9],(dynamic)args[10]);
                    return;
                case 12:
                    tDel((dynamic)args[0],(dynamic)args[1],(dynamic)args[2],(dynamic)args[3],(dynamic)args[4],(dynamic)args[5],(dynamic)args[6],(dynamic)args[7],(dynamic)args[8],(dynamic)args[9],(dynamic)args[10],(dynamic)args[11]);
                    return;
                    #endregion
            }
        }
    }
}
