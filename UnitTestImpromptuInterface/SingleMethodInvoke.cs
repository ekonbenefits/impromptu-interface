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
#elif !MONO
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
            var tExpando = new PropPoco();

            var tSetValue = "1";

            Impromptu.InvokeSet(tExpando, "Prop1", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Prop1);

        }
		
		  [Test,TestMethod]
        public void TestSetTimed()
        {
            var tPoco = new PropPoco();

            var tSetValue = "1";
			
			var tWatch =TimeIt.Go(()=>{ Impromptu.InvokeSet(tPoco, "Prop1",tSetValue);}, 50000);
			var tWatch2 =TimeIt.Go(()=>{tPoco.GetType().GetProperty("Prop1").SetValue(tPoco,tSetValue,new object[]{});}, 50000);

		    Console.WriteLine("Impromptu: "+ tWatch.Elapsed);
			Console.WriteLine("Refelection: "+ tWatch2.Elapsed);
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
        public void TestPropStaticGetValueTimed()
        {
        

                  
            var tSetValue = "1";
            var tAnon = new { Test = tSetValue };

            
			
			var tWatch =TimeIt.Go(()=>{var tOut = Impromptu.InvokeGet(tAnon, "Test");}, 50000);
			var tWatch2 =TimeIt.Go(()=>{var tOut = tAnon.GetType().GetProperty("Test").GetValue(tAnon,null);}, 50000);

		Console.WriteLine("Impromptu: "+ tWatch.Elapsed);
			Console.WriteLine("Refelection: "+ tWatch2.Elapsed);
           
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
        public void TestMethodStaticGetValueTimed()
        {
        

            var tValue = 1;

            
			
			var tWatch =TimeIt.Go(()=>{var tOut = Impromptu.InvokeMember(tValue, "ToString");}, 50000);
			var tWatch2 =TimeIt.Go(()=>{var tOut = tValue.GetType().GetMethod("ToString",new Type[]{}).Invoke(tValue,new object[]{});}, 50000);

		Console.WriteLine("Impromptu: "+ tWatch.Elapsed);
			Console.WriteLine("Refelection: "+ tWatch2.Elapsed);
           
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
		
		
		     
		[Test, TestMethod]
		public void TestFastDynamicInvoke(){
			Func<int,bool> tFunc = it=> it > 10;
			var tStopWatch1 =TimeIt.Go(()=> tFunc.FastDynamicInvoke(5));
			
		    var tStopWatch2 =TimeIt.Go(()=> tFunc.DynamicInvoke(5));

			Console.WriteLine(tStopWatch1.Elapsed);
			Console.WriteLine(tStopWatch2.Elapsed);

		}
		
		[Test, TestMethod]
		public void TestFastDynamicInvokeAction(){
			Action<int> tFunc = it=> it.ToString();
			var tStopWatch1 =TimeIt.Go(()=> tFunc.FastDynamicInvoke(5));
			
		    var tStopWatch2 =TimeIt.Go(()=> tFunc.DynamicInvoke(5));

			Console.WriteLine("Impromptu: "+ tStopWatch1.Elapsed);
			Console.WriteLine("Refelection: "+ tStopWatch2.Elapsed);

		}
    }
}
