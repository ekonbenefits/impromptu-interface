Option Strict Off

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text
Imports NUnit.Framework
Imports ImpromptuInterface
Imports ImpromptuInterface.Dynamic


Namespace VBNET

    <TestFixture()> _
    Public Class Serialization
        Inherits Helper
        <Test()> _
        Public Sub TestRoundTripSerial()

            Dim value As ISimpeleClassProps = New PropPoco() With { _
              .Prop1 = "POne", _
              .Prop2 = 45L, _
              .Prop3 = Guid.NewGuid() _
            }.ActLike(Of ISimpeleClassProps)()

            Dim tDeValue = SerialAndDeSerialize(value)
            Assert.AreEqual(value.Prop1, tDeValue.Prop1)
            Assert.AreEqual(value.Prop2, tDeValue.Prop2)
            Assert.AreEqual(value.Prop3, tDeValue.Prop3)
        End Sub

        <Test()> _
        Public Sub TestSerializeDictionary()
            Dim [New] = Builder.[New](Of ImpromptuDictionary)()

            Dim tObj = [New].Test(Test:="One", Second:="Two")
            Dim tDeObj = SerialAndDeSerialize(tObj)

            Assert.AreEqual(tObj.Test, tDeObj.Test)
            Assert.AreEqual(tObj.Second, tDeObj.Second)
        End Sub

        <Test()> _
        Public Sub TestSerializeDictionaryWithInterface()
            Dim [New] = Builder.[New](Of ImpromptuDictionary)()

            Dim value As ISimpeleClassProps = [New].Test(Prop1:="POne", Prop2:=45L, Prop3:=Guid.NewGuid()).ActLike(Of ISimpeleClassProps)()


            Dim tDeValue = SerialAndDeSerialize(value)

            Assert.AreEqual(value.Prop1, tDeValue.Prop1)
            Assert.AreEqual(value.Prop2, tDeValue.Prop2)
            Assert.AreEqual(value.Prop3, tDeValue.Prop3)
        End Sub

        <Test()> _
        Public Sub TestSerializeChainableDictionaryWithInterface()
            Dim [New] = Builder.[New]()

            Dim value As ISimpeleClassProps = [New].Test(Prop1:="POne", Prop3:=Guid.NewGuid()).ActLike(Of ISimpeleClassProps)()


            Dim tDeValue = SerialAndDeSerialize(value)

            Assert.AreEqual(value.Prop1, tDeValue.Prop1)
            Assert.AreEqual(value.Prop2, tDeValue.Prop2)
            Assert.AreEqual(value.Prop3, tDeValue.Prop3)
        End Sub

        <Test()> _
        Public Sub TestSerializeChainableDictionaryWithList()
            Dim [New] = Builder.[New]()

            Dim value = [New].Test(Prop1:="POne", Prop2:=45L, Prop3:=Guid.NewGuid(), More:=[New].List("Test1", "Test2", "Test3"))
            Dim tDeValue = SerialAndDeSerialize(value)

            Assert.AreEqual(value.Prop1, tDeValue.Prop1)
            Assert.AreEqual(value.Prop2, tDeValue.Prop2)
            Assert.AreEqual(value.Prop3, tDeValue.Prop3)
            Assert.AreEqual(value.More(2), tDeValue.More(2))
        End Sub


        Private Function SerialAndDeSerialize(Of T)(value As T) As T
            Using stream = New MemoryStream()
                Dim formatter = New BinaryFormatter()
                formatter.Serialize(stream, value)
                stream.Seek(0, SeekOrigin.Begin)
                Return DirectCast(formatter.Deserialize(stream), T)
            End Using
        End Function
    End Class
End Namespace