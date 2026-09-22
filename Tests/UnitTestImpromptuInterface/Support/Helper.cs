using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyUnit;
using AnyUnit.Run;
using AnyUnit.Style.Nunit;
using AnyUnit.Constraints;
using AnyUnit.Constraints.Pieces;

namespace UnitTestImpromptuInterface
{
  

   
    public class WriteLineContext
    {
        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format,args);
        }
    }

    // ClassicAssert is generated - see Tests/AssertShimGenerator.
    public class Helper : AnyUnit.Run.AssertionHelper
    {
        private ClassicAssert _classic;

        /// <summary>
        /// Shadows the injected <see cref="IAssert"/> with instance methods (see
        /// <see cref="ClassicAssert"/>). The interface property the engine assigns through is the
        /// base's, so the runner is unaffected.
        /// </summary>
        public new ClassicAssert Assert => _classic ?? (_classic = new ClassicAssert(base.Assert));

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
