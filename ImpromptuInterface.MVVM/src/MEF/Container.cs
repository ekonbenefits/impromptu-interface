using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using ImpromptuInterface.Dynamic;

namespace ImpromptuInterface.MVVM.MEF
{
    /// <summary>
    /// An IContainer wrapping a MEF CompositionContainer for ImpromptuInterface.MVVM
    /// </summary>
    public sealed class Container : IContainer
    {
        private readonly CompositionContainer _container;
        private readonly Dictionary<Type, string> _viewLookup = new Dictionary<Type, string>();
        private readonly FluentStringLookup _viewStringLookup;
        private readonly FluentStringLookup _viewModelStringLookup;

        /// <summary>
        /// Default ctor, requires a MEF CompositionContainer
        /// </summary>
        /// <param name="container"></param>
        public Container(CompositionContainer container)
        {
            _container = container;
            _viewStringLookup = new FluentStringLookup(GetView);
            _viewModelStringLookup = new FluentStringLookup(GetViewModel);
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
            return Get(name + IoC.View);
        }

        /// <summary>
        /// Gets a View of the specified name View.Name()
        /// </summary>
        /// <value>The view.</value>
        public dynamic View
        {
            get { return _viewStringLookup; }
        }

        /// <summary>
        /// Gets a ViewModel of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic GetViewModel(string name)
        {
            return Get(name + IoC.ViewModel);
        }

        /// <summary>
        /// Gets a ViewModel of the specified name ViewModel.Name()
        /// </summary>
        /// <value>The view.</value>
        public dynamic ViewModel
        {
            get { return _viewModelStringLookup; }
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
                    name = attribute.ContractName.Replace(IoC.ViewModel, string.Empty);
                    _viewLookup[type] = name;
                    return GetView(name);
                }
            }

            throw new Exception("View not found!");
        }

        /// <summary>
        /// Registers a View/ViewModel pair with the container
        /// </summary>
        /// <param name="name"></param>
        /// <param name="viewType"></param>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        public IContainer AddView(string name, Type viewType, Type viewModelType)
        {
            throw new NotSupportedException("MEF does not support this operation!");
        }
    }
}
