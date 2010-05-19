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



            var tActsLike = Dynamic.ActsLike<SimpeleAnnoymousClassProps>(tAnon);
        

            dynamic tdynAnon = tAnon;
            string tTest = tdynAnon.Prop1;

            Assert.AreEqual(tAnon.Prop1,tActsLike.Prop1);
            Assert.AreEqual(tAnon.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tAnon.Prop3, tActsLike.Prop3);
        }

        [Test]
        public void ExpandoPropertyTest()
        {
          
            dynamic tNew = new ExpandoObject();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            tNew.Prop3 = Guid.NewGuid();

            var tActsLike = Dynamic.ActsLike<SimpeleAnnoymousClassProps>(tNew);
   



            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1);
            Assert.AreEqual(tNew.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tNew.Prop3, tActsLike.Prop3);
        }


        [Test]
        public void SimplePropertyTest()
        {
            var tAnon = "Test 123";
            var tActsLike = tAnon.ActsLike<SimpleStringProperty>();
            BuildProxy.SaveDynamicDll();

            Assert.AreEqual(tAnon.Length, tActsLike.Length);
        }

        [Test]
        public void SimpleMethodTest()
        {
            var tAnon = "Test 123";
            var tActsLike = tAnon.ActsLike<SimpleStringMethod>();
            BuildProxy.SaveDynamicDll();

            Assert.AreEqual(tAnon.StartsWith("Te"),tActsLike.StartsWith("Te"));
        }


    }
}
