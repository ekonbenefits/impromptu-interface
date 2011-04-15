using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#elif !SELFRUNNER
using NUnit.Framework;
#endif

using ImpromptuInterface.MVVM;

namespace UnitTestImpromptuInterface
{

       
    [TestClass,TestFixture]
	public class MVVM:Helper
	{

         [Test,TestMethod]
        public void TestToStringProxy()
         {
             dynamic tProxy =
                 new {Test1 = "One", Test2 = "Two", TestAgain = "Again"}.ProxyToString(
                     it => string.Format("{0}:{1}", it.Test1, it.TestAgain));

             Assert.AreEqual("One:Again",tProxy.ToString());
         }

         [Test, TestMethod]
         public void TestToStringProxyCall()
         {
             var tAnon = new PropPoco() { Prop1 = "A", Prop2 = 1 };

             dynamic tProxy = tAnon.ProxyToString(
                     it => string.Format("{0}:{1}", it.Prop1, it.Prop2));


             Assert.AreEqual(tAnon.Prop2, tProxy.Prop2);
         }


         [Test,TestMethod]
        public void TestToStringProxyConvertExplicit()
         {
             var tAnon = new {Test1 = "One", Test2 = "Two", TestAgain = "Again"};

             dynamic tProxy = tAnon.ProxyToString(
                     it => string.Format("{0}:{1}", it.Test1, it.TestAgain));

             var tAnon2 = ExplictAnonCast(tAnon, tProxy);
             Assert.AreEqual(tAnon, tAnon2);
         }



         [Test, TestMethod]
         public void TestToStringProxyConvertImplicit()
         {
             var tAnon = new PropPoco(){Prop1 = "A", Prop2 = 1};

             dynamic tProxy = tAnon.ProxyToString(
                     it => string.Format("{0}:{1}", it.Prop1, it.Prop2));

             var tAnon2 = ImplicitCast(tProxy);
             Assert.AreEqual(tAnon.GetType(), tAnon2.GetType());
         }

         [Test, TestMethod]
         public void TestToStringProxyConvertImplicit2()
         {
             var tAnon = new PropPoco() { Prop1 = "A", Prop2 = 1 };

             dynamic tProxy = tAnon.ProxyToString(
                     it => string.Format("{0}:{1}", it.Prop1, it.Prop2));

             var tAnon2 = ImplicitCastDynamic(tProxy);
             Assert.AreEqual(tAnon.GetType(), tAnon2.GetType());
         }

        private T ExplictAnonCast<T>(T dummy, dynamic value)
        {
            return (T) value;
        }

        private PropPoco ImplicitCast(PropPoco value)
        {
            return value;
        }
        private PropPoco ImplicitCastDynamic(dynamic value)
        {
            return value;
        }
    }
}
