using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using NUnit.Framework;

namespace TestImpromtuInterface
{



  

    [TestFixture]
    public class PrivateTest:AssertionHelper
    {
        [Test]
        public void Test()
        {
			
			
            var tTest =new TestWithPrivateMethod();

            //tTest.Test(); //Doesn't work cuz it's private

			
            var tExposed =tTest.ActLike<IExposePrivateMethod>();

            Assert.AreEqual(3,tExposed.Test());//Works
        }
		

    }
	
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
}
