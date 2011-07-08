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

        public delegate bool TestDeclaredDelagate(string value);
        [Test, TestMethod]
        public void TestBasicCastNonGenericDelegate()
        {
            Func<string, string, bool> tContains = (x, y) => y.Contains(x);
            var tCurriedContains = Impromptu.Curry(tContains)("it");
            var tCastToDel = (TestDeclaredDelagate) tCurriedContains;
            var tResult = tCastToDel("bait");
            Assert.AreEqual(true, tResult);
        }
        public delegate void TestRunDelagate(string value);
        [Test, TestMethod]
        public void TestBasicCastNonGenericDelegateAction()
        {
            var tBool = false;
            Action<string, string> tContains = (x, y) => tBool =y.Contains(x);
            var tCurriedContains = Impromptu.Curry(tContains)("it");
            var tCastToDel = (TestRunDelagate)tCurriedContains;
            tCastToDel("bait");
            Assert.AreEqual(true, tBool);
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
