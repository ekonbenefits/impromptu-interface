Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Windows.Media
Imports NUnit.Framework
Imports ImpromptuInterface
Imports ImpromptuInterface.Dynamic
Imports System.Windows
Imports System.Windows.Controls



Imports ImpromptuInterface.MVVM


Namespace VBNET

    <TestFixture()> _
    Public Class MVVM
        Inherits Helper

        <Test()> _
        Public Sub TestToStringProxy()
            Dim tProxy As Object = New With { _
       Key .Test1 = "One", _
       Key .Test2 = "Two", _
       Key .TestAgain = "Again" _
      }.ProxyToString(Function(it) String.Format("{0}:{1}", it.Test1, it.TestAgain))

            Assert.AreEqual("One:Again", tProxy.ToString())
        End Sub

        <Test()> _
        Public Sub TestResultToStringProxyDictionary()

            Dim tTarget As Object = New With { _
                Key .Test1 = "One", _
                Key .Test2 = 5.24, _
                Key .TestAgain = 5, _
                Key .TestFinal = New List(Of String)() _
            }

            Dim tProxy As Object = New ImpromptuResultToString(tTarget) From { _
       New Func(Of Integer, String)(Function(it) "int"), _
       New Func(Of Double, String)(Function(it) "double"), _
       New Func(Of String, String)(Function(it) "string"), _
       New Func(Of IList(Of String), String)(Function(it) "List") _
      }

            Assert.AreEqual("string", tProxy.Test1.ToString())
            Assert.AreEqual("double", tProxy.Test2.ToString())
            Assert.AreEqual("int", tProxy.TestAgain.ToString())
            Assert.AreEqual("List", tProxy.TestFinal.ToString())
        End Sub


        <Test()> _
        Public Sub TestToStringProxyCall()
            Dim tAnon As PropPoco = New PropPoco()
            tAnon.Prop1 = "Prop1"
            tAnon.Prop1 = "Prop2"

            Dim tProxy As Object = tAnon.ProxyToString(Function(it) String.Format("{0}:{1}", it.Prop1, it.Prop2))


            Assert.AreEqual(tAnon.Prop2, tProxy.Prop2)
        End Sub














        Private Function ImplicitCast(value As PropPoco) As PropPoco
            Return value
        End Function
        Private Function ImplicitCastDynamic(value As Object) As PropPoco
            Return value
        End Function

        Friend Class TestDependency
            Inherits DependencyObject


            Public Event TextChange As TextChangedEventHandler


            Public Sub OnTextChanged(sender As Object, e As TextChangedEventArgs)
                RaiseEvent TextChange(sender, e)
            End Sub


            Public Event TextChange2 As EventHandler(Of TextChangedEventArgs)


            Public Sub OnTextChanged2(sender As Object, e As TextChangedEventArgs)
                RaiseEvent TextChange2(sender, e)
            End Sub
        End Class






        <Test()> _
        Public Sub TestEventBindingNonGenericType()
            Dim tRun = False
            Dim tTextBox = New TestDependency()
            Dim tViewModel = Build(Of ImpromptuViewModel).NewObject(TestEvent:=New Action(Of Object, EventArgs)(Function(sender, e) InlineAssignHelper(tRun, True)))


            [Event].SetBind(tTextBox, tViewModel.Events.TextChange.[To]("TestEvent"))

            tTextBox.OnTextChanged(tTextBox, Nothing)

            Assert.AreEqual(True, tRun)
        End Sub

        <Test()> _
        Public Sub TestEventBindingGenericType()
            Dim tRun = False
            Dim tTextBox = New TestDependency()
            Dim tViewModel = Build(Of ImpromptuViewModel).NewObject(TestEvent:=New Action(Of Object, EventArgs)(Function(sender, e) InlineAssignHelper(tRun, True)))


            [Event].SetBind(tTextBox, tViewModel.Events.TextChange2.[To]("TestEvent"))

            tTextBox.OnTextChanged2(tTextBox, Nothing)

            Assert.AreEqual(True, tRun)
        End Sub

        <Test()> _
        Public Sub TestTypeConverter()
            Dim tNew As Object = Build.NewObject(ColorViaString:="Blue")
            Dim tColorHolder = tNew.ActLike(Of IColor)()

            Assert.AreEqual(Colors.Blue, tColorHolder.ColorViaString)


        End Sub


        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function


    End Class

End Namespace