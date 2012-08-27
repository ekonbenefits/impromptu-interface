Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports NUnit.Framework

Namespace VBNET

    Public Class WriteLineContext
        Public Sub WriteLine(format As String, ParamArray args As Object())
            Console.WriteLine(format, args)
        End Sub
    End Class

    Public Class Helper
        Inherits AssertionHelper
        Public ReadOnly Property TestContext() As WriteLineContext
            Get
                Return New WriteLineContext()
            End Get
        End Property

        Public Sub AssertException(Of T As Exception)(action As TestDelegate)
            Assert.Throws(Of T)(action)
        End Sub
    End Class
End Namespace
