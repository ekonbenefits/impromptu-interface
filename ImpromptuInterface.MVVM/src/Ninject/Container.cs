using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImpromptuInterface.Dynamic;

namespace ImpromptuInterface.MVVM.Ninject
{
    /// <summary>
    /// IoC container for Ninject
    /// </summary>
    public sealed class Container : IContainer
    {
        private readonly dynamic _kernel;
        private string[] _fullyQualifiedTypes;
        private InvokeContext _staticContext;

        /// <summary>
        /// Constructor, taking in an IKernel
        /// </summary>
        /// <param name="kernel"></param>
        public Container(dynamic kernel)
        {
            _kernel = kernel;

#if SILVERLIGHT
            var assembly = Assembly.GetCallingAssembly();
#else
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
#endif

            LateBind();
            ReflectNamespaces(assembly);
        }

        /// <summary>
        /// Constructor, taking in an IKernel and list of assemblies that Views and ViewModel could be found in
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="assemblies"></param>
        public Container(dynamic kernel, params Assembly[] assemblies)
        {
            _kernel = kernel;

            LateBind();
            ReflectNamespaces(assemblies);
        }

        /// <summary>
        /// Gets an exported value of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class
        {
            var name = InvokeMemberName.Create;
            return Impromptu.InvokeMember(_staticContext, name("Get", new Type[] {typeof (T)}), _kernel);
        }

        /// <summary>
        /// Gets an exported value by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic Get(string name)
        {
            return Impromptu.InvokeMember(_staticContext, "Get", _kernel, typeof(object), name);
        }

        /// <summary>
        /// Gets a list of exported values of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetMany<T>() where T : class
        {
            var name = InvokeMemberName.Create;
            return Impromptu.InvokeMember(_staticContext, name("GetAll", new Type[] { typeof(T) }), _kernel);
        }

        /// <summary>
        /// Gets a list of exported values by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetMany(string name)
        {
            return Impromptu.InvokeMember(_staticContext, "GetAll", _kernel, typeof(object), name);
        }

        /// <summary>
        /// Gets a View of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic GetView(string name)
        {
            return Impromptu.InvokeMember(_staticContext, "Get", _kernel, FindType(name + "View"));
        }

        /// <summary>
        /// Gets a ViewModel of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic GetViewModel(string name)
        {
            return Impromptu.InvokeMember(_staticContext, "Get", _kernel, FindType(name + "ViewModel"));
        }

        /// <summary>
        /// Gets the View for specified ViewModel
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public dynamic GetViewFor(dynamic viewModel)
        {
            Type type = viewModel.GetType();
            return Impromptu.InvokeMember(_staticContext, "Get", _kernel, FindType(type.Name.Replace("Model", string.Empty)));
        }

        private void LateBind()
        {
            Type type = _kernel.GetType();
            Type staticType = Type.GetType("Ninject.ResolutionExtensions, " + type.Assembly.FullName, true);
            _staticContext = InvokeContext.CreateStatic(staticType);
        }

        private void ReflectNamespaces(params Assembly[] assemblies)
        {
            _fullyQualifiedTypes = assemblies
                .Select(a => a.GetTypes().AsEnumerable())
                .Flatten()
                .Select(t => t.Namespace + ".{0}, " + t.Assembly.FullName)
                .Distinct()
                .ToArray();
        }

        private Type FindType(string name)
        {
            Type type = null;
            foreach (var typeName in _fullyQualifiedTypes)
            {
                type = Type.GetType(string.Format(typeName, name));
                if (type != null)
                {
                    return type;
                }
            }

            throw new Exception(string.Format("Could not find type for {0}!", name));
        }

        public IContainer AddView(string name, Type viewType, Type viewModelType)
        {
            throw new NotImplementedException();
        }
    }
}
