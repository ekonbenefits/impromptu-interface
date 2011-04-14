using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface
{
    public static class InvokeMemberNameExtension
    {
        public static InvokeMemberName WithGenericArgs(this string name, params Type[] genericArgs)
        {
            return new InvokeMemberName(name, genericArgs);
        }
    }


    /// <summary>
    /// String or InvokeMemberName
    /// </summary>
    public abstract class String_OR_InvokeMemberName
    {
        public static implicit operator String_OR_InvokeMemberName(string name)
        {
            return new InvokeMemberName(name, null);
        }


        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public abstract string Name { get;  }
        /// <summary>
        /// Gets the generic args.
        /// </summary>
        /// <value>The generic args.</value>
        public abstract Type[] GenericArgs { get; }
    }

    public class InvokeMemberName:String_OR_InvokeMemberName
    {
        
        public static implicit operator InvokeMemberName(string name)
        {
            return new InvokeMemberName(name,null);
        }

        private string _name;
        public override string Name
        {
            get { return _name; }
        }

        private Type[] _genericArgs;
        public override Type[] GenericArgs
        {
            get { return _genericArgs; }
        }

        public InvokeMemberName(string name, params Type[] genericArgs)
        {
            _name = name;
            _genericArgs = genericArgs;
        }

        public bool Equals(InvokeMemberName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name)
                && ((ReferenceEquals(GenericArgs, other.GenericArgs) 
                || (!ReferenceEquals(GenericArgs, null) && GenericArgs.SequenceEqual(other.GenericArgs))));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (InvokeMemberName)) return false;
            return Equals((InvokeMemberName) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (GenericArgs != null ? GenericArgs.GetHashCode() : 0);
            }
        }
    }
}
