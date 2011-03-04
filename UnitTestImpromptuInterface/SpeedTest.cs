using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

#if SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#elif !MONO
using ImpromptuInterface;
using NUnit.Framework;
#endif


namespace UnitTestImpromptuInterface
{
    [TestFixture, TestClass]
    public class SpeedTest : Helper
    {
        [Test, TestMethod]
        public void TestSetTimed()
        {
            var tPoco = new PropPoco();

            var tSetValue = "1";

            var tWatch = TimeIt.Go(() => Impromptu.InvokeSet(tPoco, "Prop1", tSetValue), 500000);
            var tWatch2 = TimeIt.Go(() => tPoco.GetType().GetProperty("Prop1").SetValue(tPoco, tSetValue, new object[] { }), 500000);

            Console.WriteLine("Impromptu: " + tWatch.Elapsed);
            Console.WriteLine("Refelection: " + tWatch2.Elapsed);

            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }


        [Test, TestMethod]
        public void TestPropStaticGetValueTimed()
        {



            var tSetValue = "1";
            var tAnon = new { Test = tSetValue };



            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeGet(tAnon, "Test"); }, 500000);
            var tWatch2 = TimeIt.Go(() => { var tOut = tAnon.GetType().GetProperty("Test").GetValue(tAnon, null); }, 500000);

            Console.WriteLine("Impromptu: " + tWatch.Elapsed);
            Console.WriteLine("Refelection: " + tWatch2.Elapsed);

            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }


        [Test, TestMethod]
        public void TestMethodStaticGetValueTimed()
        {


            var tValue = 1;



            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeMember(tValue, "ToString"); }, 500000);
            var tWatch2 = TimeIt.Go(() => { var tOut = tValue.GetType().GetMethod("ToString", new Type[] { }).Invoke(tValue, new object[] { }); }, 500000);

            Console.WriteLine("Impromptu: " + tWatch.Elapsed);
            Console.WriteLine("Refelection: " + tWatch2.Elapsed);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test, TestMethod]
        public void TestFastDynamicInvoke()
        {
            Func<int, bool> tFunc = it => it > 10;
            var tStopWatch1 = TimeIt.Go(() => tFunc.FastDynamicInvoke(5));

            var tStopWatch2 = TimeIt.Go(() => tFunc.DynamicInvoke(5));

            Console.WriteLine(tStopWatch1.Elapsed);
            Console.WriteLine(tStopWatch2.Elapsed);
            Assert.Less(tStopWatch1.Elapsed, tStopWatch2.Elapsed);
        }

        [Test, TestMethod]
        public void TestFastDynamicInvokeAction()
        {
            Action<int> tFunc = it => it.ToString();
            var tStopWatch1 = TimeIt.Go(() => tFunc.FastDynamicInvoke(5));

            var tStopWatch2 = TimeIt.Go(() => tFunc.DynamicInvoke(5));

            Console.WriteLine("Impromptu: " + tStopWatch1.Elapsed);
            Console.WriteLine("Refelection: " + tStopWatch2.Elapsed);
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
