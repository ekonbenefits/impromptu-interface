using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace UnitTestImpromptuInterface
{
    public class TestMethodAttribute : Attribute
    {

    }

    public class TestClassAttribute : Attribute
    {

    }

    public class Helper:AssertionHelper
    {
        public void AssertException<T>(TestDelegate action) where T : Exception
        {
            Assert.Throws<T>(action);
        }
    }
}
