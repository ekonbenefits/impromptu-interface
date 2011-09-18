using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using ImpromptuInterface.MVVM.MEF;

#if !SELFRUNNER
using NUnit.Framework;
#endif

using ImpromptuInterface.MVVM;

#if SILVERLIGHT
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{
    [TestFixture]
    public class MEFTest
    {
        #region Test Types

        [View("Test")]
        private sealed class TestView
        {

        }

        [ViewModel("Test")]
        private sealed class TestViewModel : ImpromptuViewModel
        {

        }

        private interface ITestInterface
        {
            void Test();
        }

        [Export(typeof(ITestInterface))]
        private sealed class TestClassA : ITestInterface
        {
            public void Test()
            {
                throw new NotImplementedException();
            }
        }

        [Export(typeof(ITestInterface))]
        private sealed class TestClassB : ITestInterface
        {
            public void Test()
            {
                throw new NotImplementedException();
            }
        }

        [Export("Testing123")]
        private sealed class TestClassC : ITestInterface
        {
            public void Test()
            {
                throw new NotImplementedException();
            }
        }

        [Export("Testing123")]
        private sealed class TestClassD : ITestInterface
        {
            public void Test()
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        [Test]
        public void Get_View()
        {
            var catalog = new TypeCatalog(typeof(TestView));
            var compositionContainer = new CompositionContainer(catalog);

            IContainer container = new Container(compositionContainer);

            var view = container.GetView("Test");

            Assert.IsInstanceOf<TestView>(view);
        }

        [Test]
        public void Get_ViewModel()
        {
            var catalog = new TypeCatalog(typeof(TestViewModel));
            var compositionContainer = new CompositionContainer(catalog);

            IContainer container = new Container(compositionContainer);

            var viewModel = container.GetViewModel("Test");

            Assert.IsInstanceOf<TestViewModel>(viewModel);
        }

        [Test]
        public void Get_ViewFor()
        {
            var catalog = new TypeCatalog(typeof(TestViewModel), typeof(TestView));
            var compositionContainer = new CompositionContainer(catalog);

            IContainer container = new Container(compositionContainer);

            var viewModel = container.GetViewModel("Test");
            var view = container.GetViewFor(viewModel);

            Assert.IsInstanceOf<TestView>(view);
        }

        [Test]
        public void Get_Many()
        {
            var catalog = new TypeCatalog(typeof(TestClassA), typeof(TestClassB));
            var compositionContainer = new CompositionContainer(catalog);

            IContainer container = new Container(compositionContainer);

            foreach (var item in container.GetMany<ITestInterface>())
            {
                Assert.IsInstanceOf<ITestInterface>(item);
            }
        }

        [Test]
        public void Get_Many_Dynamic()
        {
            var catalog = new TypeCatalog(typeof(TestClassC), typeof(TestClassD));
            var compositionContainer = new CompositionContainer(catalog);

            IContainer container = new Container(compositionContainer);

            foreach (var item in container.GetMany("Testing123"))
            {
                Assert.IsInstanceOf<ITestInterface>(item);
            }
        }

        [Test]
        public void Get()
        {
            var catalog = new TypeCatalog(typeof(TestClassA));
            var compositionContainer = new CompositionContainer(catalog);

            IContainer container = new Container(compositionContainer);

            var item = container.Get<ITestInterface>();

            Assert.IsInstanceOf<TestClassA>(item);
        }

        [Test]
        public void Get_Dynamic()
        {
            var catalog = new TypeCatalog(typeof(TestClassC));
            var compositionContainer = new CompositionContainer(catalog);

            IContainer container = new Container(compositionContainer);

            var item = container.Get("Testing123");

            Assert.IsInstanceOf<TestClassC>(item);
        }
    }
}
