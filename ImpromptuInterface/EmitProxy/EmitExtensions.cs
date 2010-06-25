

namespace ImpromptuInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using Microsoft.CSharp.RuntimeBinder;
    using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

    public static class EmitExtensions
    {
        public class BranchTrueOverBlock : IDisposable
        {
            private readonly ILGenerator _generator;
            private readonly Label _label;

            public BranchTrueOverBlock(ILGenerator generator)
            {
                _generator = generator;
                _label = generator.DefineLabel();
                _generator.Emit(OpCodes.Brtrue, _label);
            }

            public void Dispose()
            {
                //_generator.Emit(OpCodes.Br_S, _label);
                _generator.MarkLabel(_label);
            }
        }

        public static FieldInfo GetFieldEvenIfGeneric(this Type type, string fieldName)
        {
            if (type is TypeBuilder)
            {
                var tGenDef = type.GetGenericTypeDefinition();
                var tField = tGenDef.GetField(fieldName);
                return TypeBuilder.GetField(type, tField);
            }
            return type.GetField(fieldName);
        }

        public static MethodInfo GetMethodEvenIfGeneric(this Type type, string methodName, Type[] argTypes)
        {
            if (type is TypeBuilder)
            {
                var tGenDef = type.GetGenericTypeDefinition();
                var tMethodInfo = tGenDef.GetMethod(methodName, argTypes);
                return TypeBuilder.GetMethod(type, tMethodInfo);
            }
            return type.GetMethod(methodName, argTypes);
        }


        public static MethodInfo GetMethodEvenIfGeneric(this Type type, string methodName)
        {
            if (type is TypeBuilder)
            {
                var tGenDef = type.GetGenericTypeDefinition();
                var tMethodInfo = tGenDef.GetMethod(methodName);
                return TypeBuilder.GetMethod(type, tMethodInfo);
            }
            return type.GetMethod(methodName);
        }

        public static BranchTrueOverBlock EmitBranchTrue(this ILGenerator generator)
        {
            return new BranchTrueOverBlock(generator);
        }


        public static void EmitCallsiteCreate(this ILGenerator generator, Type funcType)
        {
            generator.Emit(OpCodes.Call, typeof(CallSite<>).MakeGenericType(funcType).GetMethodEvenIfGeneric("Create", new[] { typeof(CallSiteBinder) }));
        }

        public static void EmitCallInvokeFunc(this ILGenerator generator, Type funcType, bool isAction = false)
        {
            generator.Emit(OpCodes.Callvirt, funcType.GetMethodEvenIfGeneric("Invoke"));
        }

        public static void EmitArray(this ILGenerator generator, Type arrayType, IList<Action<ILGenerator>> emitElements)
        {
            var tLocal = generator.DeclareLocal(arrayType.MakeArrayType());
            generator.Emit(OpCodes.Ldc_I4, emitElements.Count);
            generator.Emit(OpCodes.Newarr, arrayType);
            generator.EmitStoreLocation(tLocal.LocalIndex);

            for (var i = 0; i < emitElements.Count; i++)
            {
                generator.EmitLoadLocation(tLocal.LocalIndex);
                generator.Emit(OpCodes.Ldc_I4, i);
                emitElements[i](generator);
                generator.Emit(OpCodes.Stelem_Ref);
            }
            generator.EmitLoadLocation(tLocal.LocalIndex);
        }

        public static void EmitStoreLocation(this ILGenerator generator, int location)
        {
            switch (location)
            {
                case 0:
                    generator.Emit(OpCodes.Stloc_0);
                    return;
                case 1:
                    generator.Emit(OpCodes.Stloc_1);
                    return;
                case 2:
                    generator.Emit(OpCodes.Stloc_2);
                    return;
                case 3:
                    generator.Emit(OpCodes.Stloc_3);
                    return;
                default:
                    generator.Emit(OpCodes.Stloc, location);
                    return;
            }
        }


        public static void EmitLoadArgument(this ILGenerator generator, int location)
        {
            switch (location)
            {
                case 0:
                    generator.Emit(OpCodes.Ldarg_0);
                    return;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    return;
                case 2:
                    generator.Emit(OpCodes.Ldarg_2);
                    return;
                case 3:
                    generator.Emit(OpCodes.Ldarg_3);
                    return;
                default:
                    generator.Emit(OpCodes.Ldarg, location);
                    return;
            }
        }

        public static void EmitLoadLocation(this ILGenerator generator, int location)
        {
            switch (location)
            {
                case 0:
                    generator.Emit(OpCodes.Ldloc_0);
                    return;
                case 1:
                    generator.Emit(OpCodes.Ldloc_1);
                    return;
                case 2:
                    generator.Emit(OpCodes.Ldloc_2);
                    return;
                case 3:
                    generator.Emit(OpCodes.Ldloc_3);
                    return;
                default:
                    generator.Emit(OpCodes.Ldloc, location);
                    return;
            }
        }


        public static void EmitDynamicMethodInvokeBinder(this ILGenerator generator, CSharpBinderFlags flag, string name, Type context, params Type[] argTypes)
        {
            generator.Emit(OpCodes.Ldc_I4, (int)flag);
            generator.Emit(OpCodes.Ldstr, name);
            generator.Emit(OpCodes.Ldnull);
            generator.EmitTypeOf(context);
            var tList = new List<Action<ILGenerator>> { gen => gen.EmitCreateCSharpArgumentInfo(CSharpArgumentInfoFlags.None) };
            tList.AddRange(argTypes.Select(arg => (Action<ILGenerator>)(gen => gen.EmitCreateCSharpArgumentInfo(CSharpArgumentInfoFlags.UseCompileTimeType))));
            generator.EmitArray(typeof(CSharpArgumentInfo), tList);
            generator.Emit(OpCodes.Call, typeof(Binder).GetMethod("InvokeMember", new[] { typeof(CSharpBinderFlags), typeof(string), typeof(IEnumerable<Type>), typeof(Type), typeof(CSharpArgumentInfo[]) }));
        }

        public static void EmitDynamicSetBinder(this ILGenerator generator, CSharpBinderFlags flag, string name, Type context, params Type[] argTypes)
        {
            generator.Emit(OpCodes.Ldc_I4, (int)flag);
            if (argTypes.Length == 1)
                generator.Emit(OpCodes.Ldstr, name);
            generator.EmitTypeOf(context);
            var tList = new List<Action<ILGenerator>> { gen => gen.EmitCreateCSharpArgumentInfo(CSharpArgumentInfoFlags.None) };
            tList.AddRange(argTypes.Select(tArg => (Action<ILGenerator>)(gen => gen.EmitCreateCSharpArgumentInfo(CSharpArgumentInfoFlags.UseCompileTimeType))));
            generator.EmitArray(typeof(CSharpArgumentInfo), tList);

            if (argTypes.Length == 1)
                generator.Emit(OpCodes.Call, typeof(Binder).GetMethod("SetMember", new[] { typeof(CSharpBinderFlags), typeof(string), typeof(Type), typeof(CSharpArgumentInfo[]) }));
            else
                generator.Emit(OpCodes.Call, typeof(Binder).GetMethod("SetIndex", new[] { typeof(CSharpBinderFlags), typeof(Type), typeof(CSharpArgumentInfo[]) }));


        }


        public static void EmitDynamicGetBinder(this ILGenerator generator, CSharpBinderFlags flag, string name, Type context, params Type[] argTypes)
        {
            generator.Emit(OpCodes.Ldc_I4, (int)flag);
            if (!argTypes.Any())
                generator.Emit(OpCodes.Ldstr, name);
            generator.EmitTypeOf(context);
            var tList = new List<Action<ILGenerator>> { gen => gen.EmitCreateCSharpArgumentInfo(CSharpArgumentInfoFlags.None) };
            tList.AddRange(argTypes.Select(tArg => (Action<ILGenerator>)(gen => gen.EmitCreateCSharpArgumentInfo(CSharpArgumentInfoFlags.UseCompileTimeType))));
            generator.EmitArray(typeof(CSharpArgumentInfo), tList);
            if (!argTypes.Any())
                generator.Emit(OpCodes.Call, typeof(Binder).GetMethod("GetMember", new[] { typeof(CSharpBinderFlags), typeof(string), typeof(Type), typeof(CSharpArgumentInfo[]) }));
            else
                generator.Emit(OpCodes.Call, typeof(Binder).GetMethod("GetIndex", new[] { typeof(CSharpBinderFlags), typeof(Type), typeof(CSharpArgumentInfo[]) }));
        }

        public static void EmitCreateCSharpArgumentInfo(this ILGenerator generator, CSharpArgumentInfoFlags flag, string name = null)
        {
            generator.Emit(OpCodes.Ldc_I4, (int)flag);
            if (String.IsNullOrEmpty(name))
                generator.Emit(OpCodes.Ldnull);
            else
                generator.Emit(OpCodes.Ldstr, name);
            generator.Emit(OpCodes.Call, typeof(CSharpArgumentInfo).GetMethod("Create", new[] { typeof(CSharpArgumentInfoFlags), typeof(string) }));
        }


        public static void EmitDynamicConvertBinder(this ILGenerator generator, CSharpBinderFlags flag, Type returnType, Type context)
        {
            generator.Emit(OpCodes.Ldc_I4, (int)flag);
            generator.EmitTypeOf(returnType);
            generator.EmitTypeOf(context);
            generator.Emit(OpCodes.Call, typeof(Binder).GetMethod("Convert", new[] { typeof(CSharpBinderFlags), typeof(Type), typeof(Type) }));
        }

        public static void EmitTypeOf(this ILGenerator generator, Type type)
        {

            generator.Emit(OpCodes.Ldtoken, type);
            var tTypeMeth = typeof(Type).GetMethod("GetTypeFromHandle", new[] { typeof(RuntimeTypeHandle) });
            generator.Emit(OpCodes.Call, tTypeMeth);
        }

        public static void EmitTypeOf(this ILGenerator generator, TypeToken type)
        {

            generator.Emit(OpCodes.Ldtoken, type.Token);
            var tTypeMeth = typeof(Type).GetMethod("GetTypeFromHandle", new[] { typeof(RuntimeTypeHandle) });
            generator.Emit(OpCodes.Call, tTypeMeth);
        }
    }
}
