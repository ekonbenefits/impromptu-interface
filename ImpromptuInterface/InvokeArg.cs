using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface.Dynamic;

namespace ImpromptuInterface
{
    /// <summary>
    /// Use for Named arguments passed to InvokeMethods
    /// </summary>
    public class InvokeArg
    {
        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Collections.Generic.KeyValuePair&lt;System.String,System.Object&gt;"/> to <see cref="ImpromptuInterface.InvokeArg"/>.
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator InvokeArg(KeyValuePair<string, object> pair)
        {
            return new InvokeArg(pair.Key,pair.Value);
        }

        /// <summary>
        /// Create Function can set to variable to make cleaner syntax;
        /// </summary>
        public static readonly Func<string, object, InvokeArg> Create =
            Return<InvokeArg>.Arguments<string, object>((n, v) => new InvokeArg(n, v));


        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeArg"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public InvokeArg(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the argument name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the argument value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; private set; }
    }

    /// <summary>
    /// InvokeArg that makes it easier to Cast from any IDictionaryValue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InvokeArg<T> : InvokeArg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeArg&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public InvokeArg(string name, object value):base(name,value){}

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Collections.Generic.KeyValuePair&lt;System.String,T&gt;"/> to <see cref="ImpromptuInterface.InvokeArg&lt;T&gt;"/>.
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator InvokeArg<T>(KeyValuePair<string, T> pair)
        {
            return new InvokeArg<T>(pair.Key, pair.Value);
        }

    }
}
