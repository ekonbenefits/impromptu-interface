using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace ImpromptuInterface.MVVM.MEF
{
    /// <summary>
    /// A set of extensions for setting up MEF with NoMvvm
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Sets up MEF with default assembly (startup assembly)
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public static Runtime SetupMEF(this Runtime runtime)
        {
#if SILVERLIGHT
            var container = new CompositionContainer(new AssemblyCatalog(Assembly.GetCallingAssembly()));
#else
            var container = new CompositionContainer(new AssemblyCatalog(Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()));
#endif
            return runtime.SetupMEF(container);
        }

        /// <summary>
        /// Sets up MEF with specified CompositionContainer
        /// </summary>
        /// <param name="runtime"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static Runtime SetupMEF(this Runtime runtime, CompositionContainer container)
        {
            return runtime.SetupContainer(new Container(container));
        }
    }
}
