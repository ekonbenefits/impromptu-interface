using System;
using System.Collections.Generic;
using System.Linq;
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

        public readonly Type[] Types;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeHash"/> class.
        /// </summary>
        /// <param name="moreTypes">The more types.</param>
        public TypeHash(IEnumerable<Type> moreTypes)
        {
            Types = moreTypes.OrderBy(it => it.Name).ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeHash"/> class.
        /// For use when you have must distinguish one type;
        /// </summary>
        /// <param name="type1">The type1.</param>
        /// <param name="type2">The type2.</param>
        /// <param name="moreTypes">The more types.</param>
        public TypeHash(Type type1, params Type[] moreTypes)
        {
            Types = new[] { type1}.Concat(moreTypes).OrderBy(it => it.Name).ToArray();
        }
    }
}
