using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;

namespace UnitTestImpromptuInterface
{
    public class TestMethodAttribute : Attribute
    {

    }

    public class TestClassAttribute : Attribute
    {

    }
    public class TestFixtureAttribute : Attribute
    {

    }

    public class TestFixtureSetUpAttribute : Attribute
    {

    }

    public class TestInitializeAttribute : Attribute
    {

    }

    public class TestAttribute : Attribute
    {

    }

    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string label) { }
    }

    public class AssertionException: Exception
    {
        public AssertionException()
        {
            
        }

        public AssertionException(string message) : base(message)
        {
            
        }
        public AssertionException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }

    public class IgnoreException : Exception
    {
        public IgnoreException()
        {

        }

        public IgnoreException(string message)
            : base(message)
        {

        }
        public IgnoreException(string message, Exception inner)
            : base(message, inner)
        {

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
        public Helper()
		{
            Assert = new AssertHelper();
            

		}

        public WriteLineContext TestContext
        {
            get { return new WriteLineContext(); }
        }

        public AssertHelper Assert { get; private set; }

        public void AssertException<T>(Action action) where T:Exception
        {
            bool tSuccess = false;
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
            catch (Exception ex)
            {
                tEc = ex;
            }

            if (!tSuccess)
            {
                throw new AssertionException(String.Format("Expected Exception {0} instead of {1}",
                typeof(T).ToString(),
                tEc == null ? "No Exception" : tEc.GetType().ToString()));
            }
        }

    }

 

    public class AssertHelper
    {
        public void AreEqual(dynamic expected, dynamic actual)
        { 
            if(expected ==null && actual==null)
                return;
            if(expected ==null || !expected.Equals(actual))
                FailExpected(expected, actual);
        }

        public void AreNotEqual(dynamic expected, dynamic actual)
        {
            if (expected == null && actual != null)
                return;
            if (expected == null || expected.Equals(actual))
                FailExpected(expected, actual);
        } 
		
		
		   public void Less(dynamic smaller, dynamic larger)
        { 
         
            if(smaller > larger)
                FailLess(smaller, larger);
        }   
      
        public void IsTrue(bool actual,string message)
        {
            if (!actual)
            {
                throw new AssertionException(message ?? "Expected True");
            }
        }
        public void IsTrue(bool actual)
        {
            IsTrue(actual, null);
        }   

        public void IsFalse(bool actual)
        {
            IsFalse(actual, null);
        }   

        public void IsFalse(bool actual, string message)
        {
            if (actual)
            {
                throw new AssertionException(message ?? "Expected False");
            }
        }

        public void Ignore(string message)
        {
            throw new IgnoreException(message);
        }

        public void Fail()
        {
            throw new AssertionException();
        }

        private static void FailExpected(object expected, object actual)
        {
            throw new AssertionException(String.Format("Expected {0} instead got {1}", expected,actual));
        }
		 private static void FailLess(object expected, object actual)
        {
            throw new AssertionException(String.Format("Expected {0} to be less {1}", expected,actual));
        }

        public void Fail(string message)
        {
            throw new AssertionException(message);
        }

        public void Throws<T>(Action func) where T:Exception
        {
            var tFail = true;
            try
            {
                func();
            }
            catch (T e)
            {
                tFail = false;
            }

            if (tFail)
            {
                throw new AssertionException(String.Format("Expected {0} to be Thrown", typeof(T)));
            }
        }
    }

  
}
