using System.ComponentModel.Composition.Hosting;
using TinyIoC;
using Ninject;

#if !SELFRUNNER
using NUnit.Framework;
#endif

using ImpromptuInterface;
using ImpromptuInterface.MVVM;

#if SILVERLIGHT
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{
    [TestFixture]
    public class RuntimeTest
    {
        [SetUp]
        public void SetUp()
        {
            var staticContext = InvokeContext.CreateStatic;

            Impromptu.InvokeSet(staticContext(typeof(IoC)), "Container", null);
        }

        #region MEF Tests

        [Test]
        public void Standard_MEF_Default_Container()
        {
            var runtime = Runtime.Initialize();
            runtime.SetupContainer();
            runtime.Start.Test();
        }

        [Test]
        public void Standard_MEF_Auto_Container()
        {
            var runtime = Runtime.Initialize();
            runtime.Start.Test();
        }

        [Test]
        public void Standard_MEF_IContainer()
        {
            var container = new CompositionContainer(new TypeCatalog(typeof (TestView), typeof (TestViewModel)));
            var runtime = Runtime.Initialize();
            runtime.SetupContainer(new ImpromptuInterface.MVVM.MEF.Container(container));
            runtime.Start.Test();
        }

        [Test]
        public void Standard_MEF_Container()
        {
            var container = new CompositionContainer(new TypeCatalog(typeof(TestView), typeof(TestViewModel)));
            var runtime = Runtime.Initialize();
            runtime.SetupContainer(container);
            runtime.Start.Test();
        }

        [Test]
        public void Fluent_MEF_Default_Container()
        {
            Runtime.Initialize().SetupContainer().Start.Test();
        }

        [Test]
        public void Fluent_MEF_Auto_Container()
        {
            Runtime.Initialize().Start.Test();
        }

        [Test]
        public void Fluent_MEF_IContainer()
        {
            var container = new CompositionContainer(new TypeCatalog(typeof(TestView), typeof(TestViewModel)));
            Runtime.Initialize().SetupContainer(new ImpromptuInterface.MVVM.MEF.Container(container)).Start.Test();
        }

        [Test]
        public void Fluent_MEF_Container()
        {
            var container = new CompositionContainer(new TypeCatalog(typeof(TestView), typeof(TestViewModel)));
            Runtime.Initialize().SetupContainer(container).Start.Test();
        }

        #endregion 

        #region TinyIoC Tests

        [Test]
        public void Standard_TinyIoC_IContainer()
        {
            var tinyContainer = new TinyIoCContainer();
            var container = new ImpromptuInterface.MVVM.TinyIoC.Container(tinyContainer);
            container.AddView("Test", typeof(TestView), typeof(TestViewModel));
            var runtime = Runtime.Initialize();
            runtime.SetupContainer(container);
            runtime.Start.Test();
        }

        [Test]
        public void Standard_TinyIoC_Container()
        {
            var tinyContainer = new TinyIoCContainer();
            var runtime = Runtime.Initialize();
            runtime.SetupContainer(tinyContainer);
            IoC.AddView("Test", typeof(TestView), typeof(TestViewModel));
            runtime.Start("Test");
        }

        [Test]
        public void Fluent_TinyIoC_IContainer()
        {
            var tinyContainer = new TinyIoCContainer();
            var container = new ImpromptuInterface.MVVM.TinyIoC.Container(tinyContainer);
            container.AddView("Test", typeof(TestView), typeof(TestViewModel));
            Runtime.Initialize().SetupContainer(container).Start("Test");
        }

        [Test]
        public void Fluent_TinyIoC_Container()
        {
            var tinyContainer = new TinyIoCContainer();
            var runtime = Runtime.Initialize().SetupContainer(tinyContainer);
            IoC.AddView("Test", typeof(TestView), typeof(TestViewModel));
            runtime.Start("Test");
        }

        #endregion 

        #region Ninject Tests

        [Test]
        public void Standard_Ninject_IContainer()
        {
            var kernel = new StandardKernel();
            var container = new ImpromptuInterface.MVVM.Ninject.Container(kernel,typeof(IKernel));
            var runtime = Runtime.Initialize();
            runtime.SetupContainer(container);
            runtime.Start("Test");
        }

        [Test]
        public void Standard_Ninject_Container()
        {
            var kernel = new StandardKernel();
            var runtime = Runtime.Initialize();
            runtime.SetupContainer(kernel);
            runtime.Start("Test");
        }

        [Test]
        public void Fluent_Ninject_IContainer()
        {
            var kernel = new StandardKernel();
            var container = new ImpromptuInterface.MVVM.Ninject.Container(kernel, typeof(IKernel));
            Runtime.Initialize().SetupContainer(container).Start("Test");
        }

        [Test]
        public void Fluent_Ninject_Container()
        {
            var kernel = new StandardKernel();
            var runtime = Runtime.Initialize().SetupContainer(kernel);
            runtime.Start.Test();
        }

        #endregion 
    }
}
