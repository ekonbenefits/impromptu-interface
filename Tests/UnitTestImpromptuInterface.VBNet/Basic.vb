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
Imports System.Runtime.Serialization
Imports NUnit.Framework
Imports ImpromptuInterface.Dynamic
Imports Microsoft.CSharp.RuntimeBinder
Imports ImpromptuInterface
Imports System.Dynamic

Namespace VBNET


    <TestFixture()> _
    Public Class Basic
        Inherits Helper



        <Test()> _
        Public Sub AnonPropertyTest()
            Dim tAnon = New With { _
                Key .Prop1 = "Test", _
                Key .Prop2 = 42L, _
                Key .Prop3 = Guid.NewGuid() _
            }

            Dim tActsLike = Impromptu.ActLike(Of ISimpeleClassProps)(tAnon)


            Assert.AreEqual(tAnon.Prop1, tActsLike.Prop1)
            Assert.AreEqual(tAnon.Prop2, tActsLike.Prop2)
            Assert.AreEqual(tAnon.Prop3, tActsLike.Prop3)
        End Sub





        <Test()> _
        Public Sub CacheTest()
            Dim tAnon = New With { _
                Key .Prop1 = "Test 1", _
                Key .Prop2 = 42L, _
                Key .Prop3 = Guid.NewGuid() _
            }
            Dim tAnon2 = New With { _
                Key .Prop1 = "Test 2", _
                Key .Prop2 = 43L, _
                Key .Prop3 = Guid.NewGuid() _
            }

            Dim tActsLike = Impromptu.ActLike(Of ISimpeleClassProps)(tAnon)
            Dim tActsLike2 = Impromptu.ActLike(Of ISimpeleClassProps)(tAnon2)

            Assert.AreEqual(tActsLike.[GetType](), tActsLike2.[GetType]())

            Assert.AreEqual(tAnon.Prop1, tActsLike.Prop1)
            Assert.AreEqual(tAnon.Prop2, tActsLike.Prop2)
            Assert.AreEqual(tAnon.Prop3, tActsLike.Prop3)

            Assert.AreEqual(tAnon2.Prop1, tActsLike2.Prop1)
            Assert.AreEqual(tAnon2.Prop2, tActsLike2.Prop2)
            Assert.AreEqual(tAnon2.Prop3, tActsLike2.Prop3)

        End Sub

        <Test()> _
        Public Sub AnonEqualsTest()
            Dim tAnon = New With { _
                Key .Prop1 = "Test 1", _
                Key .Prop2 = 42L, _
                Key .Prop3 = Guid.NewGuid() _
            }

            Dim tActsLike = Impromptu.ActLike(Of ISimpeleClassProps)(tAnon)
            Dim tActsLike2 = Impromptu.ActLike(Of ISimpeleClassProps)(tAnon)

            Assert.AreEqual(tActsLike, tActsLike2)


        End Sub

        <Test()> _
        Public Sub ExpandoPropertyTest()

            Dim tNew As Object = New ExpandoObject()
            tNew.Prop1 = "Test"
            tNew.Prop2 = 42L
            tNew.Prop3 = Guid.NewGuid()

            Dim tActsLike As ISimpeleClassProps = Impromptu.ActLike(Of ISimpeleClassProps)(tNew)




            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1)
            Assert.AreEqual(tNew.Prop2, tActsLike.Prop2)
            Assert.AreEqual(tNew.Prop3, tActsLike.Prop3)
        End Sub


        <Test()> _
        Public Sub ImpromptuConversionPropertyTest()

            Dim tNew As Object = New ImpromptuDictionary()
            tNew.Prop1 = "Test"
            tNew.Prop2 = "42"
            tNew.Prop3 = Guid.NewGuid()

            Dim tActsLike = Impromptu.ActLike(Of ISimpeleClassProps)(tNew)




            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1)
            Assert.AreEqual(42L, tActsLike.Prop2)
            Assert.AreEqual(tNew.Prop3, tActsLike.Prop3)
        End Sub


        <Test()> _
        Public Sub DictIndexTest()


            Dim tNew As Object = New ImpromptuDictionary()
            tNew.Prop1 = "Test"
            tNew.Prop2 = "42"
            tNew.Prop3 = Guid.NewGuid()

            Dim tActsLike As IObjectStringIndexer = Impromptu.ActLike(Of IObjectStringIndexer)(tNew)


            Assert.AreEqual(tNew("Prop1"), tActsLike("Prop1"))
        End Sub

        <Test()> _
        Public Sub ArrayIndexTest()


            Dim tNew As String() = New String() {"Test1", "Test2"}

            Dim tActsLike = tNew.ActLike(Of IStringIntIndexer)()




            Assert.AreEqual(tNew(1), tActsLike(1))
        End Sub

        <Test()> _
        Public Sub AnnonMethodsTest()

            Dim tNew = New With { _
                Key .Action1 = New Action(AddressOf Assert.Fail), _
                Key .Action2 = New Action(Of Boolean)(AddressOf Assert.IsFalse), _
                Key .Action3 = New Func(Of String)(Function() "test") _
            }.ActLike(Of ISimpeleClassMeth)()

            Dim tActsLike As ISimpeleClassMeth = tNew



            AssertException(Of AssertionException)(AddressOf tActsLike.Action1)

            Assert.AreEqual("test", tActsLike.Action3())


        End Sub


        <Test()> _
        Public Sub ExpandoMethodsTest()

            Dim tNew As Object = New ExpandoObject()
            tNew.Action1 = New Action(AddressOf Assert.Fail)
            tNew.Action2 = New Action(Of Boolean)(AddressOf Assert.IsFalse)
            tNew.Action3 = New Func(Of String)(Function() "test")

            Dim tActsLike As ISimpeleClassMeth = Impromptu.ActLike(Of ISimpeleClassMeth)(tNew)



            AssertException(Of AssertionException)(AddressOf tActsLike.Action1)


            Assert.AreEqual("test", tActsLike.Action3())


        End Sub





        <Test()> _
        Public Sub StringPropertyTest()
            Dim tAnon As String = "Test 123"
            Dim tActsLike = tAnon.ActLike(Of ISimpleStringProperty)()


            Assert.AreEqual(tAnon.Length, tActsLike.Length)
        End Sub

        <Test()> _
        Public Sub StringMethodTest()
            Dim tAnon As String = "Test 123"
            Dim tActsLike = tAnon.ActLike(Of ISimpleStringMethod)()


            Assert.AreEqual(tAnon.StartsWith("Te"), tActsLike.StartsWith("Te"))
        End Sub



        <Test()> _
        Public Sub DynamicArgMethodTest2()
            Dim tPoco As Object = New PocoNonDynamicArg()
            Dim tActsLike As Object = Impromptu.ActLike(Of IDynamicArg)(tPoco)



            Assert.AreEqual(DynamicArgsHelper(tPoco, New Int32() {1, 2, 3}), tActsLike.Params(1, 2, 3))
            Assert.AreEqual(tPoco.Params("test"), tActsLike.Params("test"))
        End Sub

        Private Function DynamicArgsHelper(obj As Object, ParamArray objects As Object()) As Boolean
            Return obj.Params(objects)
        End Function



        <Test()> _
        Public Sub InformalPropTest()
            Dim tNew As Object = New ExpandoObject()
            tNew.Prop1 = "Test"
            tNew.Prop2 = 42L
            Dim tActsLike = Impromptu.ActLikeProperties(tNew, New Dictionary(Of String, Type)() From { _
                {"Prop1", GetType(String)} _
            })


            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1)
            AssertException(Of MissingMemberException)(Function()
                                                           Dim tTest = tActsLike.Prop2

                                                       End Function)
        End Sub






        <Test()> _
        Public Sub OutMethodTest()
            Dim tPoco As MethOutPoco = New MethOutPoco()
            Dim tActsLike = tPoco.ActLike(Of IMethodOut)()

            Dim tResult As String = [String].Empty

            Dim tOut = tActsLike.Func(tResult)

            Assert.AreEqual(True, tOut)
            Assert.AreEqual("success", tResult)
        End Sub

        <Test()> _
        Public Sub OutMethodTest2()
            Dim tPoco As GenericMethOutPoco = New GenericMethOutPoco()
            Dim tActsLike = tPoco.ActLike(Of IMethodOut)()

            Dim tResult As String = "success"

            Dim tOut = tActsLike.Func(tResult)

            Assert.AreEqual(True, tOut)
            Assert.AreEqual(Nothing, tResult)
        End Sub

        <Test()> _
        Public Sub OutMethodTest3()
            Dim tPoco As GenericMethOutPoco = New GenericMethOutPoco()
            Dim tActsLike = tPoco.ActLike(Of IMethodOut2)()

            Dim tResult As Integer = 3

            Dim tOut = tActsLike.Func(tResult)

            Assert.AreEqual(True, tOut)
            Assert.AreEqual(0, tResult)
        End Sub

        <Test()> _
        Public Sub GenericOutMethodTest()
            Dim tPoco As GenericMethOutPoco = New GenericMethOutPoco()
            Dim tActsLike = tPoco.ActLike(Of IGenericMethodOut)()

            Dim tResult As Integer = 3

            Dim tOut = tActsLike.Func(tResult)

            Assert.AreEqual(True, tOut)
            Assert.AreEqual(0, tResult)

            Dim tResult2 As String = "success"

            Dim tOut2 = tActsLike.Func(tResult2)

            Assert.AreEqual(True, tOut2)
            Assert.AreEqual(Nothing, tResult2)
        End Sub

        <Test()> _
        Public Sub RefMethodTest()
            Dim tPoco As MethRefPoco = New MethRefPoco()
            Dim tActsLike = tPoco.ActLike(Of IMethodRef)()

            Dim tResult As Integer = 1

            Dim tOut = tActsLike.Func(tResult)

            Assert.AreEqual(True, tOut)
            Assert.AreEqual(3, tResult)

            Dim tResult2 As Integer = 2

            tOut = tActsLike.Func(tResult2)

            Assert.AreEqual(True, tOut)
            Assert.AreEqual(4, tResult2)
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class

End Namespace