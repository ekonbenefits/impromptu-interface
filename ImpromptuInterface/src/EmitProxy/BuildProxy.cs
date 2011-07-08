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

using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using ImpromptuInterface.Optimization;

namespace ImpromptuInterface.Build
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using Microsoft.CSharp.RuntimeBinder;
  

    ///<summary>
    /// Does most of the work buiding and caching proxies
    ///</summary>
    public static class BuildProxy
    {
        

        private static ModuleBuilder _builder;
        internal static ModuleBuilder _tempBuilder;
        internal static AssemblyBuilder _tempSaveAssembly;

        private static AssemblyBuilder _ab;
        private static readonly Dictionary<TypeHash, Type> _typeHash = new Dictionary<TypeHash, Type>();
        private static readonly object TypeCacheLock = new object();

        private static readonly Dictionary<TypeHash, Type> _delegateCache = new Dictionary<TypeHash, Type>();
        private static readonly object DelegateCacheLock = new object();

#if !SILVERLIGHT
        internal class TempBuilder : IDisposable
        {
            private readonly string _name;
            private bool _disposed;
            internal TempBuilder(string name)
            {
                _name = name;
            }

            public void Close()
            {
                Dispose();
            }
          
           
            public void Dispose()
            {
                if (_disposed)
                    throw new MethodAccessException("Can't Call Dispose Twice!!");
                _disposed = true;
            
                 
                   _tempSaveAssembly.Save(string.Format("{0}.dll", _name));
               
                _tempSaveAssembly = null;
                _tempBuilder = null;
            } 
          
        }

        /// <summary>
        /// Writes the out DLL of types created between this call and being closed used for debugging of emitted IL code
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks>
        ///     This may be used for generating an assembly for preloading proxies, however you must be very careful when doing so as 
        ///     changes could make the emitted asssembly out of date very easily.
        /// </remarks>
        public static IDisposable WriteOutDll(string name)
        {  
              GenerateAssembly(name, AssemblyBuilderAccess.RunAndSave,ref _tempSaveAssembly,ref  _tempBuilder);
              
              return new TempBuilder(name);
        }

#endif


        /// <summary>
        /// Builds the type for the static proxy or returns from cache
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="mainInterface">The main interface.</param>
        /// <param name="otherInterfaces">The other interfaces.</param>
        /// <returns></returns>
        public static Type BuildType(Type contextType, Type mainInterface, params Type[] otherInterfaces)
        {
            lock (TypeCacheLock)
            {
                contextType = contextType.FixContext();
                var tNewHash = TypeHash.Create(contextType, new[]{mainInterface}.Concat(otherInterfaces).ToArray());
                Type tType = null;
                if (!_typeHash.TryGetValue(tNewHash, out tType))
                {
                    tType = BuildTypeHelper(Builder,contextType,new[]{mainInterface}.Concat(otherInterfaces).ToArray());
                    _typeHash[tNewHash] = tType;
                }

                return _typeHash[tNewHash];
            }

        }

        /// <summary>
        /// Builds the type.
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="informalInterface">The informal interface.</param>
        /// <returns></returns>
        public static Type BuildType(Type contextType, IDictionary<string,Type> informalInterface)
        {
            lock (TypeCacheLock)
            {
                var tNewHash = TypeHash.Create(contextType, informalInterface);
                Type tType = null;
                if (!_typeHash.TryGetValue(tNewHash, out tType))
                {
                    tType = BuildTypeHelper(Builder, contextType, informalInterface);

                    _typeHash[tNewHash] = tType;
                }

                return _typeHash[tNewHash];
            }

        }

        /// <summary>
        /// Preloads a proxy for ActLike to use.
        /// </summary>
        /// <param name="proxyType">Type of the proxy.</param>
        /// <param name="attribute">The ActLikeProxyAttribute, if not provide it will be looked up.</param>
        /// <returns>Returns false if there already is a proxy registered for the same type.</returns>
        public static bool PreLoadProxy(Type proxyType, ActLikeProxyAttribute attribute = null)
        {
            var tSuccess = true;
            if (attribute == null)
                attribute = proxyType.GetCustomAttributes(typeof(ActLikeProxyAttribute), inherit: false).Cast<ActLikeProxyAttribute>().FirstOrDefault();

            if(attribute == null)
                throw new Exception("Proxy Type must have ActLikeProxyAttribute");

            if (!typeof(IActLikeProxyInitialize).IsAssignableFrom(proxyType))
                throw new Exception("Proxy Type must implement IActLikeProxyInitialize");

            foreach (var tIType in attribute.Interfaces)
            {
                if (!tIType.IsAssignableFrom(proxyType))
                {
                    throw new Exception(String.Format("Proxy Type {0} must implement declared interfaces {1}", proxyType, tIType));
                }
            }

            lock (TypeCacheLock)
            {
                var tNewHash = TypeHash.Create(attribute.Context, attribute.Interfaces);

                if (!_typeHash.ContainsKey(tNewHash))
                {
                    _typeHash[tNewHash] = proxyType;
                }
                else
                {
                    tSuccess = false;
                }
            }
            return tSuccess;
        }

        /// <summary>
        /// Preloads proxies that ActLike uses from assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Returns false if there already is a proxy registered for the same type.</returns>
        public static bool PreLoadProxiesFromAssembly(Assembly assembly)
        {
            var tSuccess = true;
            var typesWithMyAttribute =
             from tType in assembly.GetTypes()
             let tAttributes = tType.GetCustomAttributes(typeof(ActLikeProxyAttribute), inherit:false)
             where tAttributes != null && tAttributes.Length == 1
             select new { Type = tType, Impromptu = tAttributes.Cast<ActLikeProxyAttribute>().Single() };
            foreach (var tTypeCombo in typesWithMyAttribute)
            {
                lock (TypeCacheLock)
                {
                    if (!PreLoadProxy(tTypeCombo.Type, tTypeCombo.Impromptu))
                        tSuccess = false;
                }
            }
            return tSuccess;
        }

        private static Type BuildTypeHelper(ModuleBuilder builder, Type contextType, IDictionary<string,Type> informalInterface)
        {


            var tB = builder.DefineType(
                string.Format("ActLike_{0}_{1}", "InformalInterface", Guid.NewGuid().ToString("N")), TypeAttributes.Public | TypeAttributes.Class,
                typeof(ActLikeProxy));




            foreach (var tInterface in informalInterface)
            {

                MakePropertyDescribedProperty(builder, tB, contextType, tInterface.Key, tInterface.Value);
               
            }
            var tType = tB.CreateType();
            return tType;
        }

        private static void MakePropertyDescribedProperty(ModuleBuilder builder, TypeBuilder typeBuilder, Type contextType, string tName, Type tReturnType)
        {
        
         
            var tGetName = "get_"+tName;





            MakePropertyHelper(null, tName, builder, tReturnType, null, typeBuilder, tGetName, contextType, true);
        }

        private class MethodSigHash
        {
            public readonly string Name;
            public readonly Type[] Parameters;

            public MethodSigHash(MethodInfo info)
            {
                Name = info.Name;
                Parameters = info.GetParameters().Select(it => it.ParameterType).ToArray();
            }

            public MethodSigHash(string name, Type[] parameters)
            {
                Name = name;
                Parameters = parameters;
            }

            public bool Equals(MethodSigHash other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Name, Name) && StructuralComparisons.StructuralEqualityComparer.Equals(other.Parameters, Parameters);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (MethodSigHash)) return false;
                return Equals((MethodSigHash) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Name.GetHashCode() * 397) ^ StructuralComparisons.StructuralEqualityComparer.GetHashCode(Parameters);
                }
            }
        }

        private static Type BuildTypeHelper(ModuleBuilder builder,Type contextType,params Type[] interfaces)
        {

            var tInterfacesMainList = interfaces.Distinct().ToArray();
            var tB = builder.DefineType(
                string.Format("ActLike_{0}_{1}", tInterfacesMainList.First().Name, Guid.NewGuid().ToString("N")), TypeAttributes.Public | TypeAttributes.Class,
                typeof(ActLikeProxy), tInterfacesMainList);

            tB.SetCustomAttribute(
                new CustomAttributeBuilder(typeof(ActLikeProxyAttribute).GetConstructor(new[]{typeof(Type).MakeArrayType(),typeof(Type)}),
                    new object[]{interfaces,contextType}));
            tB.SetCustomAttribute(new CustomAttributeBuilder(typeof(SerializableAttribute).GetConstructor(Type.EmptyTypes),new object[]{}));


            var tInterfaces = tInterfacesMainList.Concat(tInterfacesMainList.SelectMany(it => it.GetInterfaces()));


            var tPropertyNameHash = new HashSet<string>();
            var tMethodHashSet = new HashSet<MethodSigHash>();

            object tAttr=null;
            foreach (var tInterface in tInterfaces.Distinct())
            {

#if !SILVERLIGHT
                
                    if (tInterface != null && tAttr ==null)
                    {
                        var tCustomAttributes = tInterface.GetCustomAttributesData();
                        foreach (var tCustomAttribute in tCustomAttributes.Where(it=>typeof(DefaultMemberAttribute).IsAssignableFrom(it.Constructor.DeclaringType)))
                        {
                            try
                            {
                                tB.SetCustomAttribute(GetAttributeBuilder(tCustomAttribute));
                            }
                            catch
                            {
                            } //For most proxies not having the same attributes won't really matter,
                            //but just incase we don't want to stop for some unknown attribute that we can't initialize
                        }
                    }

#else
                if (tInterface != null && tAttr ==null){
                        var tAttrs =tInterface.GetCustomAttributes(typeof(DefaultMemberAttribute),true);

                    tAttr = tAttrs.FirstOrDefault();
                    if(tAttr !=null){
                             tB.SetCustomAttribute(new CustomAttributeBuilder(typeof(DefaultMemberAttribute)
                                 .GetConstructor(new[]{typeof(String)}),new object[]{"Item"}));
                        }
                }
#endif

                foreach (var tInfo in tInterface.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    MakeProperty(builder, tInfo, tB, contextType, defaultImp: tPropertyNameHash.Add(tInfo.Name));
                }
                foreach (var tInfo in tInterface.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(it => !it.IsSpecialName))
                {
                    MakeMethod(builder, tInfo, tB, contextType, defaultImp: tMethodHashSet.Add(new MethodSigHash(tInfo)));
                }
                foreach (var tInfo in tInterface.GetEvents(BindingFlags.Public | BindingFlags.Instance).Where(it => !it.IsSpecialName))
                {
                    MakeEvent(builder, tInfo, tB, contextType, defaultImp: tPropertyNameHash.Add(tInfo.Name));
                }
            }
            var tType = tB.CreateType();
            return tType;
        }

    
        private static IEnumerable<Type> FlattenGenericParameters(Type type)
        {
            if (type.IsByRef || type.IsArray || type.IsPointer)
            {
                return FlattenGenericParameters(type.GetElementType());
            }

            if (type.IsGenericParameter)
                return new[] {type};
            if(type.ContainsGenericParameters)
            {
                return type.GetGenericArguments().SelectMany(FlattenGenericParameters);
            }
            return new Type[]{};
        }

        private static Type ReplaceTypeWithGenericBuilder(Type type, IDictionary<Type,GenericTypeParameterBuilder> dict)
        {
            var tStartType = type;
            Type tReturnType;
            if (type.IsByRef || type.IsArray || type.IsPointer)
            {
                tStartType = type.GetElementType();
            }


            if(tStartType.IsGenericTypeDefinition)
            {
                var tArgs = tStartType.GetGenericArguments().Select(it=>ReplaceTypeWithGenericBuilder(it,dict));

                var tNewType = tStartType.MakeGenericType(tArgs.ToArray());
                tReturnType = tNewType;
            }else if (dict.ContainsKey(tStartType))
                {
                    var tNewType = dict[tStartType];
                    var tAttributes = tStartType.GenericParameterAttributes;
                    tNewType.SetGenericParameterAttributes(tAttributes);
                    foreach (var tConstraint in tStartType.GetGenericParameterConstraints())
                    {
                        if (tConstraint.IsInterface)
                            tNewType.SetInterfaceConstraints(tConstraint);
                        else
                            tNewType.SetBaseTypeConstraint(tConstraint);
                    }
                    tReturnType = tNewType;
                }
                else
                {
                    tReturnType = tStartType;
                }

            if (type.IsByRef)
            {
                return tReturnType.MakeByRefType();
            }

            if (type.IsArray)
            {
                return tReturnType.MakeArrayType();
            }

            if (type.IsPointer)
            {
                return tReturnType.MakePointerType();
            }

            return tReturnType;
        }
#if !SILVERLIGHT
        private static object CustomAttributeTypeArgument(CustomAttributeTypedArgument argument)
        {
            if (argument.Value is ReadOnlyCollection<CustomAttributeTypedArgument>)
            {
                var tValue = argument.Value as ReadOnlyCollection<CustomAttributeTypedArgument>;
                return
                    new ArrayList(tValue.Select(it => it.Value).ToList()).ToArray(argument.ArgumentType.GetElementType());
            }

            return argument.Value;
        }
       

        private static CustomAttributeBuilder GetAttributeBuilder(CustomAttributeData data)
        {
            var tPropertyInfos = new List<PropertyInfo>();
            var tPropertyValues = new List<object>();
            var tFieldInfos = new List<FieldInfo>();
            var tFieldValues = new List<object>();

            if (data.NamedArguments != null)
            {
                foreach (var namedArg in data.NamedArguments)
                {
                    var fi = namedArg.MemberInfo as FieldInfo;
                    var pi = namedArg.MemberInfo as PropertyInfo;

                    if (fi != null)
                    {
                        tFieldInfos.Add(fi);
                        tFieldValues.Add(namedArg.TypedValue.Value);
                    }
                    else if (pi != null)
                    {
                        tPropertyInfos.Add(pi);
                        tPropertyValues.Add(namedArg.TypedValue.Value);
                    }
                }
            }

            return new CustomAttributeBuilder(
              data.Constructor,
              data.ConstructorArguments.Select(CustomAttributeTypeArgument).ToArray(),
              tPropertyInfos.ToArray(),
              tPropertyValues.ToArray(),
              tFieldInfos.ToArray(),
              tFieldValues.ToArray());
        }
#endif

        private static void MakeMethod(ModuleBuilder builder,MethodInfo info, TypeBuilder typeBuilder, Type contextType, bool defaultImp =true)
        {


            var tName = info.Name;

            var tParamAttri = info.GetParameters();
            Type[] tParamTypes = tParamAttri.Select(it => it.ParameterType).ToArray();


            IEnumerable<string> tArgNames;
            if (info.GetCustomAttributes(typeof(Dynamic.UseNamedArgumentAttribute), false).Any())
            {
                tArgNames = tParamAttri.Select(it => it.Name).ToList();
            }
            else
            {
                var tParam = tParamAttri.Zip(Enumerable.Range(0, tParamTypes.Count()), (p, i) => new { i, p })
                    .FirstOrDefault(it => it.p.GetCustomAttributes(typeof(Dynamic.UseNamedArgumentAttribute), false).Any());

                tArgNames = tParam == null
                    ? Enumerable.Repeat(default(string), tParamTypes.Length)
                    : Enumerable.Repeat(default(string), tParam.i).Concat(tParamAttri.Skip(Math.Min(tParam.i - 1, 0)).Select(it => it.Name)).ToList();
            }


            var tReturnType = typeof(void);
            if (info.ReturnParameter != null)
                tReturnType = info.ReturnParameter.ParameterType;


            var tCallSiteInvokeName = string.Format("Impromptu_Callsite_{1}_{0}", Guid.NewGuid().ToString("N"), tName);
            var tCStp = DefineBuilderForCallSite(builder, tCallSiteInvokeName);



            var tReplacedTypes = GetParamTypes(tCStp, info);
            if (tReplacedTypes != null)
            {
                tReturnType = tReplacedTypes.Item1;
                tParamTypes = tReplacedTypes.Item2;
            }

            var tConvert = "Convert_Method";
            Type tConvertFuncType = null;
            if (tReturnType != typeof(void))
            {
                tConvertFuncType = tCStp.DefineCallsiteField(tConvert, tReturnType);
            }

            var tInvokeMethod = "Invoke_Method";
            var tInvokeFuncType = tCStp.DefineCallsiteFieldForMethod(tInvokeMethod, tReturnType != typeof(void) ? typeof(object) : typeof(void), tParamTypes, info);




            var tCallSite = tCStp.CreateType();

            var tPublicPrivate = MethodAttributes.Public;
            var tPrefixName = tName;
            if (!defaultImp)
            {
                tPrefixName = String.Format("{0}.{1}", info.DeclaringType.FullName, tPrefixName);

                tPublicPrivate = MethodAttributes.Private;
            }


            var tMethodBuilder = typeBuilder.DefineMethod(tPrefixName,
                                                tPublicPrivate | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot);

         

            tReplacedTypes = GetParamTypes(tMethodBuilder, info);
            var tReducedParams = tParamTypes.Select(ReduceToElementType).ToArray();
            if (tReplacedTypes != null)
            {
                tReturnType = tReplacedTypes.Item1;
                tParamTypes = tReplacedTypes.Item2;

                tReducedParams = tParamTypes.Select(ReduceToElementType).ToArray();

                tCallSite = tCallSite.GetGenericTypeDefinition().MakeGenericType(tReducedParams);
                if (tConvertFuncType != null)
                    tConvertFuncType = UpdateCallsiteFuncType(tConvertFuncType, tReturnType);
                tInvokeFuncType = UpdateCallsiteFuncType(tInvokeFuncType, tReturnType != typeof(void) ? typeof(object) : typeof(void), tReducedParams);
            }

            tMethodBuilder.SetReturnType(tReturnType);
            tMethodBuilder.SetParameters(tParamTypes);

            foreach (var tParam in info.GetParameters())
            {
                var tParamBuilder = tMethodBuilder.DefineParameter(tParam.Position + 1, AttributesForParam(tParam), tParam.Name);
#if !SILVERLIGHT


                var tCustomAttributes = tParam.GetCustomAttributesData();
                foreach (var tCustomAttribute in tCustomAttributes)
                {
                    try
                    {
                        tParamBuilder.SetCustomAttribute(GetAttributeBuilder(tCustomAttribute));
                    }
                    catch { }//For most proxies not having the same attributes won't really matter,
                    //but just incase we don't want to stop for some unknown attribute that we can't initialize
                }
#else
               var tAny =tParam.GetCustomAttributes(typeof(ParamArrayAttribute), true).Any();
               if(tAny)
                    tParamBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(ParamArrayAttribute).GetConstructor(Type.EmptyTypes),new object[]{}));
               var tDynAttr =(DynamicAttribute)tParam.GetCustomAttributes(typeof(DynamicAttribute), true).FirstOrDefault();
                if(tDynAttr !=null)
                {
                    tParamBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(DynamicAttribute).GetConstructor(new[]{typeof(bool[])}
), new object[] { tDynAttr.TransformFlags.ToArray() }));
                }
#endif

            }

            if (!defaultImp)
            {
                typeBuilder.DefineMethodOverride(tMethodBuilder, info);
            }

            EmitMethodBody(tName, tReducedParams, tParamAttri, tReturnType, tConvert, tInvokeMethod, tMethodBuilder, tCallSite, contextType, tConvertFuncType, tInvokeFuncType, tArgNames);
        }

        private static TypeBuilder DefineBuilderForCallSite(ModuleBuilder builder, string tCallSiteInvokeName)
        {
            return builder.DefineType(tCallSiteInvokeName,

                                      TypeAttributes.NotPublic
                                      | TypeAttributes.Sealed
                                      | TypeAttributes.AutoClass
                                      |TypeAttributes.BeforeFieldInit 
                                      | TypeAttributes.Abstract
                );
        }

        private static Tuple<Type, Type[]> GetParamTypes(dynamic builder, MethodInfo info)
        {
            var paramTypes = info.GetParameters().Select(it => it.ParameterType).ToArray();
            var returnType = typeof(void);
            if (info.ReturnParameter != null)
                returnType = info.ReturnParameter.ParameterType;

            var tGenericParams = paramTypes
                .SelectMany(FlattenGenericParameters)
                .Distinct().ToDictionary(it => it.GenericParameterPosition, it => new { Type = it, Gen = default(GenericTypeParameterBuilder) });
            var tParams = tGenericParams;
            var tReturnParameters = FlattenGenericParameters(returnType).Where(it => !tParams.ContainsKey(it.GenericParameterPosition));
            foreach(var tReParm in tReturnParameters)
                tGenericParams.Add(tReParm.GenericParameterPosition, new { Type = tReParm, Gen = default(GenericTypeParameterBuilder) });
            var tGenParams = tGenericParams.OrderBy(it => it.Key).Select(it => it.Value.Type.Name);
            if (tGenParams.Any())
            {
                GenericTypeParameterBuilder[] tBuilders = builder.DefineGenericParameters(tGenParams.ToArray());
                var tDict = tGenericParams.ToDictionary(param => param.Value.Type, param => tBuilders[param.Key]);

                returnType = ReplaceTypeWithGenericBuilder(returnType, tDict);
                if (tDict.ContainsKey(returnType))
                {
                    returnType = tDict[returnType];
                }
                paramTypes = paramTypes.Select(it => ReplaceTypeWithGenericBuilder(it,tDict)).ToArray();
                return Tuple.Create(returnType, paramTypes);
            }
            return null;
        }

        private static void EmitMethodBody(
            string name,
            Type[] paramTypes, 
            ParameterInfo[] paramInfo, 
            Type returnType, 
            string convert,
            string invokeMethod, 
            MethodBuilder methodBuilder, 
            Type callSite,
            Type contextType, 
            Type convertFuncType, 
            Type invokeFuncType,
            IEnumerable<string> argNames
            )
        {
            var tIlGen = methodBuilder.GetILGenerator();

            var tConvertField = callSite.GetFieldEvenIfGeneric(convert);
            if (returnType != typeof(void))
            {

              
                using (tIlGen.EmitBranchTrue(gen=>gen.Emit(OpCodes.Ldsfld, tConvertField)))
                {
                    tIlGen.EmitDynamicConvertBinder(CSharpBinderFlags.None, returnType, contextType);
                    tIlGen.EmitCallsiteCreate(convertFuncType);
                    tIlGen.Emit(OpCodes.Stsfld, tConvertField);
                }
            }
            
            var tInvokeField = callSite.GetFieldEvenIfGeneric(invokeMethod);

            using (tIlGen.EmitBranchTrue(gen=>gen.Emit(OpCodes.Ldsfld, tInvokeField)))
            {
                tIlGen.EmitDynamicMethodInvokeBinder(returnType == typeof(void) ? CSharpBinderFlags.ResultDiscarded : CSharpBinderFlags.None, name, contextType, paramInfo, argNames);
                tIlGen.EmitCallsiteCreate(invokeFuncType);
                tIlGen.Emit(OpCodes.Stsfld, tInvokeField);
            }

            if (returnType != typeof(void))
            {
                tIlGen.Emit(OpCodes.Ldsfld, tConvertField);
                tIlGen.Emit(OpCodes.Ldfld, typeof(CallSite<>).MakeGenericType(convertFuncType).GetFieldEvenIfGeneric("Target"));
                tIlGen.Emit(OpCodes.Ldsfld, tConvertField);
            }

            tIlGen.Emit(OpCodes.Ldsfld, tInvokeField);
            tIlGen.Emit(OpCodes.Ldfld, typeof(CallSite<>).MakeGenericType(invokeFuncType).GetFieldEvenIfGeneric("Target"));
            tIlGen.Emit(OpCodes.Ldsfld, tInvokeField);
            tIlGen.Emit(OpCodes.Ldarg_0);
            tIlGen.Emit(OpCodes.Call, typeof(ActLikeProxy).GetProperty("Original").GetGetMethod());
            for (var i = 1; i <= paramTypes.Length; i++)
            {

                tIlGen.EmitLoadArgument(i);
            }
            tIlGen.EmitCallInvokeFunc(invokeFuncType, returnType == typeof(void));
            if (returnType != typeof(void))
            {
                tIlGen.EmitCallInvokeFunc(convertFuncType);
            }

            tIlGen.Emit(OpCodes.Ret);
        }


        private static void MakeProperty(ModuleBuilder builder,PropertyInfo info, TypeBuilder typeBuilder, Type contextType, bool defaultImp =true)
        {
            var tName = info.Name;

            var tGetMethod = info.GetGetMethod();
            var tSetMethod = info.GetSetMethod();
            var tReturnType = tGetMethod.ReturnType;
            var tGetName = tGetMethod.Name;


         


            MakePropertyHelper(info, tName, builder, tReturnType, tSetMethod, typeBuilder, tGetName, contextType, defaultImp);
        }

        private static void MakeEvent(ModuleBuilder builder, EventInfo info, TypeBuilder typeBuilder, Type contextType, bool defaultImp)
        {
            var tName = info.Name;
             var tAddMethod = info.GetAddMethod();
            var tRemoveMethod = info.GetRemoveMethod();
            var tReturnType = info.EventHandlerType;


            var tCallSiteInvokeName = string.Format("Impromptu_Callsite_{1}_{0}", Guid.NewGuid().ToString("N"), tName);
            var tCStp = DefineBuilderForCallSite(builder, tCallSiteInvokeName);


            var tInvokeIsEvent = "Invoke_IsEvent";
            var tInvokeIseventFuncType = tCStp.DefineCallsiteField(tInvokeIsEvent, typeof(bool));


            var tInvokeAddAssign = "Invoke_AddAssign";
            var tInvokeAddAssignFuncType = tCStp.DefineCallsiteField(tInvokeAddAssign, typeof(object), tReturnType);

            var tInvokeSubtractAssign = "Invoke_SubtractAssign";
            var tInvokeSubtractAssignFuncType = tCStp.DefineCallsiteField(tInvokeSubtractAssign, typeof(object), tReturnType);

            var tAddParamTypes = tRemoveMethod.GetParameters().Select(it => it.ParameterType).ToArray();
            var tInvokeAdd = "Invoke_Add";
            var tInvokeAddFuncType = tCStp.DefineCallsiteField(tInvokeAdd, typeof(object), tAddParamTypes);
            
            var tRemoveParamTypes = tRemoveMethod.GetParameters().Select(it => it.ParameterType).ToArray();
            var tInvokeRemove = "Invoke_Remove";
            var tInvokeRemoveFuncType = tCStp.DefineCallsiteField(tInvokeRemove, typeof(object), tRemoveParamTypes);

            var tInvokeGet = "Invoke_Get";
            var tInvokeGetFuncType = tCStp.DefineCallsiteField(tInvokeGet, typeof(object));

            var tInvokeSet = "Invoke_Set";

            var tInvokeSetFuncType = tCStp.DefineCallsiteField(tInvokeSet, typeof(object), typeof(object));

            var tCallSite = tCStp.CreateType();

            var tMp = typeBuilder.DefineEvent(tName, EventAttributes.None, tReturnType);

            //AddMethod
            var tPublicPrivate = MethodAttributes.Public;
            var tAddPrefixName = tAddMethod.Name;
            if (!defaultImp)
            {
                tAddPrefixName = String.Format("{0}.{1}", info.DeclaringType.FullName, tAddPrefixName);

                tPublicPrivate = MethodAttributes.Private;
            }

            var tAddBuilder = typeBuilder.DefineMethod(tAddPrefixName,
                                                             tPublicPrivate | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
                                                             typeof(void),
                                                             tAddParamTypes);

            if (!defaultImp)
            {
                typeBuilder.DefineMethodOverride(tAddBuilder,info.GetAddMethod());
            }


            foreach (var tParam in tAddMethod.GetParameters())
            {
                tAddBuilder.DefineParameter(tParam.Position + 1, AttributesForParam(tParam), tParam.Name);
            }


            var tIlGen = tAddBuilder.GetILGenerator();

            var tIsEventField = tCallSite.GetFieldEvenIfGeneric(tInvokeIsEvent);

            using (tIlGen.EmitBranchTrue(gen => gen.Emit(OpCodes.Ldsfld, tIsEventField)))
            {
                tIlGen.EmitDynamicIsEventBinder(CSharpBinderFlags.None, tName, contextType);
                tIlGen.EmitCallsiteCreate(tInvokeIseventFuncType);
                tIlGen.Emit(OpCodes.Stsfld, tIsEventField);
            }
          
            var tSetField = tCallSite.GetFieldEvenIfGeneric(tInvokeSet);
            var tGetField = tCallSite.GetFieldEvenIfGeneric(tInvokeGet);



            using (tIlGen.EmitBranchTrue(
                      load => load.EmitInvocation(
                           target => target.EmitInvocation(
                               t => t.Emit(OpCodes.Ldsfld, tIsEventField),
                               i => i.Emit(OpCodes.Ldfld, tIsEventField.FieldType.GetFieldEvenIfGeneric("Target"))
                           ),
                           invoke => invoke.EmitCallInvokeFunc(tInvokeIseventFuncType),
                           param => param.Emit(OpCodes.Ldsfld, tIsEventField),
                           param => param.EmitInvocation(
                                  t => t.Emit(OpCodes.Ldarg_0),
                                  i => i.Emit(OpCodes.Call, typeof(ActLikeProxy).GetProperty("Original").GetGetMethod())
                           )
                     )
              )
            ) //if IsEvent Not True
            {
         
                using (tIlGen.EmitBranchTrue(gen=>gen.Emit(OpCodes.Ldsfld, tSetField)))
                {
                    tIlGen.EmitDynamicSetBinderDynamicParams(CSharpBinderFlags.ValueFromCompoundAssignment, tName, contextType, typeof(Object));
                    tIlGen.EmitCallsiteCreate(tInvokeSetFuncType);
                    tIlGen.Emit(OpCodes.Stsfld, tSetField);
                }

                var tAddAssigneField = tCallSite.GetFieldEvenIfGeneric(tInvokeAddAssign);

                using (tIlGen.EmitBranchTrue(gen => gen.Emit(OpCodes.Ldsfld, tAddAssigneField)))
                {
                    tIlGen.EmitDynamicBinaryOpBinder(CSharpBinderFlags.None, ExpressionType.AddAssign, contextType, tReturnType);
                    tIlGen.EmitCallsiteCreate(tInvokeAddAssignFuncType);
                    tIlGen.Emit(OpCodes.Stsfld, tAddAssigneField);
                }
               
                using (tIlGen.EmitBranchTrue(gen=>gen.Emit(OpCodes.Ldsfld, tGetField)))
                {
                    tIlGen.EmitDynamicGetBinder(CSharpBinderFlags.None, tName, contextType);
                    tIlGen.EmitCallsiteCreate(tInvokeGetFuncType);
                    tIlGen.Emit(OpCodes.Stsfld, tGetField);
                }

               


                tIlGen.Emit(OpCodes.Ldsfld, tSetField);
                tIlGen.Emit(OpCodes.Ldfld, tSetField.FieldType.GetFieldEvenIfGeneric("Target"));
                tIlGen.Emit(OpCodes.Ldsfld, tSetField);
                tIlGen.Emit(OpCodes.Ldarg_0);
                tIlGen.Emit(OpCodes.Call, typeof(ActLikeProxy).GetProperty("Original").GetGetMethod());

                tIlGen.Emit(OpCodes.Ldsfld, tAddAssigneField);
                tIlGen.Emit(OpCodes.Ldfld, tAddAssigneField.FieldType.GetFieldEvenIfGeneric("Target"));
                tIlGen.Emit(OpCodes.Ldsfld, tAddAssigneField);

                tIlGen.Emit(OpCodes.Ldsfld, tGetField);
                tIlGen.Emit(OpCodes.Ldfld, tGetField.FieldType.GetFieldEvenIfGeneric("Target"));
                tIlGen.Emit(OpCodes.Ldsfld, tGetField);
                tIlGen.Emit(OpCodes.Ldarg_0);
                tIlGen.Emit(OpCodes.Call, typeof(ActLikeProxy).GetProperty("Original").GetGetMethod());

                tIlGen.EmitCallInvokeFunc(tInvokeGetFuncType);
                
                tIlGen.Emit(OpCodes.Ldarg_1);
                tIlGen.EmitCallInvokeFunc(tInvokeAddAssignFuncType);

                tIlGen.EmitCallInvokeFunc(tInvokeSetFuncType);
                tIlGen.Emit(OpCodes.Pop);
                tIlGen.Emit(OpCodes.Ret);
              
            }

            var tAddCallSiteField = tCallSite.GetFieldEvenIfGeneric(tInvokeAdd);

            using (tIlGen.EmitBranchTrue(gen=>gen.Emit(OpCodes.Ldsfld, tAddCallSiteField)))
            {
                tIlGen.EmitDynamicMethodInvokeBinder(
                    CSharpBinderFlags.InvokeSpecialName | CSharpBinderFlags.ResultDiscarded,
                    tAddMethod.Name,
                    contextType,
                    tAddMethod.GetParameters(),
                    Enumerable.Repeat(default(string),
                    tAddParamTypes.Length));
                tIlGen.EmitCallsiteCreate(tInvokeAddFuncType);
                tIlGen.Emit(OpCodes.Stsfld, tAddCallSiteField);
            }
            tIlGen.Emit(OpCodes.Ldsfld, tAddCallSiteField);
            tIlGen.Emit(OpCodes.Ldfld, tAddCallSiteField.FieldType.GetFieldEvenIfGeneric("Target"));
            tIlGen.Emit(OpCodes.Ldsfld, tAddCallSiteField);
            tIlGen.Emit(OpCodes.Ldarg_0);
            tIlGen.Emit(OpCodes.Call, typeof(ActLikeProxy).GetProperty("Original").GetGetMethod());
            for (var i = 1; i <= tAddParamTypes.Length; i++)
            {
                tIlGen.EmitLoadArgument(i);
            }
            tIlGen.EmitCallInvokeFunc(tInvokeAddFuncType);
            tIlGen.Emit(OpCodes.Pop);
            tIlGen.Emit(OpCodes.Ret);

            tMp.SetAddOnMethod(tAddBuilder);

            var tRemovePrefixName = tRemoveMethod.Name;
            if (!defaultImp)
            {
                tRemovePrefixName = String.Format("{0}.{1}", info.DeclaringType.FullName, tRemovePrefixName);

            }

            //Remove Method
            var tRemoveBuilder = typeBuilder.DefineMethod(tRemovePrefixName,
                                                           tPublicPrivate | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
                                                           typeof(void), 
                                                           tAddParamTypes);
            if (!defaultImp)
            {
                typeBuilder.DefineMethodOverride(tRemoveBuilder, info.GetRemoveMethod());
            }

            foreach (var tParam in tRemoveMethod.GetParameters())
            {
                tRemoveBuilder.DefineParameter(tParam.Position + 1, AttributesForParam(tParam), tParam.Name);
            }


            tIlGen = tRemoveBuilder.GetILGenerator();


            using (tIlGen.EmitBranchTrue(load => load.Emit(OpCodes.Ldsfld, tIsEventField)))
            {
                tIlGen.EmitDynamicIsEventBinder(CSharpBinderFlags.None, tName, contextType);
                tIlGen.EmitCallsiteCreate(tInvokeIseventFuncType);
                tIlGen.Emit(OpCodes.Stsfld, tIsEventField);
            }

            using (tIlGen.EmitBranchTrue(
                             load => load.EmitInvocation(
                                  target => target.EmitInvocation(
                                      t=>t.Emit(OpCodes.Ldsfld, tIsEventField),
                                      i=>i.Emit(OpCodes.Ldfld, tIsEventField.FieldType.GetFieldEvenIfGeneric("Target"))
                                  ),
                                  invoke => invoke.EmitCallInvokeFunc(tInvokeIseventFuncType),
                                  param => param.Emit(OpCodes.Ldsfld, tIsEventField),
                                  param => param.EmitInvocation(
                                         t => t.Emit(OpCodes.Ldarg_0),
                                         i => i.Emit(OpCodes.Call, typeof(ActLikeProxy).GetProperty("Original").GetGetMethod())
                                  ) 
                            )
                     )
                ) //if IsEvent Not True
            {
                
                using (tIlGen.EmitBranchTrue(gen=>gen.Emit(OpCodes.Ldsfld, tSetField)))
                {
                    tIlGen.EmitDynamicSetBinderDynamicParams(CSharpBinderFlags.ValueFromCompoundAssignment, tName, contextType, tReturnType);
                    tIlGen.EmitCallsiteCreate(tInvokeSetFuncType);
                    tIlGen.Emit(OpCodes.Stsfld, tSetField);
                }

                var tSubrtractAssignField = tCallSite.GetFieldEvenIfGeneric(tInvokeSubtractAssign);
             
                using (tIlGen.EmitBranchTrue(gen => gen.Emit(OpCodes.Ldsfld, tSubrtractAssignField)))
                {
                    tIlGen.EmitDynamicBinaryOpBinder(CSharpBinderFlags.None, ExpressionType.SubtractAssign, contextType, tReturnType);
                    tIlGen.EmitCallsiteCreate(tInvokeSubtractAssignFuncType);
                    tIlGen.Emit(OpCodes.Stsfld, tSubrtractAssignField);
                }


                using (tIlGen.EmitBranchTrue(gen => gen.Emit(OpCodes.Ldsfld, tGetField)))
                {
                    tIlGen.EmitDynamicGetBinder(CSharpBinderFlags.None, tName, contextType);
                    tIlGen.EmitCallsiteCreate(tInvokeGetFuncType);
                    tIlGen.Emit(OpCodes.Stsfld, tGetField);
                }

                tIlGen.Emit(OpCodes.Ldsfld, tSetField);
                tIlGen.Emit(OpCodes.Ldfld, tSetField.FieldType.GetFieldEvenIfGeneric("Target"));
                tIlGen.Emit(OpCodes.Ldsfld, tSetField);
                tIlGen.Emit(OpCodes.Ldarg_0);
                tIlGen.Emit(OpCodes.Call, typeof(ActLikeProxy).GetProperty("Original").GetGetMethod());

                tIlGen.Emit(OpCodes.Ldsfld, tSubrtractAssignField);
                tIlGen.Emit(OpCodes.Ldfld, tSubrtractAssignField.FieldType.GetFieldEvenIfGeneric("Target"));
                tIlGen.Emit(OpCodes.Ldsfld, tSubrtractAssignField);

                tIlGen.Emit(OpCodes.Ldsfld, tGetField);
                tIlGen.Emit(OpCodes.Ldfld, tGetField.FieldType.GetFieldEvenIfGeneric("Target"));
                tIlGen.Emit(OpCodes.Ldsfld, tGetField);
                tIlGen.Emit(OpCodes.Ldarg_0);
                tIlGen.Emit(OpCodes.Call, typeof(ActLikeProxy).GetProperty("Original").GetGetMethod());

                tIlGen.EmitCallInvokeFunc(tInvokeGetFuncType);

                tIlGen.Emit(OpCodes.Ldarg_1);
                tIlGen.EmitCallInvokeFunc(tInvokeSubtractAssignFuncType);
                
                tIlGen.EmitCallInvokeFunc(tInvokeSetFuncType);

                tIlGen.Emit(OpCodes.Pop);
                tIlGen.Emit(OpCodes.Ret);
             
            }

            var tRemoveCallSiteField = tCallSite.GetFieldEvenIfGeneric(tInvokeRemove);
            using (tIlGen.EmitBranchTrue(gen => gen.Emit(OpCodes.Ldsfld, tRemoveCallSiteField)))
            {
                tIlGen.EmitDynamicMethodInvokeBinder(
                    CSharpBinderFlags.InvokeSpecialName | CSharpBinderFlags.ResultDiscarded,
                    tRemoveMethod.Name,
                    contextType,
                    tRemoveMethod.GetParameters(),
                    Enumerable.Repeat(default(string),
                    tRemoveParamTypes.Length));
                tIlGen.EmitCallsiteCreate(tInvokeRemoveFuncType);
                tIlGen.Emit(OpCodes.Stsfld, tRemoveCallSiteField);
            }
            tIlGen.Emit(OpCodes.Ldsfld, tRemoveCallSiteField);
            tIlGen.Emit(OpCodes.Ldfld, tRemoveCallSiteField.FieldType.GetFieldEvenIfGeneric("Target"));
            tIlGen.Emit(OpCodes.Ldsfld, tRemoveCallSiteField);
            tIlGen.Emit(OpCodes.Ldarg_0);
            tIlGen.Emit(OpCodes.Call, typeof(ActLikeProxy).GetProperty("Original").GetGetMethod());
            tIlGen.Emit(OpCodes.Ldarg_1);
            tIlGen.EmitCallInvokeFunc(tInvokeRemoveFuncType);
            tIlGen.Emit(OpCodes.Pop);
            tIlGen.Emit(OpCodes.Ret);

            tMp.SetRemoveOnMethod(tRemoveBuilder);
        }


        /// <summary>
        /// Makes the property helper.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="tName">Name of the t.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="tReturnType">Type of the t return.</param>
        /// <param name="tSetMethod">The t set method.</param>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="tGetName">Name of the t get.</param>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="defaultImp">if set to <c>true</c> [default imp].</param>
        private static void MakePropertyHelper(PropertyInfo info, string tName, ModuleBuilder builder, Type tReturnType, MethodInfo tSetMethod, TypeBuilder typeBuilder, string tGetName, Type contextType, bool defaultImp)
        {
            var tIndexParamTypes = new Type[]{};
            if(info!=null)
                 tIndexParamTypes = info.GetIndexParameters().Select(it => it.ParameterType).ToArray();
            Type[] tSetParamTypes = null;
            Type tInvokeSetFuncType = null;

            var tCallSiteInvokeName = string.Format("Impromptu_Callsite_{1}_{0}", Guid.NewGuid().ToString("N"), tName);
            var tCStp = DefineBuilderForCallSite(builder, tCallSiteInvokeName);


            var tConvertGet = "Convert_Get";
           
            var tConvertFuncType = tCStp.DefineCallsiteField(tConvertGet, tReturnType);

            var tInvokeGet = "Invoke_Get";
            var tInvokeGetFuncType = tCStp.DefineCallsiteField(tInvokeGet, typeof(object), tIndexParamTypes);
            
            var tInvokeSet = "Invoke_Set";
            if (tSetMethod != null)
            {
                tSetParamTypes = tSetMethod.GetParameters().Select(it => it.ParameterType).ToArray();
                
                tInvokeSetFuncType = tCStp.DefineCallsiteField(tInvokeSet, typeof (object), tSetParamTypes);
            }

            var tCallSite = tCStp.CreateType();


         
            var tPublicPrivate = MethodAttributes.Public;
            var tPrefixedGet = tGetName;
            var tPrefixedName = tName;
            if (!defaultImp)
            {
                tPublicPrivate = MethodAttributes.Private;
                tPrefixedGet = String.Format("{0}.{1}", info.DeclaringType.FullName, tPrefixedGet);

                tPrefixedName = String.Format("{0}.{1}", info.DeclaringType.FullName, tPrefixedName);
            }

           
            var tMp = typeBuilder.DefineProperty(tPrefixedName, PropertyAttributes.None, 
#if !SILVERLIGHT
                CallingConventions.HasThis,
#endif
 tReturnType, tIndexParamTypes);


    

            //GetMethod
            var tGetMethodBuilder = typeBuilder.DefineMethod(tPrefixedGet,
                                                             tPublicPrivate
                                                             | MethodAttributes.SpecialName 
                                                             | MethodAttributes.HideBySig
                                                             | MethodAttributes.Virtual
                                                             | MethodAttributes.Final 
                                                             | MethodAttributes.NewSlot,
                                                             tReturnType,
                                                             tIndexParamTypes);


            if (!defaultImp)
            {
                typeBuilder.DefineMethodOverride(tGetMethodBuilder, info.GetGetMethod());
            }


            if (info != null)
            {
                foreach (var tParam in info.GetGetMethod().GetParameters())
                {


                    tGetMethodBuilder.DefineParameter(tParam.Position + 1, AttributesForParam(tParam), tParam.Name);
                }
            }

            EmitProperty(
                info,
                tName, 
                tConvertGet,
                tReturnType, 
                tInvokeGet, 
                tIndexParamTypes,
                tSetMethod, 
                tInvokeSet, 
                tSetParamTypes, 
                typeBuilder,
                tGetMethodBuilder,
                tCallSite, 
                contextType,
                tConvertFuncType, 
                tInvokeGetFuncType, 
                tMp, 
                tInvokeSetFuncType, defaultImp);
        }

        private static void EmitProperty(
            PropertyInfo info, 
            string name, 
            string convertGet,
            Type tGetReturnType, 
            string invokeGet, 
            Type[] indexParamTypes,
            MethodInfo setMethod,
            string invokeSet,
            Type[] setParamTypes, 
            TypeBuilder typeBuilder,
            MethodBuilder getMethodBuilder, 
            Type callSite,
            Type contextType, 
            Type tConvertFuncType, 
            Type invokeGetFuncType, 
            PropertyBuilder tMp, 
            Type invokeSetFuncType, bool defaultImp)
        {
           
            if (indexParamTypes == null) throw new ArgumentNullException("indexParamTypes");
            var tIlGen = getMethodBuilder.GetILGenerator();

            var tConvertCallsiteField = callSite.GetFieldEvenIfGeneric(convertGet);
            var tReturnLocal = tIlGen.DeclareLocal(tGetReturnType);


       
            
            using (tIlGen.EmitBranchTrue(gen => gen.Emit(OpCodes.Ldsfld, tConvertCallsiteField)))
            {
                tIlGen.EmitDynamicConvertBinder(CSharpBinderFlags.None, tGetReturnType, contextType);
                tIlGen.EmitCallsiteCreate(tConvertFuncType);
                tIlGen.Emit(OpCodes.Stsfld, tConvertCallsiteField);
            }

            var tInvokeGetCallsiteField = callSite.GetFieldEvenIfGeneric(invokeGet);

            using (tIlGen.EmitBranchTrue(gen => gen.Emit(OpCodes.Ldsfld, tInvokeGetCallsiteField)))
            {
                tIlGen.EmitDynamicGetBinder(CSharpBinderFlags.None, name, contextType, indexParamTypes);
                tIlGen.EmitCallsiteCreate(invokeGetFuncType);
                tIlGen.Emit(OpCodes.Stsfld, tInvokeGetCallsiteField);
            }


            tIlGen.Emit(OpCodes.Ldsfld, tConvertCallsiteField);
            tIlGen.Emit(OpCodes.Ldfld, tConvertCallsiteField.FieldType.GetFieldEvenIfGeneric("Target"));
            tIlGen.Emit(OpCodes.Ldsfld, tConvertCallsiteField);
            tIlGen.Emit(OpCodes.Ldsfld, tInvokeGetCallsiteField);
            tIlGen.Emit(OpCodes.Ldfld, tInvokeGetCallsiteField.FieldType.GetFieldEvenIfGeneric("Target"));
            tIlGen.Emit(OpCodes.Ldsfld, tInvokeGetCallsiteField);
            tIlGen.Emit(OpCodes.Ldarg_0);
            tIlGen.Emit(OpCodes.Call, typeof(ActLikeProxy).GetProperty("Original").GetGetMethod());
            for (var i = 1; i <= indexParamTypes.Length; i++)
            {
                tIlGen.EmitLoadArgument(i);
            }
            tIlGen.EmitCallInvokeFunc(invokeGetFuncType);
            tIlGen.EmitCallInvokeFunc(tConvertFuncType);
            tIlGen.EmitStoreLocation(tReturnLocal.LocalIndex);
            var tReturnLabel =tIlGen.DefineLabel();
            tIlGen.Emit(OpCodes.Br_S, tReturnLabel);
            tIlGen.MarkLabel(tReturnLabel);
            tIlGen.EmitLoadLocation(tReturnLocal.LocalIndex);
            tIlGen.Emit(OpCodes.Ret);
            tMp.SetGetMethod(getMethodBuilder);

            if (setMethod != null)
            {

             
                MethodAttributes tPublicPrivate = MethodAttributes.Public;
                var tPrefixedSet = setMethod.Name;
                if (!defaultImp)
                {
                    tPublicPrivate = MethodAttributes.Private;
                    tPrefixedSet = String.Format("{0}.{1}", info.DeclaringType.FullName, tPrefixedSet);
                }


                var tSetMethodBuilder = typeBuilder.DefineMethod(tPrefixedSet,
                                                                 tPublicPrivate | MethodAttributes.SpecialName |
                                                                 MethodAttributes.HideBySig | MethodAttributes.Virtual |
                                                                 MethodAttributes.Final | MethodAttributes.NewSlot,
                                                                 null,
                                                                 setParamTypes);

                if (!defaultImp)
                {
                    typeBuilder.DefineMethodOverride(tSetMethodBuilder, info.GetSetMethod());
                }

                foreach (var tParam in info.GetSetMethod().GetParameters())
                {
                    tSetMethodBuilder.DefineParameter(tParam.Position + 1, AttributesForParam(tParam), tParam.Name);
                }

                tIlGen = tSetMethodBuilder.GetILGenerator();
                var tSetCallsiteField = callSite.GetFieldEvenIfGeneric(invokeSet);

                using (tIlGen.EmitBranchTrue(gen=>gen.Emit(OpCodes.Ldsfld, tSetCallsiteField)))
                {
                    tIlGen.EmitDynamicSetBinder(CSharpBinderFlags.None, name, contextType, setParamTypes);
                    tIlGen.EmitCallsiteCreate(invokeSetFuncType);
                    tIlGen.Emit(OpCodes.Stsfld, tSetCallsiteField);
                }
                tIlGen.Emit(OpCodes.Ldsfld, tSetCallsiteField);
                tIlGen.Emit(OpCodes.Ldfld, tSetCallsiteField.FieldType.GetFieldEvenIfGeneric("Target"));
                tIlGen.Emit(OpCodes.Ldsfld, tSetCallsiteField);
                tIlGen.Emit(OpCodes.Ldarg_0);
                tIlGen.Emit(OpCodes.Call, typeof (ActLikeProxy).GetProperty("Original").GetGetMethod());
                for (var i = 1; i <= setParamTypes.Length; i++)
                {
                    tIlGen.EmitLoadArgument(i);
                }
                tIlGen.EmitCallInvokeFunc(invokeSetFuncType);
                tIlGen.Emit(OpCodes.Pop);
                tIlGen.Emit(OpCodes.Ret);
                tMp.SetSetMethod(tSetMethodBuilder);
            }
        }


        private static Type UpdateCallsiteFuncType(Type tFuncGeneric, Type returnType, params Type[] argTypes)
        {
            var tList = new List<Type> { typeof(CallSite), typeof(object) };
            tList.AddRange(argTypes);
            if (returnType != typeof(void))
                tList.Add(returnType);

            IEnumerable<Type> tTypeArguments = tList;


            var tDef = tFuncGeneric.GetGenericTypeDefinition();

            if (tDef.GetGenericArguments().Count() != tTypeArguments.Count())
            {
                tTypeArguments = tTypeArguments.Where(it => it.IsGenericParameter);
            }

            var tFuncType = tDef.MakeGenericType(tTypeArguments.ToArray());

            return tFuncType;
        }

        private static Type ReduceToElementType(Type type)
        {
            if (type.IsByRef || type.IsPointer || type.IsArray)
                return type.GetElementType();
            return type;
        }

        private static Type DefineCallsiteFieldForMethod(this TypeBuilder builder, string name, Type returnType, IEnumerable<Type> argTypes, MethodInfo info)
        {
            Type tFuncType = GenerateCallSiteFuncType(argTypes, returnType, info, builder);
            Type tReturnType = typeof(CallSite<>).MakeGenericType(tFuncType);

            builder.DefineField(name, tReturnType, FieldAttributes.Static | FieldAttributes.Public);
            return tFuncType;

        }

        private static Type DefineCallsiteField(this TypeBuilder builder, string name, Type returnType, params Type[] argTypes)
        {
            Type tFuncType = GenerateCallSiteFuncType(argTypes, returnType);
            Type tReturnType = typeof(CallSite<>).MakeGenericType(tFuncType);

            builder.DefineField(name, tReturnType, FieldAttributes.Static | FieldAttributes.Public);
           return tFuncType;
            
        }

     


        /// <summary>
        /// Generates the delegate type of the call site function.
        /// </summary>
        /// <param name="argTypes">The arg types.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="methodInfo">The method info. Required for reference types or delegates with more than 16 arguments.</param>
        /// <param name="builder">The Type Builder. Required for reference types or delegates with more than 16 arguments.</param>
        /// <returns></returns>
        internal static Type GenerateCallSiteFuncType(IEnumerable<Type> argTypes, Type returnType, MethodInfo methodInfo =null, TypeBuilder builder =null)
        {
            bool tIsFunc = returnType != typeof(void);


            var tList = new List<Type> { typeof(CallSite), typeof(object) };
            tList.AddRange(argTypes.Select(it => (it.IsNotPublic && !it.IsByRef) ? typeof(object) : it));

            

            lock (DelegateCacheLock)
            {
               

                TypeHash tHash;
             
                if (tList.Any(it => it.IsByRef) || tList.Count > 16)
                {
                    tHash = TypeHash.Create(strictOrder: true, moreTypes: methodInfo);
                }else
                {
                    tHash = TypeHash.Create(strictOrder: true, moreTypes: tList.Concat(new[] {returnType}).ToArray());
                }

                Type tType =null;
                if (_delegateCache.TryGetValue(tHash, out tType))
                {
                    return tType;
                }

                if (tList.Any(it => it.IsByRef)
                    || (tIsFunc && tList.Count >= InvokeHelper.FuncKinds.Length)
                    || (!tIsFunc && tList.Count >= InvokeHelper.ActionKinds.Length))
                {
                    tType = GenerateFullDelegate(builder, methodInfo);
                    _delegateCache[tHash] = tType;
                    return tType;
                }



                if (tIsFunc)
                    tList.Add(returnType);

                var tFuncGeneric = Impromptu.GenericDelegateType(tList.Count, !tIsFunc);


                var tFuncType = tFuncGeneric.MakeGenericType(tList.ToArray());

                _delegateCache[tHash] = tFuncType;

                return tFuncType;

            }



        }

// ReSharper disable UnusedParameter.Local
// May switch to nested types if i figure out how to do it, thus would need the typebuilder
        private static Type GenerateFullDelegate(TypeBuilder builder,MethodInfo info)
// ReSharper restore UnusedParameter.Local
        {
                var tBuilder = Builder.DefineType(
                    string.Format("Impromptu_{0}_{1}", "Delegate", Guid.NewGuid().ToString("N")),
                    TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.NotPublic,
                    typeof (MulticastDelegate));

                var tReplacedTypes = GetParamTypes(tBuilder, info);

                var tReturnType = typeof (object);
                var tParamTypes = info.GetParameters().Select(it => it.ParameterType).ToList();
                if (tReplacedTypes != null)
                {
                    tParamTypes = tReplacedTypes.Item2.ToList();
                }

                tParamTypes.Insert(0, typeof (object));
                tParamTypes.Insert(0, typeof (CallSite));

                var tCon = tBuilder.DefineConstructor(
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig |
                    MethodAttributes.RTSpecialName, CallingConventions.Standard,
                    new[] {typeof (object), typeof (IntPtr)});

                tCon.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

                var tMethod = tBuilder.DefineMethod("Invoke",
                                                    MethodAttributes.Public | MethodAttributes.HideBySig |
                                                    MethodAttributes.NewSlot |
                                                    MethodAttributes.Virtual);

                tMethod.SetReturnType(tReturnType);
                tMethod.SetParameters(tParamTypes.ToArray());

                foreach (var tParam in info.GetParameters())
                {
                    //+3 because of the callsite and target are added
                    tMethod.DefineParameter(tParam.Position + 3, AttributesForParam(tParam), tParam.Name);
                }

                tMethod.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);



                return tBuilder.CreateType();
            
        }

        private static ParameterAttributes AttributesForParam(ParameterInfo param)
        {
            return param.Attributes;
        }

     
        ///<summary>
        /// Module Builder for buiding proxies
        ///</summary>
        static internal ModuleBuilder Builder
        {
            get
            {
                if (_builder == null)
                {

                    var access = AssemblyBuilderAccess.Run;
                    var tPlainName = "ImpromptuInterfaceDynamicAssembly";


                    GenerateAssembly(tPlainName, access, ref _ab,ref _builder);
                }
                return _tempBuilder ?? _builder;
            }
        }

        private static void GenerateAssembly(string name, AssemblyBuilderAccess access, ref AssemblyBuilder ab, ref ModuleBuilder mb )
        {
            var tName = new AssemblyName(name);

            ab =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    tName,
                    access);

            #if !SILVERLIGHT
            if (access== AssemblyBuilderAccess.RunAndSave || access == AssemblyBuilderAccess.Save)
                mb = ab.DefineDynamicModule("MainModule", string.Format("{0}.dll", tName.Name));
            else
            #endif
            mb = ab.DefineDynamicModule("MainModule");
        }
    }
    
}
