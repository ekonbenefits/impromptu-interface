using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Simple static class for service location, provides all methods on IContainer
    /// </summary>
    public static class IoC
    {
        private static IContainer _container = null;

        /// <summary>
        /// Sets the container
        /// </summary>
        /// <param name="container"></param>
        internal static void Initialize(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Gets an exported value of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>()
             where T : class
        {
            return _container.Get<T>();
        }

        /// <summary>
        /// Gets an exported value by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static dynamic Get(string name)
        {
            return _container.Get(name);
        }

        /// <summary>
        /// Gets a list of exported values of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>()
             where T : class
        {
            return _container.GetMany<T>();
        }

        /// <summary>
        /// Gets a list of exported values by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> GetMany(string name)
        {
            return _container.GetMany(name);
        }

        /// <summary>
        /// Gets a View of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static dynamic GetView(string name)
        {
            return _container.GetView(name);
        }

        /// <summary>
        /// Gets a ViewModel of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static dynamic GetViewModel(string name)
        {
            return _container.GetViewModel(name);
        }

        /// <summary>
        /// Gets the View for specified ViewModel
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static dynamic GetViewFor(dynamic viewModel)
        {
            return _container.GetViewFor(viewModel);
        }
    }
}
