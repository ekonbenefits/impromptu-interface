using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using ImpromptuInterface.InvokeExt;
using Microsoft.CSharp.RuntimeBinder;
using UnitTestSupportLibrary;


#if !SELFRUNNER
using NUnit.Framework;
#endif

#if SILVERLIGHT
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{
    [TestFixture]
    public class PrivateTest : Helper
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
            var tNonExposed = tTest.WithContext(this).ActLike<IExposePrivateMethod>();
            AssertException<RuntimeBinderException>(() => tNonExposed.Test());
        }

        [Test]
        public void TestInvokePrivateMethod()
        {
            var tTest = new TestWithPrivateMethod();
            Assert.AreEqual(3, Impromptu.InvokeMember(tTest,"Test"));
        }

        [Test]
        public void TestInvokePrivateMethodAcrossAssemblyBoundries()
        {
            var tTest = new PublicType();
            Assert.AreEqual(true, Impromptu.InvokeMember(tTest, "PrivateMethod", 3));
        }

        [Test]
        public void TestInvokeInternalTypeMethodAcrossAssemblyBoundries()
        {
            var tTest = PublicType.InternalInstance;
            Assert.AreEqual(true, Impromptu.InvokeMember(tTest, "InternalMethod", 3));
        }

        [Test]
        public void TestInvokeDoNotExposePrivateMethod()
        {
            var tTest = new TestWithPrivateMethod();
            AssertException<RuntimeBinderException>(() => Impromptu.InvokeMember(tTest.WithContext(this), "Test"));
        }

        [Test]
        public void TestCacheableDoNotExposePrivateMethod()
        {
            var tTest = new TestWithPrivateMethod();
            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Test");
            AssertException<RuntimeBinderException>(() => tCachedInvoke.Invoke(tTest));
        }

        [Test]
        public void TestCacheableExposePrivateMethodViaInstance()
        {
            var tTest = new TestWithPrivateMethod();
            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Test", context: tTest);
            Assert.AreEqual(3, tCachedInvoke.Invoke(tTest));
        }

        [Test]
        public void TestCacheableExposePrivateMethodViaType()
        {
            var tTest = new TestWithPrivateMethod();
            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Test", context:typeof(TestWithPrivateMethod));
            Assert.AreEqual(3, tCachedInvoke.Invoke(tTest)); 
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