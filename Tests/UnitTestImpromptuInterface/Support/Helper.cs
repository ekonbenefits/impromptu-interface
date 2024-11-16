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
    
    //Not rewriting test for no good reason
#pragma warning disable CS0618 // Type or member is obsolete
    public class Helper:AssertionHelper
#pragma warning restore CS0618 // Type or member is obsolete
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
