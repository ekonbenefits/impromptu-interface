using System;
using System.ComponentModel.Composition;
using ImpromptuInterface;

namespace UnitTestImpromptuInterface
{


    public interface ITestAlias
    {
        [Alias("events_are_ridiculous")]
        event EventHandler EventsAreRidiculous;

        [Alias("手伝ってくれますか？")]
        string CanYouHelpMe(string arg);

        [Alias("★✪The Best Named Property in World!!☮")]
        string URTrippin
        {
            get;
            set;
        }
    }

    public interface ITestInterface
    {
        void Test();
    }

    [Export(typeof(ITestInterface))]
    public sealed class TestClassA : ITestInterface
    {
        public void Test()
        {
            throw new NotImplementedException();
        }
    }

    [Export(typeof(ITestInterface))]
    public sealed class TestClassB : ITestInterface
    {
        public void Test()
        {
            throw new NotImplementedException();
        }
    }

    [Export("Testing123")]
    public sealed class TestClassC : ITestInterface
    {
        public void Test()
        {
            throw new NotImplementedException();
        }
    }

    [Export("Testing123")]
    public sealed class TestClassD : ITestInterface
    {
        public void Test()
        {
            throw new NotImplementedException();
        }
    }
}
