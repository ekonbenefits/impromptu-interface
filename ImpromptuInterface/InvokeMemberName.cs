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

    public class InvokeMemberName
    {
       


        public static implicit operator InvokeMemberName(string name)
        {
            return new InvokeMemberName(name,null);
        }

        public string Name { get; set; }
        public Type[] GenericArgs { get; set; }

        public InvokeMemberName(string name, params Type[] genericArgs)
        {
            Name = name;
            GenericArgs = genericArgs;
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
