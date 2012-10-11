using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using ClaySharp;
using ClaySharp.Behaviors;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using ImpromptuInterface.Optimization;
#if !SELFRUNNER
using NUnit.Framework;
#endif

#if SILVERLIGHT
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{   
    
    //Test data modified from MS-PL Clay http://clay.codeplex.com
    /// <summary>
    /// Testing Integration of Clay with Impromptu-Interface
    /// </summary>
    [TestFixture]
    public class ClayTest : Helper
    {

        [Test]
        public void InvokeMemberContainsNameWithImpromptuInterface()
        {
            var clay = new Clay(new TestBehavior()).ActLike<ISimpeleClassMeth>();
            var result = clay.Action3();
            Assert.IsTrue(result.Contains("[name:Action3]"), "Does Not Match Argument Name");
            Assert.IsTrue(result.Contains("[count:0]"), "Does Not Match Argument Count");

        }

        [Test]
        public void InvokeMemberContainsNameWithImpromptuInvoke()
        {
            var clay = new Clay(new TestBehavior());
            var result = Impromptu.InvokeMember(clay, "Help", "Test");
            Assert.IsTrue(result.Contains("[name:Help]"), "Does Not Match Argument Name");
            Assert.IsTrue(result.Contains("[count:1]"), "Does Not Match Argument Count");

        }

        [Test]
        public void TestRecorder()
        {
            dynamic New = Builder.New<ImpromptuRecorder>();

            ImpromptuRecorder tRecording = New.Watson(Test: "One", Test2: 2, NameLast: "Watson");

            
            dynamic tVar =tRecording.ReplayOn(new ExpandoObject());

            Assert.AreEqual("One", tVar.Test);
            Assert.AreEqual(2, tVar.Test2);
            Assert.AreEqual("Watson", tVar.NameLast);
        }

        [Test]
        public void TestClay()
        {
            dynamic New = new ClayFactory();

            var directory = New.Array(
                   New.Person().Name("Louis").Aliases(new[] { "Lou" }),
                   New.Person().Name("Bertrand").Aliases("bleroy", "boudin")
                   ).Name("Orchard folks");

            Assert.AreEqual(2, directory.Count);
            Assert.AreEqual("Orchard folks", directory.Name);
            Assert.AreEqual("Louis",directory[0].Name);
            Assert.AreEqual(1, directory[0].Aliases.Count);
            Assert.AreEqual("Lou",directory[0].Aliases[0]);
            Assert.AreEqual("Bertrand",directory[1].Name);
            Assert.AreEqual(2, directory[1].Aliases.Count);
            Assert.AreEqual("bleroy",directory[1].Aliases[0]);
            Assert.AreEqual("boudin",directory[1].Aliases[1]);
        }


        [Test]
        public void TestBuilderWithClay()
          {
		
			
              var New = Builder.New<Clay>()
                  .ObjectSetup(
                   Return<object[]>.Arguments(
                  () => new object[]{new IClayBehavior[]{
                    new InterfaceProxyBehavior(),
                    new PropBehavior(),   
                    new ArrayPropAssignmentBehavior(),
                    new NilResultBehavior()}}))
                  .ArraySetup(
                  Return<object[]>.Arguments(
                  ()=>new object[]{ new IClayBehavior[]{
                    new InterfaceProxyBehavior(),
                    new PropBehavior(),
                    new ArrayPropAssignmentBehavior(),
                    new ArrayBehavior(),
                    new NilResultBehavior()}}));



              var directory = New.Array(
                        New.Person().Name("Louis").Aliases(new[] { "Lou" }),
                        New.Person().Name("Bertrand").Aliases("bleroy", "boudin")
                        ).Name("Orchard folks");

             Assert.AreEqual(2, directory.Count);
             Assert.AreEqual("Orchard folks", directory.Name);
             Assert.AreEqual("Louis", directory[0].Name);
             Assert.AreEqual(1, directory[0].Aliases.Count);
             Assert.AreEqual("Lou", directory[0].Aliases[0]);
             Assert.AreEqual("Bertrand", directory[1].Name);
             Assert.AreEqual(2, directory[1].Aliases.Count);
             Assert.AreEqual("bleroy", directory[1].Aliases[0]);
             Assert.AreEqual("boudin", directory[1].Aliases[1]);
        }


         /// <summary>
         /// Impromptu's Interface Proxy is about the same Speed as Clay's
         /// </summary>
        [Test]
        [Category("performance")]
        public void SpeedTestInterface()
        {   
            dynamic New = new ClayFactory();
            IRobot tRobot = New.Robot().Name("Bender");
            IRobot tRobotI = Impromptu.ActLike<IRobot>(New.Robot().Name("Bender"));
         
             var tInteration = 50000;
             var tWatchC = TimeIt.Go(() =>
                                         {
                                             var tOut =
                                                 Impromptu.ActLike<IRobot>(New.Robot().Name("Bender"));
                                         }, tInteration);
            var tWatchC2 = TimeIt.Go(() =>
                                         {
                                             IRobot tOut = New.Robot().Name("Bender");
                                         },tInteration );
           
            TestContext.WriteLine("*CreateInterface*");
            TestContext.WriteLine("Impromptu: " + tWatchC.Elapsed);
            TestContext.WriteLine("Clay: " + tWatchC2.Elapsed);
            TestContext.WriteLine("Impromptu VS Clay: {0:0.0} x faster", (double)tWatchC2.ElapsedTicks / tWatchC.ElapsedTicks);
            Assert.Less(tWatchC.Elapsed, tWatchC2.Elapsed);

            var tWatch = TimeIt.Go(() => { var tOut = tRobotI.Name; }, tInteration);

            var tWatch2 = TimeIt.Go(() => { var tOut = tRobot.Name; }, tInteration);
            
            TestContext.WriteLine("*Get from Interface*");
            TestContext.WriteLine("Impromptu: " + tWatch.Elapsed);
            TestContext.WriteLine("Clay: " + tWatch2.Elapsed);

            TestContext.WriteLine("Impromptu VS Clay: {0:0.0} x faster", (double)tWatch2.ElapsedTicks / tWatch.ElapsedTicks);

             Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        }

        [Test]
        [Category("performance")]
        public void SpeedTestPrototype()
        {
            dynamic NewI = Builder.New();
            dynamic NewE = Builder.New<ExpandoObject>();
            dynamic NewP = Builder.New<Robot>();
            dynamic NewC = new ClayFactory();

            var tRobotI = NewI.Robot(Name: "Bender");
            var tRobotC = NewC.Robot(Name: "Bender");
            var tRobotE = NewE.Robot(Name: "Bender");
            Robot tRobotP = NewP.Robot(Name: "Bender");

            var tWatchI = TimeIt.Go(() =>
            {
                var tOut = tRobotI.Name;
            });
   
            var tWatchC = TimeIt.Go(() =>
             {
                 var tOut =tRobotC.Name;
             } );

            var tWatchE = TimeIt.Go(() =>
            {
                var tOut = tRobotE.Name;
            });

            var tWatchP = TimeIt.Go(() =>
            {
                var tOut = tRobotP.Name;
            });
            TestContext.WriteLine("Impromptu: " + tWatchI.Elapsed);
            TestContext.WriteLine("Clay: " + tWatchC.Elapsed);
            TestContext.WriteLine("Expando: " + tWatchE.Elapsed);
            TestContext.WriteLine("POCO: " + tWatchP.Elapsed);

            Assert.Less(tWatchI.Elapsed, tWatchC.Elapsed);

            TestContext.WriteLine("Impromptu VS Clay: {0:0.0} x faster", (double)tWatchC.ElapsedTicks / tWatchI.ElapsedTicks);
            TestContext.WriteLine("Expando  VS Clay:{0:0.0}  x faster", (double)tWatchC.ElapsedTicks / tWatchE.ElapsedTicks);
            TestContext.WriteLine("POCO  VS Clay:{0:0.0}  x faster", (double)tWatchC.ElapsedTicks / tWatchP.ElapsedTicks);
            TestContext.WriteLine("POCO  VS Impromptu:{0:0.0}  x faster", (double)tWatchI.ElapsedTicks / tWatchP.ElapsedTicks);
            TestContext.WriteLine("POCO  VS Expando:{0:0.0}  x faster", (double)tWatchE.ElapsedTicks / tWatchP.ElapsedTicks);
            TestContext.WriteLine("Expando  VS Impromptu:{0:0.0}  x faster", (double)tWatchI.ElapsedTicks / tWatchE.ElapsedTicks);
        }

        //TestBehavoir from MS-PL ClaySharp http://clay.codeplex.com
        class TestBehavior : ClayBehavior
        {
            public override object InvokeMember(Func<object> proceed, object self, string name, INamedEnumerable<object> args)
            {
                return string.Format("[name:{0}] [count:{1}]", name ?? "<null>", args.Count());
            }
        }
    }
    
}
