using System;
using ImpromptuInterface.MVVM.TinyIoC;

using TinyIoC;

#if !SELFRUNNER
using NUnit.Framework;
#endif

using ImpromptuInterface.MVVM;
using Assert = NUnit.Framework.Assert;

#if SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{
    [TestFixture]
    public class TinyIoCTest
    {
        [Test]
        public void Get_View()
        {
            var tinyContainer = new TinyIoCContainer();
            tinyContainer.Register<object>(new TestView(), "Test__View__");
            IContainer container = new Container(tinyContainer);

            var view = container.View.Test();

            Assert.IsInstanceOf<TestView>(view);
        }

        [Test]
        public void Get_ViewModel()
        {
            var tinyContainer = new TinyIoCContainer();
            tinyContainer.Register<object>(new TestViewModel(), "Test__ViewModel__");
            IContainer container = new Container(tinyContainer);

            var viewModel = container.ViewModel.Test();

            Assert.IsInstanceOf<TestViewModel>(viewModel);
        }

        [Test]
        public void Get_ViewFor()
        {
            var tinyContainer = new TinyIoCContainer();
            var container = new Container(tinyContainer);
            container.AddView("Test", typeof(TestView), typeof(TestViewModel));

            var viewModel = container.GetViewModel("Test");
            var view = container.GetViewFor(viewModel);

            Assert.IsInstanceOf<TestView>(view);
        }

        [Test]
        public void Get_Many()
        {
            var tinyContainer = new TinyIoCContainer();
            tinyContainer.Register<ITestInterface>(new TestClassA());
            tinyContainer.Register<ITestInterface>(new TestClassB());
            IContainer container = new Container(tinyContainer);

            foreach (var item in container.GetMany<ITestInterface>())
            {
                Assert.IsNotNull(item);
            }
        }

        [Test]
        public void Get_Many_Dynamic()
        {
            var tinyContainer = new TinyIoCContainer();
            tinyContainer.Register<object>(new TestClassC(), "Testing123");
            tinyContainer.Register<object>(new TestClassD(), "Testing123");
            IContainer container = new Container(tinyContainer);

            Assert.Throws<NotSupportedException>(() => container.GetMany("Testing123"));
        }

        [Test]
        public void Get()
        {
            var tinyContainer = new TinyIoCContainer();
            tinyContainer.Register<ITestInterface>(new TestClassA());
            IContainer container = new Container(tinyContainer);

            var item = container.Get<ITestInterface>();

            Assert.IsInstanceOf<TestClassA>(item);
        }

        [Test]
        public void Get_Dynamic()
        {
            var tinyContainer = new TinyIoCContainer();
            tinyContainer.Register<object>(new TestClassC(), "Testing123");
            IContainer container = new Container(tinyContainer);

            var item = container.Get("Testing123");

            Assert.IsInstanceOf<TestClassC>(item);
        }
    }
}
