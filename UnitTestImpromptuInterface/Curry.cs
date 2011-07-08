using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;

#if SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#elif !SELFRUNNER
using NUnit.Framework;
#endif


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
        public void TestBasicNamedCurry()
        {
            Func<int, int, int> tSub = (x, y) => x - y;
            var tCurriedSub7 = Impromptu.Curry(tSub)(arg2:7);
            var tResult = tCurriedSub7(arg1:10);


            Assert.AreEqual(3, tResult);

        }
    
        [Test, TestMethod]
        public void TestBasicConvertDelegateCurry()
        {
            Func<string, string, string> tAdd = (x, y) => x + y;
            var tCurriedAdd4 = Impromptu.Curry(tAdd)("4");
            var tCastToFunc = (Func<string, string>)tCurriedAdd4;
            var tResult2 = tCastToFunc("10");

            Assert.AreEqual("410", tResult2);
        }
        [Test, TestMethod]
        public void TestBasicConvertDelegateCurryReturnValueType()
        {
            Func<string, string, int> tAdd = (x, y) => Int32.Parse(x) + Int32.Parse(y);
            var tCurriedAdd4 = Impromptu.Curry(tAdd)("4");
            Func<string, int> tCastToFunc = tCurriedAdd4;
            var tResult2 = tCastToFunc("10");

            Assert.AreEqual(14, tResult2);
        }

        public delegate bool TestDeclaredDelagate(string value);
        [Test, TestMethod]
        public void TestBasicConvertNonGenericDelegate()
        {
            Func<string, string, bool> tContains = (x, y) => y.Contains(x);
            var tCurriedContains = Impromptu.Curry(tContains)("it");
            TestDeclaredDelagate tCastToDel =  tCurriedContains;
            var tResult = tCastToDel("bait");
            Assert.AreEqual(true, tResult);
        }
        public delegate void TestRunDelagate(string value);
        [Test, TestMethod]
        public void TestBasicConvertNonGenericDelegateAction()
        {
            var tBool = false;
            Action<string, string> tContains = (x, y) => tBool =y.Contains(x);
            var tCurriedContains = Impromptu.Curry(tContains)("it");
            TestRunDelagate tCastToDel = tCurriedContains;
            tCastToDel("bait");
            Assert.AreEqual(true, tBool);
        }

        [Test, TestMethod]
        public void TestBasicConvertDelegateCurryParamValueType()
        {
            Func<int, int, int> tAdd = (x, y) => x + y;
            var tCurriedAdd4 = Impromptu.Curry(tAdd)(4);
            Func<int, int> tCastToFunc = tCurriedAdd4;
            var tResult2 = tCastToFunc(10);

            Assert.AreEqual(14, tResult2);
        }

        [Test, TestMethod]
        public void TestPococMethodCurry()
        {
            var tNewObj = new PocoAdder();

            var tCurry = Impromptu.Curry(tNewObj).Add(4);
            var tResult = tCurry(10);
            Assert.AreEqual(14, tResult);
            //Test cached invocation;
            var tResult2 = tCurry(30);
            Assert.AreEqual(34, tResult2);
        }

        [Test, TestMethod]
        public void TestDynamicMethodCurry()
        {
            var tNewObj =Build.NewObject(Add: Return<int>.Arguments<int, int>((x, y) => x + y));

            var tCurry = Impromptu.Curry(tNewObj).Add(4);
            var tResult = tCurry(10);
            Assert.AreEqual(14,tResult);
            //Test cached invocation;
            var tResult2 = tCurry(30);
            Assert.AreEqual(34, tResult2);
        }

        [Test, TestMethod]
        public void UnboundedCurry()
        {
            var tNewObject = Impromptu.Curry(Build.NewObject);
            var tCurriedNewObject =tNewObject(One: 1);
            var tResult = tCurriedNewObject(Two: 2);
            Assert.AreEqual(1,tResult.One);
            Assert.AreEqual(2,tResult.Two);

        }
        [Test, TestMethod]
        public void UnboundedCurryCont()
        {
            var tNewObject = Impromptu.Curry(Build.NewObject);
            tNewObject = tNewObject(One: 1);
            tNewObject = Impromptu.Curry(tNewObject)(Two: 2);
            var tResult = tNewObject(Three: 3);
            Assert.AreEqual(1, tResult.One);
            Assert.AreEqual(2, tResult.Two);
            Assert.AreEqual(3, tResult.Three);
        }

        [Test, TestMethod]
        public void BoundedCurryCont()
        {
            var tNewObject = Impromptu.Curry(Build.NewObject,3);
            tNewObject = tNewObject(One: 1);
            tNewObject = tNewObject(Two: 2); 
            var tResult = tNewObject(Three: 3);
            Assert.AreEqual(1, tResult.One);
            Assert.AreEqual(2, tResult.Two);
            Assert.AreEqual(3, tResult.Three);
        }

        [Test, TestMethod]
        public void TestPococMethodPartialApply()
        {
            var tNewObj = new PocoAdder();
            var tCurry = Impromptu.Curry(tNewObj).Add(4,6);
            var tResult = tCurry();
            Assert.AreEqual(10, tResult);
        }
        
        [Test, TestMethod]
        public void UnboundedPartialApply()
        {
            var tNewObject = Impromptu.Curry(Build.NewObject);
            tNewObject = tNewObject(One: 1,Two:2);
            var tResult = tNewObject(Three: 3, Four:4);
            Assert.AreEqual(1, tResult.One);
            Assert.AreEqual(2, tResult.Two);
            Assert.AreEqual(3, tResult.Three);
            Assert.AreEqual(4, tResult.Four);

        }
    }
}
