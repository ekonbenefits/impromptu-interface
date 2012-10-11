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
using Assert = NUnit.Framework.Assert;
using MSTest =Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NUnit.Framework
{
    public delegate void TestDelegate();


    public class Assert
    {
        public static void Throws<T>(TestDelegate fun) where T:Exception
        {
            var run = false;
            try
            {
                fun();
            }
            catch (Exception e)
            {

                run = e is T || e is MSTest.AssertFailedException && new AssertionException("Dummy") is T;
            }

            MSTest.Assert.IsTrue(run,"Did Not Catch " + typeof(T).Name);
        }

        public static void AreEqual(dynamic a, dynamic b)
        {
            MSTest.Assert.AreEqual(a,b);
        }
        public static void AreNotEqual(dynamic a, dynamic b)
        {
            MSTest.Assert.AreNotEqual(a, b);
        }

        public static void Fail()
        {
            MSTest.Assert.Fail();
        }

        public static void IsFalse(bool test)
        {
            MSTest.Assert.IsFalse(test);
        }
        
        public static void IsInstanceOf<T>(object obj)
        {
            MSTest.Assert.IsInstanceOfType(obj,typeof(T));
        }

        public static void IsNotNull(object obj)
        {
            MSTest.Assert.IsNotNull(obj);
        }

        public static void Less(dynamic a, dynamic b)
        {
            MSTest.Assert.IsTrue( a < b, String.Format("Expected {0} to be less than {1}",a,b));
        }
    }
    
    public class AssertionException:MSTest.AssertFailedException
    {
        public AssertionException(string message) : base(message)
        {
        }

        public AssertionException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class TestFixtureSetUpAttribute : Attribute
    {

    }

    public class SetUpAttribute : Attribute
    {

    }

    public class TestFixtureAttribute:Attribute
    {
        
    }

    public class TestAttribute : Attribute
    {

    }

    public class CategoryAttribute:Attribute
    {
        public CategoryAttribute(string label){}
    }
}


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
