using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface.Internal.Support
{

#if !SILVERLIGHT5

    public interface ICustomTypeProvider
    {
       
    }

#endif

#if SILVERLIGHT
    /// <summary>
    /// Added for silverlight compile time compatability
    /// </summary>
    public sealed class SerializableAttribute : Attribute
    {
    }

    public sealed class NonSerialized : Attribute
    {
    }

    /// <summary>
    /// Added for silverlight compile time compatability
    /// </summary>
    public interface IObjectReference { }
    /// <summary>
    /// Added for silverlight compile time compatability
    /// </summary>
    public interface ISerializable { }
    /// <summary>
    /// Added for silverlight compile time compatability
    /// </summary>
    public interface ITypedList { }


    /// <summary>
    /// Added for silverlight compile time compatability
    /// </summary>
    public static class StructuralComparisons
    {

        private static IEqualityComparer _structuralEqualityComparer;
        /// <summary>
        /// Added for silverlight compile time compatability
        /// </summary>
        public static IEqualityComparer StructuralEqualityComparer
        {
            get { return _structuralEqualityComparer ?? (_structuralEqualityComparer = new StructuralEqualityComparer()); }
        }
    }

    [Serializable]
    internal class StructuralEqualityComparer : IEqualityComparer
    {
        public new bool Equals(Object x, Object y)
        {
            if (x != null)
            {
                var tObj = x as IStructuralEquatable;

                return tObj != null ? tObj.Equals(y, this) : x.Equals(y);
            }
            return y == null;
        }

        public int GetHashCode(Object obj)
        {
            var tObj = obj as IStructuralEquatable;

            return tObj != null ? tObj.GetHashCode(this) : obj.GetHashCode();
        }
    }
#endif
}
