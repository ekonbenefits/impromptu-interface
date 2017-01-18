Option Strict Off

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Dynamic
Imports System.Linq
Imports System.Runtime.Serialization
Imports System.Text
Imports NUnit.Framework
Imports ImpromptuInterface

Namespace VBNET


    <TestFixture()> _
    Public Class Generics
        Inherits Helper

        Delegate Function Test(Of T)() As String
        
        <Test()> _
        Public Sub TestFunctionWithGenericParameter()
            GenericMethHelper(3, "3")
            GenericMethHelper(4, "4")
            GenericMethHelper(True, "True")
        End Sub
        
        <Test()> _
        Public Sub TestFunctionWithGenericParameterAndReturnValue()
            GenericMethHelper2(3)
            GenericMethHelper2(4)
            GenericMethHelper2(True)
            GenericMethHelper2("test'")
        End Sub

        <Test()> _
        Public Sub TestFunctionWithGenericArgument()
            GenericMethHelper3(Of String)(GetType(String).Name)
        End Sub

        Private Sub GenericMethHelper(Of T)(param As T, expected As String)
            Dim tNew As Object = New ExpandoObject()
            tNew.Action = New Func(Of T, String)(Function(it) it.ToString())
            Dim tActsLike As IGenericMeth = Impromptu.ActLike(Of IGenericMeth)(tNew)

            Assert.AreEqual(expected, tActsLike.Action(param))
        End Sub

        Private Sub GenericMethHelper2(Of T)(param As T)
            Dim tNew As Object = New ExpandoObject()
            tNew.Action2 = New Func(Of T, T)(Function(it) it)
            Dim tActsLike As IGenericMeth = Impromptu.ActLike(Of IGenericMeth)(tNew)

            Assert.AreEqual(param, tActsLike.Action2(param))
        End Sub

        Private Sub GenericMethHelper3(Of T)(expected As String)
            Dim tNew As Object = New ExpandoObject()
            tNew.Action3 = CType(AddressOf Action3Stub(Of T), Test(Of T))
            Dim tActsLike As IGenericMeth = Impromptu.ActLike(Of IGenericMeth)(tNew)

            Assert.AreEqual(expected, tActsLike.Action3(Of T))
        End Sub

        Private Function Action3Stub(Of T)() As String
            Return GetType(T).Name
        End Function

        <Test()> _
        Public Sub TestGenericType()

            GenericHelper(3, "3")
            GenericHelper(4, "4")
            GenericHelper(True, "True")
        End Sub

        Private Sub GenericHelper(Of T)(param As T, expected As String)
            Dim tNew As Object = New ExpandoObject()
            tNew.Funct = New Func(Of T, String)(Function(it) it.ToString())
            Dim tActsLike As IGenericType(Of T) = Impromptu.ActLike(Of IGenericType(Of T))(tNew)

            Assert.AreEqual(expected, tActsLike.Funct(param))
        End Sub

        <Test()> _
        Public Sub TestGenericTypeConstraints()

            Dim tObj = New [Object]()
            GenericHelperConstraints(tObj, tObj.ToString())
        End Sub

        Private Sub GenericHelperConstraints(Of T As Class)(param As T, expected As String)
            Dim tNew As Object = New ExpandoObject()
            tNew.Funct = New Func(Of T, String)(Function(it) it.ToString())
            Dim tActsLike = Impromptu.ActLike(Of IGenericTypeConstraints(Of T))(tNew)

            Assert.AreEqual(expected, tActsLike.Funct(param))
        End Sub



        <Test()> _
        Public Sub TestConstraintsMethGeneric()
            Dim tObj = New [Object]()
            GenericMethConstraintsHelper(tObj, tObj.ToString())

        End Sub

        Private Sub GenericMethConstraintsHelper(Of T As Class)(param As T, expected As String)
            Dim tNew As Object = New ExpandoObject()
            tNew.Action = New Func(Of T, String)(Function(it) it.ToString())
            Dim tActsLike = Impromptu.ActLike(Of IGenericMethWithConstraints)(tNew)

            Assert.AreEqual(expected, tActsLike.Action(param))
        End Sub


    End Class
End Namespace