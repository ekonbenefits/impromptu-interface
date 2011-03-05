using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;

#if SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#elif !MONO
using NUnit.Framework;
#endif

namespace UnitTestImpromptuInterface
{
    [TestFixture,TestClass]
    public class ImpromptuDynamic : Helper
    {

        [Test, TestMethod]
        public void DictionaryPropertyTest()
        {

            dynamic tNew = new ImpromptuDictionary();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            tNew.Prop3 = Guid.NewGuid();

            var tActsLike = Impromptu.ActLike<ISimpeleClassProps>(tNew);

            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1);
            Assert.AreEqual(tNew.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tNew.Prop3, tActsLike.Prop3);
        }

        [Test, TestMethod]
        public void DictionaryNullPropertyTest()
        {

            dynamic tNew = new ImpromptuDictionary();


            ISimpeleClassProps tActsLike = Impromptu.ActLike<ISimpeleClassProps>(tNew);

            Assert.AreEqual(default(string), tActsLike.Prop1);
            Assert.AreEqual(default(long), tActsLike.Prop2);
            Assert.AreEqual(default(Guid), tActsLike.Prop3);
        }

        [Test, TestMethod]
        public void GetterAnonTest()
        {
            var tAnon = new { Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid() };

            dynamic tTest =new ImpromptuGet(tAnon);

            Assert.AreEqual(tAnon.Prop1, tTest.Prop1);
            Assert.AreEqual(tAnon.Prop2, tTest.Prop2);
            Assert.AreEqual(tAnon.Prop3, tTest.Prop3);
        }


        [Test, TestMethod]
        public void GetterDynamicTest()
        {
            dynamic tNew = new ExpandoObject();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            tNew.Prop3 = Guid.NewGuid();

            dynamic tTest = new ImpromptuGet(tNew);


            Assert.AreEqual(tNew.Prop1, tTest.Prop1);
            Assert.AreEqual(tNew.Prop2, tTest.Prop2);
            Assert.AreEqual(tNew.Prop3, tTest.Prop3);
        }

        [Test, TestMethod]
        public void DictionaryMethodsTest()
        {

            dynamic tNew = new ImpromptuDictionary();
            tNew.Action1 = new Action(Assert.Fail);
            tNew.Action2 = new Action<bool>(Assert.IsFalse);
            tNew.Action3 = new Func<string>(() => "test");

            ISimpeleClassMeth tActsLike = Impromptu.ActLike<ISimpeleClassMeth>(tNew);



            AssertException<AssertionException>(tActsLike.Action1);
            AssertException<AssertionException>(() => tActsLike.Action2(true));

            Assert.AreEqual("test", tActsLike.Action3());
      

        }

        [Test, TestMethod]
        public void DictionaryNullMethodsTest()
        {

            dynamic tNew = new ImpromptuDictionary();

            ISimpleStringMethod tActsLike = Impromptu.ActLike<ISimpleStringMethod>(tNew);

            Assert.AreEqual(false, tActsLike.StartsWith("Te"));

 

        }


        [Test, TestMethod]
        public void DynamicDictionaryWrappedTest()
        {

            var tDictionary = new Dictionary<string, object>
                                  {
                                      {"Test1", 1},
                                      {"Test2", 2},
                                      {"TestD",  new Dictionary<string,object>()
                                                     {
                                                         {"TestA","A"},
                                                         {"TestB","B"}
                                                     }
                                      }
                                  };

            dynamic tNew = new ImpromptuDictionary(tDictionary);

            Assert.AreEqual(1, tNew.Test1);
            Assert.AreEqual(2, tNew.Test2);
            Assert.AreEqual("A", tNew.TestD.TestA);
            Assert.AreEqual("B", tNew.TestD.TestB);
        }

        [Test, TestMethod]
        public void InterfaceDictionaryWrappedTest()
        {

            var tDictionary = new Dictionary<string, object>
                                  {
                                      {"Test1", 1},
                                      {"Test2", 2L},
                                      {"TestD",  new Dictionary<string,object>()
                                                     {
                                                         {"TestA","A"},
                                                         {"TestB","B"}
                                                     }
                                      }
                                  };

            dynamic tDynamic = ImpromptuDictionary.Create<IDynamicDict>(tDictionary);
            dynamic tNotDynamic = ImpromptuDictionary.Create<INonDynamicDict>(tDictionary);

            Assert.AreEqual(tDynamic, tNotDynamic);

            Assert.AreEqual(1, tDynamic.Test1);
            Assert.AreEqual(2L, tDynamic.Test2);
            Assert.AreEqual("A", tDynamic.TestD.TestA);
            Assert.AreEqual("B", tDynamic.TestD.TestB);

            Assert.AreEqual(1, tNotDynamic.Test1);
            Assert.AreEqual(2L, tNotDynamic.Test2);


            Assert.AreEqual(typeof(Dictionary<string, object>), tNotDynamic.TestD.GetType());
            Assert.AreEqual(typeof(ImpromptuDictionary), tDynamic.TestD.GetType());
        }

        [Test, TestMethod]
        public void DynamicObjectEqualsTest()
        {
            var tDictionary = new Dictionary<string, object>
                                  {
                                      {"Test1", 1},
                                      {"Test2", 2},
                                      {"TestD",  new Dictionary<string,object>()
                                                     {
                                                         {"TestA","A"},
                                                         {"TestB","B"}
                                                     }
                                      }
                                  };

            dynamic tDynamic = ImpromptuDictionary.Create<IDynamicDict>(tDictionary);
            dynamic tNotDynamic = ImpromptuDictionary.Create<INonDynamicDict>(tDictionary);

            Assert.AreEqual(tDynamic, tNotDynamic);

            Assert.AreEqual(tDynamic, tDictionary);

            Assert.AreEqual(tNotDynamic, tDictionary);
        }

        [TestMethod,Test]
        public void DynamicAnnonymousWrapper()
        {
            var tData = new Dictionary<int, string> {{1, "test"}};
            var tDyn = ImpromptuGet.Create(new
                                               {
                                                   Test1 = 1,
                                                   Test2 = "2",
                                                   IsGreaterThan5 = Return<bool>.Arguments<int>(it => it > 5),
                                                   ClearData = ReturnVoid.Arguments(() => tData.Clear())
                                               });

            Assert.AreEqual(1,tDyn.Test1);
            Assert.AreEqual("2", tDyn.Test2);
            Assert.AreEqual(true, tDyn.IsGreaterThan5(6));
            Assert.AreEqual(false, tDyn.IsGreaterThan5(4));

            Assert.AreEqual(1, tData.Count);
            tDyn.ClearData();
            Assert.AreEqual(0, tData.Count);

        }   

        [Test, TestMethod]
        public void TestAnonInterface()
        {
            var tInterface = ImpromptuGet.Create<ICollection>(new
                                                                  {
                                                                     CopyArray = ReturnVoid.Arguments<Array,int>((ar,i) => Enumerable.Range(1,10)),
                                                                     Count =  10,
                                                                     IsSynchronized = false,
                                                                     SyncRoot = this,
                                                                     GetEnumerator = Return<IEnumerator>.Arguments(()=>Enumerable.Range(1, 10).GetEnumerator())
                                                                  });

            Assert.AreEqual(10, tInterface.Count);
            Assert.AreEqual(false, tInterface.IsSynchronized);
            Assert.AreEqual(this, tInterface.SyncRoot);
            Assert.AreEqual(true,tInterface.GetEnumerator().MoveNext());
        }
    }
}
