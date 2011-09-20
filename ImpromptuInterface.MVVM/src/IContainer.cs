using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Interface describing an IoC container for use within Impromptu.MVVM
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Gets an exported value of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>() where T : class;

        /// <summary>
        /// Gets an exported value by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        dynamic Get(string name);

        /// <summary>
        /// Gets a list of exported values of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetMany<T>() where T : class;

        /// <summary>
        /// Gets a list of exported values by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetMany(string name);

        /// <summary>
        /// Gets a View of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        dynamic GetView(string name);

        /// <summary>
        /// Gets a ViewModel of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        dynamic GetViewModel(string name);

        /// <summary>
        /// Gets the View for specified ViewModel
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        dynamic GetViewFor(dynamic viewModel);

        /// <summary>
        /// Registers a View/ViewModel pair with the container
        /// </summary>
        /// <param name="name"></param>
        /// <param name="viewType"></param>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        IContainer AddView(string name, Type viewType, Type viewModelType);
    }
}
