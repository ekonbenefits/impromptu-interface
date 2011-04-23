using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ImpromptuInterface.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace ImpromptuInterface.Optimization
{

    internal class DummmyNull
    {

    }

    internal static partial class InvokeHelper
    {
        internal static object InvokeMethodDelegate(this object target, Delegate tFunc, object[] args)
        {
            object result;
            try
            {
                result = tFunc.FastDynamicInvoke(
                    tFunc.IsSpecialThisDelegate()
                        ? new[] { target }.Concat(args).ToArray()
                        : args
                    );
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw ex;
            }
            return result;
        }

        internal delegate object DynamicInvokeMemberConstructorValueType(
            CallSite funcSite,
            Type funcTarget,
            ref CallSite callsite,
            CallSiteBinder binder,
            String_OR_InvokeMemberName name,
            bool staticContext,
            Type context,
            string[] argNames,
            Type target,
            object[] args);

        internal static readonly IDictionary<Type, CallSite<DynamicInvokeMemberConstructorValueType>> _dynamicInvokeMemberSite = new Dictionary<Type, CallSite<DynamicInvokeMemberConstructorValueType>>();

        internal static dynamic DynamicInvokeStaticMember(Type tReturn, ref CallSite callsite, CallSiteBinder binder,
                                       string name,
                                     bool staticContext,
                                     Type context,
                                     string[] argNames,
                                     Type target, params object[] args)
        {
            CallSite<DynamicInvokeMemberConstructorValueType> tSite;
            if (!_dynamicInvokeMemberSite.TryGetValue(tReturn, out tSite))
            {
                tSite = CallSite<DynamicInvokeMemberConstructorValueType>.Create(
                        Binder.InvokeMember(
                            CSharpBinderFlags.None,
                            "InvokeMemberTargetType",
                            new[] { typeof(Type), tReturn },
                            typeof(InvokeHelper),
                            new[]
                                {
                                    CSharpArgumentInfo.Create(
                                        CSharpArgumentInfoFlags.IsStaticType |
                                        CSharpArgumentInfoFlags.UseCompileTimeType, null),
                                     CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsRef, null),
                                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
                                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
                                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
                                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
                                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
                                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
                                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
                                }
                            )
                    );
                _dynamicInvokeMemberSite[tReturn] = tSite;
            }

            return tSite.Target(tSite, typeof(InvokeHelper), ref callsite, binder, name, staticContext, context, argNames, target, args);
        }


        internal static TReturn InvokeMember<TReturn>(ref CallSite callsite, CallSiteBinder binder,
                                       String_OR_InvokeMemberName name,
                                     bool staticContext,
                                     Type context,
                                     string[] argNames,
                                     object target, params object[] args)
        {
            return InvokeMemberTargetType<object, TReturn>(ref callsite, binder, name, staticContext, context, argNames, target, args);
        }

        internal static object InvokeGetCallSite(object target, string name, Type context, bool staticContext, ref CallSite callsite)
        {
            if (callsite == null)
            {
                var tTargetFlag = CSharpArgumentInfoFlags.None;
                CallSiteBinder tBinder;
                if (staticContext) //CSharp Binder won't call Static properties, grrr.
                {
                    var tStaticFlag = CSharpBinderFlags.None;
                    if (Util.IsMono) //Mono only works if InvokeSpecialName is set and .net only works if it isn't
                        tStaticFlag |= CSharpBinderFlags.InvokeSpecialName;

                    tBinder = Binder.InvokeMember(tStaticFlag, "get_" + name,
                                                         null,
                                                         context,
                                                         new List<CSharpArgumentInfo>()
                                                             {
                                                                 CSharpArgumentInfo.Create(
                                                                     CSharpArgumentInfoFlags.IsStaticType |
                                                                     CSharpArgumentInfoFlags.UseCompileTimeType,
                                                                     null)
                                                             });
                }
                else
                {

                    tBinder = Binder.GetMember(CSharpBinderFlags.None, name,
                                                      context,
                                                      new List<CSharpArgumentInfo>()
                                                          {
                                                              CSharpArgumentInfo.Create(
                                                                  tTargetFlag, null)
                                                          });

                }


                callsite = Impromptu.CreateCallSite<Func<CallSite, object, object>>(tBinder, name, context);
            }
            var tCallSite = (CallSite<Func<CallSite, object, object>>) callsite;
            return tCallSite.Target(tCallSite, target);
            
        }

        internal static void InvokeSetCallSite(object target, string name, object value, Type context, bool staticContext, ref CallSite callSite)
        {
            if (callSite == null)
            {
                CallSiteBinder tBinder;
                if (staticContext) //CSharp Binder won't call Static properties, grrr.
                {

                    var tStaticFlag = CSharpBinderFlags.ResultDiscarded;
                    if (Util.IsMono) //Mono only works if InvokeSpecialName is set and .net only works if it isn't
                        tStaticFlag |= CSharpBinderFlags.InvokeSpecialName;

                    tBinder = Binder.InvokeMember(tStaticFlag, "set_" + name,
                                                  null,
                                                  context,
                                                  new List<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>()
                                                      {
                                                          CSharpArgumentInfo.Create(
                                                              CSharpArgumentInfoFlags.IsStaticType |
                                                              CSharpArgumentInfoFlags.UseCompileTimeType, null),
                                                          CSharpArgumentInfo.Create(

                                                              CSharpArgumentInfoFlags.None

                                                              , null)
                                                      });
                }
                else
                {

                    tBinder = Binder.SetMember(CSharpBinderFlags.ResultDiscarded, name,
                                               context,
                                               new List<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>()
                                                   {
                                                       CSharpArgumentInfo.Create(
                                                           CSharpArgumentInfoFlags.None, null),
                                                       CSharpArgumentInfo.Create(

                                                           CSharpArgumentInfoFlags.None

                                                           , null)

                                                   });
                }


                callSite = Impromptu.CreateCallSite<Action<CallSite, object, object>>(tBinder, name, context);
            }
            var tCallSite = (CallSite<Action<CallSite, object, object>>) callSite;
            tCallSite.Target(callSite, target, value);
        }
    }
}
