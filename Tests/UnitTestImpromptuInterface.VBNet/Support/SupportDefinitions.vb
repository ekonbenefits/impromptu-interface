Option Strict Off
' 
'  Copyright 2010  Ekon Benefits
' 
'    Licensed under the Apache License, Version 2.0 (the "License");
'    you may not use this file except in compliance with the License.
'    You may obtain a copy of the License at
' 
'        http://www.apache.org/licenses/LICENSE-2.0
' 
'    Unless required by applicable law or agreed to in writing, software
'    distributed under the License is distributed on an "AS IS" BASIS,
'    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'    See the License for the specific language governing permissions and
'    limitations under the License.

Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Diagnostics
Imports ImpromptuInterface.Dynamic
Imports System.Windows.Media

Namespace VBNET




    Public Class TestForwarder
        Inherits ImpromptuForwarder
        Public Sub New(target As Object)
            MyBase.New(target)
        End Sub
    End Class

    Public Interface IDynamicArg
        Function ReturnIt(arg As Object) As Object

        Function Params(ParamArray args As Object()) As Boolean
    End Interface

    Public Class PocoNonDynamicArg
        Public Function ReturnIt(i As Integer) As Integer
            Return i
        End Function


        Public Function ReturnIt(i As List(Of String)) As List(Of String)
            Return i
        End Function

        Public Function Params(fallback As Object) As Boolean
            Return False
        End Function

        Public Function Params(ParamArray args As Integer()) As Boolean
            Return True
        End Function
    End Class

    Public NotInheritable Class StaticType
        Private Sub New()
        End Sub
        Public Shared Function Create(Of TReturn)(type As Integer) As TReturn
            Return Nothing
        End Function

        Public Shared ReadOnly Property Test() As Boolean
            Get
                Return True
            End Get
        End Property

        Public Shared Property TestSet() As Integer
            Get
                Return m_TestSet
            End Get
            Set(value As Integer)
                m_TestSet = value
            End Set
        End Property
        Private Shared m_TestSet As Integer
    End Class

    Public Interface ISimpeleClassProps
        ReadOnly Property Prop1() As String

        ReadOnly Property Prop2() As Long

        ReadOnly Property Prop3() As Guid
    End Interface

    Public Interface IPropPocoProp
        Property ReturnProp() As PropPoco
    End Interface

    Public Interface IEvent
        Event [Event] As EventHandler(Of EventArgs)
        Sub OnEvent(obj As Object, args As EventArgs)
    End Interface

    Public Class PocoEvent
        Public Event [Event] As EventHandler(Of EventArgs)

        Public Sub OnEvent(obj As Object, args As EventArgs)
            RaiseEvent [Event](obj, args)
        End Sub
    End Class


    Public Class PocoOptConstructor
        Public Property One() As String
            Get
                Return m_One
            End Get
            Set(value As String)
                m_One = value
            End Set
        End Property
        Private m_One As String
        Public Property Two() As String
            Get
                Return m_Two
            End Get
            Set(value As String)
                m_Two = value
            End Set
        End Property
        Private m_Two As String
        Public Property Three() As String
            Get
                Return m_Three
            End Get
            Set(value As String)
                m_Three = value
            End Set
        End Property
        Private m_Three As String

        Public Sub New(Optional one__1 As String = "-1", Optional two__2 As String = "-2", Optional three__3 As String = "-3")
            One = one__1
            Two = two__2
            Three = three__3
        End Sub
    End Class

    Public Interface IDynamicDict
        ReadOnly Property Test1() As Integer

        ReadOnly Property Test2() As Long

        ReadOnly Property TestD() As Object
    End Interface

    Public Interface INonDynamicDict
        ReadOnly Property Test1() As Integer

        ReadOnly Property Test2() As Long

        ReadOnly Property TestD() As IDictionary(Of String, Object)
    End Interface

    Public Interface ISimpleStringProperty
        ReadOnly Property Length() As Integer

    End Interface

    Public Interface IRobot
        ReadOnly Property Name() As String
    End Interface
    Public Class Robot
        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_Name As String
    End Class

    Public Interface ISimpleStringMethod
        Function StartsWith(value As String) As Boolean

    End Interface

    Public Interface ISimpeleClassMeth
        Sub Action1()
        Sub Action2(value As Boolean)
        Function Action3() As String
    End Interface

    Public Interface ISimpeleClassMeth2
        Inherits ISimpeleClassMeth

        Function Action4(arg As Integer) As String
    End Interface

    Public Interface IGenericMeth
        Function Action(Of T)(arg As T) As String

        Function Action2(Of T)(arg As T) As T
    End Interface

    Public Interface IStringIntIndexer
        Default Property Item(index As Integer) As String
    End Interface

    Public Interface IObjectStringIndexer
        Default Property Item(index As String) As Object
    End Interface

    Public Interface IGenericMethWithConstraints
        Function Action(Of T As Class)(arg As T) As String
        Function Action2(Of T As IComparable)(arg As T) As String
    End Interface

    Public Interface IGenericType(Of T)
        Function Funct(arg As T) As String


    End Interface

    Public Interface IGenericTypeConstraints(Of T As Class)
        Function Funct(arg As T) As String

    End Interface


    Public Interface IOverloadingMethod
        Function Func(arg As Integer) As String

        Function Func(arg As Object) As String
    End Interface

    <Serializable()> _
    Public Class PropPoco
        Public Property Prop1() As String
            Get
                Return m_Prop1
            End Get
            Set(value As String)
                m_Prop1 = value
            End Set
        End Property
        Private m_Prop1 As String

        Public Property Prop2() As Long
            Get
                Return m_Prop2
            End Get
            Set(value As Long)
                m_Prop2 = value
            End Set
        End Property
        Private m_Prop2 As Long

        Public Property Prop3() As Guid
            Get
                Return m_Prop3
            End Get
            Set(value As Guid)
                m_Prop3 = value
            End Set
        End Property
        Private m_Prop3 As Guid

        Public Property [Event]() As Integer
            Get
                Return m_Event
            End Get
            Set(value As Integer)
                m_Event = value
            End Set
        End Property
        Private m_Event As Integer
    End Class


    Public Interface IVoidMethod
        Sub Action()
    End Interface

    Public Class VoidMethodPoco
        Public Sub Action()
            Console.WriteLine("VoidFunc")
        End Sub
    End Class



    ''' <summary>
    ''' Dynamic Delegates need to return object or void, first parameter should be a CallSite, second object, followed by the expected arguments
    ''' </summary>
    Public Delegate Function DynamicTryString(callsite As CallSite, target As Object, ByRef result As String) As Object

    Public Class MethOutPoco
        Public Function Func(ByRef result As String) As Boolean
            result = "success"
            Return True
        End Function



    End Class
    Public Class GenericMethOutPoco
        Public Function Func(Of T)(ByRef result As T) As Boolean
            result = Nothing
            Return True
        End Function
    End Class

    Public Interface IGenericMethodOut
        Function Func(Of T)(ByRef result As T) As Boolean
    End Interface

    Public Interface IMethodOut2
        Function Func(ByRef result As Integer) As Boolean
    End Interface


    Public Interface IMethodOut
        Function Func(ByRef result As String) As Boolean
    End Interface

    Public Class MethRefPoco
        Public Function Func(ByRef result As Integer) As Boolean
            result = result + 2
            Return True
        End Function

    End Class

    Public Interface IMethodRef
        Function Func(ByRef result As Integer) As Boolean
    End Interface

    Public Interface IBuilder
        Function Nester(props As Object) As INest
        Function Nester2(props As Object) As INested

        <UseNamedArgument()> _
        Function Nester(NameLevel1 As String, Nested As INested) As INest

        Function Nester2(<UseNamedArgument()> NameLevel2 As String) As INested
    End Interface

    Public Interface INest
        Property NameLevel1() As [String]
        Property Nested() As INested
    End Interface

    Public Interface INested
        ReadOnly Property NameLevel2() As String
    End Interface


    Public Interface IColor
        ReadOnly Property ColorViaString() As Color
    End Interface

End Namespace