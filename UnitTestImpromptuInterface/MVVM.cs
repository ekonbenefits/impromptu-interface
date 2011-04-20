using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using System.Windows;
using System.Windows.Controls;

#if SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#elif !SELFRUNNER
using NUnit.Framework;
#endif

using ImpromptuInterface.MVVM;

namespace UnitTestImpromptuInterface
{

       
    [TestClass,TestFixture]
	public class MVVM:Helper
	{

         [Test,TestMethod]
        public void TestToStringProxy()
         {
             dynamic tProxy =
                 new {Test1 = "One", Test2 = "Two", TestAgain = "Again"}.ProxyToString(
                     it => string.Format("{0}:{1}", it.Test1, it.TestAgain));

             Assert.AreEqual("One:Again",tProxy.ToString());
         }

         [Test, TestMethod]
         public void TestToStringProxyCall()
         {
             var tAnon = new PropPoco() { Prop1 = "A", Prop2 = 1 };

             dynamic tProxy = tAnon.ProxyToString(
                     it => string.Format("{0}:{1}", it.Prop1, it.Prop2));


             Assert.AreEqual(tAnon.Prop2, tProxy.Prop2);
         }


         [Test,TestMethod]
        public void TestToStringProxyConvertExplicit()
         {
             var tAnon = new {Test1 = "One", Test2 = "Two", TestAgain = "Again"};

             dynamic tProxy = tAnon.ProxyToString(
                     it => string.Format("{0}:{1}", it.Test1, it.TestAgain));

             var tAnon2 = ExplictAnonCast(tAnon, tProxy);
             Assert.AreEqual(tAnon, tAnon2);
         }



         [Test, TestMethod]
         public void TestToStringProxyConvertImplicit()
         {
             var tAnon = new PropPoco(){Prop1 = "A", Prop2 = 1};

             dynamic tProxy = tAnon.ProxyToString(
                     it => string.Format("{0}:{1}", it.Prop1, it.Prop2));

             var tAnon2 = ImplicitCast(tProxy);
             Assert.AreEqual(tAnon.GetType(), tAnon2.GetType());
         }

         [Test, TestMethod]
         public void TestToStringProxyConvertImplicit2()
         {
             var tAnon = new PropPoco() { Prop1 = "A", Prop2 = 1 };

             dynamic tProxy = tAnon.ProxyToString(
                     it => string.Format("{0}:{1}", it.Prop1, it.Prop2));

             var tAnon2 = ImplicitCastDynamic(tProxy);
             Assert.AreEqual(tAnon.GetType(), tAnon2.GetType());
         }


         [Test, TestMethod]
         public void TestToStringProxyDictionaryMassage()
         {
             var tPropPoco = new PropPoco() {Prop1 = "A"};

             dynamic tData = Builder.New().Object(ReturnProp: tPropPoco.ProxyToString(it=>"Print Me"));

             IPropPocoProp tInter =Impromptu.ActLike<IPropPocoProp>(tData);

             Assert.AreEqual(tPropPoco.GetType(), tInter.ReturnProp.GetType());
         }

        private T ExplictAnonCast<T>(T dummy, dynamic value)
        {
            return (T) value;
        }

        private PropPoco ImplicitCast(PropPoco value)
        {
            return value;
        }
        private PropPoco ImplicitCastDynamic(dynamic value)
        {
            return value;
        }

        internal class TestDependency:DependencyObject
        {


            public event TextChangedEventHandler TextChange;


            public void OnTextChanged(object sender,TextChangedEventArgs e)
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




   

        [Test, TestMethod]
        public void TestEventBindingNonGenericType()
        {
            var tRun = false;
            var tTextBox = new TestDependency();
            var tViewModel = Build<ImpromptuViewModel>.NewObject(TestEvent:new Action<object,EventArgs>((sender,e)=>tRun =true));


            Event.SetBind(tTextBox, tViewModel.Events.TextChange.To["TestEvent"]);

            tTextBox.OnTextChanged(tTextBox, null);

            Assert.AreEqual(true, tRun);
        }

        [Test, TestMethod]
        public void TestEventBindingGenericType()
        {
            var tRun = false;
            var tTextBox = new TestDependency();
            var tViewModel = Build<ImpromptuViewModel>.NewObject(TestEvent: new Action<object, EventArgs>((sender, e) => tRun = true));


            Event.SetBind(tTextBox, tViewModel.Events.TextChange2.To["TestEvent"]);

            tTextBox.OnTextChanged2(tTextBox, null);

            Assert.AreEqual(true, tRun);
        }
    }
}
