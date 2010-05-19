using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using NUnit.Framework;

namespace TestImpromtuInterface
{
    public class TestWithPrivateMethod
    {
        private int Test()
        {
            return 3;
        }
    }


    public interface  IExposePrivateMethod
    {
        int Test();
    }


    
    public class PrivateTest
    {
        [Test]
        public void Test()
        {
            var tTest =new TestWithPrivateMethod();

            //tTest.Test(); //Doesn't work cuz it's private

            var tExposed =tTest.ActsLike<IExposePrivateMethod>();

            Assert.AreEqual(3,tExposed.Test());
        }
    }
}
