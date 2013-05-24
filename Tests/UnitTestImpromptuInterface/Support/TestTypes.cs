using System;
using System.ComponentModel.Composition;
using ImpromptuInterface.Dynamic;
using ImpromptuInterface.MVVM;

namespace UnitTestImpromptuInterface
{
    /// <summary>
    /// Eventually should have the same for Silverlight and WPF here
    /// - WPF fails with STA issue with Control
    /// - Silverlight fails with Mimic b/c it requires a cast to UIElement
    /// </summary>
    [View("Test")]
#if SILVERLIGHT
    public sealed class TestView : System.Windows.Controls.Control
#else
    public sealed class TestView : Mimic
#endif
    {
        public void Show()
        {
            //Do nothing
        }
    }

    [ViewModel("Test")]
    public sealed class TestViewModel : ImpromptuViewModel
    {
        
    }

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
