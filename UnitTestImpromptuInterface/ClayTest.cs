using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using ClaySharp;
using ClaySharp.Behaviors;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
#if !SELFRUNNER
using NUnit.Framework;
#endif

namespace UnitTestImpromptuInterface
{   
    
    //Test data modified from MS-PL Clay http://clay.codeplex.com
    [TestClass,TestFixture]
    public class ClayTest : Helper
    { 
        
        
        
        [Test,TestMethod]
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


         [Test,TestMethod]
        public void TestBuilderWithClay()
          {
              var New = Builder.New<Clay>()
                  .ObjectSetup(
                   Return<object[]>.Arguments(
                  () => new object[]{
                    new InterfaceProxyBehavior(),
                    new PropBehavior(),   
                    new ArrayPropAssignmentBehavior(),
                    new NilResultBehavior()}))
                  .ArraySetup(
                  Return<object[]>.Arguments(
                  ()=>new object[]{
                    new InterfaceProxyBehavior(),
                    new PropBehavior(),
                    new ArrayPropAssignmentBehavior(),
                    new ArrayBehavior(),
                    new NilResultBehavior()}));



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
        [Test, TestMethod]
        public void SpeedTestInterface()
        {   
            dynamic New = new ClayFactory();
            IRobot tRobot = New.Robot().Name("Bender");
            IRobot tRobotI = Impromptu.ActLike<IRobot>(New.Robot().Name("Bender"));
            
            var tWatchC = TimeIt.Go(() =>
                                         {
                                             var tOut =
                                                 Impromptu.ActLike<IRobot>(New.Robot().Name("Bender"));
                                         }, 50000);
            var tWatchC2 = TimeIt.Go(() =>
                                         {
                                             IRobot tOut = New.Robot().Name("Bender");
                                         },50000 );

            Console.WriteLine("Impromptu: " + tWatchC.Elapsed);
            Console.WriteLine("Clay: " + tWatchC2.Elapsed);

            Assert.Less(tWatchC.Elapsed, tWatchC2.Elapsed);

            var tWatch = TimeIt.Go(() => { var tOut = tRobotI.Name; }, 50000);

            var tWatch2 = TimeIt.Go(() => { var tOut = tRobot.Name; }, 50000);

            Console.WriteLine("Impromptu: " + tWatch.Elapsed);
            Console.WriteLine("Clay: " + tWatch2.Elapsed);

             var tDiffernce = (tWatch.Elapsed - tWatch2.Elapsed);
             Console.WriteLine("50000 Difference: " + tDiffernce);

             Assert.Less(tWatch.Elapsed, tWatch2.Elapsed);
        } 
           
        [Test, TestMethod]
        public void SpeedTestPrototype()
        {
            dynamic NewI = Builder.New();
            dynamic NewE = Builder.New<ExpandoObject>();
            dynamic NewC = new ClayFactory();

            var tRobotI = NewI.Robot(Name: "Bender");
            var tRobotC = NewC.Robot(Name: "Bender");
            var tRobotE = NewE.Robot(Name: "Bender");
         

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

            Console.WriteLine("Impromptu: " + tWatchI.Elapsed);
            Console.WriteLine("Clay: " + tWatchC.Elapsed);
            Console.WriteLine("Expando: " + tWatchE.Elapsed);

            Assert.Less(tWatchI.Elapsed, tWatchC.Elapsed);

            Console.WriteLine("Impromptu VS Clay: {0:0.0} x faster", (double)tWatchC.ElapsedTicks / tWatchI.ElapsedTicks);
            Console.WriteLine("Expando  VS Clay:{0:0.0}  x faster", (double)tWatchC.ElapsedTicks / tWatchE.ElapsedTicks);
            Console.WriteLine("Expando  VS Impromptu:{0:0.0}  x faster", (double)tWatchI.ElapsedTicks / tWatchE.ElapsedTicks);
        }
    }
    
}
