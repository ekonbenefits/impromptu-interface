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
        public void TestBasicConvertMoreCurryParamValueType()
        {
            Func<int, int, int, int> tAdd = (x, y, z) => x + y + z;
            Func<int, Func<int,int>> Curry1 =Impromptu.Curry(tAdd)(4);
            Func<int, int> Curry2 = Curry1(6);
            int tResult = Curry2(10);

            Assert.AreEqual(20, tResult);
        }

        [Test, TestMethod]
        public void TestBasicConvertMoreMoreCurryParamValueType()
        {
            Func<int, int, int, int, int> tAdd = (x, y, z, bbq) => x + y + z +bbq;
            Func<int, Func<int, Func<int, Func<int, int>>>> Curry0 = Impromptu.Curry(tAdd);
            var Curry1 = Curry0(4);
            var Curry2 = Curry1(5);
            var Curry3 = Curry2(6);
            var tResult = Curry3(20);

            Assert.AreEqual(35, tResult);
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
        public void TestStaticMethodCurry()
        {

            var curry = Impromptu.Curry((StaticContext)typeof(string), 5).Format(); // curry method target include argument count
            curry = curry("Test {0}, {1}, {2}, {3}");
            curry = curry("A");
            curry = curry("B");
            curry = curry("C");
            string result = curry("D");
            Assert.AreEqual("Test A, B, C, D", result);
        }
              
      

        [Test, TestMethod]
        public void TestStaticMethodCurry2()
        {

            object curriedJoin = Impromptu.Curry((StaticContext)typeof(string), 51).Join(",");

            Func<dynamic, int, dynamic> applyFunc = (result, each) => result(each.ToString());

            string final = Enumerable.Range(1, 100)
                .Where(i => i % 2 == 0)
                .Aggregate(curriedJoin, applyFunc);

            Console.WriteLine(final);


        }
#if !SILVERLIGHT
        [Test, TestMethod]
        public void TestStaticMethodCurry3()
        {
            var tFormat =Enumerable.Range(0, 100).Aggregate(new StringBuilder(), (result, each) => result.Append("{" + each + "}")).ToString();


            dynamic curriedWrite = Impromptu.Curry(Console.Out, 101).WriteLine(tFormat);

            Func<dynamic, int, dynamic> applyArgs = (result, each) => result(each.ToString());

            Enumerable.Range(0, 100).Aggregate((object)curriedWrite, applyArgs);

        }
#endif

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
