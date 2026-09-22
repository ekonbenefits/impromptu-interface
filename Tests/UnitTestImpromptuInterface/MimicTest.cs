using System;
using Dynamitey.DynamicObjects;
#if !SELFRUNNER
using AnyUnit.Run;
using AnyUnit.Style.Nunit;
using AnyUnit.Constraints;
using AnyUnit.Constraints.Pieces;
#endif


#if SILVERLIGHT
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{
    /// <summary>
    /// This is the craziest set of tests I've ever written in my life...
    /// </summary>
    [TestFixture]
    public class MimicTest : Helper
    {
        private class SubMimic : Mimic
        {
            public int Add(int x, int y)
            {
                return x + y;
            }

            public string Add(string x, string y)
            {
                return x + y;
            }
        }

        [Test]
        public void Get_Property()
        {
            dynamic mimic = new Mimic();
            dynamic result = mimic.I.Can.Get.Any.Property.I.Want.And.It.Wont.Blow.Up;
            Assert.IsInstanceOf<Mimic>(result);          // every member is another Mimic
        }

        [Test]
        public void Set_Property()
        {
            dynamic mimic = new Mimic();
            dynamic result = mimic.I.Can.Set.Any.Property.I.Want.And.It.Wont.Blow = "Up";
            Assert.AreEqual("Up", result);               // an assignment's value is what was assigned
        }

        [Test]
        public void Call_Method()
        {
            dynamic mimic = new Mimic();
            dynamic result = mimic.I.Can.Call.Any.Method.I.Want.And.It.Wont.Blow.Up();
            Assert.IsInstanceOf<Mimic>(result);
        }

        [Test]
        public void Call_Method_With_Parameters()
        {
            dynamic mimic = new Mimic();
            dynamic result = mimic.I().Can().Call().Any().Method().I().Want().And().It().Wont().Blow().Up("And", "Any", "Parameter", "I", "Want", 1, 2, 3, 44.99m);
            Assert.IsInstanceOf<Mimic>(result);
        }

        [Test]
        public void Get_Index()
        {
            dynamic mimic = new Mimic();
            dynamic result = mimic["I"]["Can"]["Get"]["Indexes"]["All"]["Day"]["Like"]["It"]["Aint"]["No"]["Thang"];
            Assert.IsInstanceOf<Mimic>(result);
        }

        [Test]
        public void Set_Index()
        {
            dynamic mimic = new Mimic();
            Assert.DoesNotThrow(() => mimic["I"]["Can"]["Set"]["Indexes"]["All"]["Day"]["Like"]["It"]["Aint"]["No"] = "Thang");
        }

        [Test]
        public void Cast()
        {
            dynamic mimic = new Mimic();

            int Int32 = mimic;
            double Double = mimic;
            float Float = mimic;
            object Object = mimic;
            Guid Guid = mimic;
            DateTime DateTime = mimic;

            // A conversion gives the target type's default-constructed value.
            Assert.AreEqual(0, Int32);
            Assert.AreEqual(0d, Double);
            Assert.AreEqual(0f, Float);
            Assert.IsNotNull(Object);
            Assert.AreEqual(Guid.Empty, Guid);
            Assert.AreEqual(default(DateTime), DateTime);
        }

        [Test]
        public void Unary()
        {
            dynamic mimic = new Mimic();
            dynamic result;

            result = !mimic;
            result = ++mimic;
            result = --mimic;
            result = mimic++;
            result = mimic--;
            result = mimic += 1;
            result = mimic -= 1;
            result = mimic /= 2;
            result = mimic *= 4;
            result = mimic ^= true;
            result = mimic |= true;
            result = mimic &= false;
            result = mimic %= 5;
            Assert.IsInstanceOf<Mimic>(result);          // an operator on a Mimic is a Mimic
        }

        [Test]
        public void Binary()
        {
            dynamic thing1 = new Mimic();
            dynamic thing2 = new Mimic();
            dynamic result;

            result = thing1 + thing2;
            result = thing1 - thing2;
            result = thing1 / thing2;
            result = thing1 * thing2;
            result = thing1 | thing2;
            result = thing1 & thing2;
            result = thing1 ^ thing2;
            result = thing1 % thing2;
            Assert.IsInstanceOf<Mimic>(result);
        }

        [Test]
        public void Inheritance_Int()
        {
            dynamic mimic = new SubMimic();
            int result = mimic.Add(2, 2);
            Assert.AreEqual(4, result);
        }

        [Test]
        public void Inheritance_String()
        {
            dynamic mimic = new SubMimic();
            string result = mimic.Add("He", "llo");
            Assert.AreEqual("Hello", result);
        }

        [Test]
        public void Inheritance_No_Match()
        {
            dynamic mimic = new SubMimic();
            int result = mimic.Add(1, "llo");
            Assert.AreEqual(default(int), result);
        }
    }
}
