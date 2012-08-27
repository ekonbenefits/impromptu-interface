Option Strict Off

Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports NUnit.Framework
Imports ImpromptuInterface
Imports ImpromptuInterface.Dynamic
Imports ImpromptuInterface.InvokeExt
Imports Microsoft.CSharp.RuntimeBinder
Imports UnitTestSupportLibrary


Namespace VBNET

    <TestFixture()> _
    Public Class PrivateTest
        Inherits Helper
        <Test()> _
        Public Sub TestExposePrivateMethod()
            Dim tTest As TestWithPrivateMethod = New TestWithPrivateMethod()
            Dim tExposed = tTest.ActLike(Of IExposePrivateMethod)()
            Assert.AreEqual(3, tExposed.Test())
        End Sub

        <Test()> _
        Public Sub TestDoNotExposePrivateMethod()
            Dim tTest As TestWithPrivateMethod = New TestWithPrivateMethod()
            Dim tNonExposed = tTest.WithContext(Me).ActLike(Of IExposePrivateMethod)()
            AssertException(Of RuntimeBinderException)(Function() tNonExposed.Test())
        End Sub

        <Test()> _
        Public Sub TestInvokePrivateMethod()
            Dim tTest = New TestWithPrivateMethod()
            Assert.AreEqual(3, Impromptu.InvokeMember(tTest, "Test"))
        End Sub

        <Test()> _
        Public Sub TestInvokePrivateMethodAcrossAssemblyBoundries()
            Dim tTest = New PublicType()
            Assert.AreEqual(True, Impromptu.InvokeMember(tTest, "PrivateMethod", 3))
        End Sub

        <Test()> _
        Public Sub TestInvokeInternalTypeMethodAcrossAssemblyBoundries()
            Dim tTest = PublicType.InternalInstance
            Assert.AreEqual(True, Impromptu.InvokeMember(tTest, "InternalMethod", 3))
        End Sub

        <Test()> _
        Public Sub TestInvokeDoNotExposePrivateMethod()
            Dim tTest As TestWithPrivateMethod = New TestWithPrivateMethod()
            AssertException(Of RuntimeBinderException)(Function() Impromptu.InvokeMember(tTest.WithContext(Me), "Test"))
        End Sub

        <Test()> _
        Public Sub TestCacheableDoNotExposePrivateMethod()
            Dim tTest = New TestWithPrivateMethod()
            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.InvokeMember, "Test")
            AssertException(Of RuntimeBinderException)(Function() tCachedInvoke.Invoke(tTest))
        End Sub

        <Test()> _
        Public Sub TestCacheableExposePrivateMethodViaInstance()
            Dim tTest = New TestWithPrivateMethod()
            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.InvokeMember, "Test", context:=tTest)
            Assert.AreEqual(3, tCachedInvoke.Invoke(tTest))
        End Sub

        <Test()> _
        Public Sub TestCacheableExposePrivateMethodViaType()
            Dim tTest = New TestWithPrivateMethod()
            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.InvokeMember, "Test", context:=GetType(TestWithPrivateMethod))
            Assert.AreEqual(3, tCachedInvoke.Invoke(tTest))
        End Sub
    End Class

    Public Class TestWithPrivateMethod
        Private Function Test() As Integer
            Return 3
        End Function
    End Class


    Public Interface IExposePrivateMethod
        Function Test() As Integer
    End Interface
End Namespace
