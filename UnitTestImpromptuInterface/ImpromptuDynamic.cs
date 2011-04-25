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
#elif !SELFRUNNER
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
        public void GetterVoidTest()
        {
            var tPoco = new VoidMethodPoco();

            dynamic tTest = new ImpromptuGet(tPoco);

            tTest.Action();
        }

        [Test, TestMethod]
        public void GetterArrayTest()
        {
		
			
            var tArray = new int[]{1,2,3};

            IStringIntIndexer tTest =  ImpromptuGet.Create<IStringIntIndexer>(tArray);

            Assert.AreEqual(tArray[2].ToString(), tTest[2]);
        }

        [Test, TestMethod]
        public void GetterEventTest()
        {
            var tActsLike = ImpromptuGet.Create<IEvent>(new PocoEvent());
            var tSet = false;
            tActsLike.Event += (obj, args) => tSet = true;

            tActsLike.OnEvent(null, null);
            Assert.AreEqual(true, tSet);

        }


        [Test, TestMethod]
        public void GetterEventTest2()
        {
            var tActsLike = ImpromptuGet.Create<IEvent>(new PocoEvent());
            var tSet = false;
            EventHandler<EventArgs> tActsLikeOnEvent = (obj, args) => tSet = true;
            tActsLike.Event += tActsLikeOnEvent;
            tActsLike.Event -= tActsLikeOnEvent;
            tActsLike.OnEvent(null, null);
            Assert.AreEqual(false, tSet);

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
        public void ForwardAnonTest()
        {
            var tAnon = new { Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid() };

            dynamic tTest = new TestForwarder(tAnon);

            Assert.AreEqual(tAnon.Prop1, tTest.Prop1);
            Assert.AreEqual(tAnon.Prop2, tTest.Prop2);
            Assert.AreEqual(tAnon.Prop3, tTest.Prop3);
        }

        [Test, TestMethod]
        public void ForwardVoidTest()
        {
            var tPoco = new VoidMethodPoco();

            dynamic tTest = new TestForwarder(tPoco);

            tTest.Action();
        }



        [Test, TestMethod]
        public void ForwardDynamicTest()
        {
            dynamic tNew = new ExpandoObject();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            tNew.Prop3 = Guid.NewGuid();

            dynamic tTest = new TestForwarder(tNew);


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
            tNew.Action4 = new Func<int, string>(arg => "test" + arg);


            ISimpeleClassMeth2 tActsLike = Impromptu.ActLike<ISimpeleClassMeth2>(tNew);



            AssertException<AssertionException>(tActsLike.Action1);
            AssertException<AssertionException>(() => tActsLike.Action2(true));

            Assert.AreEqual("test", tActsLike.Action3());

            Assert.AreEqual("test4", tActsLike.Action4(4));
        }

        [Test, TestMethod]
        public void ForwardMethodsTest()
        {

            dynamic tNew = new ImpromptuDictionary();
            tNew.Action1 = new Action(Assert.Fail);
            tNew.Action2 = new Action<bool>(Assert.IsFalse);
            tNew.Action3 = new Func<string>(() => "test");
            tNew.Action4 = new Func<int, string>(arg => "test" + arg);


            dynamic tFwd = new TestForwarder(tNew);



            AssertException<AssertionException>(()=> tFwd.Action1());
            AssertException<AssertionException>(() => tFwd.Action2(true));

            Assert.AreEqual("test", tFwd.Action3());

            Assert.AreEqual("test4", tFwd.Action4(4));
        }

        [Test, TestMethod]
        public void DictionaryMethodsTestWithPropertyAccess()
        {

            dynamic tNew = new ImpromptuDictionary();
            tNew.PropCat = "Cat-";
            tNew.Action1 = new Action(Assert.Fail);
            tNew.Action2 = new Action<bool>(Assert.IsFalse);
            tNew.Action3 = new ThisFunc<string>(@this => @this.PropCat + "test"); 

            ISimpeleClassMeth tActsLike = Impromptu.ActLike<ISimpeleClassMeth>(tNew);



            AssertException<AssertionException>(tActsLike.Action1);
            AssertException<AssertionException>(() => tActsLike.Action2(true));

            Assert.AreEqual("Cat-test", tActsLike.Action3());


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
		  
		[Test, TestMethod]
		public void TestBuilder(){
			var New = Builder.New<ExpandoObject>();
			
			  var tExpando =New.Object(
										Test:"test1",
										Test2:"Test 2nd"
									);
		   Assert.AreEqual("test1",tExpando.Test );
		   Assert.AreEqual("Test 2nd",tExpando.Test2);
			
		    dynamic NewD = new ImpromptuBuilder<ExpandoObject>();

			
			var tExpandoNamedTest =NewD.Robot(
				LeftArm:"Rise",
				RightArm:"Clamp"
				);
			
			 Assert.AreEqual("Rise",tExpandoNamedTest.LeftArm);
		  	 Assert.AreEqual("Clamp",tExpandoNamedTest.RightArm);
		}
				
		[Test, TestMethod]
		public void TestSetupOtherTypes(){
			var New = Builder.New().Setup(
					Expando: typeof(ExpandoObject),
					Dict: typeof(ImpromptuDictionary)
				);
			
			var tExpando =New.Expando(
				LeftArm:"Rise",
				RightArm:"Clamp"
				);
			
			var tDict =New.Dict(
				LeftArm:"RiseD",
				RightArm:"ClampD"
				);
			
			 Assert.AreEqual("Rise",tExpando.LeftArm);
		  	 Assert.AreEqual("Clamp",tExpando.RightArm);
			Assert.AreEqual(typeof(ExpandoObject),tExpando.GetType());
			
			 Assert.AreEqual("RiseD",tDict.LeftArm);
			 Assert.AreEqual("ClampD",tDict.RightArm);
			Assert.AreEqual(typeof(ImpromptuDictionary),tDict.GetType());

		}
		
		[Test, TestMethod]

        //This test data is modified from MS-PL Clay project http://clay.codeplex.com
        public void TestClayFactorySyntax()
        {
            dynamic New = Builder.New();

            {
                var person = New.Person();
                person.FirstName = "Louis";
                person.LastName = "Dejardin";
                Assert.AreEqual("Louis",person.FirstName );
                Assert.AreEqual("Dejardin", person.LastName);
            }
            {
                var person = New.Person();
                person["FirstName"] = "Louis";
                person["LastName"] = "Dejardin";
                Assert.AreEqual("Louis", person.FirstName);
                Assert.AreEqual("Dejardin", person.LastName);
            }
            {
                var person = New.Person(
                    FirstName: "Bertrand",
                    LastName: "Le Roy"
                    ).Aliases("bleroy", "boudin");

                Assert.AreEqual("Bertrand", person.FirstName);
                Assert.AreEqual("Le Roy", person.LastName);
                Assert.AreEqual("boudin", person.Aliases[1]);
            }

            {
                var person = New.Person()
                    .FirstName("Louis")
                    .LastName("Dejardin")
                    .Aliases(new[] {"Lou"});

                Assert.AreEqual(person.FirstName, "Louis");
                Assert.AreEqual(person.Aliases[0], "Lou");
            }

            {
                var person = New.Person(new
                {
                    FirstName = "Louis",
                    LastName = "Dejardin"
                });
                Assert.AreEqual(person.FirstName, "Louis");
                Assert.AreEqual(person.LastName, "Dejardin");
            }

        }
		
		[Test, TestMethod]
		public void TestBuilderActLikeAnon()
		{
		    var New = Builder.New().ActLike<IBuilder>();

		var tNest =New.Nester(new {
				NameLevel1 = "Lvl1",
				Nested =  New.Nester2(new{
					NameLevel2 = "Lvl2"
				})
			});
			
			Assert.AreEqual("Lvl1", tNest.NameLevel1);
			Assert.AreEqual("Lvl2",tNest.Nested.NameLevel2);
	    }

        [Test, TestMethod]
        public void TestBuilderActLikeNamed()
        {
            var New = Builder.New().ActLike<IBuilder>();

            var tNest = New.Nester(
                NameLevel1 :"Lvl1",
                Nested : New.Nester2(
                            NameLevel2 : "Lvl2"
                        )
            );

            Assert.AreEqual("Lvl1", tNest.NameLevel1);
            Assert.AreEqual("Lvl2", tNest.Nested.NameLevel2);
        }
		
		
			[Test, TestMethod]
        //This test data is modified from MS-PL Clay project http://clay.codeplex.com
		public void TestFactoryListSyntax(){
			dynamic New = Builder.New();
			
			//Test using Clay Syntax
			var people = New.Array(
    						 New.Person().FirstName("Louis").LastName("Dejardin"),
   							 New.Person().FirstName("Bertrand").LastName("Le Roy")
						 );
			
			Assert.AreEqual("Dejardin",people[0].LastName);
		  	Assert.AreEqual("Le Roy",people[1].LastName);
			
			var people2 =  new ImpromptuList(){
							New.Robot(Name:"Bender"),
							New.Robot(Name:"RobotDevil")
						   };	
			
			
			Assert.AreEqual("Bender", people2[0].Name);
		  	Assert.AreEqual("RobotDevil",people2[1].Name);

		}
              
        [Test, TestMethod]
            public void TestQuicListSyntax()
            {
                var tList =Build.NewList("test", "one", "two");
                Assert.AreEqual("one", tList[1]);
            }
		
    }
}
