// 
//  Copyright 2010  Ekon Benefits
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using ImpromptuInterface;
using System.Dynamic;
namespace UnitTestImpromptuInterface
{
	[TestFixture()]
	public class Test:AssertionHelper
	{
	    private IDisposable Builder;

        [TestFixtureSetUp]
        public void Setup()
        {
            Builder = BuildProxy.WriteOutDll("ImpromptuEmit");
        }
        [TestFixtureTearDown]
        public void TearDown()
        {
            Builder.Dispose();
        }

		[Test]
        public void AnonPropertyTest()
        {
            var tAnon = new {Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid()};

            var tActsLike = tAnon.ActLike<ISimpeleClassProps>();

		   
            Assert.AreEqual(tAnon.Prop1,tActsLike.Prop1);
            Assert.AreEqual(tAnon.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tAnon.Prop3, tActsLike.Prop3);
        }
		
		

        [Test]
        public void CacheTest()
        {
            var tAnon = new { Prop1 = "Test 1", Prop2 = 42L, Prop3 = Guid.NewGuid() };
            var tAnon2 = new { Prop1 = "Test 2", Prop2 = 43L, Prop3 = Guid.NewGuid() };

            var tActsLike = tAnon.ActLike<ISimpeleClassProps>();
            var tActsLike2 = tAnon2.ActLike<ISimpeleClassProps>();

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

            var tActsLike = Impromptu.ActLike<ISimpeleClassProps>(tNew);
   



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

            ISimpeleClassMeth tActsLike = Impromptu.ActLike<ISimpeleClassMeth>(tNew);

			
		
            Assert.Throws<AssertionException>(tActsLike.Action1);
            Assert.Throws<AssertionException>(() => tActsLike.Action2(true));
        
            Assert.AreEqual("test",tActsLike.Action3());
			Console.Write("test");
          
        }

        [Test]
        public void TestGenericMeth()
        {

            GenericMethHelper(3, "3");
            GenericMethHelper(4, "4");
            GenericMethHelper(true, "True");

            GenericMethHelper2(3);
            GenericMethHelper2(4);
            GenericMethHelper2(true);
            GenericMethHelper2("test'");
        }

        private void GenericMethHelper<T>(T param, string expected)
        {
            dynamic tNew = new ExpandoObject();
            tNew.Action = new Func<T,string>(it=>it.ToString());
            IGenericMeth tActsLike = Impromptu.ActLike<IGenericMeth>(tNew);

            Assert.AreEqual(expected, tActsLike.Action(param));
        }

        private void GenericMethHelper2<T>(T param)
        {
            dynamic tNew = new ExpandoObject();
            tNew.Action2 = new Func<T, T>(it => it);
            IGenericMeth tActsLike = Impromptu.ActLike<IGenericMeth>(tNew);

            Assert.AreEqual(param, tActsLike.Action2(param));
        }


        [Test]
        public void TestGenericType()
        {

            GenericHelper(3, "3");
            GenericHelper(4, "4");
            GenericHelper(true, "True");
        }

        private void GenericHelper<T>(T param, string expected)
        {
            dynamic tNew = new ExpandoObject();
            tNew.Funct = new Func<T, string>(it => it.ToString());
            IGenericType<T> tActsLike = Impromptu.ActLike<IGenericType<T>>(tNew);

            Assert.AreEqual(expected, tActsLike.Funct(param));
        }

        [Test]
        public void TestGenericTypeConstraints()
        {

            var tObj = new Object();
            GenericHelperConstraints(tObj, tObj.ToString());
        }

        private void GenericHelperConstraints<T>(T param, string expected) where T: class 
        {
            dynamic tNew = new ExpandoObject();
            tNew.Funct = new Func<T, string>(it => it.ToString());
            var tActsLike = Impromptu.ActLike<IGenericTypeConstraints<T>>(tNew);

            Assert.AreEqual(expected, tActsLike.Funct(param));
        }

      

        [Test]
        public void TestConstraintsMethGeneric()
        {
            var tObj = new Object();
            GenericMethConstraintsHelper(tObj, tObj.ToString());
            var tTest = new Uri(@"http://google.com");
            GenericMethConstraintsHelper2(tTest, tTest.ToString());
        }

        private void GenericMethConstraintsHelper<T>(T param, string expected) where T: class 
        {
            dynamic tNew = new ExpandoObject();
            tNew.Action = new Func<T, string>(it => it.ToString());
            var tActsLike = Impromptu.ActLike<IGenericMethWithConstraints>(tNew);

            Assert.AreEqual(expected, tActsLike.Action(param));
        }

        private void GenericMethConstraintsHelper2<T>(T param, string expected) where T : ISerializable
        {
            dynamic tNew = new ExpandoObject();
            tNew.Action2 = new Func<T, string>(it => it.ToString());
            var tActsLike = Impromptu.ActLike<IGenericMethWithConstraints>(tNew);

            Assert.AreEqual(expected, tActsLike.Action2(param));
        }

        [Test]
        public void StringPropertyTest()
        {
            var tAnon = "Test 123";
            var tActsLike = tAnon.ActLike<ISimpleStringProperty>();
   

            Assert.AreEqual(tAnon.Length, tActsLike.Length);
        }

        [Test]
        public void StringMethodTest()
        {
            var tAnon = "Test 123";
            var tActsLike = tAnon.ActLike<ISimpleStringMethod>();
      

            Assert.AreEqual(tAnon.StartsWith("Te"),tActsLike.StartsWith("Te"));
        }
		
	}
}

