using System;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Alias to swap method/property/event call name invoked on original
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Method
                    | System.AttributeTargets.Property
                    | System.AttributeTargets.Event )]
    [Obsolete("Use ImpromptuInterface.AliasAttribute")]
    public sealed class AliasAttribute : ImpromptuInterface.AliasAttribute
    {
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="AliasAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public AliasAttribute(string name):base(name)
        {

        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return _name; }

        }
    }
}