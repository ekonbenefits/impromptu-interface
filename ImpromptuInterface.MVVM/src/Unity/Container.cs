using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface.Dynamic;

namespace ImpromptuInterface.MVVM.Unity
{
    /// <summary>
    /// An IContainer wrapping a Unity container for NoMvvm
    /// </summary>
    public class Container : IContainer
    {
        private readonly dynamic _container;
        private readonly Type _containerInterface;
        private readonly Dictionary<Type, string> _viewLookup = new Dictionary<Type, string>();
        private InvokeContext _staticContext;
        private dynamic _unityContainerExtensions;
        private readonly FluentStringLookup _viewStringLookup;
        private FluentStringLookup _viewModelStringLookup;

        /// <summary>
        /// Default ctor, requires an IUnityContainer
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="containerInterface">The container interface.</param>
        public Container(dynamic container, Type containerInterface)
        {
            _container = container;
            _containerInterface = containerInterface;
            _viewStringLookup = new FluentStringLookup(GetView);
            _viewModelStringLookup = new FluentStringLookup(GetViewModel);
            LateBind();
        }

        private void LateBind()
        {
            Type staticType = Type.GetType("Microsoft.Practices.Unity.UnityContainerExtensions, " + _containerInterface.Assembly.FullName, true);
            _unityContainerExtensions = new ImpromptuLateLibraryType(staticType);
            _staticContext = InvokeContext.CreateStatic(staticType);
        }

        /// <summary>
        /// Gets an exported value of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class
        {
            return Impromptu.InvokeMember(_staticContext, new InvokeMemberName("Resolve", typeof (T)), _container);
        }

        /// <summary>
        /// Gets an exported value by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic Get(string name)
        {
            return Impromptu.InvokeMember(_staticContext, new InvokeMemberName("Resolve", typeof(object)), _container, name);
        }

        /// <summary>
        /// Gets a list of exported values of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetMany<T>() where T : class
        {
            return Impromptu.InvokeMember(_staticContext, new InvokeMemberName("ResolveAll", typeof(T)), _container);
        }

        /// <summary>
        /// Gets a list of exported values by export name
        /// NOTE: TinyIoC does not support this operation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetMany(string name)
        {
            throw new NotSupportedException("Unity does not support this operation!");
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
            string name;
            if (_viewLookup.TryGetValue(viewModel.GetType(), out name))
            {
                return GetView(name);
            }

            //TODO: better error info here
            throw new Exception("View not found!");
        }

        /// <summary>
        /// Registers a View/ViewModel pair with the Unity container
        /// </summary>
        /// <param name="name"></param>
        /// <param name="viewType"></param>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        public IContainer AddView(string name, Type viewType, Type viewModelType)
        {

            _unityContainerExtensions.RegisterType(_container,typeof(object), viewType, name + IoC.View);
            _unityContainerExtensions.RegisterType(_container,typeof(object), viewModelType, name + IoC.ViewModel);
            _viewLookup[viewModelType] = name;
            return this;
        }
    }
}
