using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ImpromptuInterface.Build;
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
                                                  new List<CSharpArgumentInfo>()
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
                                               new List<CSharpArgumentInfo>()
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

        internal static object InvokeMemberCallSite(object target,  String_OR_InvokeMemberName name, object[] args, string[] tArgNames, Type tContext, bool tStaticContext, ref CallSite callSite)
        {
            CallSiteBinder tBinder = null;

            if (callSite == null)
            {
                var tList = Impromptu.GetBindingArgumentList(args, tArgNames,  tContext, tStaticContext);
                var tFlag = CSharpBinderFlags.None;
                if (name.IsSpecialName)
                {
                    tFlag |= CSharpBinderFlags.InvokeSpecialName;
                }
                tBinder = Binder.InvokeMember(tFlag, name.Name, name.GenericArgs,
                                              tContext, tList);

            }


            return InvokeMember<object>(ref callSite, tBinder, name, tStaticContext, tContext,tArgNames, target, args);
        }

        internal static object InvokeGetIndexCallSite(object target, object[] indexes, string[] argNames, Type context, bool tStaticContext,ref CallSite callSite)
        {
            CallSiteBinder tBinder=null;
            if (callSite == null)
            {
                var tList = Impromptu.GetBindingArgumentList(indexes, argNames, context, tStaticContext);
                tBinder = Binder.GetIndex(CSharpBinderFlags.None, context, tList);
            }

            return InvokeMember<object>(ref callSite, tBinder, Invocation.IndexBinderName, tStaticContext, context, argNames, target, indexes);
        }

        internal static void InvokeSetIndexCallSite(object target, object[] indexesThenValue, string[] tArgNames, Type tContext, bool tStaticContext, CallSite tCallSite)
        {
            CallSiteBinder tBinder =null;
            if (tCallSite == null)
            {
                var tList = Impromptu.GetBindingArgumentList(indexesThenValue, tArgNames, tContext, tStaticContext);
                tBinder = Binder.SetIndex(CSharpBinderFlags.None, tContext, tList);
            }

            InvokeMemberAction(ref tCallSite, tBinder, Invocation.IndexBinderName, tStaticContext, tContext, tArgNames, target, indexesThenValue);
        }

        internal static void InvokeMemberActionCallSite(object target,String_OR_InvokeMemberName name, object[] args, string[] tArgNames, Type tContext, bool tStaticContext,ref CallSite callSite)
        {
            CallSiteBinder tBinder =null;
            if (callSite == null)
            {


                IEnumerable<CSharpArgumentInfo> tList;
                tList = Impromptu.GetBindingArgumentList(args, tArgNames, tContext, tStaticContext);

                var tFlag = CSharpBinderFlags.ResultDiscarded;
                if (name.IsSpecialName)
                {
                    tFlag |= CSharpBinderFlags.InvokeSpecialName;
                }

                tBinder = Binder.InvokeMember(tFlag, name.Name, name.GenericArgs,
                                                     tContext, tList);
            }


            InvokeMemberAction(ref callSite, tBinder, name, tStaticContext, tContext, tArgNames, target, args);
        }

        internal static bool InvokeIsEventCallSite(object target, string name, Type tContext, ref CallSite callSite)
        {
            if (callSite == null)
            {
                var tBinder = Binder.IsEvent(CSharpBinderFlags.None, name, tContext);

                callSite = Impromptu.CreateCallSite<Func<CallSite, object, bool>>(tBinder, name, tContext, isEvent: true);
            }
            var tCallSite = (CallSite<Func<CallSite, object, bool>>)callSite;

            return tCallSite.Target(tCallSite, target);
        }

        internal static void InvokeAddAssignCallSite(object target, string name, object[] args, string[] argNames, Type context, bool staticContext, ref CallSite callSiteIsEvent, ref CallSite callSiteAdd, ref CallSite callSiteGet, ref CallSite callSiteSet)
        {

            if (InvokeIsEventCallSite(target, name, context, ref callSiteIsEvent))
            {
                InvokeMemberActionCallSite(target, InvokeMemberName.CreateSpecialName("add_" + name), args, argNames, context, staticContext, ref callSiteAdd);
            }
            else
            {
                dynamic tGet = InvokeGetCallSite(target,name, context, staticContext, ref callSiteGet);
                tGet += (dynamic)(args.First());
                InvokeSetCallSite(target, name,  (object)tGet, context, staticContext, ref callSiteSet);
            }
        }

        internal static void InvokeSubtractAssignCallSite(object target, string name, object[] args, string[] argNames, Type context, bool staticContext, ref CallSite callSiteIsEvent, ref CallSite callSiteRemove, ref CallSite callSiteGet, ref CallSite callSiteSet)
        {
            if (InvokeIsEventCallSite(target, name, context, ref callSiteIsEvent))
            {
                InvokeMemberActionCallSite(target, InvokeMemberName.CreateSpecialName("remove_" + name), args, argNames, context, staticContext, ref callSiteRemove);
            }
            else
            {
                dynamic tGet = InvokeGetCallSite(target, name, context, staticContext, ref callSiteGet);
                tGet -= (dynamic)(args.First());
                InvokeHelper.InvokeSetCallSite(target, name, tGet, context, staticContext, ref callSiteSet);
            }
        }

        internal static object InvokeConvertCallSite(object target, bool explict, Type type, Type context, ref CallSite callSite)
        {
            if (callSite == null)
            {
                var tFlags = explict ? CSharpBinderFlags.ConvertExplicit : CSharpBinderFlags.None;

                var tBinder = Binder.Convert(tFlags, type, context);

                var tFunc = BuildProxy.GenerateCallSiteFuncType(new Type[] {}, type);


                callSite = Impromptu.CreateCallSite(tFunc, tBinder,
                                          explict
                                              ? Invocation.ExplicitConvertBinderName
                                              : Invocation.ImplicitConvertBinderName, context);
            }
            dynamic tDynCallSite = callSite;
            return tDynCallSite.Target(callSite, target);

        }

        internal static object InvokeConstructorCallSite(Type type, bool isValueType, object[] args, string[] argNames,Type context, ref CallSite callSite)
        {
            CallSiteBinder tBinder = null;
            if (callSite == null || isValueType)
            {
                if (isValueType && args.Length == 0)  //dynamic invocation doesn't see constructors of value types
                {
                    return Activator.CreateInstance(type);
                }

                var tList = Impromptu.GetBindingArgumentList(args, argNames, context, true);
                tBinder = Binder.InvokeConstructor(CSharpBinderFlags.None, type, tList);
            }


            if (isValueType || Util.IsMono)
            {
                CallSite tDummy =null;
                return DynamicInvokeStaticMember(type, ref tDummy, tBinder, Invocation.ConstructorBinderName, true, type,
                                                              argNames, type, args);
            }
            return InvokeMemberTargetType<Type, object>(ref callSite, tBinder, Invocation.ConstructorBinderName, true, type, argNames,
                                                                     type, args);
        }
    }
}
