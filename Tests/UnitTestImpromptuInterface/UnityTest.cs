using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;

#if !SELFRUNNER

using NUnit.Framework;
#endif

using ImpromptuInterface.MVVM;
using ImpromptuInterface.MVVM.Unity;
using Assert = NUnit.Framework.Assert;

#if SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{
    [TestFixture]
    public class UnityTest
    {
        [Test]
        public void Get_View()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterInstance(typeof(object), "Test__View__", new TestView());
            IContainer container = new Container(unityContainer, typeof(IUnityContainer));

            var view = container.View.Test();

            Assert.IsInstanceOf<TestView>(view);
        }

        [Test]
        public void Get_ViewModel()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterInstance(typeof(object), "Test__ViewModel__", new TestViewModel());
            IContainer container = new Container(unityContainer, typeof(IUnityContainer));

            var viewModel = container.ViewModel.Test();

            Assert.IsInstanceOf<TestViewModel>(viewModel);
        }

        [Test]
        public void Get_ViewFor()
        {
            var unityContainer = new UnityContainer();
            var container = new Container(unityContainer, typeof(IUnityContainer));
            container.AddView("Test", typeof(TestView), typeof(TestViewModel));

            var viewModel = container.GetViewModel("Test");
            var view = container.GetViewFor(viewModel);

            Assert.IsInstanceOf<TestView>(view);
        }

        [Test]
        public void Get_Many()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<ITestInterface, TestClassA>();
            unityContainer.RegisterType<ITestInterface, TestClassB>();
            IContainer container = new Container(unityContainer, typeof(IUnityContainer));

            foreach (var item in container.GetMany<ITestInterface>())
            {
                Assert.IsNotNull(item);
            }
        }

        [Test, ExpectedException(typeof(NotSupportedException))]
        public void Get_Many_Dynamic()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterInstance(typeof(object), "Testing123", new TestClassC());
            unityContainer.RegisterInstance(typeof(object), "Testing123", new TestClassD());
            IContainer container = new Container(unityContainer, typeof(IUnityContainer));

            foreach (var item in container.GetMany("Testing123"))
            {
                Assert.IsInstanceOf<ITestInterface>(item);
            }
        }

        [Test]
        public void Get()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<ITestInterface, TestClassA>();
            IContainer container = new Container(unityContainer, typeof(IUnityContainer));

            var item = container.Get<ITestInterface>();

            Assert.IsInstanceOf<TestClassA>(item);
        }

        [Test]
        public void Get_Dynamic()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterInstance(typeof(object), "Testing123", new TestClassC());
            IContainer container = new Container(unityContainer, typeof(IUnityContainer));

            var item = container.Get("Testing123");

            Assert.IsInstanceOf<TestClassC>(item);
        }
    }
}
