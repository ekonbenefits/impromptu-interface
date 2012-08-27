using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject;

#if !SELFRUNNER
using NUnit.Framework;
#endif

using ImpromptuInterface.MVVM;
using ImpromptuInterface.MVVM.Ninject;

#if SILVERLIGHT
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{
    [TestFixture]
    public class NinjectTest
    {
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        [Test]
        public void Get_View()
        {
            var kernel = new StandardKernel();
            var container = new Container(kernel, typeof(IKernel), _assembly);
            var view = container.View.Test();

            Assert.IsInstanceOf<TestView>(view);
        }

        [Test]
        public void Get_ViewModel()
        {
            var kernel = new StandardKernel();
            var container = new Container(kernel, typeof(IKernel), _assembly);
            var viewModel = container.ViewModel.Test();

            Assert.IsInstanceOf<TestViewModel>(viewModel);
        }

        [Test]
        public void Get_ViewFor()
        {
            var kernel = new StandardKernel();
            var container = new Container(kernel, typeof(IKernel), _assembly);

            var viewModel = container.GetViewModel("Test");
            var view = container.GetViewFor(viewModel);

            Assert.IsInstanceOf<TestView>(view);
        }

        [Test]
        public void Get_Many()
        {
            var kernel = new StandardKernel();
            var container = new Container(kernel, typeof(IKernel), _assembly);

            foreach (var item in container.GetMany<ITestInterface>())
            {
                Assert.IsNotNull(item);
            }
        }

        [Test]
        public void Get_Many_Dynamic()
        {
            var kernel = new StandardKernel();
            var container = new Container(kernel, typeof(IKernel), _assembly);

            foreach (var item in container.GetMany("Testing123"))
            {
                Assert.IsInstanceOf<ITestInterface>(item);
            }
        }

        [Test]
        public void Get()
        {
            var kernel = new StandardKernel();
            var container = new Container(kernel, typeof(IKernel), _assembly);
            kernel.Bind<ITestInterface>().To<TestClassA>();
            var item = container.Get<ITestInterface>();

            Assert.IsInstanceOf<TestClassA>(item);
        }

        [Test]
        public void Get_Dynamic()
        {
            var kernel = new StandardKernel();
            var container = new Container(kernel, typeof(IKernel), _assembly);
            kernel.Bind<object>().To<TestClassC>().Named("Testing123");
            var item = container.Get("Testing123");

            Assert.IsInstanceOf<TestClassC>(item);
        }
    }
}
