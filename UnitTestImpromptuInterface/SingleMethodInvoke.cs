using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using NUnit.Framework;

namespace UnitTestImpromptuInterface
{
    [TestFixture]
    public class SingleMethodInvoke:AssertionHelper
    {
        [Test]
        public void TestDynamicSet()
        {
            dynamic tExpando = new ExpandoObject();

            var tSetValue = "1";

            Impromptu.InvokeSet(tExpando, "Test", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test);

        }

        [Test]
        public void TestGetStatic()
        {
            
            var tSetValue = "1";
            var tAnon = new { Test = tSetValue };



            var tOut =Impromptu.InvokeGet(tAnon, "Test");

            Assert.AreEqual(tSetValue, tOut);

        }

        
        [Test]
        public void TestMethodDynamicPassAndGetValue()
        {
            dynamic tExpando = new ExpandoObject();
            tExpando.Func = new Func<int, string>(it => it.ToString());

            var tValue = 1;

            var tOut = Impromptu.InvokeMember(tExpando, "Func", tValue);

            Assert.AreEqual(tValue.ToString(), tOut);
        }


        [Test]
        public void TestMethodDynamicPassVoid()
        {
            var tTest = "Wrong";

            var tValue = "Correct";

            dynamic tExpando = new ExpandoObject();
            tExpando.Action = new Action<string>(it => tTest = it);



            Impromptu.InvokeMember(tExpando, "Action", tValue);

            Assert.AreEqual(tValue, tTest);
        }


        [Test]
        public void TestMethodStaticGetValue()
        {
        

            var tValue = 1;

            var tOut = Impromptu.InvokeMember(tValue, "ToString");

            Assert.AreEqual(tValue.ToString(), tOut);
        }

        [Test]
        public void TestMethodStaticPassAndGetValue()
        {


            var tValue = "Test";

            var tParam ="Te";

            HelpTestStaticPassAndGetValue("Test", "Te");


            HelpTestStaticPassAndGetValue("Test", "st");
        }

        private void HelpTestStaticPassAndGetValue(string tValue, string tParam)
        {
            var tExpected = tValue.StartsWith(tParam);

            var tOut = Impromptu.InvokeMember(tValue, "StartsWith", tParam);

            Assert.AreEqual(tExpected, tOut);
        }


        [Test]
        public void TestGetDynamic()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = tSetValue;



            var tOut = Impromptu.InvokeGet(tExpando, "Test");

            Assert.AreEqual(tSetValue, tOut);

        }
    }
}
