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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ImpromptuInterface
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

        public readonly MemberInfo[] Types;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeHash"/> class.
        /// </summary>
        /// <param name="moreTypes">The more types.</param>
        public TypeHash(IEnumerable<Type> moreTypes):this(false,moreTypes.ToArray())
        {
          
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeHash"/> class.
        /// For use when you have must distinguish one type; and the rest aren't strict
        /// </summary>
        /// <param name="type1">The type1.</param>
        /// <param name="type2">The type2.</param>
        /// <param name="moreTypes">The more types.</param>
        public TypeHash(Type type1, params Type[] moreTypes)
        {
            Types = new[] { type1 }.Concat(moreTypes.OrderBy(it => it.Name)).ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeHash"/> class.
        /// </summary>
        /// <param name="strictOrder">if set to <c>true</c> [strict order].</param>
        /// <param name="moreTypes">types.</param>
        public TypeHash(bool strictOrder, params MemberInfo[] moreTypes)
        {
            if (strictOrder)
            {
                Types = moreTypes;
            }
            else
            {
                Types = moreTypes.OrderBy(it => it.Name).ToArray();
            }


        }
    }
}
