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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestImpromptuInterface
{
    public class TestFixtureAttribute : Attribute
    {
        
    }

    public class TestAttribute : Attribute
    {
    
    }


    public class Helper
    {
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

            Assert.IsTrue(tSuccess,"Expected Exception {0} instead of {1}",
                typeof(T).ToString(),
                tEc ==null ? "No Exception" : tEc.GetType().ToString() );
        }
    }
}
