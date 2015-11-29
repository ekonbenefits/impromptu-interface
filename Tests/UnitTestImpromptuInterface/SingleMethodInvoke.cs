// 
//  Copyright 2011  Ekon Benefits
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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;
using BinderFlags = Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags;
using Info = Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo;
using InfoFlags = Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags;
using ImpromptuInterface.InvokeExt;
using ImpromptuInterface.Optimization;
using Moq;

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
    public class SingleMethodInvoke : Helper
    {
        [Test]
        public void TestDynamicSet()
        {
            dynamic tExpando = new ExpandoObject();

            var tSetValue = "1";

            Impromptu.InvokeSet(tExpando, "Test", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test);

        }

 

		     [Test]
        public void TestPocoSet()
        {
            var tPoco = new PropPoco();

            var tSetValue = "1";

            Impromptu.InvokeSet(tPoco, "Prop1", tSetValue);

            Assert.AreEqual(tSetValue, tPoco.Prop1);

        }


             [Test]
             public void TestStructSet()
             {
                 object tPoco = new PropStruct();

                 var tSetValue = "1";

                 Impromptu.InvokeSet(tPoco, "Prop1", tSetValue);

                 Assert.AreEqual(tSetValue, ((PropStruct)tPoco).Prop1);

             }

             [Test]
             public void TestCacheableDynamicSetAndPocoSetAndSetNull()
             {
                 dynamic tExpando = new ExpandoObject();
                 var tSetValueD = "4";


                 var tCachedInvoke = new CacheableInvocation(InvocationKind.Set, "Prop1");

                 tCachedInvoke.Invoke((object)tExpando, tSetValueD);
             

                 Assert.AreEqual(tSetValueD, tExpando.Prop1);

                 var tPoco = new PropPoco();
                 var tSetValue = "1";

                 tCachedInvoke.Invoke(tPoco, tSetValue);

                 Assert.AreEqual(tSetValue, tPoco.Prop1);
                 
                 String tSetValue2 = null;

                 tCachedInvoke.Invoke(tPoco, tSetValue2);

                 Assert.AreEqual(tSetValue2, tPoco.Prop1);
             }

      
        
        [Test]
        public void TestConvert()
        {
            var tEl = new XElement("Test","45");

            var tCast = Impromptu.InvokeConvert(tEl, typeof (int), @explicit:true);
           
            Assert.AreEqual(typeof(int), tCast.GetType());
            Assert.AreEqual(45,tCast);
        }

        [Test]
        public void TestConvertCacheable()
        {
            var tEl = new XElement("Test", "45");

            var tCacheInvoke = new CacheableInvocation(InvocationKind.Convert, convertType: typeof (int),
                                                       convertExplicit: true);
            var tCast = tCacheInvoke.Invoke(tEl);

            Assert.AreEqual(typeof(int), tCast.GetType());
            Assert.AreEqual(45, tCast);
        }

        [Test]
        public void TestConstruct()
        {
            var tCast = Impromptu.InvokeConstructor(typeof (List<object>), new object[]
                                                                              {
                                                                                  new string[] {"one", "two", "three"}
                                                                              });
        
            Assert.AreEqual("two", tCast[1]);
        }


        [Test]
        public void TestCacheableConstruct()
        {
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor, argCount: 1);

            dynamic tCast = tCachedInvoke.Invoke(typeof(List<object>), new object[]
                                                                              {
                                                                                  new string[] {"one", "two", "three"}
                                                                              });

            Assert.AreEqual("two", tCast[1]);
        }


        [Test]
        public void TestConstructOptional()
        {
            PocoOptConstructor tCast = Impromptu.InvokeConstructor(typeof(PocoOptConstructor), "3".WithArgumentName("three"));

            Assert.AreEqual("-1", tCast.One);
            Assert.AreEqual("-2", tCast.Two);
            Assert.AreEqual("3", tCast.Three);
        }

        [Test]
        public void TestCacheableConstructOptional()
        {
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor, argCount: 1, argNames:new[]{"three"});

            var tCast = (PocoOptConstructor)tCachedInvoke.Invoke(typeof(PocoOptConstructor), "3");

            Assert.AreEqual("-1", tCast.One);
            Assert.AreEqual("-2", tCast.Two);
            Assert.AreEqual("3", tCast.Three);
        }

        [Test]
        public void TestOptionalArgumentActivationNoneAndCacheable()
            {
                AssertException<MissingMethodException>(() => Activator.CreateInstance<ImpromptuList>());

               var tList= Impromptu.InvokeConstructor(typeof (ImpromptuList));


               Assert.AreEqual(typeof(ImpromptuList),tList.GetType());

               var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor);

               var tList1 = tCachedInvoke.Invoke(typeof(ImpromptuList));


               Assert.AreEqual(typeof(ImpromptuList), tList1.GetType());
          }

   

        [Test]
        public void TestConstructValueType()
        {
            var tCast = Impromptu.InvokeConstructor(typeof(DateTime), 2009,1,20);

            Assert.AreEqual(20, tCast.Day);

        }

        [Test]
        public void TestCacheableConstructValueType()
        {
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor, argCount: 3);
            dynamic tCast = tCachedInvoke.Invoke(typeof(DateTime), 2009, 1, 20);

            Assert.AreEqual(20, tCast.Day);

        }
		
		     [Test]
        public void TestConstructValueTypeJustDynamic()
        {
			dynamic day =20;
			dynamic year =2009;
			dynamic month =1;
            var tCast = new DateTime(year,month,day);
			DateTime tDate = tCast;
            Assert.AreEqual(20, tDate.Day);
        }

        [Test]
        public void TestConstructprimativetype()
        {
            var tCast = Impromptu.InvokeConstructor(typeof(Int32));

            Assert.AreEqual(default(Int32), tCast);
        }


        [Test]
        public void TestConstructDateTimeNoParams()
        {
            var tCast = Impromptu.InvokeConstructor(typeof(DateTime));

            Assert.AreEqual(default(DateTime), tCast);
        }

        [Test]
        public void TestConstructOBjectNoParams()
        {
            var tCast = Impromptu.InvokeConstructor(typeof(object));

            Assert.AreEqual(typeof(object), tCast.GetType());
        }

        [Test]
        public void TestConstructNullableprimativetype()
        {
            var tCast = Impromptu.InvokeConstructor(typeof(Nullable<Int32>));

            Assert.AreEqual(null, tCast);
        }

        [Test]
        public void TestConstructGuid()
        {
            var tCast = Impromptu.InvokeConstructor(typeof(Guid));

            Assert.AreEqual(default(Guid), tCast);
        }
          
        [Test]
        public void TestCacheablePrimativeDateTimeObjectNullableAndGuidNoParams()
        {
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor);

            dynamic tCast = tCachedInvoke.Invoke(typeof(Int32));

            Assert.AreEqual(default(Int32), tCast);

            tCast = tCachedInvoke.Invoke(typeof(DateTime));

            Assert.AreEqual(default(DateTime), tCast);

            tCast = tCachedInvoke.Invoke(typeof(List<string>));

            Assert.AreEqual(typeof(List<string>), tCast.GetType());

            tCast = tCachedInvoke.Invoke(typeof(object));

            Assert.AreEqual(typeof(object), tCast.GetType());

            tCast = tCachedInvoke.Invoke(typeof(Nullable<Int32>));

            Assert.AreEqual(null, tCast);

            tCast = tCachedInvoke.Invoke(typeof(Guid));

            Assert.AreEqual(default(Guid), tCast);
        }


        [Test]
        public void TestStaticCall()
        {
            
            var tOut = Impromptu.InvokeMember(typeof (StaticType).WithStaticContext(),
                                              "Create".WithGenericArgs(typeof(bool)), 1);
            Assert.AreEqual(false,tOut);
        }

        [Test]
        public void TestCacheableStaticCall()
        {

            var tCached = new CacheableInvocation(InvocationKind.InvokeMember, "Create".WithGenericArgs(typeof (bool)), argCount: 1,
                                    context: typeof (StaticType).WithStaticContext());

            var tOut = tCached.Invoke(typeof(StaticType), 1);
            Assert.AreEqual(false, tOut);
        }

        [Test]
        public void TestImplicitConvert()
        {
            var tEl = 45;

            var tCast = Impromptu.InvokeConvert(tEl, typeof(long));

            Assert.AreEqual(typeof(long), tCast.GetType());
        }


        [Test]
        public void TestCoerceConverterDBNULL()
        {
            var tEl = DBNull.Value;

            var tCast = Impromptu.CoerceConvert(tEl, typeof(long));

            Assert.AreEqual(typeof(long), tCast.GetType());

            var tCast2 = Impromptu.CoerceConvert(tEl, typeof(string));
            Assert.AreEqual(null, tCast2);

            Assert.AreNotEqual(null, tEl);
        }   

 

        [Test]
        public void TestCacheableImplicitConvert()
        {
            var tEl = 45;

            var tCachedInvoke = CacheableInvocation.CreateConvert(typeof (long));

            var tCast = tCachedInvoke.Invoke(tEl);

            Assert.AreEqual(typeof(long), tCast.GetType());
        }


        [Test]
        public void TestCacheableGet()
        {
            var tCached =new CacheableInvocation(InvocationKind.Get, "Prop1");

            var tSetValue = "1";
            var tAnon = new PropPoco{ Prop1 = tSetValue };

            var tOut = tCached.Invoke(tAnon);
            Assert.AreEqual(tSetValue, tOut);

            var tSetValue2 = "2";
            tAnon = new PropPoco { Prop1 = tSetValue2 };


            var tOut2 = tCached.Invoke(tAnon);


            Assert.AreEqual(tSetValue2, tOut2);

        }

        [Test]
        public void TestGetIndexer()
        {
       
            dynamic tSetValue = "1";
            var tAnon = new [] { tSetValue, "2" };


            string tOut = Impromptu.InvokeGetIndex(tAnon,0);

            Assert.AreEqual(tSetValue, tOut);

        }

 
        [Test]
        public void TestGetIndexerValue()
        {

            
            var tAnon = new int[] { 1, 2};


            int tOut = Impromptu.InvokeGetIndex(tAnon, 1);

            Assert.AreEqual(tAnon[1], tOut);

        }


        [Test]
        public void TestGetLengthArray()
        {
            var tAnon = new []  { "1", "2" };


            int tOut = Impromptu.InvokeGet(tAnon, "Length");

            Assert.AreEqual(2, tOut);

        }

        [Test]
        public void TestGetIndexerArray()
        {
            dynamic tSetValue = "1";
            var tAnon = new List<string> { tSetValue, "2" };
     

            string tOut = Impromptu.InvokeGetIndex(tAnon, 0);

            Assert.AreEqual(tSetValue, tOut);

        }


        [Test]
        public void TestCacheableIndexer()
        {

            var tStrings = new[] { "1", "2" };

            var tCachedInvoke = new CacheableInvocation(InvocationKind.GetIndex, argCount: 1);

            var tOut = (string)tCachedInvoke.Invoke(tStrings, 0);

            Assert.AreEqual(tStrings[0], tOut);

            var tOut2 = (string)tCachedInvoke.Invoke(tStrings, 1);

            Assert.AreEqual(tStrings[1], tOut2);

            var tInts = new int[] { 3, 4 };

            var tOut3 = (int)tCachedInvoke.Invoke(tInts, 0);

            Assert.AreEqual(tInts[0], tOut3);

            var tOut4 = (int)tCachedInvoke.Invoke(tInts, 1);

            Assert.AreEqual(tInts[1], tOut4);

            var tList = new List<string> { "5", "6" };

            var tOut5 = (string)tCachedInvoke.Invoke(tList, 0);

            Assert.AreEqual(tList[0], tOut5);

            var tOut6 = (string)tCachedInvoke.Invoke(tList, 0);

            Assert.AreEqual(tList[0], tOut6);
        }

        [Test]
        public void TestSetIndexer()
        {
          
            dynamic tSetValue = "3";
            var tAnon =  new List<string> { "1", "2" };

            Impromptu.InvokeSetIndex(tAnon, 0, tSetValue);

            Assert.AreEqual(tSetValue, tAnon[0]);

        }

        [Test]
        public void TestCacheableSetIndexer()
        {

            dynamic tSetValue = "3";
            var tList = new List<string> { "1", "2" };


            var tCachedInvoke = new CacheableInvocation(InvocationKind.SetIndex, argCount:2);

            tCachedInvoke.Invoke(tList, 0, tSetValue);

            Assert.AreEqual(tSetValue, tList[0]);

        }



        [Test]
        public void TestMethodDynamicPassAndGetValue()
        {
            dynamic tExpando = new ExpandoObject();
            tExpando.Func = new Func<int, string>(it => it.ToString());

            var tValue = 1;

            var tOut = Impromptu.InvokeMember(tExpando, "Func", tValue);

            Assert.AreEqual(tValue.ToString(), tOut);
        }


        [Test]
        public void TestCacheableMethodDynamicPassAndGetValue()
        {
            dynamic tExpando = new ExpandoObject();
            tExpando.Func = new Func<int, string>(it => it.ToString());

            var tValue = 1;

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Func", 1);

            var tOut = tCachedInvoke.Invoke((object) tExpando, tValue);

            Assert.AreEqual(tValue.ToString(), tOut);
        }


        [Test]
        public void TestMethodStaticOverloadingPassAndGetValue()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var tOut = Impromptu.InvokeMember(tPoco, "Func", tValue);

            Assert.AreEqual("int", tOut);

            Assert.AreEqual("int", (object)tOut); //should still be int because this uses runtime type


            var tOut2 = Impromptu.InvokeMember(tPoco, "Func", 1m);

            Assert.AreEqual("object", tOut2);

            var tOut3 = Impromptu.InvokeMember(tPoco, "Func", new{Anon =1});

            Assert.AreEqual("object", tOut3);
        }

        [Test]
        public void TestCachedMethodStaticOverloadingPassAndGetValue()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Func", argCount: 1);


            var tOut = tCachedInvoke.Invoke(tPoco, tValue);

            Assert.AreEqual("int", tOut);

            Assert.AreEqual("int", (object)tOut); //should still be int because this uses runtime type


            var tOut2 = tCachedInvoke.Invoke(tPoco, 1m);

            Assert.AreEqual("object", tOut2);

            var tOut3 = tCachedInvoke.Invoke(tPoco, new { Anon = 1 });

            Assert.AreEqual("object", tOut3);
        }

        [Test]
        public void TestMethodPocoOverloadingPassAndGetValueArg()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var tOut = Impromptu.InvokeMember(tPoco, "Func", new InvokeArg("arg", tValue));

            Assert.AreEqual("int", tOut);

            Assert.AreEqual("int", (object)tOut); //should still be int because this uses runtime type


            var tOut2 = Impromptu.InvokeMember(tPoco, "Func", 1m);

            Assert.AreEqual("object", tOut2);

            var tOut3 = Impromptu.InvokeMember(tPoco, "Func", new { Anon = 1 });

            Assert.AreEqual("object", tOut3);
        }

        [Test]
        public void TestMethodPocoOverloadingPassAndGetValueArgOptional()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var arg = InvokeArg.Create;

            var tOut = Impromptu.InvokeMember(tPoco, "Func",  arg("two", tValue));

            Assert.AreEqual("object named", tOut);

            Assert.AreEqual("object named", (object)tOut); 
        }

        [Test]
        public void TestCacheableMethodPocoOverloadingPassAndGetValueArgOptional()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var tCachedIvnocation = new CacheableInvocation(InvocationKind.InvokeMember, "Func", argCount: 1,
                                                            argNames: new[] {"two"});

            var tOut = tCachedIvnocation.Invoke(tPoco, tValue);

            Assert.AreEqual("object named", tOut);

            Assert.AreEqual("object named", (object)tOut);
        }


        [Test]
        public void TestMethodPocoOverloadingPass2AndGetValueArgOptional()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var arg = InvokeArg.Create;

            var tOut = Impromptu.InvokeMember(tPoco, "Func", arg("two", tValue), arg("one", tValue));

            Assert.AreEqual("object named", tOut);

            Assert.AreEqual("object named", (object)tOut);
        }

         [Test]
        public void TestMethodPocoOverloadingPassAndGetValueNull()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var tOut = Impromptu.InvokeMember(tPoco, "Func", tValue);

            Assert.AreEqual("int", tOut);

            Assert.AreEqual("int", (object)tOut); //should still be int because this uses runtime type


            var tOut2 = Impromptu.InvokeMember(tPoco, "Func", 1m);

            Assert.AreEqual("object", tOut2);

            var tOut3 = Impromptu.InvokeMember(tPoco, "Func", null);

            Assert.AreEqual("object", tOut3);

            var tOut4 = Impromptu.InvokeMember(tPoco, "Func", null, null, "test", null, null, null);

            Assert.AreEqual("object 6", tOut4);

            var tOut5 = Impromptu.InvokeMember(tPoco, "Func", null, null, null, null, null, null);

            Assert.AreEqual("object 6", tOut5);
        }

        /// <summary>
        /// To dynamically invoke a method with out or ref parameters you need to know the signature
        /// </summary>
        [Test]
        public void TestOutMethod()
        {



            string tResult = String.Empty;

            var tPoco = new MethOutPoco();


            var tName = "Func";
            var tContext = GetType();
            var tBinder =
                Binder.InvokeMember(BinderFlags.None, tName, null, tContext,
                                            new[]
                                                {
                                                    Info.Create(
                                                        InfoFlags.None, null),
                                                    Info.Create(
                                                        InfoFlags.IsOut |
                                                        InfoFlags.UseCompileTimeType, null)
                                                });


            var tSite = Impromptu.CreateCallSite<DynamicTryString>(tBinder, tName, tContext);

          
            tSite.Target.Invoke(tSite, tPoco, out tResult);

            Assert.AreEqual("success", tResult);

        }


        [Test]
        public void TestMethodDynamicPassVoid()
        {
            var tTest = "Wrong";

            var tValue = "Correct";

            dynamic tExpando = new ExpandoObject();
            tExpando.Action = new Action<string>(it => tTest = it);



            Impromptu.InvokeMemberAction(tExpando, "Action", tValue);

            Assert.AreEqual(tValue, tTest);
        }

        [Test]
        public void TestCacheableMethodDynamicPassVoid()
        {
            var tTest = "Wrong";

            var tValue = "Correct";

            dynamic tExpando = new ExpandoObject();
            tExpando.Action = new Action<string>(it => tTest = it);

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMemberAction, "Action", argCount: 1);

            tCachedInvoke.Invoke((object) tExpando, tValue);

            Assert.AreEqual(tValue, tTest);
        }

        [Test]
        public void TestCacheableMethodDynamicUnknowns()
        {
            var tTest = "Wrong";

            var tValue = "Correct";

            dynamic tExpando = new ExpandoObject();
            tExpando.Action = new Action<string>(it => tTest = it);
            tExpando.Func = new Func<string,string>(it => it);

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMemberUnknown, "Action", argCount: 1);

            tCachedInvoke.Invoke((object)tExpando, tValue);

            Assert.AreEqual(tValue, tTest);

            var tCachedInvoke2 = new CacheableInvocation(InvocationKind.InvokeMemberUnknown, "Func", argCount: 1);

            var Test2 =tCachedInvoke2.Invoke((object)tExpando, tValue);

            Assert.AreEqual(tValue, Test2);
        }



        [Test]
        public void TestMethodPocoGetValue()
        {
        

            var tValue = 1;

            var tOut = Impromptu.InvokeMember(tValue, "ToString");

            Assert.AreEqual(tValue.ToString(), tOut);
        }

  

        [Test]
        public void TestMethodPocoPassAndGetValue()
        {


            HelpTestPocoPassAndGetValue("Test", "Te");


            HelpTestPocoPassAndGetValue("Test", "st");
        }

        private void HelpTestPocoPassAndGetValue(string tValue, string tParam)
        {
            var tExpected = tValue.StartsWith(tParam);

            var tOut = Impromptu.InvokeMember(tValue, "StartsWith", tParam);

            Assert.AreEqual(tExpected, tOut);
        }


        [Test]
        public void TestGetDynamic()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = tSetValue;



            var tOut = Impromptu.InvokeGet(tExpando, "Test");

            Assert.AreEqual(tSetValue, tOut);
        }

        [Test]
        public void TestGetDynamicChained()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = new ExpandoObject();
            tExpando.Test.Test2 = new ExpandoObject();
            tExpando.Test.Test2.Test3 = tSetValue;


            var tOut = Impromptu.InvokeGetChain(tExpando, "Test.Test2.Test3");

            Assert.AreEqual(tSetValue, tOut);
        }

        [Test]
        public void TestGetDynamicChainedWithIndexes()
        {

            var tSetValue = "1";
            dynamic tExpando = Build.NewObject(
                Test: Build.NewObject(
                        Test2: Build.NewList(
                        Build.NewObject(Test3: Build.NewObject(Test4: tSetValue))
                        )
                    )
                );
  


            var tOut = Impromptu.InvokeGetChain(tExpando, "Test.Test2[0].Test3['Test4']");

            Assert.AreEqual(tSetValue, tOut);
        }


        [Test]
        public void TestSetDynamicChained()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = new ExpandoObject();
            tExpando.Test.Test2 = new ExpandoObject();


           Impromptu.InvokeSetChain(tExpando, "Test.Test2.Test3", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test.Test2.Test3);
        }

        [Test]
        public void TestSetDynamicChainedWithInexes()
        {
            var tSetValue = "1";
            dynamic tExpando = Build.NewObject(
                Test: Build.NewObject(
                        Test2: Build.NewList(
                        Build.NewObject(Test3: Build.NewObject())
                        )
                    )
                );


            var tOut = Impromptu.InvokeSetChain(tExpando, "Test.Test2[0].Test3['Test4']", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test.Test2[0].Test3["Test4"]);

            Assert.AreEqual(tSetValue, tOut);
        }

        [Test]
        public void TestSetDynamicAllDict()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = new ExpandoObject();
            tExpando.Test.Test2 = new ExpandoObject();


            Impromptu.InvokeSetAll(tExpando, new Dictionary<string, object> {{"Test.Test2.Test3", tSetValue},{"One",1},{"Two",2}});

            Impromptu.InvokeSetChain(tExpando, "Test.Test2.Test3", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test.Test2.Test3);
            Assert.AreEqual(1, tExpando.One);
            Assert.AreEqual(2, tExpando.Two);
        }

        [Test]
        public void TestSetDynamicAllAnonymous()
        {
            dynamic tExpando = new ExpandoObject();

            Impromptu.InvokeSetAll(tExpando, new{One=1,Two=2,Three=3});

        
            Assert.AreEqual(1, tExpando.One);
            Assert.AreEqual(2, tExpando.Two);
            Assert.AreEqual(3, tExpando.Three);
        }

        [Test]
        public void TestSetDynamicAllNamed()
        {
            dynamic tExpando = new ExpandoObject();

            Impromptu.InvokeSetAll(tExpando,  One:1, Two:2, Three:3);


            Assert.AreEqual(1, tExpando.One);
            Assert.AreEqual(2, tExpando.Two);
            Assert.AreEqual(3, tExpando.Three);
        }

        [Test]
        public void TestSetDynamicChainedOne()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();


            Impromptu.InvokeSetChain(tExpando, "Test", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test);
        }

        [Test]
        public void TestGetDynamicChainedOne()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = tSetValue;



            var tOut = Impromptu.InvokeGetChain(tExpando, "Test");

            Assert.AreEqual(tSetValue, tOut);
        }

        [Test]
        public void TestCacheableGetDynamic()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = tSetValue;

            var tCached = new CacheableInvocation(InvocationKind.Get, "Test");

            var tOut = tCached.Invoke((object) tExpando);

            Assert.AreEqual(tSetValue, tOut);
        }

        [Test]
        public void TestStaticGet()
        {
            var tDate = Impromptu.InvokeGet(typeof(DateTime).WithStaticContext(), "Today");
            Assert.AreEqual(DateTime.Today, tDate);
        }

        [Test]
        public void TestCacheableStaticGet()
        {
            var tCached = new CacheableInvocation(InvocationKind.Get, "Today", context: typeof(DateTime).WithStaticContext());

            var tDate = tCached.Invoke(typeof(DateTime));
            Assert.AreEqual(DateTime.Today, tDate);
        }


        [Test]
        public void TestStaticGet2()
        {
            var tVal = Impromptu.InvokeGet(typeof(StaticType).WithStaticContext(), "Test");
            Assert.AreEqual(true, tVal);
        }

        [Test]
        public void TestStaticGet3()
        {
            var tVal = Impromptu.InvokeGet((StaticContext)typeof(StaticType), "Test");
            Assert.AreEqual(true, tVal);
        }
        [Test]
        public void TestStaticSet()
        {
            int tValue = 12;
            Impromptu.InvokeSet(typeof(StaticType).WithStaticContext(), "TestSet", tValue);
            Assert.AreEqual(tValue, StaticType.TestSet);
        }

        [Test]
        public void TestCacheableStaticSet()
        {
            int tValue = 12;

            var tCachedInvoke = new CacheableInvocation(InvocationKind.Set, "TestSet",
                                                        context: typeof (StaticType).WithStaticContext());
            tCachedInvoke.Invoke(typeof(StaticType), tValue);
            Assert.AreEqual(tValue, StaticType.TestSet);
        }

        [Test]
        public void TestStaticDateTimeMethod()
        {
            object tDateDyn = "01/20/2009";
            var tDate = Impromptu.InvokeMember(typeof(DateTime).WithStaticContext(), "Parse", tDateDyn);
            Assert.AreEqual(new DateTime(2009,1,20), tDate);
        }

        [Test]
        public void TestCacheableStaticDateTimeMethod()
        {
            object tDateDyn = "01/20/2009";
            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Parse", 1,
                                                        context: typeof (DateTime).WithStaticContext());
            var tDate = tCachedInvoke.Invoke(typeof(DateTime), tDateDyn);
            Assert.AreEqual(new DateTime(2009, 1, 20), tDate);
        }
        
       

        [Test]
        public void TestIsEvent()
        {
            dynamic tPoco = new PocoEvent();

            var tResult = Impromptu.InvokeIsEvent(tPoco, "Event");

            Assert.AreEqual(true, tResult);
        }

        [Test]
        public void TestCacheableIsEventAndIsNotEvent()
        {
            object tPoco = new PocoEvent();

            var tCachedInvoke = new CacheableInvocation(InvocationKind.IsEvent, "Event");

            var tResult = tCachedInvoke.Invoke(tPoco);

            Assert.AreEqual(true, tResult);

            dynamic tDynamic = new ImpromptuDictionary();

            tDynamic.Event = null;

            var tResult2 = tCachedInvoke.Invoke((object) tDynamic);

            Assert.AreEqual(false, tResult2);
        }

         [Test]
        public void TestIsNotEvent()
        {
            dynamic tDynamic = new ImpromptuDictionary();

            tDynamic.Event = null;
        
            var tResult = Impromptu.InvokeIsEvent(tDynamic, "Event");

            Assert.AreEqual(false, tResult);

            bool tTest = false;
            bool tTest2 = false;


            tDynamic.Event += new EventHandler<EventArgs>((@object, args) => { tTest = true; });

            tDynamic.Event += new EventHandler<EventArgs>((@object, args) => { tTest2 = true; });
           
             Assert.AreEqual(false, tTest);

             Assert.AreEqual(false, tTest2);

             tDynamic.Event(null, null);

             Assert.AreEqual(true, tTest);

             Assert.AreEqual(true, tTest2);

        }

         [Test]
         public void TestPocoAddAssign()
         {
             var tPoco = new PocoEvent();
             bool tTest = false;

             Impromptu.InvokeAddAssignMember(tPoco, "Event", new EventHandler<EventArgs>((@object, args) => { tTest = true; }));

             tPoco.OnEvent(null, null);

             Assert.AreEqual(true, tTest);

             var tPoco2 = new PropPoco() { Prop2 = 3 };

             Impromptu.InvokeAddAssignMember(tPoco2, "Prop2", 4);

             Assert.AreEqual(7L, tPoco2.Prop2);
         }

         [Test]
         public void TestCacheablePocoAddAssign()
         {
             var tPoco = new PocoEvent();
             bool tTest = false;

             var tCachedInvoke = new CacheableInvocation(InvocationKind.AddAssign, "Event");

             tCachedInvoke.Invoke(tPoco, new EventHandler<EventArgs>((@object, args) => { tTest = true; }));

             tPoco.OnEvent(null, null);

             Assert.AreEqual(true, tTest);

             var tPoco2 = new PropPoco() { Event = 3 };

             tCachedInvoke.Invoke(tPoco2, 4);

             Assert.AreEqual(7L, tPoco2.Event);
         }

         [Test]
         public void TestPocoSubtractAssign()
         {
             var tPoco = new PocoEvent();
             bool tTest = false;
             var tEvent = new EventHandler<EventArgs>((@object, args) => { tTest = true; });

             tPoco.Event += tEvent;

             Impromptu.InvokeSubtractAssignMember(tPoco, "Event", tEvent);

             tPoco.OnEvent(null, null);

             Assert.AreEqual(false, tTest);

             Impromptu.InvokeSubtractAssignMember(tPoco, "Event", tEvent);//Test Second Time

             var tPoco2 = new PropPoco() {Prop2 = 3};

             Impromptu.InvokeSubtractAssignMember(tPoco2, "Prop2", 4);

             Assert.AreEqual( -1L,tPoco2.Prop2);
         }

         [Test]
         public void TestCacheablePocoSubtractAssign()
         {
             var tPoco = new PocoEvent();
             bool tTest = false;
             var tEvent = new EventHandler<EventArgs>((@object, args) => { tTest = true; });

             var tCachedInvoke = new CacheableInvocation(InvocationKind.SubtractAssign, "Event");

             tPoco.Event += tEvent;

             tCachedInvoke.Invoke(tPoco, tEvent);

             tPoco.OnEvent(null, null);

             Assert.AreEqual(false, tTest);

             tCachedInvoke.Invoke(tPoco, tEvent);//Test Second Time

             var tPoco2 = new PropPoco() { Event = 3 };

             tCachedInvoke.Invoke(tPoco2, 4);

             Assert.AreEqual(-1, tPoco2.Event);
         }

         [Test]
         public void TestDynamicAddAssign()
         {
             var tDynamic = Build.NewObject(Prop2: 3, Event: null, OnEvent: new ThisAction<object, EventArgs>((@this, obj, args) => @this.Event(obj, args)));
             bool tTest = false;

             Impromptu.InvokeAddAssignMember(tDynamic, "Event", new EventHandler<EventArgs>((@object, args) => { tTest = true; }));

             tDynamic.OnEvent(null, null);

             Assert.AreEqual(true, tTest);

             Impromptu.InvokeAddAssignMember(tDynamic, "Prop2", 4);

             Assert.AreEqual(7L, tDynamic.Prop2);
         }

         [Test]
         public void TestCacheableDynamicAddAssign()
         {
             var tDynamic = Build.NewObject(Prop2: 3, Event: null, OnEvent: new ThisAction<object, EventArgs>((@this, obj, args) => @this.Event(obj, args)));
             var tDynamic2 = Build.NewObject(Event: 3);
             bool tTest = false;

             var tCachedInvoke = new CacheableInvocation(InvocationKind.AddAssign, "Event");

             tCachedInvoke.Invoke((object) tDynamic, new EventHandler<EventArgs>((@object, args) => { tTest = true; }));

             tDynamic.OnEvent(null, null);

             Assert.AreEqual(true, tTest);

             tCachedInvoke.Invoke((object)tDynamic2, 4);

             Assert.AreEqual(7, tDynamic2.Event);
         }

         [Test]
         public void TestDynamicSubtractAssign()
         {
             var tDynamic = Build.NewObject(Prop2: 3, Event: null, OnEvent: new ThisAction<object, EventArgs>((@this, obj, args) => @this.Event(obj, args)));
             bool tTest = false;
             var tEvent = new EventHandler<EventArgs>((@object, args) => { tTest = true; });

             tDynamic.Event += tEvent;

             Impromptu.InvokeSubtractAssignMember(tDynamic, "Event", tEvent);

             tDynamic.OnEvent(null, null);

             Assert.AreEqual(false, tTest);


             Impromptu.InvokeSubtractAssignMember(tDynamic, "Prop2", 4);

             Assert.AreEqual(-1L, tDynamic.Prop2);
         }


         [Test]
         public void TestCacheableDynamicSubtractAssign()
         {
             var tDynamic = Build.NewObject(Prop2: 3, Event: null, OnEvent: new ThisAction<object, EventArgs>((@this, obj, args) => @this.Event(obj, args)));
             var tDynamic2 = Build.NewObject(Event: 3);

             bool tTest = false;
             var tEvent = new EventHandler<EventArgs>((@object, args) => { tTest = true; });
           
             var tCachedInvoke = new CacheableInvocation(InvocationKind.SubtractAssign, "Event");

             tDynamic.Event += tEvent;

             tCachedInvoke.Invoke((object) tDynamic, tEvent);

             tDynamic.OnEvent(null, null);

             Assert.AreEqual(false, tTest);


             tCachedInvoke.Invoke((object)tDynamic2, 4);

             Assert.AreEqual(-1, tDynamic2.Event);
         }

        [Test]
        public void TestDynamicMemberNamesExpando()
        {
            ExpandoObject tExpando = Build<ExpandoObject>.NewObject(One: 1);

            Assert.AreEqual("One", Impromptu.GetMemberNames(tExpando,dynamicOnly:true).Single());
        }

        [Test]
        public void TestDynamicMemberNamesImpromput()
        {
            ImpromptuDictionary tDict = Build.NewObject(Two: 2);

            Assert.AreEqual("Two", Impromptu.GetMemberNames(tDict, dynamicOnly: true).Single());
        }


       

        private DynamicObject CreateMock(ExpressionType op)
        {
            var tMock = new Mock<DynamicObject>() { CallBase = true };
            object result = It.IsAny<object>();
            tMock.Setup(
                s => s.TryBinaryOperation(It.Is<BinaryOperationBinder>(b => b.Operation == op), It.IsAny<object>(), out result)
                ).Returns(true);
            return tMock.Object;
        }

        [Test]
        public void TestInvokeAdd()
        {
            Assert.AreEqual(Impromptu.InvokeBinaryOperator(1, ExpressionType.Add, 2), 3);
        }

        [Test]
        public void TestInvokeAddDynamic()
        {
            var tMock = CreateMock(ExpressionType.Add);
            Impromptu.InvokeBinaryOperator(tMock, ExpressionType.Add, 4);
        }
             
        
        [Test]
        public  void TestInvokeSubtract()
        {
            Assert.AreEqual(Impromptu.InvokeBinaryOperator(1, ExpressionType.Subtract, 2), -1);
        }


        [Test]
        public void TestInvokeSubtractDynamic()
        {
            var tType = ExpressionType.Subtract;
            var tMock = CreateMock(tType);
            Impromptu.InvokeBinaryOperator(tMock, tType, 4);
        }

    
    }
    
}
