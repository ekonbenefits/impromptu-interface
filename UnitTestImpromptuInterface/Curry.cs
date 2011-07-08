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
        public void TestBasicConvert()
        {
            var tONe = new Func<object, string>(it => it.ToString());
            Func<string, string> tTest= tONe;
            Func<object, string> tTest2 = tONe;

            tTest("test");
            tTest2(2);
        }

        [Test, TestMethod]
        public void TestBasicCastDelegateCurry()
        {
            Func<string, string, string> tAdd = (x, y) => x + y;
            var tCurriedAdd4 = Impromptu.Curry(tAdd)("4");
            var tCastToFunc = (Func<string, string>)tCurriedAdd4;
            var tResult2 = tCastToFunc("10");

            Assert.AreEqual("410", tResult2);
        }
        [Test, TestMethod]
        public void TestBasicCastDelegateCurryReturnValueType()
        {
            Func<string, string, int> tAdd = (x, y) => Int32.Parse(x) + Int32.Parse(y);
            var tCurriedAdd4 = Impromptu.Curry(tAdd)("4");
            var tCastToFunc = (Func<string, int>)tCurriedAdd4;
            var tResult2 = tCastToFunc("10");

            Assert.AreEqual(14, tResult2);
        }

        [Test, TestMethod]
        public void TestBasicCastDelegateCurryParamValueType()
        {
            Func<int, int, int> tAdd = (x, y) => x + y;
            var tCurriedAdd4 = Impromptu.Curry(tAdd)(4);
            var tCastToFunc = (Func<int, int>)tCurriedAdd4;
            var tResult2 = tCastToFunc(10);

            Assert.AreEqual(14, tResult2);
        }

    }
}
