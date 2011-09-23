using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Class for setting up NoMvvm
    /// </summary>
    public sealed class Runtime
    {
        private readonly Dictionary<string, Func<dynamic, Assembly, Type, IContainer>> _containerLookup = new Dictionary<string, Func<dynamic, Assembly, Type, IContainer>>
        {
            { "System.ComponentModel.Composition.Hosting.CompositionContainer", (c, a, t) => new MEF.Container(c) },
            { "TinyIoC.TinyIoCContainer", (c, a, t) => new TinyIoC.Container(c) },
            { "Microsoft.Practices.Unity.IUnityContainer", (c, a, t) => new Unity.Container(c,t) },
            { "Ninject.IKernel", (c, a, t) => new Ninject.Container(c, t, a) },
        };
        private Assembly _callingAssembly = null;

        private Runtime()
        { }

        /// <summary>
        /// Gets the "Main" view of the application, the one invoked from Runtime.Start()
        /// </summary>
        public static dynamic MainView
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates an instance of Runtime
        /// </summary>
        /// <returns></returns>
        public static Runtime Initialize()
        {
            return new Runtime
            {
                _callingAssembly = Assembly.GetCallingAssembly(),
            };
        }

        /// <summary>
        /// Initializes the runtime with the default IoC container
        /// </summary>
        /// <returns></returns>
        public Runtime SetupContainer()
        {
            var container = new CompositionContainer(new AssemblyCatalog(_callingAssembly));
            IoC.Initialize(new MEF.Container(container));
            return this;
        }

        /// <summary>
        /// Initializes the runtime with specified IoC container
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public Runtime SetupContainer(dynamic container)
        {
            if (container is IContainer)
            {
                IoC.Initialize(container);
            }
            else
            {
                Type type = container.GetType();
                Func<dynamic, Assembly, Type, IContainer> func;
                if (_containerLookup.TryGetValue(type.FullName, out func))
                {
                    IoC.Initialize(func(container, _callingAssembly, type));
                }
                else
                {
                    foreach (var @interface in type.GetInterfaces())
                    {
                        if (_containerLookup.TryGetValue(@interface.FullName, out func))
                        {
                            IoC.Initialize(func(container, _callingAssembly, @interface));
                            return this;
                        }
                    }

                    throw new ArgumentException(string.Format("Container of type '{0}' is not a valid IoC container!", type));
                }
            }
            return this;
        }

#if SILVERLIGHT
        /// <summary>
        /// Starts the runtime by setting the specified View to Application.Current.RootVisual
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Runtime Start(string name)
        {
            if (!IoC.Initialized)
            {
                SetupContainer();
            }
            dynamic viewModel = IoC.GetViewModel(name);
            MainView =
                System.Windows.Application.Current.RootVisual = viewModel.View;
            return this;
        }
#else
        /// <summary>
        /// Starts the runtime by opening the specified View (Window)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Runtime Start(string name)
        {
            if (!IoC.Initialized)
            {
                SetupContainer();
            }
            dynamic viewModel = IoC.GetViewModel(name);
            MainView = viewModel.View;
            MainView.Show();
            return this;
        }
#endif
    }
}
