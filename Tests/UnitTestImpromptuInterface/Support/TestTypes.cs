using System;
using System.ComponentModel.Composition;
using ImpromptuInterface.Dynamic;
using ImpromptuInterface.MVVM;

namespace UnitTestImpromptuInterface
{
    /// <summary>
    /// Eventually should be `public sealed class TestView : Mimic`
    /// </summary>
    [View("Test")]
    public sealed class TestView : System.Windows.Controls.Control
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
