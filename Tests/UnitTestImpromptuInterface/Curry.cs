using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;


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
    public class Curry : Helper
    {
        [Test]
        public void TestBasicDelegateCurry()
        {
            Func<int, int, int> tAdd = (x, y) => x + y;
            var tCurriedAdd4 = Impromptu.Curry(tAdd)(4);
            var tResult = tCurriedAdd4(6);


            Assert.AreEqual(10, tResult);

        }



        [Test]
        public void TestBasicNamedCurry()
        {
            Func<int, int, int> tSub = (x, y) => x - y;
            var tCurriedSub7 = Impromptu.Curry(tSub)(arg2: 7);
            var tResult = tCurriedSub7(arg1: 10);


            Assert.AreEqual(3, tResult);

        }

        [Test]
        public void TestBasicConvertDelegateCurry()
        {
            Func<string, string, string> tAdd = (x, y) => x + y;
            var tCurriedAdd4 = Impromptu.Curry(tAdd)("4");
            var tCastToFunc = (Func<string, string>)tCurriedAdd4;
            var tResult2 = tCastToFunc("10");

            Assert.AreEqual("410", tResult2);
        }
        [Test]
        public void TestBasicConvertDelegateCurryReturnValueType()
        {
            Func<string, string, int> tAdd = (x, y) => Int32.Parse(x) + Int32.Parse(y);
            var tCurriedAdd4 = Impromptu.Curry(tAdd)("4");
            Func<string, int> tCastToFunc = tCurriedAdd4;
            var tResult2 = tCastToFunc("10");

            Assert.AreEqual(14, tResult2);
        }

        public delegate bool TestDeclaredDelagate(string value);
        [Test]
        public void TestBasicConvertNonGenericDelegate()
        {
            Func<string, string, bool> tContains = (x, y) => y.Contains(x);
            var tCurriedContains = Impromptu.Curry(tContains)("it");
            TestDeclaredDelagate tCastToDel = tCurriedContains;
            var tResult = tCastToDel("bait");
            Assert.AreEqual(true, tResult);
        }
        public delegate void TestRunDelagate(string value);
        [Test]
        public void TestBasicConvertNonGenericDelegateAction()
        {
            var tBool = false;
            Action<string, string> tContains = (x, y) => tBool = y.Contains(x);
            var tCurriedContains = Impromptu.Curry(tContains)("it");
            TestRunDelagate tCastToDel = tCurriedContains;
            tCastToDel("bait");
            Assert.AreEqual(true, tBool);
        }

        [Test]
        public void TestBasicConvertDelegateCurryParamValueType()
        {
            Func<int, int, int> tAdd = (x, y) => x + y;
            var tCurriedAdd4 = Impromptu.Curry(tAdd)(4);
            Func<int, int> tCastToFunc = tCurriedAdd4;
            var tResult2 = tCastToFunc(10);

            Assert.AreEqual(14, tResult2);
        }

        [Test]
        public void TestBasicConvertMoreCurryParamValueType()
        {
            Func<int, int, int, int> tAdd = (x, y, z) => x + y + z;
            Func<int, Func<int, int>> Curry1 = Impromptu.Curry(tAdd)(4);
            Func<int, int> Curry2 = Curry1(6);
            int tResult = Curry2(10);

            Assert.AreEqual(20, tResult);
        }

        [Test]
        public void TestBasicConvertMoreMoreCurryParamValueType()
        {
            Func<int, int, int, int, int> tAdd = (x, y, z, bbq) => x + y + z + bbq;
            Func<int, Func<int, Func<int, Func<int, int>>>> Curry0 = Impromptu.Curry(tAdd);
            var Curry1 = Curry0(4);
            var Curry2 = Curry1(5);
            var Curry3 = Curry2(6);
            var tResult = Curry3(20);

            Assert.AreEqual(35, tResult);
        }



        [Test]
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

        [Test]
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

        [Test]
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
        [Test]
        public void TestStaticMethodCurry3()
        {
            var tFormat = Enumerable.Range(0, 100).Aggregate(new StringBuilder(), (result, each) => result.Append("{" + each + "}")).ToString();


            dynamic curriedWrite = Impromptu.Curry(Console.Out, 101).WriteLine(tFormat);

            Func<dynamic, int, dynamic> applyArgs = (result, each) => result(each.ToString());

            Enumerable.Range(0, 100).Aggregate((object)curriedWrite, applyArgs);

        }
#endif

        [Test]
        public void TestDynamicMethodCurry()
        {
            var tNewObj = Build.NewObject(Add: Return<int>.Arguments<int, int>((x, y) => x + y));

            var tCurry = Impromptu.Curry(tNewObj).Add(4);
            var tResult = tCurry(10);
            Assert.AreEqual(14, tResult);
            //Test cached invocation;
            var tResult2 = tCurry(30);
            Assert.AreEqual(34, tResult2);
        }

        [Test]
        public void UnboundedCurry()
        {
            var tNewObject = Impromptu.Curry(Build.NewObject);
            var tCurriedNewObject = tNewObject(One: 1);
            var tResult = tCurriedNewObject(Two: 2);
            Assert.AreEqual(1, tResult.One);
            Assert.AreEqual(2, tResult.Two);

        }
        [Test]
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

        [Test]
        public void BoundedCurryCont()
        {
            var tNewObject = Impromptu.Curry(Build.NewObject, 3);
            tNewObject = tNewObject(One: 1);
            tNewObject = tNewObject(Two: 2);
            var tResult = tNewObject(Three: 3);
            Assert.AreEqual(1, tResult.One);
            Assert.AreEqual(2, tResult.Two);
            Assert.AreEqual(3, tResult.Three);
        }

        [Test]
        public void TestCurryNamedMethods()
        {
            Person adam = new Person();
            dynamic jump = Impromptu.Curry(adam).Jump();

            Assert.Throws<NotImplementedException>(() => jump(cheer: "yay", height: (uint)3));
        }

        private class Person
        {
            public void Jump(uint height, string cheer)
            {
                throw new NotImplementedException();
            }
        }


        [Test]
        public void TestPococMethodPartialApply()
        {
            var tNewObj = new PocoAdder();
            var tCurry = Impromptu.Curry(tNewObj).Add(4, 6);
            var tResult = tCurry();
            Assert.AreEqual(10, tResult);
        }

        [Test]
        public void UnboundedPartialApply()
        {
            var tNewObject = Impromptu.Curry(Build.NewObject);
            tNewObject = tNewObject(One: 1, Two: 2);
            var tResult = tNewObject(Three: 3, Four: 4);
            Assert.AreEqual(1, tResult.One);
            Assert.AreEqual(2, tResult.Two);
            Assert.AreEqual(3, tResult.Three);
            Assert.AreEqual(4, tResult.Four);

        }

        [Test]
        public void BasicCurryTest()
        {
             Func<int, double, float, double> adder = (x, y, z) => x + y + z;

             var curried = Impromptu.Curry(adder);

             Assert.AreEqual(6, curried(1, 2, 3)); 

         Assert.AreEqual(6, curried(1, 2)(3));

        Assert.AreEqual(6, curried(1)(2, 3));

        Assert.AreEqual(6,curried(1)(2)(3));
        }
    }
}
