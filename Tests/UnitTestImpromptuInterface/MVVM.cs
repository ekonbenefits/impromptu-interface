using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using System.Windows;
using System.Windows.Controls;


#if !SELFRUNNER
using NUnit.Framework;
#endif

using ImpromptuInterface.MVVM;

#if SILVERLIGHT
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{


    [TestFixture]
    public class MVVM : Helper
    {

        [Test]
        public void TestToStringProxy()
        {
            dynamic tProxy =
                new { Test1 = "One", Test2 = "Two", TestAgain = "Again" }.ProxyToString(
                    it => string.Format("{0}:{1}", it.Test1, it.TestAgain));

            Assert.AreEqual("One:Again", tProxy.ToString());
        }

        [Test]
        public void TestResultToStringProxyDictionary()
        {

            object tTarget =
                new { Test1 = "One", Test2 = 5.24d, TestAgain = 5, TestFinal = new List<string>() };

            dynamic tProxy = new ImpromptuResultToString(tTarget)
                             {
                                 new Func<int, string>(it => "int"),
                                 new Func<double, string>(it => "double"),
                                 new Func<string, string>(it => "string"),
                                  new Func<IList<string>, string>(it => "List"),
                             };

            Assert.AreEqual("string", tProxy.Test1.ToString());
            Assert.AreEqual("double", tProxy.Test2.ToString());
            Assert.AreEqual("int", tProxy.TestAgain.ToString());
            Assert.AreEqual("List", tProxy.TestFinal.ToString());
        }


        [Test]
        public void TestToStringProxyCall()
        {
            var tAnon = new PropPoco() { Prop1 = "A", Prop2 = 1 };

            dynamic tProxy = tAnon.ProxyToString(
                    it => string.Format("{0}:{1}", it.Prop1, it.Prop2));


            Assert.AreEqual(tAnon.Prop2, tProxy.Prop2);
        }


        [Test]
        public void TestToStringProxyConvertExplicit()
        {
            var tAnon = new { Test1 = "One", Test2 = "Two", TestAgain = "Again" };

            dynamic tProxy = tAnon.ProxyToString(
                    it => string.Format("{0}:{1}", it.Test1, it.TestAgain));

            var tAnon2 = ExplictAnonCast(tAnon, tProxy);
            Assert.AreEqual(tAnon, tAnon2);
        }



        [Test]
        public void TestToStringProxyConvertImplicit()
        {
            var tAnon = new PropPoco() { Prop1 = "A", Prop2 = 1 };

            dynamic tProxy = tAnon.ProxyToString(
                    it => string.Format("{0}:{1}", it.Prop1, it.Prop2));

            var tAnon2 = ImplicitCast(tProxy);
            Assert.AreEqual(tAnon.GetType(), tAnon2.GetType());
        }

        [Test]
        public void TestToStringProxyConvertImplicit2()
        {
            var tAnon = new PropPoco() { Prop1 = "A", Prop2 = 1 };

            dynamic tProxy = tAnon.ProxyToString(
                    it => string.Format("{0}:{1}", it.Prop1, it.Prop2));

            var tAnon2 = ImplicitCastDynamic(tProxy);
            Assert.AreEqual(tAnon.GetType(), tAnon2.GetType());
        }





        [Test]
        public void TestToStringProxyDictionaryMassage()
        {
            var tPropPoco = new PropPoco() { Prop1 = "A" };

            dynamic tData = Builder.New().Object(ReturnProp: tPropPoco.ProxyToString(it => "Print Me"));

            IPropPocoProp tInter = Impromptu.ActLike<IPropPocoProp>(tData);

            Assert.AreEqual(tPropPoco.GetType(), tInter.ReturnProp.GetType());
        }



        private T ExplictAnonCast<T>(T dummy, dynamic value)
        {
            return (T)value;
        }

        private PropPoco ImplicitCast(PropPoco value)
        {
            return value;
        }
        private PropPoco ImplicitCastDynamic(dynamic value)
        {
            return value;
        }

        internal class TestDependency : DependencyObject
        {


            public event TextChangedEventHandler TextChange;


            public void OnTextChanged(object sender, TextChangedEventArgs e)
            {
                if (TextChange != null)
                {
                    TextChange(sender, e);
                }
            }


            public event EventHandler<TextChangedEventArgs> TextChange2;


            public void OnTextChanged2(object sender, TextChangedEventArgs e)
            {
                if (TextChange2 != null)
                {
                    TextChange2(sender, e);
                }
            }
        }



        [Test]
        public void TestCommandBindingWithArg()
        {
            var tRun = false;
            var tViewModel = Build<ImpromptuViewModel>.NewObject(TestCommand: new Action<object>((arg) => tRun = true));


            ICommand command = tViewModel.Command.TestCommand;

            command.Execute(null);

            Assert.AreEqual(true, tRun);
        }

        [Test]
        public void TestCommandBindingWithError()
        {
            var tRun = false;
            var tViewModel = Build<ImpromptuViewModel>.NewObject(TestCommand: new Action<object>((arg) =>
                                                                                                                    {
                                                                                                                        throw
                                                                                                                            new Exception
                                                                                                                                ("Test");

                                                                                                                    }));
            Action<Exception> onError = (ex) => tRun = true;
            tViewModel.Setup.CommandErrorHandler += onError;

            ICommand command = tViewModel.Command.TestCommand;

            command.Execute(null);

            Assert.AreEqual(true, tRun);
        }

        [Test]
        public void TestCommandBindingWithNoArg()
        {
            var tRun = false;
            var tViewModel = Build<ImpromptuViewModel>.NewObject(TestCommand: new Action(() => tRun = true));


            ICommand command = tViewModel.Command.TestCommand;

            command.Execute(null);

            Assert.AreEqual(true, tRun);
        }

        [Test]
        public void TestCommandBindingCanProperty()
        {
            var tRun = false;
            var tViewModel = Build<ImpromptuViewModel>.NewObject(TestCommand: new Action(() => tRun = true), CanTestCommand: false);


            ICommand command = tViewModel.Command.TestCommand;

            var tCanCommand = command.CanExecute(null);

            Assert.AreEqual(false, tCanCommand);
        }
        [Test]
        public void TestCommandBindingCanNoArg()
        {
            var tRun = false;
            var tViewModel = Build<ImpromptuViewModel>.NewObject(TestCommand: new Action(() => tRun = true), CanTestCommand: new Func<bool>(() => false));


            ICommand command = tViewModel.Command.TestCommand;

            var tCanCommand = command.CanExecute(null);

            Assert.AreEqual(false, tCanCommand);
        }

        [Test]
        public void TestCommandBindingCanArg()
        {
            var tRun = false;
            var tViewModel = Build<ImpromptuViewModel>.NewObject(TestCommand: new Action(() => tRun = true), CanTestCommand: new Func<object, bool>((param) => false));


            ICommand command = tViewModel.Command.TestCommand;

            var tCanCommand = command.CanExecute(null);

            Assert.AreEqual(false, tCanCommand);
        }

        [Test]
        public void TestEventBinding()
        {
            var tRun = false;
            var tTextBox = new TestDependency();
            var tViewModel = Build<ImpromptuViewModel>.NewObject(TestEvent: new Action<object, EventArgs>((sender, e) => tRun = true));


            Event.SetBind(tTextBox, tViewModel.Events.TextChange.To["TestEvent"]);

            tTextBox.OnTextChanged(tTextBox, null);

            Assert.AreEqual(true, tRun);
        }


        [Test]
        public void TestTypeConverter()
        {
            object tNew = Build.NewObject(ColorViaString: "Blue");
            var tColorHolder = tNew.ActLike<IColor>();

            Assert.AreEqual(Colors.Blue, tColorHolder.ColorViaString);
        }

        [Test]
        public void TestOnChangeDependencyCalls()
        {
            dynamic tNewViewModel = new ImpromptuViewModel();

            tNewViewModel.Prop1 = "Setup";

            tNewViewModel.Setup.Property.Prop2.DependsOn.Prop1();

            int tEvent1Count = 0;
            int tEvent2Count = 0;

            var tEvent1 = ImpromptuViewModel.ChangedHandler((sender, e) => tEvent1Count++);
            Action<object, EventArgs> tEvent2Func = (sender, e) => tEvent2Count++;
            var tEvent2 = new PropertyChangedEventHandler(tEvent2Func);
            var tEvent2Again = new PropertyChangedEventHandler(tEvent2Func);

            tNewViewModel.Setup.Property.Prop1.OnChange += tEvent1;
            tNewViewModel.Setup.Property.Prop1.OnChange += tEvent2;
            tNewViewModel.Setup.Property.Prop2.OnChange += tEvent2Again;


            tNewViewModel.Prop1 = "Run";

            Assert.AreEqual(1, tEvent1Count);

            Assert.AreEqual(1, tEvent2Count);

        }

        
    }
}
