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
using Test =Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestImpromptuInterface
{
    public class SerializableAttribute : Attribute
    {
    }

    public class TestFixtureAttribute : Attribute
    {
        
    }

    public class TestAttribute : Attribute
    {
    
    }

    public class AssertHelp
    {
        public void AreEqual(dynamic expected, dynamic actual)
        {
            Test.Assert.AreEqual(expected, actual);
        }

        public void Less(dynamic smaller, dynamic larger)
        {

            if (smaller > larger)
                Test.Assert.Fail("{0} is expected to be less than {1}", smaller, larger);
        }
        
        public void Ignore(string message)
        {
            Test.Assert.Inconclusive(message);
        }

        public void IsFalse(bool actual)
        {
            Test.Assert.IsFalse(actual);
        }

        public void Fail()
        {
            Test.Assert.Fail();
        }
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

    public class Helper
    {
        public AssertHelp Assert{get{return new AssertHelp();}}


        public void AssertException<T>(Action action) where T:Exception
        {
            var tSuccess = false;
            Exception tEc = null;
            try
            {
                action();
            }
            catch (T ex)
            {
                tSuccess = true;
                tEc = ex;
            }
            catch(Exception ex)
            {
                tEc = ex;
            }

            Test.Assert.IsTrue(tSuccess,"Expected Exception {0} instead of {1}",
                typeof(T).ToString(),
                tEc ==null ? "No Exception" : tEc.GetType().ToString() );
        }
    }
}
