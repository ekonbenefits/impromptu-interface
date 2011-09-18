using System;
using System.ComponentModel.Composition;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Attribute denoting a ViewModel by a specified name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewModelAttribute : ExportAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">Name of ViewModel</param>
        public ViewModelAttribute(string name)
            : base(name + Constants.ViewModel)
        { }
    }
}
