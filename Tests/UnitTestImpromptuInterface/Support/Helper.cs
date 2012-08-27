using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace UnitTestImpromptuInterface
{
  

   
    public class WriteLineContext
    {
        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format,args);
        }
    }

    public class Helper:AssertionHelper
    {
        public WriteLineContext TestContext
        {
            get { return new WriteLineContext(); }
        }

        public void AssertException<T>(TestDelegate action) where T : Exception
        {
            Assert.Throws<T>(action);
        }
    }
}
