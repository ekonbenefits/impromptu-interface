using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface
{
    /// <summary>
    /// Extensions methods for InvokeMemberName
    /// </summary>
    public static class InvokeMemberNameExtension
    {
        /// <summary>
        /// attaches generic args to string
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArgs">The generic args.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="ImpromptuInterface.String_OR_InvokeMemberName"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator String_OR_InvokeMemberName(string name)
        {
            return new InvokeMemberName(name, null);
        }


        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }
        /// <summary>
        /// Gets the generic args.
        /// </summary>
        /// <value>The generic args.</value>
        public Type[] GenericArgs { get; protected set; }
    }

    /// <summary>
    /// Name of Member with associated Generic parameterss
    /// </summary>
    public sealed class InvokeMemberName:String_OR_InvokeMemberName
    {

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="ImpromptuInterface.InvokeMemberName"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator InvokeMemberName(string name)
        {
            return new InvokeMemberName(name,null);
        }

       
        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeMemberName"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArgs">The generic args.</param>
        public InvokeMemberName(string name, params Type[] genericArgs)
        {
            Name = name;
            GenericArgs = genericArgs;
        }

        public bool Equals(InvokeMemberName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualsHelper(other);
        }
        
        private bool EqualsHelper(InvokeMemberName other)
        {

            var tGenArgs = GenericArgs;
            var tOtherGenArgs = other.GenericArgs;


            return Equals(other.Name, Name)
                && !(tOtherGenArgs == null ^ tGenArgs == null)
                && (tGenArgs == null || 
                //Exclusive Or makes sure this doesn't happen
// ReSharper disable AssignNullToNotNullAttribute
                tGenArgs.SequenceEqual(tOtherGenArgs));
// ReSharper restore AssignNullToNotNullAttribute
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is InvokeMemberName)) return false;
            return EqualsHelper((InvokeMemberName) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (GenericArgs != null ? GenericArgs.GetHashCode() * 397 : 0) ^ (Name.GetHashCode());
            }
        }
    }
}
