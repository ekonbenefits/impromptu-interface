using System;
using System.Collections;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ImpromptuInterface
{
#if SILVERLIGHT
    /// <summary>
    /// Added for silverlight compile time compatability
    /// </summary>
    public class SerializableAttribute:Attribute{
    }
    /// <summary>
    /// Added for silverlight compile time compatability
    /// </summary>
    public interface IObjectReference {}
    /// <summary>
    /// Added for silverlight compile time compatability
    /// </summary>
    public interface ISerializable {}
    /// <summary>
    /// Added for silverlight compile time compatability
    /// </summary>
    public interface ITypedList { }
#endif

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
}
