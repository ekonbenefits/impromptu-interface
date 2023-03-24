using ImpromptuInterface;
using NUnit.Framework;
using System;
using System.Reflection;

namespace UnitTestImpromptuInterface
{
    #region From unreferenced assembly - loaded dynamically

    public enum Status
    {
        Started,
        Finished
    }

    public enum SimulationType
    {
        Ss, Ts, Th
    }

    public interface ISimulator
    {
        void RunSimulation(SimulationType simulationType);
        Status Status { get; }
    }

    public class Simulator : ISimulator
    {
        public Simulator()
        {
            Status = Status.Started;
        }

        public void RunSimulation(SimulationType simulationType)
        {
            switch (simulationType)
            {
                case SimulationType.Ss:
                    Status = Status.Finished;
                    break;
                case SimulationType.Ts:
                case SimulationType.Th:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(simulationType), simulationType, null);
            }
        }

        public Status Status { get; private set; }
    }

    #endregion

    public interface ISim
    {
        void RunSimulation(int simulationType); // desired...
        void RunSimulation(dynamic simulationType); // required... to avoid BinderException
        object Status { get; } // returns object to avoid BinderException
    }

    public static class AssemblyExtensions
    {
        public static object GetEnumValue(this Assembly assemblyWithEnum, string enumTypeName, int value)
        {
            var enumType = assemblyWithEnum.GetType(enumTypeName);
            if (!(enumType.IsEnum))
                throw new ArgumentException($"{enumTypeName} is not an Enum");
            return Enum.Parse(enumType, value.ToString());
        }
    }
    
    [TestFixture]
    public class EnumTest
    {
        [Test]
        public void Can_use_return_of_enum_with_hard_cast()
        {
            var sim = new Simulator().ActLike<ISim>();
            
            Assert.AreEqual(0, (int)sim.Status);
        }

        [Test]
        [Explicit]
        public void Can_use_int_as_enum_parameter_so_enum_need_not_be_referenced()
        {
            var sim = new Simulator().ActLike<ISim>();

            sim.RunSimulation(0); // BinderException

            Assert.AreEqual(1, (int)sim.Status);
        }

        [Test]
        public void Can_dynamically_convert_int_to_enum_so_enum_assembly_need_not_be_referenced()
        {
            var enumValue = Assembly.GetExecutingAssembly().GetEnumValue("UnitTestImpromptuInterface.SimulationType", 0);

            var sim = new Simulator().ActLike<ISim>();

            sim.RunSimulation(enumValue);

            Assert.AreEqual(1, (int)sim.Status);
        }
    }
}