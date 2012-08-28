using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using ImpromptuInterface.Internal.Support;
using ImpromptuInterface.Optimization;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Dynamic
{


    [Serializable]
    public class ExtensionToInstanceProxy: ImpromptuForwarder
    {
        private readonly Type _extendedType;
        private readonly Type[] _staticTypes;
        private readonly Type[] _instanceHints;

        public IEnumerable<Type> InstanceHints
        {
            get { return _instanceHints ?? KnownInterfaces; }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuObject"/> class. when deserializing
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ExtensionToInstanceProxy(SerializationInfo info,
                                           StreamingContext context) : base(info, context)
        {
            _staticTypes = info.GetValue<Type[]>("_staticTypes");
            _extendedType = info.GetValue<Type>("_extendedType");
            _instanceHints = info.GetValue<Type[]>("_instanceHints");
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_staticType", _extendedType);
            info.AddValue("_extendedType", _staticTypes);
            info.AddValue("_instanceHints", _staticTypes);
        }
#endif

        public ExtensionToInstanceProxy(dynamic target,  Type extendedType, Type[] staticTypes, Type[] instanceHints = null):base((object)target)
        {
            _staticTypes = staticTypes;
            _extendedType = extendedType;
            _instanceHints = instanceHints;

            if(target is ExtensionToInstanceProxy)
                throw new ArgumentException("Don't Nest ExtensionToInstance Objects");

            if (IsExtendedType(target))
            {
                return;
            }

            throw new ArgumentException(String.Format("Non a valid {0} to be wrapped.",_extendedType));
            
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {

            if (!base.TryGetMember(binder, out result))
            {

                var tInterface = CallTarget.GetType().GetInterface(_extendedType.Name, false);
                result = new Invoker(binder.Name,
                                     tInterface.IsGenericType ? tInterface.GetGenericArguments() : new Type[] {},null, this);
            }
            return true;
        }

        public class Invoker:ImpromptuObject
        {
            protected string _name;
            protected ExtensionToInstanceProxy _parent;
            protected IDictionary<int, Type[]> _overloadTypes;
            protected Type[] _genericParams;
            protected Type[] _genericMethodParameters;

            internal Invoker(string name, Type[] genericParameters, Type[] genericMethodParameters, ExtensionToInstanceProxy parent, Type[] overloadTypes = null)
            {
                _name = name;
                _parent = parent;
                _genericParams = genericParameters;
                _genericMethodParameters = genericMethodParameters;
                _overloadTypes = new Dictionary<int,Type[]>();

                if (overloadTypes == null)
                {

                    foreach (var tGenInterface in parent.InstanceHints)
                    {
                        var tNewType = tGenInterface;

                        if (tNewType.IsGenericType)
                        {
                            tNewType = tNewType.MakeGenericType(_genericParams);
                        }

                        var members = tNewType.GetMethods(BindingFlags.Instance |
                                                                                   BindingFlags.Public).Where(
                                                                                       it => it.Name == _name).ToList();
                        foreach (var tMethodInfo in members)
                        {
                            var tParams = tMethodInfo.GetParameters().Select(it => it.ParameterType).ToArray();

                            if (_overloadTypes.ContainsKey(tParams.Length))
                            {
                                _overloadTypes[tParams.Length] = new Type[] {};
                            }
                            else
                            {
                                _overloadTypes[tParams.Length] = tParams.Select(ReplaceGenericTypes).ToArray();
                            }
                        }

                        foreach (var tOverloadType in _overloadTypes.ToList())
                        {
                            if (tOverloadType.Value.Length == 0)
                            {
                                _overloadTypes.Remove(tOverloadType);
                            }
                        }

                    }
                }
                else
                    {
                        _overloadTypes[overloadTypes.Length] = overloadTypes;
                    }
            }

            private Type ReplaceGenericTypes(Type type)
            {
                if (type.IsGenericType && type.ContainsGenericParameters)
                {
                    var tArgs = type.GetGenericArguments();

                    tArgs = tArgs.Select(ReplaceGenericTypes).ToArray();

                    return type.GetGenericTypeDefinition().MakeGenericType(tArgs);
                }

                if (type.ContainsGenericParameters)
                {
                    return typeof (object);
                }
               
                return type;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (binder.Name == "Overloads")
                {
                    result = new OverloadInvoker(_name, _genericParams,_genericMethodParameters, _parent);
                    return true;
                }
                return base.TryGetMember(binder, out result);
            }

          

            public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
                object[] tArgs = args;
                if (_overloadTypes.ContainsKey(args.Length))
                {
                    tArgs = _overloadTypes[args.Length].Zip(args, Tuple.Create)
                        .Select(it => it.Item2 != null ? Impromptu.InvokeConvert(it.Item2, it.Item1, @explicit: true) : null).ToArray();
                    
                }

                var name = InvokeMemberName.Create(_name, _genericMethodParameters);

                result = _parent.InvokeStaticMethod(name, tArgs);
                return true;
            }

            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                result = new Invoker(_name, _genericParams, indexes.Select(it => Impromptu.InvokeConvert(it, typeof(Type), @explicit: true)).Cast<Type>().ToArray(), _parent);
                return true;
            }
        }

        public class OverloadInvoker:Invoker
        {
            internal OverloadInvoker(string name, Type[] genericParameters, Type[] genericMethodParameters, ExtensionToInstanceProxy parent)
                : base(name, genericParameters,genericMethodParameters, parent)
            {
            }


            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                result = new Invoker(_name, _genericParams,_genericMethodParameters, _parent, indexes.Select(it => Impromptu.InvokeConvert(it, typeof(Type), @explicit: true)).Cast<Type>().ToArray());
                return true;
            }
        }


        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {
            if (!base.TryInvokeMember(binder, args, out result))
            {

                Type[] types = null;
                try
                {
                    IList<Type> typeList =Impromptu.InvokeGet(binder,
                                           "Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder.TypeArguments");
                    if(typeList != null)
                    {

                        types = typeList.ToArray();
                        
                    }

                }catch(RuntimeBinderException)
                {
                    types = null;
                }

                var name=InvokeMemberName.Create;
                result = InvokeStaticMethod(name(binder.Name, types), args);
            }
            return true;
        }

        protected object InvokeStaticMethod(String_OR_InvokeMemberName name, object[] args)
        {
            var staticType = InvokeContext.CreateStatic;
            var nameArgs = InvokeMemberName.Create;

            var tList = new List<object> { CallTarget };
            tList.AddRange(args);

            object result =null;
            var sucess = false;
            var exceptionList = new List<Exception>();

            var tGenericPossibles = new List<Type[]>();
            if (name.GenericArgs != null && name.GenericArgs.Length > 0)
            {
                var tInterface = CallTarget.GetType().GetInterface(_extendedType.Name, false);
                var tTypeGenerics = (tInterface.IsGenericType ? tInterface.GetGenericArguments()
                                            : new Type[] { }).Concat(name.GenericArgs).ToArray();

                tGenericPossibles.Add(tTypeGenerics);
                tGenericPossibles.Add(name.GenericArgs);
            }
            else
            {
                tGenericPossibles.Add(null);
            }
                      


            foreach (var sType in _staticTypes)
            {
                foreach (var tGenericPossible in tGenericPossibles)
                {
                    try
                    {
                        result = Impromptu.InvokeMember(staticType(sType), nameArgs(name.Name,tGenericPossible), tList.ToArray());
                        sucess = true;
                        break;
                    }
                    catch (RuntimeBinderException ex)
                    {
                        exceptionList.Add(ex);
                    }
                }
            }

            if (!sucess)
            {

#if SILVERLIGHT && !SILVERLIGHT5
                throw exceptionList.First();
#else
                throw new AggregateException(exceptionList);
#endif
            }


            Type tOutType;
            if (TryTypeForName(name.Name, out tOutType))
            {
                if (tOutType.IsInterface)
                {
                    var tIsGeneric = tOutType.IsGenericType;
                    if (tOutType.IsGenericType)
                    {
                        tOutType = tOutType.GetGenericTypeDefinition();
                    }

                    if (InstanceHints.Select(it => tIsGeneric && it.IsGenericType ? it.GetGenericTypeDefinition() : it)
                            .Contains(tOutType))
                    {
                        result = CreateSelf(result, _extendedType, _staticTypes, _instanceHints);
                    }
                }
            }
            else
            {
                if (IsExtendedType(result))
                {
                    result = CreateSelf(result, _extendedType, _staticTypes, _instanceHints);
                }
            }

            return result;
        }

        protected virtual ExtensionToInstanceProxy CreateSelf(object target, Type extendedType, Type[] staticTypes, Type[] instanceHints)
        {
            return  new ExtensionToInstanceProxy(target,extendedType,staticTypes, instanceHints);
        }

        private bool IsExtendedType(object target)
        {

            if (target is ExtensionToInstanceProxy)
            {
                return false;
            }

            bool genericDef = _extendedType.IsGenericTypeDefinition;

            return target.GetType().GetInterfaces().Any(
                it => ((genericDef && it.IsGenericType) ? it.GetGenericTypeDefinition() : it) == _extendedType);

        }

        
    }
}
