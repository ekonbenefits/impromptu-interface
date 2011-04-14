using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ImpromptuInterface;

#if SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#elif !SELFRUNNER
using NUnit.Framework;
#endif


namespace UnitTestImpromptuInterface
{
    [TestFixture, TestClass]
    public class SpeedTest : Helper
    {
        [TestFixtureSetUp,TestInitialize]
        public void WarmUpDlr()
        {
            dynamic i = 1;
            i.ToString();
        }


        [Test, TestMethod]
        public void TestSetTimed()
        {
            var tPoco = new PropPoco();

            var tSetValue = "1";

            var tWatch = TimeIt.Go(() => Impromptu.InvokeSet(tPoco, "Prop1", tSetValue), 500000);
            var tPropertyInfo = tPoco.GetType().GetProperty("Prop1");
            var tWatch2 = TimeIt.Go(() => tPropertyInfo.SetValue(tPoco, tSetValue, new object[] { }), 500000);

            

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test, TestMethod]
        public void TestSetNullTimed()
        {
            var tPoco = new PropPoco();

            String tSetValue = null;

            var tWatch = TimeIt.Go(() => Impromptu.InvokeSet(tPoco, "Prop1", tSetValue), 500000);
            var tPropertyInfo = tPoco.GetType().GetProperty("Prop1");
            var tWatch2 = TimeIt.Go(() => tPropertyInfo.SetValue(tPoco, tSetValue, new object[] { }), 500000);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        
        [Test, TestMethod]
        public void TestPropPocoGetValueTimed()
        {



            var tSetValue = "1";
            var tAnon = new { Test = tSetValue };



            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeGet(tAnon, "Test"); }, 500000);

            var tPropertyInfo = tAnon.GetType().GetProperty("Test");
            var tWatch2 = TimeIt.Go(() =>
                                        {
                                            var tOut = tPropertyInfo.GetValue(tAnon, null);
                                        }, 500000);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }


        [Test, TestMethod]
        public void TestConstructorTimed()
        {


           


            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeConstuctor(typeof(Tuple<string>), "Test" ); });
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = Activator.CreateInstance(typeof(Tuple<string>),"Test");
            });

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }


        [Test, TestMethod]
        public void TestConstructorValueTypeTimed()
        {


            var tIter = 1000000;

            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeConstuctor(typeof(DateTime), 2010, 1, 20); }, tIter);
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = Activator.CreateInstance(typeof(DateTime), 2010, 1, 20);
            }, tIter);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }


        [Test, TestMethod]
        public void TestMethodPocoGetValueTimed()
        {


            var tValue = 1;



            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeMember(tValue, "ToString"); }, 500000);
            var tMethodInfo = tValue.GetType().GetMethod("ToString", new Type[] { });
            var tWatch2 = TimeIt.Go(() =>
                                        {
                                            var tOut = tMethodInfo.Invoke(tValue, new object[] { });
                                        }, 500000);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test, TestMethod]
        public void TestMethodStaticMethodValueTimed()
        {




            var tStaticType = typeof (DateTime);
            var tTarget = tStaticType.WithStaticContext();
            string tDate = "01/20/2009";
            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeMember(tTarget, "Parse", tDate); }, 500000);
            var tMethodInfo = typeof(DateTime).GetMethod("Parse",new[]{typeof(string)});
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = tMethodInfo.Invoke(tStaticType,new object[]{tDate});
            }, 500000);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test, TestMethod]
        public void TestMethodPocoGetValuePassNullTimed()
        {
#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif

            var tValue = new OverloadingMethPoco();


            var tInteration = 1000000;
            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeMember(tValue, "Func", null); }, tInteration);
            var tMethodInfo = tValue.GetType().GetMethod("Func", new Type[] { typeof(object)});
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = tMethodInfo.Invoke(tValue, new object[] { null});
            }, tInteration);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Reflection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test, TestMethod]
        public void TestMethodPocoGetValuePassNullDoubleCallTimed()
        {
#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif
            var tValue = new OverloadingMethPoco();


            var tInteration = 500000;
            var tWatch = TimeIt.Go(() => { 
                var tOut = Impromptu.InvokeMember(tValue, "Func", null); 
                var tOut2 = Impromptu.InvokeMember(tValue, "Func", 2); }, tInteration);

            var tMethodInfo = tValue.GetType().GetMethod("Func", new Type[] { typeof(object) });
            var tMethodInfo2 = tValue.GetType().GetMethod("Func", new Type[] { typeof(int) });
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = tMethodInfo.Invoke(tValue, new object[] { null });
                var tOut2 = tMethodInfo2.Invoke(tValue, new object[] { 2 });
            }, tInteration);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Reflection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test, TestMethod]
        public void TestMethodPocoGetValue4argsTimed()
        {


            var tValue = "test 123 45 string";



            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeMember(tValue, "IndexOf", "45", 0, 14, StringComparison.InvariantCulture); }, 500000);
            var tMethodInfo = tValue.GetType().GetMethod("IndexOf", new Type[] { typeof(string), typeof(int), typeof(int), typeof(StringComparison) });
            var tWatch2 = TimeIt.Go(() =>
                                        {
                                            var tOut = tMethodInfo.Invoke(tValue, new object[] { "45", 0, 14, StringComparison.InvariantCulture });
                                        }, 500000);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Reflection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

 

        [Test, TestMethod]
        public void TestMethodPocoVoidTimed()
        {


            var tValue = new Dictionary<object,object>();



            var tWatch = TimeIt.Go(() => Impromptu.InvokeMemberAction(tValue, "Clear"), 500000);
            var tMethodInfo = tValue.GetType().GetMethod("Clear", new Type[] { });
            var tWatch2 = TimeIt.Go(() => tMethodInfo.Invoke(tValue, new object[] { }), 500000);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Reflection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test, TestMethod]
        public void TestFastDynamicInvoke()
        {
            Func<int, bool> tFunc = it => it > 10;
            var tStopWatch1 = TimeIt.Go(() => tFunc.FastDynamicInvoke(5));

            var tStopWatch2 = TimeIt.Go(() => tFunc.DynamicInvoke(5));

            TestContext.WriteLine("Impromptu: " + tStopWatch1.Elapsed);
            TestContext.WriteLine("Reflection: " + tStopWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tStopWatch2.Elapsed.Ticks / tStopWatch1.Elapsed.Ticks);
            Assert.Less(tStopWatch1.Elapsed, tStopWatch2.Elapsed);
        }

        [Test, TestMethod]
        public void TestFastDynamicInvokeAction()
        {
            Action<int> tFunc = it => it.ToString();
            var tStopWatch1 = TimeIt.Go(() => tFunc.FastDynamicInvoke(5));

            var tStopWatch2 = TimeIt.Go(() => tFunc.DynamicInvoke(5));

            TestContext.WriteLine("Impromptu: " + tStopWatch1.Elapsed);
            TestContext.WriteLine("Reflection: " + tStopWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tStopWatch2.Elapsed.Ticks / tStopWatch1.Elapsed.Ticks);
            Assert.Less(tStopWatch1.Elapsed, tStopWatch2.Elapsed);
        }

    }

    public static class TimeIt
    {
        public static Stopwatch Go(Action action, int interation = 1000000)
        {
            var tStopwatch = new Stopwatch();
            tStopwatch.Start();
            for (int i = 0; i < interation; i++)
            {
                action();
            }
            tStopwatch.Stop();
            return tStopwatch;
        }
    }
}
