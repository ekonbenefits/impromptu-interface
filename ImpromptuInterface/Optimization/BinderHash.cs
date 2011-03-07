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
using System.Linq;
using System.Text;

namespace ImpromptuInterface.Optimization
{
    internal class BinderHash
    {
        protected BinderHash(Type delegateType, string name, Type context)
        {
            DelegateType = delegateType;
            Name = name;
            Context = context;
        }

        public static BinderHash Create(Type delType, string name, Type context)
        {
            return new BinderHash(delType, name, context);
        }



        public Type DelegateType { get; protected set; }
        public string Name { get; protected set; }
        public Type Context { get; protected set; }


        public virtual bool Equals(BinderHash other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.DelegateType, DelegateType) && Equals(other.Name, Name) && Equals(other.Context, Context);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is BinderHash)) return false;
            return Equals((BinderHash)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = DelegateType.GetHashCode();
                result = (result * 397) ^ Name.GetHashCode();
                result = (result * 397) ^ Context.GetHashCode();
                return result;
            }
        }
    }

    internal class GenericBinderHashBase : BinderHash
    {
        protected GenericBinderHashBase(Type delegateType, string name, Type context)
            : base(delegateType, name, context)
        {
        }
    }

    internal class BinderHash<T> : GenericBinderHashBase where T : class
    {

        public static BinderHash<T> Create(string name, Type context)
        {
            return new BinderHash<T>(name, context);
        }

        protected BinderHash(string name, Type context)
            : base(typeof(T), name, context)
        {
        }


        public override bool Equals(BinderHash other)
        {
            if (other is GenericBinderHashBase)
            {
                if (other is BinderHash<T>)
                {
                    return Equals(other.Name, Name) && Equals(other.Context, Context);
                }
                return false;
            }
            return base.Equals(other);
        }
    }
}
