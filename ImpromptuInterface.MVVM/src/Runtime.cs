using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Class for setting up NoMvvm
    /// </summary>
    public sealed class Runtime
    {
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
        /// Initializes the runtime
        /// </summary>
        /// <returns></returns>
        public static Runtime Initialize()
        {
            return new Runtime();
        }

        /// <summary>
        /// Initializes the runtime with specified IContainer
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public Runtime SetupContainer(IContainer container)
        {
            IoC.Initialize(container);
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
            dynamic viewModel = IoC.GetViewModel(name);
            MainView =
                Application.Current.RootVisual = viewModel.View;
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
            dynamic viewModel = IoC.GetViewModel(name);
            MainView = viewModel.View;
            MainView.Show();
            return this;
        }
#endif
    }
}
