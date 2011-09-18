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
        internal static IContainer Container
        {
            get;
            private set;
        }

        internal static bool Initialized
        {
            get { return Container != null; }
        }

        /// <summary>
        /// Sets the container
        /// </summary>
        /// <param name="container"></param>
        internal static void Initialize(IContainer container)
        {
            Container = container;
        }

        /// <summary>
        /// Gets an exported value of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>()
             where T : class
        {
            return Container.Get<T>();
        }

        /// <summary>
        /// Gets an exported value by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static dynamic Get(string name)
        {
            return Container.Get(name);
        }

        /// <summary>
        /// Gets a list of exported values of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>()
             where T : class
        {
            return Container.GetMany<T>();
        }

        /// <summary>
        /// Gets a list of exported values by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> GetMany(string name)
        {
            return Container.GetMany(name);
        }

        /// <summary>
        /// Gets a View of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static dynamic GetView(string name)
        {
            return Container.GetView(name);
        }

        /// <summary>
        /// Gets a ViewModel of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static dynamic GetViewModel(string name)
        {
            return Container.GetViewModel(name);
        }

        /// <summary>
        /// Gets the View for specified ViewModel
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static dynamic GetViewFor(dynamic viewModel)
        {
            return Container.GetViewFor(viewModel);
        }
    }
}
