using NUnit.Framework;
using System;
using System.Reflection.Emit;
using ImpromptuInterface.Build;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using Dynamitey.DynamicObjects;
using ImpromptuInterface;

namespace UnitTestImpromptuInterface

{

#if NET40_OR_GREATER
    [TestFixture]
    public class Saveable : Helper
    {

        public static SaveableAssemblyMaker SaveableMaker;

        [OneTimeSetUp]
        public void Setup()
        {
            SaveableMaker = BuildProxy.SaveableProxyMaker("II_Generated");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            SaveableMaker.Save();
        }

        [Test]
        public void AnonPropertyTest()
        {
            var tAnon = new { Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid() };

            var tActsLike = SaveableMaker.ActLike<ISimpeleClassProps>(tAnon);


            Assert.AreEqual(tAnon.Prop1, tActsLike.Prop1);
            Assert.AreEqual(tAnon.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tAnon.Prop3, tActsLike.Prop3);
        }

        [Test]
        public void InformalPropTest()
        {
            dynamic tNew = new ExpandoObject();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            var tActsLike = SaveableMaker.ActLikeProperties(tNew, new Dictionary<string, Type>() { { "Prop1", typeof(string) } });


            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1);
            AssertException<RuntimeBinderException>(() => { var tTest = tActsLike.Prop2; });
        }

        [Test]
        public void NonNestedInterfaceTest()
        {
            dynamic tNew = new ExpandoObject();
            dynamic tNew2 = new ExpandoObject();
            tNew.NameLevel1 = "one";
            tNew.Nested = new ExpandoObject();
            tNew.Nested2 = new Func<object>(() => tNew2);

            INonNest tActsLike = SaveableMaker.ActLike(tNew);

            Assert.AreEqual(tNew.NameLevel1, tActsLike.NameLevel1);
            Assert.Throws<RuntimeBinderException>(() => { var tval = tActsLike.Nested; });
            Assert.Throws<RuntimeBinderException>(() =>
            {
                var tval = tActsLike.Nested2();
                ;
            });
        }


        [Test]
        public void InterfaceDictionaryWrappedTest()
        {

            var tDictionary = new Dictionary<string, object>
            {
                {"Test1", 1},
                {"Test2", 2L},
                {"Test3",1},
                {"Test4","Two"},
                {"TestD",  new Dictionary<string,object>()
                    {
                        {"TestA","A"},
                        {"TestB","B"}
                    }
                }
            };

            dynamic tDynamic = SaveableMaker.Create<Dictionary, IDynamicDict>(tDictionary);
            dynamic tNotDynamic = SaveableMaker.Create<Dictionary, INonDynamicDict>(tDictionary);

            Assert.AreEqual(tDynamic, tNotDynamic);

            Assert.AreEqual(1, tDynamic.Test1);
            Assert.AreEqual(2L, tDynamic.Test2);
            Assert.AreEqual(TestEnum.One, tDynamic.Test3);
            Assert.AreEqual(TestEnum.Two, tDynamic.Test4);

            Assert.AreEqual("A", tDynamic.TestD.TestA);
            Assert.AreEqual("B", tDynamic.TestD.TestB);

            Assert.AreEqual(1, tNotDynamic.Test1);
            Assert.AreEqual(2L, tNotDynamic.Test2);
            Assert.AreEqual(TestEnum.One, tNotDynamic.Test3);
            Assert.AreEqual(TestEnum.Two, tNotDynamic.Test4);

            Assert.AreEqual(typeof(Dictionary<string, object>), tNotDynamic.TestD.GetType());
            Assert.AreEqual(typeof(Dictionary), tDynamic.TestD.GetType());
        }


        [Test]
        public void NestedInterfacetest()
        {
            dynamic tNew = new ExpandoObject();
            tNew.NameLevel1 = "one";
            tNew.Nested = new ExpandoObject();
            tNew.Nested.NameLevel2 = "two";

            INest tActsLike = SaveableMaker.ActLike<INest>(tNew);

            Assert.AreEqual(tNew.NameLevel1, tActsLike.NameLevel1);
            Assert.AreEqual(tNew.Nested.NameLevel2, tActsLike.Nested.NameLevel2);

        }



        [Test]
        public void PropertyAttributeTest()
        {
            dynamic tNew = new ExpandoObject();
            ISimpeleSetClassProps tActsLike = SaveableMaker.ActLike<ISimpeleSetClassProps>(tNew);
            var attribute = (DisplayNameAttribute)tActsLike.GetType().GetProperty(nameof(ISimpeleSetClassProps.Prop1)).GetCustomAttribute(typeof(DisplayNameAttribute), true);
            Assert.NotNull(attribute);
            Assert.AreEqual("testDisplayName", attribute.DisplayName);
        }
    }



#endif

}