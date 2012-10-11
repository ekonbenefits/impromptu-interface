using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using ImpromptuInterface.InvokeExt;


#if !SELFRUNNER
using NUnit.Framework;
#endif


#if SILVERLIGHT
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{
    [TestFixture][Category("performance")]
    public class SpeedTest : Helper
    {
        [TestFixtureSetUp]
        public void WarmUpDlr()
        {
            dynamic i = 1;
            i.ToString();
        }


        [Test]
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


        [Test]
        public void TestCacheableSetTimed()
        {
            var tPoco = new PropPoco();

            var tSetValue = "1";

            var tCacheable = new CacheableInvocation(InvocationKind.Set, "Prop1");
            var tWatch = TimeIt.Go(() => tCacheable.Invoke(tPoco, tSetValue), 500000);
            var tPropertyInfo = tPoco.GetType().GetProperty("Prop1");
            var tWatch2 = TimeIt.Go(() => tPropertyInfo.SetValue(tPoco, tSetValue, new object[] { }), 500000);



            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test]
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

        [Test]
        public void TestCacheableSetNullTimed()
        {
            var tPoco = new PropPoco();

            String tSetValue = null;
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Set, "Prop1");
            var tWatch = TimeIt.Go(() => tCachedInvoke.Invoke(tPoco, tSetValue), 500000);
            var tPropertyInfo = tPoco.GetType().GetProperty("Prop1");
            var tWatch2 = TimeIt.Go(() => tPropertyInfo.SetValue(tPoco, tSetValue, new object[] { }), 500000);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }



        [Test]
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



        [Test]
        public void TestCacheableGetValueTimed()
        {



            var tSetValue = "1";
            var tAnon = new PropPoco() {Prop1 = tSetValue};


            var tInvoke = new CacheableInvocation(InvocationKind.Get, "Prop1");
            var tWatch = TimeIt.Go(() => { var tOut = tInvoke.Invoke(tAnon); }, 500000);

            var tPropertyInfo = tAnon.GetType().GetProperty("Prop1");
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = tPropertyInfo.GetValue(tAnon, null);
            }, 500000);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test]
        public void TestConstructorTimed()
        {


           


            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeConstructor(typeof(Tuple<string>), "Test" ); });
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = Activator.CreateInstance(typeof(Tuple<string>),"Test");
            });

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test]
        public void TestCacheableConstructorTimed()
        {




            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor, argCount: 1);
            var tWatch = TimeIt.Go(() => { var tOut = tCachedInvoke.Invoke(typeof(Tuple<string>), "Test"); });
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = Activator.CreateInstance(typeof(Tuple<string>), "Test");
            });

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

#if !SILVERLIGHT
        [Test]
        public void TestConstructorNoARgTimed()
        {
            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeConstructor(typeof(List<string>)); });
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = Activator.CreateInstance(typeof(List<string>));
            });
            var tWatch3 = TimeIt.Go(() =>
            {
                var tOut = Activator.CreateInstance<List<string>>();
            });
            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Refelection Generic: " + tWatch3.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);

            Assert.Ignore("I don't think this is beatable at the moment");
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test]
        public void TestCachableConstructorNoARgTimed()
        {
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor);
            var tWatch = TimeIt.Go(() => { var tOut = tCachedInvoke.Invoke(typeof(List<string>)); });
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = Activator.CreateInstance(typeof(List<string>));
            });
            var tWatch3 = TimeIt.Go(() =>
            {
                var tOut = Activator.CreateInstance<List<string>>();
            });

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Refelection Generic: " + tWatch3.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);

            Assert.Ignore("I don't think this is beatable at the moment");
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }
#endif

        [Test]
        public void TestConstructorValueTypeTimed()
        {


            var tIter = 1000000;

            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeConstructor(typeof(DateTime), 2010, 1, 20); }, tIter);
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = Activator.CreateInstance(typeof(DateTime), 2010, 1, 20);
            }, tIter);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test]
        public void TestCachedConstructorValueTypeTimed()
        {


            var tIter = 1000000;
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor, argCount: 3);
            var tWatch = TimeIt.Go(() => { var tOut = tCachedInvoke.Invoke(typeof(DateTime), 2010, 1, 20); }, tIter);
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = Activator.CreateInstance(typeof(DateTime), 2010, 1, 20);
            }, tIter);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }


        [Test]
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

        [Test]
        public void TestCacheableMethodPocoGetValueTimed()
        {


            var tValue = 1;


            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember,"ToString");
            var tWatch = TimeIt.Go(() => { var tOut = tCachedInvoke.Invoke(tValue); }, 500000);
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

        [Test]
        public void TestGetStaticTimed()
        {




            var tStaticType = typeof(DateTime);
            var tTarget = tStaticType.WithStaticContext();
            var tWatch = TimeIt.Go(() => { var tOut = Impromptu.InvokeGet(tTarget, "Today"); }, 500000);
            var tMethodInfo = typeof (DateTime).GetProperty("Today").GetGetMethod();
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = tMethodInfo.Invoke(tStaticType, new object[] { });
            }, 500000);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test]
        public void TestCacheableGetStaticTimed()
        {

            var tStaticType = typeof(DateTime);
            var tContext = tStaticType.WithStaticContext();
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Get, "Today", context: tContext);

            var tWatch = TimeIt.Go(() =>
                                       {
                                           var tOut = tCachedInvoke.Invoke(tStaticType);
                                       }, 500000);
            var tMethodInfo = typeof(DateTime).GetProperty("Today").GetGetMethod();

            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = tMethodInfo.Invoke(tStaticType, new object[] { });
            }, 500000);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test]
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

        [Test]
        public void TestCacheableMethodStaticMethodValueTimed()
        {

            var tStaticType = typeof(DateTime);
            var tContext = tStaticType.WithStaticContext();
            string tDate = "01/20/2009";

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Parse", argCount: 1,
                                                        context: tContext);
            var tWatch = TimeIt.Go(() => { var tOut = tCachedInvoke.Invoke(tStaticType, tDate); }, 500000);
            var tMethodInfo = typeof(DateTime).GetMethod("Parse", new[] { typeof(string) });
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = tMethodInfo.Invoke(tStaticType, new object[] { tDate });
            }, 500000);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Refelection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }
#if !SILVERLIGHT
        [Test]
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

        [Test]
        public void TestCacheableMethodPocoGetValuePassNullTimed()
        {
#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif

            var tValue = new OverloadingMethPoco();


            var tInteration = 1000000;

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Func", argCount:1);

            var tWatch = TimeIt.Go(() => { var tOut = tCachedInvoke.Invoke(tValue, null); }, tInteration);
            var tMethodInfo = tValue.GetType().GetMethod("Func", new Type[] { typeof(object) });
            var tWatch2 = TimeIt.Go(() =>
            {
                var tOut = tMethodInfo.Invoke(tValue, new object[] { null });
            }, tInteration);

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Reflection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }


        [Test]
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
#endif
        [Test]
        public void TestCacheableMethodPocoGetValuePassNullDoubleCallTimed()
        {
            var tValue = new OverloadingMethPoco();

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Func", 1);
            var tInteration = 500000;
            var tWatch = TimeIt.Go(() =>
            {
                var tOut = tCachedInvoke.Invoke(tValue, null);
                var tOut2 = tCachedInvoke.Invoke(tValue, 2);
            }, tInteration);

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

        [Test]
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

        [Test]
        public void TestCacheableMethodPocoGetValue4argsTimed()
        {


            var tValue = "test 123 45 string";


            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "IndexOf", 4);
            var tWatch = TimeIt.Go(() =>
                                       {
                                           var tOut = tCachedInvoke.Invoke(tValue,"45", 0, 14, StringComparison.InvariantCulture);
                                       }, 500000);
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
 

        [Test]
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

        [Test]
        public void TestCacheableMethodPocoVoidTimed()
        {


            var tValue = new Dictionary<object, object>();

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMemberAction, "Clear");

            var tWatch = TimeIt.Go(() => tCachedInvoke.Invoke(tValue));
            var tMethodInfo = tValue.GetType().GetMethod("Clear", new Type[] { });
            var tWatch2 = TimeIt.Go(() => tMethodInfo.Invoke(tValue, new object[] { }));

            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Reflection: " + tWatch2.Elapsed);
            TestContext.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)tWatch2.Elapsed.Ticks / tWatch.Elapsed.Ticks);
            Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test]
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

        [Test]
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

   
}
