using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;


#if !SELFRUNNER
using NUnit.Framework;
#endif

#if SILVERLIGHT
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{

    [TestFixture]
    public class Serialization : Helper
    {
        [Test]
        public void TestRoundTripSerial()
        {

            ISimpeleClassProps value = new PropPoco() { Prop1 = "POne", Prop2 = 45L, Prop3 = Guid.NewGuid() }.ActLike<ISimpeleClassProps>();

            var tDeValue = SerialAndDeSerialize(value);
            Assert.AreEqual(value.Prop1, tDeValue.Prop1);
            Assert.AreEqual(value.Prop2, tDeValue.Prop2);
            Assert.AreEqual(value.Prop3, tDeValue.Prop3);
        }

        [Test]
        public void TestSerializeDictionary()
        {
            var New = Builder.New<ImpromptuDictionary>();

            var tObj =New.Object(Test: "One", Second: "Two");
            var tDeObj =SerialAndDeSerialize(tObj);

            Assert.AreEqual(tObj.Test, tDeObj.Test);
            Assert.AreEqual(tObj.Second, tDeObj.Second);
        }
           
        [Test]
        public void TestSerializeDictionaryWithInterface()
        {
            var New = Builder.New<ImpromptuDictionary>();

            ISimpeleClassProps value = New.Object(Prop1: "POne", Prop2: 45L, Prop3: Guid.NewGuid()).ActLike<ISimpeleClassProps>(); ;
            var tDeValue = SerialAndDeSerialize(value);

            Assert.AreEqual(value.Prop1, tDeValue.Prop1);
            Assert.AreEqual(value.Prop2, tDeValue.Prop2);
            Assert.AreEqual(value.Prop3, tDeValue.Prop3);
        }

        [Test]
        public void TestSerializeChainableDictionaryWithInterface()
        {
            var New = Builder.New();

            ISimpeleClassProps value = New.Object(Prop1: "POne", Prop3: Guid.NewGuid()).ActLike<ISimpeleClassProps>(); ;
            var tDeValue = SerialAndDeSerialize(value);

            Assert.AreEqual(value.Prop1, tDeValue.Prop1);
            Assert.AreEqual(value.Prop2, tDeValue.Prop2);
            Assert.AreEqual(value.Prop3, tDeValue.Prop3);
        }

        [Test]
        public void TestSerializeChainableDictionaryWithList()
        {
            var New = Builder.New();

            var value = New.Object(Prop1: "POne", Prop2: 45L, Prop3: Guid.NewGuid(), More: New.List("Test1","Test2","Test3"));
            var tDeValue = SerialAndDeSerialize(value);

            Assert.AreEqual(value.Prop1, tDeValue.Prop1);
            Assert.AreEqual(value.Prop2, tDeValue.Prop2);
            Assert.AreEqual(value.Prop3, tDeValue.Prop3);
            Assert.AreEqual(value.More[2], tDeValue.More[2]);
        }


        private T SerialAndDeSerialize<T>(T value)
        {
             using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, value);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
