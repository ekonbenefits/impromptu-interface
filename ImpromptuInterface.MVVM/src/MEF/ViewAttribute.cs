using System;
using System.ComponentModel.Composition;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Attribute denoting a View by a specified name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewAttribute : ExportAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">Name of View</param>
        public ViewAttribute(string name)
            : base(name + Constants.View)
        { }
    }
}
