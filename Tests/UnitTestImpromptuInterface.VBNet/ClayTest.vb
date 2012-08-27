Option Strict Off

Imports System.Collections.Generic
Imports System.Dynamic
Imports System.Linq
Imports System.Text
Imports ClaySharp
Imports ClaySharp.Behaviors
Imports NUnit.Framework
Imports ImpromptuInterface
Imports ImpromptuInterface.Dynamic
Imports ImpromptuInterface.Optimization


Namespace VBNET

    'Test data modified from MS-PL Clay http://clay.codeplex.com
    ''' <summary>
    ''' Testing Integration of Clay with Impromptu-Interface
    ''' </summary>
    <TestFixture()> _
    Public Class ClayTest
        Inherits Helper

        <Test()> _
        Public Sub InvokeMemberContainsNameWithImpromptuInterface()
            Dim clay = New Clay(New TestBehavior()).ActLike(Of ISimpeleClassMeth)()
            Dim result = clay.Action3()
            Assert.IsTrue(result.Contains("[name:Action3]"), "Does Not Match Argument Name")
            Assert.IsTrue(result.Contains("[count:0]"), "Does Not Match Argument Count")

        End Sub

        <Test()> _
        Public Sub InvokeMemberContainsNameWithImpromptuInvoke()
            Dim clay = New Clay(New TestBehavior())
            Dim result = Impromptu.InvokeMember(clay, "Help", "Test")
            Assert.IsTrue(result.Contains("[name:Help]"), "Does Not Match Argument Name")
            Assert.IsTrue(result.Contains("[count:1]"), "Does Not Match Argument Count")

        End Sub

        <Test()> _
        Public Sub TestRecorder()
            Dim [New] As Object = Builder.[New](Of ImpromptuRecorder)()

            Dim tRecording As ImpromptuRecorder = [New].Watson(Test:="One", Test2:=2, NameLast:="Watson")


            Dim tVar As Object = tRecording.ReplayOn(New ExpandoObject())

            Assert.AreEqual("One", tVar.Test)
            Assert.AreEqual(2, tVar.Test2)
            Assert.AreEqual("Watson", tVar.NameLast)
        End Sub







        'TestBehavoir from MS-PL ClaySharp http://clay.codeplex.com
        Private Class TestBehavior
            Inherits ClayBehavior
            Public Overrides Function InvokeMember(proceed As Func(Of Object), self As Object, name As String, args As INamedEnumerable(Of Object)) As Object
                Return String.Format("[name:{0}] [count:{1}]", If(name, "<null>"), args.Count())
            End Function
        End Class
    End Class


End Namespace