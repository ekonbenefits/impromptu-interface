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
using System.Runtime.CompilerServices;
using System.Text;
using ImpromptuInterface;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;
using BinderFlags = Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags;
using Info = Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo;
using InfoFlags = Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags;


#if SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#elif !SELFRUNNER
using NUnit.Framework;
#endif

namespace UnitTestImpromptuInterface
{
    [TestFixture,TestClass]
    public class SingleMethodInvoke : Helper
    {
        [Test,TestMethod]
        public void TestDynamicSet()
        {
            dynamic tExpando = new ExpandoObject();

            var tSetValue = "1";

            Impromptu.InvokeSet(tExpando, "Test", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test);

        }
		     [Test,TestMethod]
        public void TestStaticSet()
        {
            var tPoco = new PropPoco();

            var tSetValue = "1";

            Impromptu.InvokeSet(tPoco, "Prop1", tSetValue);

            Assert.AreEqual(tSetValue, tPoco.Prop1);

        }

             [Test, TestMethod]
             public void TestStaticSetNull()
             {
                 var tPoco = new PropPoco(){Prop1 = "Test default"};

                 String tSetValue = null;


                 Impromptu.InvokeSet(tPoco, "Prop1", tSetValue);

                 Assert.AreEqual(tSetValue, tPoco.Prop1);


             }
		


        [Test, TestMethod]
        public void TestGetStatic()
        {
            
            var tSetValue = "1";
            var tAnon = new { Test = tSetValue };

            var tOut =Impromptu.InvokeGet(tAnon, "Test");

            Assert.AreEqual(tSetValue, tOut);

        }

        [Test, TestMethod]
        public void TestGetIndexer()
        {
       
            dynamic tSetValue = "1";
            var tAnon = new [] { tSetValue, "2" };


            string tOut = Impromptu.InvokeGetIndex(tAnon,0);

            Assert.AreEqual(tSetValue, tOut);

        }

        [Test, TestMethod]
        public void TestGetLengthArray()
        {
            var tAnon = new []  { "1", "2" };


            int tOut = Impromptu.InvokeGet(tAnon, "Length");

            Assert.AreEqual(2, tOut);

        }

        [Test, TestMethod]
        public void TestGetIndexerArray()
        {

            dynamic tSetValue = "1";
            var tAnon = new List<string> { tSetValue, "2" };
     

            string tOut = Impromptu.InvokeGetIndex(tAnon, 0);

            Assert.AreEqual(tSetValue, tOut);

        }

        [Test, TestMethod]
        public void TestSetIndexer()
        {
          
            dynamic tSetValue = "3";
            var tAnon =  new List<string> { "1", "2" };

            dynamic index = 0;
            dynamic tDyn = tAnon;


            Impromptu.InvokeSetIndex(tAnon, 0, tSetValue);

            Assert.AreEqual(tSetValue, tAnon[0]);

        }

     



        [Test, TestMethod]
        public void TestMethodDynamicPassAndGetValue()
        {
            dynamic tExpando = new ExpandoObject();
            tExpando.Func = new Func<int, string>(it => it.ToString());

            var tValue = 1;

            var tOut = Impromptu.InvokeMember(tExpando, "Func", tValue);

            Assert.AreEqual(tValue.ToString(), tOut);
        }


        [Test, TestMethod]
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

        [Test, TestMethod]
        public void TestMethodStaticOverloadingPassAndGetValueArg()
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

        [Test, TestMethod]
        public void TestMethodStaticOverloadingPassAndGetValueArgOptional()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var arg = InvokeArg.Create;

            var tOut = Impromptu.InvokeMember(tPoco, "Func",  arg("two", tValue));

            Assert.AreEqual("object named", tOut);

            Assert.AreEqual("object named", (object)tOut); 
        }

        [Test, TestMethod]
        public void TestMethodStaticOverloadingPass2AndGetValueArgOptional()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var arg = InvokeArg.Create;

            var tOut = Impromptu.InvokeMember(tPoco, "Func", arg("two", tValue), arg("one", tValue));

            Assert.AreEqual("object named", tOut);

            Assert.AreEqual("object named", (object)tOut);
        }

         [Test, TestMethod]
        public void TestMethodStaticOverloadingPassAndGetValueNull()
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
        [Test, TestMethod]
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


        [Test, TestMethod]
        public void TestMethodDynamicPassVoid()
        {
            var tTest = "Wrong";

            var tValue = "Correct";

            dynamic tExpando = new ExpandoObject();
            tExpando.Action = new Action<string>(it => tTest = it);



            Impromptu.InvokeMemberAction(tExpando, "Action", tValue);

            Assert.AreEqual(tValue, tTest);
        }


        [Test, TestMethod]
        public void TestMethodStaticGetValue()
        {
        

            var tValue = 1;

            var tOut = Impromptu.InvokeMember(tValue, "ToString");

            Assert.AreEqual(tValue.ToString(), tOut);
        }

  

        [Test, TestMethod]
        public void TestMethodStaticPassAndGetValue()
        {


            HelpTestStaticPassAndGetValue("Test", "Te");


            HelpTestStaticPassAndGetValue("Test", "st");
        }

        private void HelpTestStaticPassAndGetValue(string tValue, string tParam)
        {
            var tExpected = tValue.StartsWith(tParam);

            var tOut = Impromptu.InvokeMember(tValue, "StartsWith", tParam);

            Assert.AreEqual(tExpected, tOut);
        }


        [Test, TestMethod]
        public void TestGetDynamic()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = tSetValue;



            var tOut = Impromptu.InvokeGet(tExpando, "Test");

            Assert.AreEqual(tSetValue, tOut);

        }
		
		
		     

    }
}
