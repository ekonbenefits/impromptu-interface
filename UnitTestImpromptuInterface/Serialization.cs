using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ImpromptuInterface;

#if SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#elif !SELFRUNNER
using NUnit.Framework;
#endif

namespace UnitTestImpromptuInterface
{

    [TestClass, TestFixture]
    public class Serialization : Helper
    {
        [Test]
        public void TestRoundTripSerial()
        {

            var value = new PropPoco() {Prop1 = "POne", Prop2 = 45L, Prop3 = Guid.NewGuid()}.ActLike<ISimpeleClassProps>();

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, value);
                stream.Seek(0, SeekOrigin.Begin);
                var tDeValue = (ISimpeleClassProps)formatter.Deserialize(stream);

                Assert.AreEqual(value.Prop1, tDeValue.Prop1);
                Assert.AreEqual(value.Prop2, tDeValue.Prop2);
                Assert.AreEqual(value.Prop3, tDeValue.Prop3);

            }
        }
    }
}
