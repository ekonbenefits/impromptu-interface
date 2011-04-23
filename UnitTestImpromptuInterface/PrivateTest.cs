using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using ImpromptuInterface.InvokeExt;
using Microsoft.CSharp.RuntimeBinder;

#if SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#elif !SELFRUNNER
using NUnit.Framework;
#endif

namespace UnitTestImpromptuInterface
{
    [TestFixture,TestClass]
    public class PrivateTest : Helper
    {
        [Test, TestMethod]
        public void TestExposePrivateMethod()
        {
            var tTest = new TestWithPrivateMethod();
            var tExposed = tTest.ActLike<IExposePrivateMethod>();
            Assert.AreEqual(3, tExposed.Test()); 
        }

        [Test, TestMethod]
        public void TestDoNotExposePrivateMethod()
        {
            var tTest = new TestWithPrivateMethod();
            var tNonExposed = tTest.WithContext(this).ActLike<IExposePrivateMethod>();
            AssertException<RuntimeBinderException>(() => tNonExposed.Test());
        }
    }

    public class TestWithPrivateMethod
    {
        private int Test()
        {
            return 3;
        }
    }


    public interface IExposePrivateMethod
    {
        int Test();
    }
}