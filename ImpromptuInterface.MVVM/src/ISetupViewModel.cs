using System;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Describes view mdoel setup properties or actions
    /// </summary>
    public interface ISetupViewModel
    {
        /// <summary>
        /// Gets the property object.
        /// 
        ///  Syntax Property.PropertyName.DependsOn.OtherPropertyName1.OtherPropertyNameEtc.OtherPopertyNameLast()
        /// 
        ///  Syntax Property.PropertyName.RemoveDependency.OtherPropertyName1.OtherPropertyNameEtc.OtherPopertyNameLast()
        /// 
        ///  Syntax Property.Property.OnChange += new Change
        /// </summary>
        /// <value>The property.</value>
        dynamic Property { get; }

        /// <summary>
        /// Occurs when [error handler].
        /// </summary>
        event Action<Exception> CommandErrorHandler;

        /// <summary>
        /// Raises the <see cref="CommandErrorHandler"/> Event.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        bool RaiseCommandErrorHandler(Exception ex);
    }
}