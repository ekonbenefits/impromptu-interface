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


        private string TestDynamic<T>(T item) where T : class
        {
            dynamic tItem = new TestClassWithPrivateMethod();

            return tItem.Test<T>(item);
        }


    }
	
		 public class TestWithPrivateMethod
    {
        private int Test()
        {
            return 3;
        }
    }

      

         public class TestClassWithPrivateMethod
         {
             private string Test<T>(T item) where T: class 
             {
                 return item.ToString();
             }
         }

	
	public interface  IExposePrivateMethod
    {
        int Test();
    }
}
