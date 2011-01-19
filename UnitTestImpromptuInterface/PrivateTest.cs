using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;

namespace UnitTestImpromptuInterface
{
    [TestFixture]
    public class PrivateTest : AssertionHelper
    {
        [Test]
        public void TestExposePrivateMethod()
        {
            var tTest = new TestWithPrivateMethod();
            var tExposed = tTest.ActLike<IExposePrivateMethod>();
            Assert.AreEqual(3, tExposed.Test()); 
        }

        [Test]
        public void TestDoNotExposePrivateMethod()
        {
            var tTest = new TestWithPrivateMethod();
            var tNonExposed = this.CallActLike<IExposePrivateMethod>(tTest);
            Assert.Throws<RuntimeBinderException>(() => tNonExposed.Test());
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