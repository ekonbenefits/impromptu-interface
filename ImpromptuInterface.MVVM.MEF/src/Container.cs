using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace ImpromptuInterface.MVVM.MEF
{
    /// <summary>
    /// An IContainer wrapping a MEF CompositionContainer for ImpromptuInterface.MVVM
    /// </summary>
    public sealed class Container : IContainer
    {
        private readonly CompositionContainer _container;
        private readonly Dictionary<Type, string> _viewLookup = new Dictionary<Type, string>();

        /// <summary>
        /// Default ctor, requires a MEF CompositionContainer
        /// </summary>
        /// <param name="container"></param>
        public Container(CompositionContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Gets an exported value of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>()
             where T : class
        {
            return _container.GetExportedValue<T>();
        }

        /// <summary>
        /// Gets an exported value by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic Get(string name)
        {
            return _container.GetExportedValue<object>(name);
        }

        /// <summary>
        /// Gets a list of exported values of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetMany<T>()
             where T : class
        {
            return _container.GetExportedValues<T>();
        }

        /// <summary>
        /// Gets a list of exported values by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetMany(string name)
        {
            return _container.GetExportedValues<object>(name);
        }

        /// <summary>
        /// Gets a View of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic GetView(string name)
        {
            return Get(name + Constants.View);
        }

        /// <summary>
        /// Gets a ViewModel of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic GetViewModel(string name)
        {
            return Get(name + Constants.ViewModel);
        }

        /// <summary>
        /// Gets the View for specified ViewModel
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public dynamic GetViewFor(dynamic viewModel)
        {
            Type type = viewModel.GetType();
            string name;
            if (_viewLookup.TryGetValue(type, out name))
            {
                return GetView(name);
            }
            else
            {
                var attribute = type
                    .GetCustomAttributes(typeof(ViewModelAttribute), false)
                    .Cast<ViewModelAttribute>()
                    .FirstOrDefault();
                if (attribute != null)
                {
                    name = attribute.ContractName.Replace(Constants.ViewModel, string.Empty);
                    _viewLookup[type] = name;
                    return GetView(name);
                }
            }

            //TODO: better error info here
            throw new Exception("View not found!");
        }
    }
}
