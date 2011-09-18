using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        [Test]
        public void Standard_MEF_Container()
        {
            var runtime = Runtime.Initialize();
            runtime.SetupContainer();
            runtime.Start("Test");
        }

        [Test]
        public void Standard_MEF_Auto_Container()
        {
            var runtime = Runtime.Initialize();
            runtime.Start("Test");
        }

        [Test]
        public void Fluent_MEF_Container()
        {
            Runtime.Initialize().SetupContainer().Start("Test");
        }

        [Test]
        public void Fluent_MEF_Auto_Container()
        {
            Runtime.Initialize().Start("Test");
        }
    }
}
