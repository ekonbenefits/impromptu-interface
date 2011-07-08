using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using NUnit.Framework;

namespace UnitTestImpromptuInterface
{
    [TestClass, TestFixture]
    public class Curry : Helper
    {
        [Test, TestMethod]
        public void TestBasicDelegateCurry()
        {
            Func<int, int, int> tAdd = (x, y) => x + y;
            var tCurriedAdd4 = Impromptu.Curry(tAdd)(4);
            var tResult = tCurriedAdd4(6);

       
            Assert.AreEqual(10, tResult);
     
        }

        [Test, TestMethod]
        public void TestBasicDelegateConversion()
        {
            Action<int> tTest = delegate { };
            Action<object> tTest2 = delegate { };
            Action<int> tFunc = (Action<int>)Delegate.Combine(tTest, tTest2);


        }

        public void c(object obj)
        {
            return;
        }

       /* [Test, TestMethod]
        public void TestBasicCastDelegateCurry()
        {
            Func<int, int, int> tAdd = (x, y) => x + y;
            var tCurriedAdd4 = Impromptu.Curry(tAdd)(4);
            var tCastToFunc = new Func<int, int> ((Func<object,int>)tCurriedAdd4);
            var tResult2 = tCastToFunc(10);

            Assert.AreEqual(14, tResult2);
        }*/


    }
}
