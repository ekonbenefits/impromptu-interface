using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using QuackInterface;
namespace TestQuackInteraface
{
    public class Tests
    {
        [Test]
        public void AnonPropertyTest()
        {
            var tAnon = new {Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid()};

            var tActsLike = tAnon.ActsLike<ISimpeleClassProps>();


            Assert.AreEqual(tAnon.Prop1,tActsLike.Prop1);
            Assert.AreEqual(tAnon.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tAnon.Prop3, tActsLike.Prop3);
        }

        [Test]
        public void CacheTest()
        {
            var tAnon = new { Prop1 = "Test 1", Prop2 = 42L, Prop3 = Guid.NewGuid() };
            var tAnon2 = new { Prop1 = "Test 2", Prop2 = 43L, Prop3 = Guid.NewGuid() };

            var tActsLike = tAnon.ActsLike<ISimpeleClassProps>();
            var tActsLike2 = tAnon2.ActsLike<ISimpeleClassProps>();

           Assert.AreEqual(tActsLike.GetType(), tActsLike2.GetType());

           Assert.AreEqual(tAnon.Prop1, tActsLike.Prop1);
           Assert.AreEqual(tAnon.Prop2, tActsLike.Prop2);
           Assert.AreEqual(tAnon.Prop3, tActsLike.Prop3);

           Assert.AreEqual(tAnon2.Prop1, tActsLike2.Prop1);
           Assert.AreEqual(tAnon2.Prop2, tActsLike2.Prop2);
           Assert.AreEqual(tAnon2.Prop3, tActsLike2.Prop3);

        }


        [Test]
        public void ExpandoPropertyTest()
        {
          
            dynamic tNew = new ExpandoObject();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            tNew.Prop3 = Guid.NewGuid();

            var tActsLike = Dynamic.ActsLike<ISimpeleClassProps>(tNew);
   



            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1);
            Assert.AreEqual(tNew.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tNew.Prop3, tActsLike.Prop3);
        }


        [Test]
        public void ExpandoMethodsTest()
        {

            dynamic tNew = new ExpandoObject();
            tNew.Action1 = new Action(Assert.Fail);
            tNew.Action2 = new Action<bool>(Assert.IsFalse);
            tNew.Action3 = new Func<string>(()=> "test");

            ISimpeleClassMeth tActsLike = Dynamic.ActsLike<ISimpeleClassMeth>(tNew);

            Assert.Throws<AssertionException>(tActsLike.Action1);
            Assert.Throws<AssertionException>(() => tActsLike.Action2(true));
           Assert.AreEqual("test",tNew.Action3());
          
        }

        [Test]
        public void TestGeneric()
        {

            GenericHelper(3,"3");
            GenericHelper(4,"4");
            GenericHelper(true,"True");
        }

        public void GenericHelper<T>(T param, string expected)
        {
            dynamic tNew = new ExpandoObject();
            tNew.Action = new Func<T,string>(it=>it.ToString());
            IGenericMeth tActsLike = Dynamic.ActsLike<IGenericMeth>(tNew);

            Assert.AreEqual(expected, tActsLike.Action(param));
        }


        [Test]
        public void StringPropertyTest()
        {
            var tAnon = "Test 123";
            var tActsLike = tAnon.ActsLike<ISimpleStringProperty>();
   

            Assert.AreEqual(tAnon.Length, tActsLike.Length);
        }

        [Test]
        public void StringMethodTest()
        {
            var tAnon = "Test 123";
            var tActsLike = tAnon.ActsLike<ISimpleStringMethod>();
      

            Assert.AreEqual(tAnon.StartsWith("Te"),tActsLike.StartsWith("Te"));
        }


    }
}
