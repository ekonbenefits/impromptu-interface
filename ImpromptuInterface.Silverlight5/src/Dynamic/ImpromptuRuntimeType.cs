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
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ImpromptuInterface.Dynamic
{

    /// <summary>
    /// Runtime Property Info
    /// </summary>
    public class ImpromptuRuntimePropertyInfo:PropertyInfo
    {
        private readonly Type _declaringType;
        private readonly string _propertyName;

        private readonly CacheableInvocation _cachedGet;
        private readonly CacheableInvocation _cachedSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuRuntimePropertyInfo"/> class.
        /// </summary>
        /// <param name="declaringType">Type of the declaring.</param>
        /// <param name="propertyName">Name of the property.</param>
        public ImpromptuRuntimePropertyInfo(Type declaringType, string propertyName)
        {
            _declaringType = declaringType;
            _propertyName = propertyName;
            _cachedGet =  new CacheableInvocation(InvocationKind.Get, propertyName);
            _cachedSet =  new CacheableInvocation(InvocationKind.Set, propertyName);
            
        }


        public override object[] GetCustomAttributes(bool inherit)
        {
            return new object[]{};
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            if (index !=null && index.Length > 0)
            {
                return Impromptu.InvokeGetIndex(obj, index);
            }

            return _cachedGet.Invoke(obj);
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            if (index != null && index.Length > 0)
            {
                Impromptu.InvokeGetIndex(obj, index.Concat(new[] {value}).ToArray() );
                return;
            }
            _cachedSet.Invoke(obj, value);
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            return  new MethodInfo[]{};
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return null;
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return null;
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            return new ParameterInfo[]{};
        }

        public override string Name
        {
            get { return _propertyName; }
        }

        public override Type DeclaringType
        {
            get { return _declaringType; }
        }

        public override Type ReflectedType
        {
            get { return _declaringType; }
        }

        public override Type PropertyType
        {
            get { return typeof (object); }
        }

        public override PropertyAttributes Attributes
        {
            get { return PropertyAttributes.None; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
           return new object[]{};
        }
    }


    /// <summary>
    /// MetaData for dynamic types
    /// </summary>
    public class ImpromptuRuntimeType:Type
    {
        private readonly Type _baseType;
        private readonly object _dynamicObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuRuntimeType"/> class.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="dynamicObject">The dynamic object.</param>
        public ImpromptuRuntimeType(Type baseType, object dynamicObject)
        {
            _baseType = baseType;
            _dynamicObject = dynamicObject;
        }


        public override object[] GetCustomAttributes(bool inherit)
        {
           return _baseType.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return IsDefined(attributeType, inherit);
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return _baseType.GetConstructors(bindingAttr);
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            return _baseType.GetInterface(name, ignoreCase);
        }

        public override Type[] GetInterfaces()
        {
            return _baseType.GetInterfaces();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            return _baseType.GetEvent(name, bindingAttr);
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return _baseType.GetEvents(bindingAttr);
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return _baseType.GetNestedTypes(bindingAttr);
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            return _baseType.GetNestedType(name, bindingAttr);
        }

        public override MemberInfo[] GetDefaultMembers()
        {

            return _baseType.GetDefaultMembers();
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return new ImpromptuRuntimePropertyInfo(this, name);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            if (_dynamicObject is DynamicObject)
            {
               return ((DynamicObject)_dynamicObject).GetDynamicMemberNames().Select(it => new ImpromptuRuntimePropertyInfo(this, it)).ToArray();
            }
            return _baseType.GetProperties(bindingAttr);
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return null;
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return _baseType.GetMethods(bindingAttr);
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return _baseType.GetField(name, bindingAttr);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return _baseType.GetFields(bindingAttr);
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            return new MemberInfo[]{};
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return TypeAttributes.Public;
        }

        protected override bool IsArrayImpl()
        {
            return false;
        }

        protected override bool IsByRefImpl()
        {
            return false;
        }

        protected override bool IsPointerImpl()
        {
            return false;
        }

        protected override bool IsPrimitiveImpl()
        {
            return false;
        }

        protected override bool IsCOMObjectImpl()
        {
            return false;
        }

        public override Type GetElementType()
        {
            return null;
        }

        protected override bool HasElementTypeImpl()
        {
            return false;
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            object tContext = typeof (object);
            InvocationKind tKind;
            if ((BindingFlags.GetProperty & invokeAttr) != 0)
            {
                tKind = name.Equals(Invocation.IndexBinderName) ? InvocationKind.GetIndex : InvocationKind.Get;
            }
            else if ((BindingFlags.SetProperty & invokeAttr) != 0)
            {
                tKind = name.Equals(Invocation.IndexBinderName) ? InvocationKind.SetIndex : InvocationKind.Set;

            }
            else
            {
                tKind = InvocationKind.InvokeMemberUnknown;
            }

            if((BindingFlags.NonPublic & invokeAttr) !=0)
            {
                tContext = target;
            }
            //Use cachedable invocation not because it's getting cached, but because the constructor matches the parameters better.
            var tCachedInvocation = new CacheableInvocation(tKind, name, args.Length, namedParameters, tContext);
            return tCachedInvocation.Invoke(target, args);
        }

        public override Type UnderlyingSystemType
        {
            get { return UnderlyingSystemType; }
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {

            return null;
        }

        public override string Name
        {
            get { return _baseType.Name; }
        }

        public override Guid GUID
        {
            get { return _baseType.GUID; }
        }

        public override Module Module
        {
            get { return _baseType.Module; }
        }

        public override Assembly Assembly
        {
            get { return _baseType.Assembly; }
        }

        public override string FullName
        {
            get { return _baseType.FullName; }
        }

        public override string Namespace
        {
            get { return _baseType.Namespace; }
        }

        public override string AssemblyQualifiedName
        {
            get { return _baseType.AssemblyQualifiedName; }
        }

        public override Type BaseType
        {
            get { return _baseType.BaseType; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _baseType.GetCustomAttributes(attributeType, inherit);
        }
    }
}
