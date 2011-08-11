using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NUnit.Framework;

namespace UnitTestImpromptuInterface
{
    public class SerializableAttribute : Attribute
    {
    }

    
    public class Stopwatch
    {
        private DateTime StartDate;
        private DateTime EndDate;

        public void Start()
        {
            StartDate = DateTime.Now;
        }

        public void Stop()
        {
            EndDate = DateTime.Now;
        }
        public TimeSpan Elapsed
        {
            get { return EndDate - StartDate; }
        }
    }


    public class WriteLineContext
    {
        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }


    public class Helper
    {
       
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public WriteLineContext TestContext
        {
            get { return new WriteLineContext(); }
        }

        public void AssertException<T>(TestDelegate action) where T:Exception
        {
            Assert.Throws<T>(action);
        }
    }
}
