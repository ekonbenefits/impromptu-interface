

namespace ImpromptuInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using Microsoft.CSharp.RuntimeBinder;

    public static class BuildProxy
    {
        public class TypeHash
        {
            public bool Equals(TypeHash other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Types.SequenceEqual(other.Types);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(TypeHash)) return false;
                return Equals((TypeHash)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return Types.Aggregate(1, (current, type) => (current * 397) ^ type.GetHashCode());
                }
            }

            public static bool operator ==(TypeHash left, TypeHash right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TypeHash left, TypeHash right)
            {
                return !Equals(left, right);
            }

            public readonly Type[] Types;
            public TypeHash(Type type1, Type type2, params Type[] moreTypes)
            {
                Types = new[] { type1,type2 }.Concat(moreTypes).OrderBy(it => it.Name).ToArray();
            }
        }

        private static ModuleBuilder _builder;
        private static AssemblyBuilder _ab;
        private static readonly Dictionary<TypeHash, Type> _typeHash = new Dictionary<TypeHash, Type>();

      

        public static Type BuildType(Type wrappedType, Type mainInterface, params Type[] otherInterfaces)
        {
            lock ("com.ImpromtuInterface.typecache")
            {
                var tNewHash = new TypeHash(wrappedType, mainInterface, otherInterfaces);

                if (!_typeHash.ContainsKey(tNewHash))
                {
                    _typeHash[tNewHash] = BuildTypeHelper(wrappedType,new[]{mainInterface}.Concat(otherInterfaces).ToArray());
                }

                return _typeHash[tNewHash];
            }

        }

        private static Type BuildTypeHelper(Type contextType,params Type[] interfaces)
        {


            var tB = Builder.DefineType(
                string.Format("ActsLike_{0}_{1}", interfaces.First().Name, Guid.NewGuid().ToString("N")), TypeAttributes.Public | TypeAttributes.Class,
                typeof(ActsLikeProxy), interfaces);

            var tC = tB.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig, CallingConventions.HasThis, new[] { typeof(object) });
            tC.DefineParameter(1, ParameterAttributes.None, "original");
            var tConstInfo = typeof(ActsLikeProxy).GetConstructor(new[] { typeof(object) });

            var tCIl = tC.GetILGenerator();
            tCIl.Emit(OpCodes.Ldarg_0);
            tCIl.Emit(OpCodes.Ldarg_1);
            tCIl.Emit(OpCodes.Call, tConstInfo);
            tCIl.Emit(OpCodes.Ret);

            var tInterfaces = interfaces.Concat(interfaces.SelectMany(it => it.GetInterfaces()));

            foreach (var tInterface in tInterfaces)
            {
                CreateProperties(tB, tInterface.GetProperties(BindingFlags.Public | BindingFlags.Instance), contextType);
                CreateMethods(tB, tInterface.GetMethods(BindingFlags.Public | BindingFlags.Instance),contextType);
            }
            var tType = tB.CreateType();
            return tType;
        }

        private static void MakeMethod(MethodInfo info, TypeBuilder typeBuilder, Type contextType)
        {
            var tName = info.Name;

            var tParamTypes = info.GetParameters().Select(it => it.ParameterType).ToArray();

            var tReturnType = info.ReturnParameter.ParameterType;


            var tCallSiteInvokeName = string.Format("Impromptu_Callsite_{1}_{0}", Guid.NewGuid().ToString("N"), tName);
            var tCStp = Builder.DefineType(tCallSiteInvokeName,
                                            TypeAttributes.NotPublic
                                            | TypeAttributes.Sealed
                                            | TypeAttributes.Class);
            var tGenericParams = tParamTypes.Where(it => it.IsGenericParameter).Distinct().ToDictionary(it => it.GenericParameterPosition, it => new { Type = it, Gen = default(GenericTypeParameterBuilder) });
            if (tReturnType.IsGenericParameter && !tGenericParams.ContainsKey(tReturnType.GenericParameterPosition))
                tGenericParams.Add(tReturnType.GenericParameterPosition, new { Type = tReturnType, Gen = default(GenericTypeParameterBuilder) });
            var tGenParams = tGenericParams.OrderBy(it => it.Key).Select(it => it.Value.Type.Name);
            if (tGenParams.Any())
            {
                var tBuilders = tCStp.DefineGenericParameters(tGenParams.ToArray());
                var tDict = tGenericParams.ToDictionary(param => param.Value.Type, param => tBuilders[param.Key]);
                if (tDict.ContainsKey(tReturnType))
                {
                    tReturnType = tDict[tReturnType];
                }
                tParamTypes = tParamTypes.Select(it => tDict.ContainsKey(it) ? tDict[it] : it).ToArray();
            }

            var tConvert = "Convert_Method";
            Type tConvertFuncType = null;
            if (tReturnType != typeof(void))
            {
                tConvertFuncType = tCStp.DefineCallsiteField(tConvert, tReturnType);
            }

            var tInvokeMethod = "Invoke_Method";
            var tInvokeFuncType = tCStp.DefineCallsiteField(tInvokeMethod, tReturnType != typeof(void) ? typeof(object) : typeof(void), tParamTypes);

            var tCallSite = tCStp.CreateType();

            var tMethodBuilder = typeBuilder.DefineMethod(tName,
                                                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot);

            tGenericParams = tParamTypes.Where(it => it.IsGenericParameter).Distinct().ToDictionary(it => it.GenericParameterPosition, it => new { Type = it, Gen = default(GenericTypeParameterBuilder) });
            if (tReturnType.IsGenericParameter && !tGenericParams.ContainsKey(tReturnType.GenericParameterPosition))
                tGenericParams.Add(tReturnType.GenericParameterPosition, new { Type = tReturnType, Gen = default(GenericTypeParameterBuilder) });
            tGenParams = tGenericParams.OrderBy(it => it.Key).Select(it => it.Value.Type.Name);
            if (tGenParams.Any())
            {
                var tMethodGenerics = tMethodBuilder.DefineGenericParameters(tGenParams.ToArray());
                var tDict = tGenericParams.ToDictionary(param => param.Value.Type, param => tMethodGenerics[param.Key]);
                if (tDict.ContainsKey(tReturnType))
                {
                    tReturnType = tDict[tReturnType];
                }
                tParamTypes = tParamTypes.Select(it => tDict.ContainsKey(it) ? tDict[it] : it).ToArray();

                tCallSite = tCallSite.MakeGenericType(tMethodGenerics);

                tConvertFuncType = UpdateGenericParametesr(tConvertFuncType, tDict);
                tInvokeFuncType = UpdateGenericParametesr(tInvokeFuncType, tDict);
            }
            tMethodBuilder.SetReturnType(tReturnType);
            tMethodBuilder.SetParameters(tParamTypes);

            foreach (var tParam in info.GetParameters())
            {
                tMethodBuilder.DefineParameter(tParam.Position + 1, AttributesForParam(tParam), tParam.Name);
            }


            var tIlGen = tMethodBuilder.GetILGenerator();

            var tConvertField = tCallSite.GetFieldEvenIfGeneric(tConvert);
            if (tReturnType != typeof(void))
            {

                tIlGen.Emit(OpCodes.Ldsfld, tConvertField);
                using (tIlGen.EmitBranchTrue())
                {
                    tIlGen.EmitDynamicConvertBinder(CSharpBinderFlags.None, tReturnType, contextType);
                    tIlGen.EmitCallsiteCreate(tConvertFuncType);
                    tIlGen.Emit(OpCodes.Stsfld, tConvertField);
                }
            }

            var tInvokeField = tCallSite.GetFieldEvenIfGeneric(tInvokeMethod);

            tIlGen.Emit(OpCodes.Ldsfld, tInvokeField);
            using (tIlGen.EmitBranchTrue())
            {
                tIlGen.EmitDynamicMethodInvokeBinder(tReturnType == typeof(void) ? CSharpBinderFlags.ResultDiscarded : CSharpBinderFlags.None, tName, contextType, tParamTypes);
                tIlGen.EmitCallsiteCreate(tInvokeFuncType);
                tIlGen.Emit(OpCodes.Stsfld, tInvokeField);
            }

            if (tReturnType != typeof(void))
            {
                tIlGen.Emit(OpCodes.Ldsfld, tConvertField);
                tIlGen.Emit(OpCodes.Ldfld, typeof(CallSite<>).MakeGenericType(tConvertFuncType).GetFieldEvenIfGeneric("Target"));
                tIlGen.Emit(OpCodes.Ldsfld, tConvertField);
            }

            tIlGen.Emit(OpCodes.Ldsfld, tInvokeField);
            tIlGen.Emit(OpCodes.Ldfld, typeof(CallSite<>).MakeGenericType(tInvokeFuncType).GetFieldEvenIfGeneric("Target"));
            tIlGen.Emit(OpCodes.Ldsfld, tInvokeField);
            tIlGen.Emit(OpCodes.Ldarg_0);
            tIlGen.Emit(OpCodes.Call, typeof(ActsLikeProxy).GetProperty("Original").GetGetMethod());
            for (var i = 1; i <= tParamTypes.Length; i++)
            {

                tIlGen.EmitLoadArgument(i);
            }
            tIlGen.EmitCallInvokeFunc(tInvokeFuncType, tReturnType == typeof(void));
            if (tReturnType != typeof(void))
            {
                tIlGen.EmitCallInvokeFunc(tConvertFuncType);
            }

            tIlGen.Emit(OpCodes.Ret);
        }

        private static void CreateMethods(TypeBuilder tB, MethodInfo[] methodInfo, Type contextType)
        {
            foreach (var tInfo in methodInfo.Where(it => !it.IsSpecialName))
            {
                MakeMethod(tInfo, tB, contextType);
            }
        }



        private static void CreateProperties(TypeBuilder tB, IEnumerable<PropertyInfo> propertyInfo, Type contextType)
        {
            foreach (var tInfo in propertyInfo)
            {
                MakeProperty(tInfo, tB, contextType);
            }
        }


        private static void MakeProperty(PropertyInfo info, TypeBuilder typeBuilder, Type contextType)
        {
            var tName = info.Name;

            var tGetMethod = info.GetGetMethod();
            var tSetMethod = info.GetSetMethod();

            Type[] tSetParamTypes = null;
            Type tInvokeSetFuncType = null;

            var tIndexParamTypes = info.GetIndexParameters().Select(it => it.ParameterType).ToArray();




            var tCallSiteInvokeName = string.Format("Impromptu_Callsite_{1}_{0}", Guid.NewGuid().ToString("N"), tName);
            var tCStp = Builder.DefineType(tCallSiteInvokeName,
                                            TypeAttributes.NotPublic | TypeAttributes.Sealed |
                                            TypeAttributes.Class);


            var tConvertGet = "Convert_Get";
            var tConvertFuncType = tCStp.DefineCallsiteField(tConvertGet, tGetMethod.ReturnType);

            var tInvokeGet = "Invoke_Get";
            var tInvokeGetFuncType = tCStp.DefineCallsiteField(tInvokeGet, typeof(object), tIndexParamTypes);
            
            var tInvokeSet = "Invoke_Set";
            if (tSetMethod != null)
            {
                tSetParamTypes = tSetMethod.GetParameters().Select(it => it.ParameterType).ToArray();
                
                tInvokeSetFuncType = tCStp.DefineCallsiteField(tInvokeSet, typeof (object), tSetParamTypes);
            }

            var tCallSite = tCStp.CreateType();

            var tMp = typeBuilder.DefineProperty(tName, PropertyAttributes.None, tGetMethod.ReturnType, tIndexParamTypes);



            //GetMethod
            var tGetMethodBuilder = typeBuilder.DefineMethod(tGetMethod.Name,
                                                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
                                                    tGetMethod.ReturnType,
                                                    tIndexParamTypes);


            foreach (var tParam in info.GetGetMethod().GetParameters())
            {
                tGetMethodBuilder.DefineParameter(tParam.Position + 1, AttributesForParam(tParam), tParam.Name);
            }

            var tIlGen = tGetMethodBuilder.GetILGenerator();

            var tConvertCallsiteField = tCallSite.GetFieldEvenIfGeneric(tConvertGet);
            var tReturnLocal =tIlGen.DeclareLocal(tGetMethod.ReturnType);


       
            tIlGen.Emit(OpCodes.Ldsfld, tConvertCallsiteField);
            using (tIlGen.EmitBranchTrue())
            {
                tIlGen.EmitDynamicConvertBinder(CSharpBinderFlags.None, tGetMethod.ReturnType, contextType);
                tIlGen.EmitCallsiteCreate(tConvertFuncType);
                tIlGen.Emit(OpCodes.Stsfld, tConvertCallsiteField);
            }

            var tInvokeGetCallsiteField = tCallSite.GetFieldEvenIfGeneric(tInvokeGet);
            tIlGen.Emit(OpCodes.Ldsfld, tInvokeGetCallsiteField);
            using (tIlGen.EmitBranchTrue())
            {
                tIlGen.EmitDynamicGetBinder(CSharpBinderFlags.None, tName, contextType, tIndexParamTypes);
                tIlGen.EmitCallsiteCreate(tInvokeGetFuncType);
                tIlGen.Emit(OpCodes.Stsfld, tInvokeGetCallsiteField);
            }


            tIlGen.Emit(OpCodes.Ldsfld, tConvertCallsiteField);
            tIlGen.Emit(OpCodes.Ldfld, tConvertCallsiteField.FieldType.GetFieldEvenIfGeneric("Target"));
            tIlGen.Emit(OpCodes.Ldsfld, tConvertCallsiteField);
            tIlGen.Emit(OpCodes.Ldsfld, tInvokeGetCallsiteField);
            tIlGen.Emit(OpCodes.Ldfld, tInvokeGetCallsiteField.FieldType.GetFieldEvenIfGeneric("Target"));
            tIlGen.Emit(OpCodes.Ldsfld, tInvokeGetCallsiteField);
            tIlGen.Emit(OpCodes.Ldarg_0);
            tIlGen.Emit(OpCodes.Call, typeof(ActsLikeProxy).GetProperty("Original").GetGetMethod());
            for (var i = 1; i <= tIndexParamTypes.Length; i++)
            {
                tIlGen.EmitLoadArgument(i);
            }
            tIlGen.EmitCallInvokeFunc(tInvokeGetFuncType);
            tIlGen.EmitCallInvokeFunc(tConvertFuncType);
            tIlGen.EmitStoreLocation(tReturnLocal.LocalIndex);
            var tReturnLabel =tIlGen.DefineLabel();
            tIlGen.Emit(OpCodes.Br_S, tReturnLabel);
            tIlGen.MarkLabel(tReturnLabel);
            tIlGen.EmitLoadLocation(tReturnLocal.LocalIndex);
            tIlGen.Emit(OpCodes.Ret);
            tMp.SetGetMethod(tGetMethodBuilder);

            if (tSetMethod != null)
            {
                var tSetMethodBuilder = typeBuilder.DefineMethod(tSetMethod.Name,
                                                                 MethodAttributes.Public | MethodAttributes.SpecialName |
                                                                 MethodAttributes.HideBySig | MethodAttributes.Virtual |
                                                                 MethodAttributes.Final | MethodAttributes.NewSlot,
                                                                 null,
                                                                 tSetParamTypes);

                foreach (var tParam in info.GetSetMethod().GetParameters())
                {
                    tSetMethodBuilder.DefineParameter(tParam.Position + 1, AttributesForParam(tParam), tParam.Name);
                }

                tIlGen = tSetMethodBuilder.GetILGenerator();
                var tSetCallsiteField = tCallSite.GetFieldEvenIfGeneric(tInvokeSet);
                tIlGen.Emit(OpCodes.Ldsfld, tSetCallsiteField);
                using (tIlGen.EmitBranchTrue())
                {
                    tIlGen.EmitDynamicSetBinder(CSharpBinderFlags.None, tName, contextType, tSetParamTypes);
                    tIlGen.EmitCallsiteCreate(tInvokeSetFuncType);
                    tIlGen.Emit(OpCodes.Stsfld, tSetCallsiteField);
                }
                tIlGen.Emit(OpCodes.Ldsfld, tSetCallsiteField);
                tIlGen.Emit(OpCodes.Ldfld, tSetCallsiteField.FieldType.GetFieldEvenIfGeneric("Target"));
                tIlGen.Emit(OpCodes.Ldsfld, tSetCallsiteField);
                tIlGen.Emit(OpCodes.Ldarg_0);
                tIlGen.Emit(OpCodes.Call, typeof (ActsLikeProxy).GetProperty("Original").GetGetMethod());
                for (var i = 1; i <= tSetParamTypes.Length; i++)
                {
                    tIlGen.EmitLoadArgument(i);
                }
                tIlGen.EmitCallInvokeFunc(tInvokeSetFuncType);
                tIlGen.Emit(OpCodes.Pop);
                tIlGen.Emit(OpCodes.Ret);
                tMp.SetSetMethod(tSetMethodBuilder);
            }
        }

        private static Type UpdateGenericParametesr(Type funcType, Dictionary<Type, GenericTypeParameterBuilder> updateType)
        {
            var tFuncAgs = funcType.GetGenericArguments();
            var tFuncDef = funcType.GetGenericTypeDefinition();
            tFuncAgs = tFuncAgs.Select(it => updateType.ContainsKey(it) ? updateType[it] : it).ToArray();
            return tFuncDef.MakeGenericType(tFuncAgs);
        }

        private static Type DefineCallsiteField(this TypeBuilder builder, string name, Type returnType, params Type[] argTypes)
        {
            var tList = new List<Type> { typeof(CallSite), typeof(object) };
            tList.AddRange(argTypes);
            if (returnType != typeof(void))
                tList.Add(returnType);

            var tTypeName = String.Format("System.Func`{0}", tList.Count);
            if (returnType == typeof(void))
                tTypeName = String.Format("System.Action`{0}", tList.Count);
            var tFuncGeneric = Type.GetType(tTypeName);
            var tFuncType = tFuncGeneric.MakeGenericType(tList.ToArray());
            var tReturnType = typeof(CallSite<>).MakeGenericType(tFuncType);
            builder.DefineField(name, tReturnType, FieldAttributes.Static | FieldAttributes.Public);
            return tFuncType;
        }
        private static ParameterAttributes AttributesForParam(ParameterInfo param)
        {
            var tAttribute = ParameterAttributes.None;
            if (param.IsIn)
                tAttribute |= ParameterAttributes.In;
            if (param.IsOut)
                tAttribute |= ParameterAttributes.Out;
            if (param.IsOptional)
                tAttribute |= ParameterAttributes.Optional;
            if (param.IsLcid)
                tAttribute |= ParameterAttributes.Lcid;
            return tAttribute;
        }

        static public void SaveDynamicDll()
        {
            _ab.Save("test.dll");
        }

        static public ModuleBuilder Builder
        {
            get
            {
                if (_builder == null)
                {
                    var tPlainName = "ImpromptuInterfaceDynamicAssembly";
                    var tName = new AssemblyName(tPlainName);



            var access = AssemblyBuilderAccess.Run;

                    _ab =
                           AppDomain.CurrentDomain.DefineDynamicAssembly(
                                   tName,
                                   access);

           _builder =
                            _ab.DefineDynamicModule(tName.Name);

                }
                return _builder;
            }
        }
    }
    
}
