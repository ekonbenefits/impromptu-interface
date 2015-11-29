Option Strict Off

' 
'  Copyright 2011  Ekon Benefits
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
Imports System.Dynamic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Xml.Linq
Imports NUnit.Framework
Imports ImpromptuInterface
Imports ImpromptuInterface.Dynamic
Imports Microsoft.CSharp.RuntimeBinder
Imports Binder = Microsoft.CSharp.RuntimeBinder.Binder
Imports BinderFlags = Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags
Imports Info = Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
Imports InfoFlags = Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags
Imports ImpromptuInterface.InvokeExt
Imports ImpromptuInterface.Optimization

Namespace VBNET

    <TestFixture()> _
    Public Class SingleMethodInvoke
        Inherits Helper
        <Test()> _
        Public Sub TestDynamicSet()
            Dim tExpando As Object = New ExpandoObject()

            Dim tSetValue = "1"

            Impromptu.InvokeSet(tExpando, "Test", tSetValue)

            Assert.AreEqual(tSetValue, tExpando.Test)

        End Sub



        <Test()> _
        Public Sub TestPocoSet()
            Dim tPoco = New PropPoco()

            Dim tSetValue = "1"

            Impromptu.InvokeSet(tPoco, "Prop1", tSetValue)

            Assert.AreEqual(tSetValue, tPoco.Prop1)

        End Sub

        <Test()> _
        Public Sub TestCacheableDynamicSetAndPocoSetAndSetNull()
            Dim tExpando As Object = New ExpandoObject()
            Dim tSetValueD = "4"


            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.[Set], "Prop1")

            tCachedInvoke.Invoke(DirectCast(tExpando, Object), tSetValueD)


            Assert.AreEqual(tSetValueD, tExpando.Prop1)

            Dim tPoco = New PropPoco()
            Dim tSetValue = "1"

            tCachedInvoke.Invoke(tPoco, tSetValue)

            Assert.AreEqual(tSetValue, tPoco.Prop1)

            Dim tSetValue2 As [String] = Nothing

            tCachedInvoke.Invoke(tPoco, tSetValue2)

            Assert.AreEqual(tSetValue2, tPoco.Prop1)
        End Sub



        <Test()> _
        Public Sub TestConvert()
            Dim tEl = New XElement("Test", "45")

            Dim tCast = Impromptu.InvokeConvert(tEl, GetType(Integer), explicit:=True)

            Assert.AreEqual(GetType(Integer), tCast.[GetType]())
            Assert.AreEqual(45, tCast)
        End Sub

        <Test()> _
        Public Sub TestConvertCacheable()
            Dim tEl = New XElement("Test", "45")

            Dim tCacheInvoke = New CacheableInvocation(InvocationKind.Convert, convertType:=GetType(Integer), convertExplicit:=True)
            Dim tCast = tCacheInvoke.Invoke(tEl)

            Assert.AreEqual(GetType(Integer), tCast.[GetType]())
            Assert.AreEqual(45, tCast)
        End Sub

        <Test()> _
        Public Sub TestConstruct()
            Dim tCast = Impromptu.InvokeConstructor(GetType(List(Of Object)), New Object() {New String() {"one", "two", "three"}})

            Assert.AreEqual("two", tCast(1))
        End Sub


        <Test()> _
        Public Sub TestCacheableConstruct()
            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.Constructor, argCount:=1)

            Dim tCast As Object = tCachedInvoke.Invoke(GetType(List(Of Object)), New Object() {New String() {"one", "two", "three"}})

            Assert.AreEqual("two", tCast(1))
        End Sub


        <Test()> _
        Public Sub TestConstructOptional()
            Dim tCast As PocoOptConstructor = Impromptu.InvokeConstructor(GetType(PocoOptConstructor), "3".WithArgumentName("three__3"))

            Assert.AreEqual("-1", tCast.One)
            Assert.AreEqual("-2", tCast.Two)
            Assert.AreEqual("3", tCast.Three)
        End Sub

        <Test()> _
        Public Sub TestCacheableConstructOptional()
            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.Constructor, argCount:=1, argNames:=New String() {"three__3"})

            Dim tCast = DirectCast(tCachedInvoke.Invoke(GetType(PocoOptConstructor), "3"), PocoOptConstructor)

            Assert.AreEqual("-1", tCast.One)
            Assert.AreEqual("-2", tCast.Two)
            Assert.AreEqual("3", tCast.Three)
        End Sub

        <Test()> _
        Public Sub TestOptionalArgumentActivationNoneAndCacheable()
            AssertException(Of MissingMethodException)(Function() Activator.CreateInstance(Of ImpromptuList)())

            Dim tList = Impromptu.InvokeConstructor(GetType(ImpromptuList))


            Assert.AreEqual(GetType(ImpromptuList), tList.[GetType]())

            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.Constructor)

            Dim tList1 = tCachedInvoke.Invoke(GetType(ImpromptuList))


            Assert.AreEqual(GetType(ImpromptuList), tList1.[GetType]())
        End Sub



        <Test()> _
        Public Sub TestConstructValueType()
            Dim tCast = Impromptu.InvokeConstructor(GetType(DateTime), 2009, 1, 20)

            Assert.AreEqual(20, tCast.Day)

        End Sub

        <Test()> _
        Public Sub TestCacheableConstructValueType()
            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.Constructor, argCount:=3)
            Dim tCast As Object = tCachedInvoke.Invoke(GetType(DateTime), 2009, 1, 20)

            Assert.AreEqual(20, tCast.Day)

        End Sub

        <Test()> _
        Public Sub TestConstructValueTypeJustDynamic()
            Dim day As Object = 20
            Dim year As Object = 2009
            Dim month As Object = 1
            Dim tCast = New DateTime(year, month, day)
            Dim tDate As DateTime = tCast
            Assert.AreEqual(20, tDate.Day)
        End Sub

        <Test()> _
        Public Sub TestConstructprimativetype()
            Dim tCast = Impromptu.InvokeConstructor(GetType(Int32))

            Assert.AreEqual(New Int32(), tCast)
        End Sub


        <Test()> _
        Public Sub TestConstructDateTimeNoParams()
            Dim tCast = Impromptu.InvokeConstructor(GetType(DateTime))

            Assert.AreEqual(New DateTime(), tCast)
        End Sub

        <Test()> _
        Public Sub TestConstructOBjectNoParams()
            Dim tCast = Impromptu.InvokeConstructor(GetType(Object))

            Assert.AreEqual(GetType(Object), tCast.[GetType]())
        End Sub

        <Test()> _
        Public Sub TestConstructNullableprimativetype()
            Dim tCast = Impromptu.InvokeConstructor(GetType(Nullable(Of Int32)))

            Assert.AreEqual(Nothing, tCast)
        End Sub

        <Test()> _
        Public Sub TestConstructGuid()
            Dim tCast = Impromptu.InvokeConstructor(GetType(Guid))

            Assert.AreEqual(New Guid(), tCast)
        End Sub

        <Test()> _
        Public Sub TestCacheablePrimativeDateTimeObjectNullableAndGuidNoParams()
            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.Constructor)

            Dim tCast As Object = tCachedInvoke.Invoke(GetType(Int32))

            Assert.AreEqual(New Int32(), tCast)

            tCast = tCachedInvoke.Invoke(GetType(DateTime))

            Assert.AreEqual(New DateTime(), tCast)

            tCast = tCachedInvoke.Invoke(GetType(List(Of String)))

            Assert.AreEqual(GetType(List(Of String)), tCast.[GetType]())

            tCast = tCachedInvoke.Invoke(GetType(Object))

            Assert.AreEqual(GetType(Object), tCast.[GetType]())

            tCast = tCachedInvoke.Invoke(GetType(Nullable(Of Int32)))

            Assert.AreEqual(Nothing, tCast)

            tCast = tCachedInvoke.Invoke(GetType(Guid))

            Assert.AreEqual(New Guid, tCast)
        End Sub


        <Test()> _
        Public Sub TestStaticCall()

            Dim tOut = Impromptu.InvokeMember(GetType(StaticType).WithStaticContext(), "Create".WithGenericArgs(GetType(Boolean)), 1)
            Assert.AreEqual(False, tOut)
        End Sub

        <Test()> _
        Public Sub TestCacheableStaticCall()

            Dim tCached = New CacheableInvocation(InvocationKind.InvokeMember, "Create".WithGenericArgs(GetType(Boolean)), argCount:=1, context:=GetType(StaticType).WithStaticContext())

            Dim tOut = tCached.Invoke(GetType(StaticType), 1)
            Assert.AreEqual(False, tOut)
        End Sub

        <Test()> _
        Public Sub TestImplicitConvert()
            Dim tEl = 45

            Dim tCast = Impromptu.InvokeConvert(tEl, GetType(Long))

            Assert.AreEqual(GetType(Long), tCast.[GetType]())
        End Sub

        <Test()> _
        Public Sub TestCacheableImplicitConvert()
            Dim tEl = 45

            Dim tCachedInvoke = CacheableInvocation.CreateConvert(GetType(Long))

            Dim tCast = tCachedInvoke.Invoke(tEl)

            Assert.AreEqual(GetType(Long), tCast.[GetType]())
        End Sub


        <Test()> _
        Public Sub TestCacheableGet()
            Dim tCached = New CacheableInvocation(InvocationKind.[Get], "Prop1")

            Dim tSetValue = "1"
            Dim tAnon = New PropPoco() With { _
             .Prop1 = tSetValue _
            }

            Dim tOut = tCached.Invoke(tAnon)
            Assert.AreEqual(tSetValue, tOut)

            Dim tSetValue2 = "2"
            tAnon = New PropPoco() With { _
             .Prop1 = tSetValue2 _
            }


            Dim tOut2 = tCached.Invoke(tAnon)


            Assert.AreEqual(tSetValue2, tOut2)

        End Sub

        <Test()> _
        Public Sub TestGetIndexer()

            Dim tSetValue As Object = "1"
            Dim tAnon = New String() {tSetValue, "2"}


            Dim tOut As String = Impromptu.InvokeGetIndex(tAnon, 0)

            Assert.AreEqual(tSetValue, tOut)

        End Sub


        <Test()> _
        Public Sub TestGetIndexerValue()


            Dim tAnon = New Integer() {1, 2}


            Dim tOut As Integer = Impromptu.InvokeGetIndex(tAnon, 1)

            Assert.AreEqual(tAnon(1), tOut)

        End Sub


        <Test()> _
        Public Sub TestGetLengthArray()
            Dim tAnon = New String() {"1", "2"}


            Dim tOut As Integer = Impromptu.InvokeGet(tAnon, "Length")

            Assert.AreEqual(2, tOut)

        End Sub

        <Test()> _
        Public Sub TestGetIndexerArray()
            Dim tSetValue As Object = "1"
            Dim tAnon = New List(Of String)() From { _
             tSetValue, _
             "2" _
            }


            Dim tOut As String = Impromptu.InvokeGetIndex(tAnon, 0)

            Assert.AreEqual(tSetValue, tOut)

        End Sub


        <Test()> _
        Public Sub TestCacheableIndexer()

            Dim tStrings = New String() {"1", "2"}

            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.GetIndex, argCount:=1)

            Dim tOut = DirectCast(tCachedInvoke.Invoke(tStrings, 0), String)

            Assert.AreEqual(tStrings(0), tOut)

            Dim tOut2 = DirectCast(tCachedInvoke.Invoke(tStrings, 1), String)

            Assert.AreEqual(tStrings(1), tOut2)

            Dim tInts = New Integer() {3, 4}

            Dim tOut3 = CInt(tCachedInvoke.Invoke(tInts, 0))

            Assert.AreEqual(tInts(0), tOut3)

            Dim tOut4 = CInt(tCachedInvoke.Invoke(tInts, 1))

            Assert.AreEqual(tInts(1), tOut4)

            Dim tList = New List(Of String)() From { _
             "5", _
             "6" _
            }

            Dim tOut5 = DirectCast(tCachedInvoke.Invoke(tList, 0), String)

            Assert.AreEqual(tList(0), tOut5)

            Dim tOut6 = DirectCast(tCachedInvoke.Invoke(tList, 0), String)

            Assert.AreEqual(tList(0), tOut6)
        End Sub

        <Test()> _
        Public Sub TestSetIndexer()

            Dim tSetValue As Object = "3"
            Dim tAnon = New List(Of String)() From { _
             "1", _
             "2" _
            }

            Impromptu.InvokeSetIndex(tAnon, 0, tSetValue)

            Assert.AreEqual(tSetValue, tAnon(0))

        End Sub

        <Test()> _
        Public Sub TestCacheableSetIndexer()

            Dim tSetValue As Object = "3"
            Dim tList = New List(Of String)() From { _
             "1", _
             "2" _
            }


            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.SetIndex, argCount:=2)

            tCachedInvoke.Invoke(tList, 0, tSetValue)

            Assert.AreEqual(tSetValue, tList(0))

        End Sub



        <Test()> _
        Public Sub TestMethodDynamicPassAndGetValue()
            Dim tExpando As Object = New ExpandoObject()
            tExpando.Func = New Func(Of Integer, String)(Function(it) it.ToString())

            Dim tValue = 1

            Dim tOut = Impromptu.InvokeMember(tExpando, "Func", tValue)

            Assert.AreEqual(tValue.ToString(), tOut)
        End Sub


        <Test()> _
        Public Sub TestCacheableMethodDynamicPassAndGetValue()
            Dim tExpando As Object = New ExpandoObject()
            tExpando.Func = New Func(Of Integer, String)(Function(it) it.ToString())

            Dim tValue = 1

            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.InvokeMember, "Func", 1)

            Dim tOut = tCachedInvoke.Invoke(DirectCast(tExpando, Object), tValue)

            Assert.AreEqual(tValue.ToString(), tOut)
        End Sub








        ''' <summary>
        ''' To dynamically invoke a method with out or ref parameters you need to know the signature
        ''' </summary>
        <Test()> _
        Public Sub TestOutMethod()



            Dim tResult As String = [String].Empty

            Dim tPoco = New MethOutPoco()


            Dim tName As String = "Func"
            Dim tContext As Type = [GetType]()
            Dim tBinder = Binder.InvokeMember(BinderFlags.None, tName, Nothing, tContext, New Info() {Info.Create(InfoFlags.None, Nothing), Info.Create(InfoFlags.IsRef Or InfoFlags.UseCompileTimeType, Nothing)})


            Dim tSite = Impromptu.CreateCallSite(Of DynamicTryString)(tBinder, tName, tContext)


            tSite.Target.Invoke(tSite, tPoco, tResult)

            Assert.AreEqual("success", tResult)

        End Sub


        <Test()> _
        Public Sub TestMethodDynamicPassVoid()
            Dim tTest = "Wrong"

            Dim tValue = "Correct"

            Dim tExpando As Object = New ExpandoObject()
            tExpando.Action = New Action(Of String)(Function(it) InlineAssignHelper(tTest, it))



            Impromptu.InvokeMemberAction(tExpando, "Action", tValue)

            Assert.AreEqual(tValue, tTest)
        End Sub

        <Test()> _
        Public Sub TestCacheableMethodDynamicPassVoid()
            Dim tTest = "Wrong"

            Dim tValue = "Correct"

            Dim tExpando As Object = New ExpandoObject()
            tExpando.Action = New Action(Of String)(Function(it) InlineAssignHelper(tTest, it))

            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.InvokeMemberAction, "Action", argCount:=1)

            tCachedInvoke.Invoke(DirectCast(tExpando, Object), tValue)

            Assert.AreEqual(tValue, tTest)
        End Sub

        <Test()> _
        Public Sub TestCacheableMethodDynamicUnknowns()
            Dim tTest = "Wrong"

            Dim tValue = "Correct"

            Dim tExpando As Object = New ExpandoObject()
            tExpando.Action = New Action(Of String)(Function(it) InlineAssignHelper(tTest, it))
            tExpando.Func = New Func(Of String, String)(Function(it) it)

            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.InvokeMemberUnknown, "Action", argCount:=1)

            tCachedInvoke.Invoke(DirectCast(tExpando, Object), tValue)

            Assert.AreEqual(tValue, tTest)

            Dim tCachedInvoke2 = New CacheableInvocation(InvocationKind.InvokeMemberUnknown, "Func", argCount:=1)

            Dim Test2 = tCachedInvoke2.Invoke(DirectCast(tExpando, Object), tValue)

            Assert.AreEqual(tValue, Test2)
        End Sub



        <Test()> _
        Public Sub TestMethodPocoGetValue()


            Dim tValue = 1

            Dim tOut = Impromptu.InvokeMember(tValue, "ToString")

            Assert.AreEqual(tValue.ToString(), tOut)
        End Sub



        <Test()> _
        Public Sub TestMethodPocoPassAndGetValue()


            HelpTestPocoPassAndGetValue("Test", "Te")


            HelpTestPocoPassAndGetValue("Test", "st")
        End Sub

        Private Sub HelpTestPocoPassAndGetValue(tValue As String, tParam As String)
            Dim tExpected = tValue.StartsWith(tParam)

            Dim tOut = Impromptu.InvokeMember(tValue, "StartsWith", tParam)

            Assert.AreEqual(tExpected, tOut)
        End Sub


        <Test()> _
        Public Sub TestGetDynamic()

            Dim tSetValue = "1"
            Dim tExpando As Object = New ExpandoObject()
            tExpando.Test = tSetValue



            Dim tOut = Impromptu.InvokeGet(tExpando, "Test")

            Assert.AreEqual(tSetValue, tOut)
        End Sub

        <Test()> _
        Public Sub TestGetDynamicChained()

            Dim tSetValue = "1"
            Dim tExpando As Object = New ExpandoObject()
            tExpando.Test = New ExpandoObject()
            tExpando.Test.Test2 = New ExpandoObject()
            tExpando.Test.Test2.Test3 = tSetValue


            Dim tOut = Impromptu.InvokeGetChain(tExpando, "Test.Test2.Test3")

            Assert.AreEqual(tSetValue, tOut)
        End Sub



        <Test()> _
        Public Sub TestSetDynamicChained()

            Dim tSetValue = "1"
            Dim tExpando As Object = New ExpandoObject()
            tExpando.Test = New ExpandoObject()
            tExpando.Test.Test2 = New ExpandoObject()


            Impromptu.InvokeSetChain(tExpando, "Test.Test2.Test3", tSetValue)

            Assert.AreEqual(tSetValue, tExpando.Test.Test2.Test3)
        End Sub

        <Test()> _
        Public Sub TestSetDynamicChainedOne()

            Dim tSetValue = "1"
            Dim tExpando As Object = New ExpandoObject()


            Impromptu.InvokeSetChain(tExpando, "Test", tSetValue)

            Assert.AreEqual(tSetValue, tExpando.Test)
        End Sub

        <Test()> _
        Public Sub TestGetDynamicChainedOne()

            Dim tSetValue = "1"
            Dim tExpando As Object = New ExpandoObject()
            tExpando.Test = tSetValue



            Dim tOut = Impromptu.InvokeGetChain(tExpando, "Test")

            Assert.AreEqual(tSetValue, tOut)
        End Sub

        <Test()> _
        Public Sub TestCacheableGetDynamic()

            Dim tSetValue = "1"
            Dim tExpando As Object = New ExpandoObject()
            tExpando.Test = tSetValue

            Dim tCached = New CacheableInvocation(InvocationKind.[Get], "Test")

            Dim tOut = tCached.Invoke(DirectCast(tExpando, Object))

            Assert.AreEqual(tSetValue, tOut)
        End Sub

        <Test()> _
        Public Sub TestStaticGet()
            Dim tDate = Impromptu.InvokeGet(GetType(DateTime).WithStaticContext(), "Today")
            Assert.AreEqual(DateTime.Today, tDate)
        End Sub

        <Test()> _
        Public Sub TestCacheableStaticGet()
            Dim tCached = New CacheableInvocation(InvocationKind.[Get], "Today", context:=GetType(DateTime).WithStaticContext())

            Dim tDate = tCached.Invoke(GetType(DateTime))
            Assert.AreEqual(DateTime.Today, tDate)
        End Sub


        <Test()> _
        Public Sub TestStaticGet2()
            Dim tVal = Impromptu.InvokeGet(GetType(StaticType).WithStaticContext(), "Test")
            Assert.AreEqual(True, tVal)
        End Sub

        <Test()> _
        Public Sub TestStaticSet()
            Dim tValue As Integer = 12
            Impromptu.InvokeSet(GetType(StaticType).WithStaticContext(), "TestSet", tValue)
            Assert.AreEqual(tValue, StaticType.TestSet)
        End Sub

        <Test()> _
        Public Sub TestCacheableStaticSet()
            Dim tValue As Integer = 12

            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.[Set], "TestSet", context:=GetType(StaticType).WithStaticContext())
            tCachedInvoke.Invoke(GetType(StaticType), tValue)
            Assert.AreEqual(tValue, StaticType.TestSet)
        End Sub

        <Test()> _
        Public Sub TestStaticDateTimeMethod()
            Dim tDateDyn As Object = "01/20/2009"
            Dim tDate = Impromptu.InvokeMember(GetType(DateTime).WithStaticContext(), "Parse", tDateDyn)
            Assert.AreEqual(New DateTime(2009, 1, 20), tDate)
        End Sub

        <Test()> _
        Public Sub TestCacheableStaticDateTimeMethod()
            Dim tDateDyn As Object = "01/20/2009"
            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.InvokeMember, "Parse", 1, context:=GetType(DateTime).WithStaticContext())
            Dim tDate = tCachedInvoke.Invoke(GetType(DateTime), tDateDyn)
            Assert.AreEqual(New DateTime(2009, 1, 20), tDate)
        End Sub



        <Test()> _
        Public Sub TestIsEvent()
            Dim tPoco As Object = New PocoEvent()

            Dim tResult = Impromptu.InvokeIsEvent(tPoco, "Event")

            Assert.AreEqual(True, tResult)
        End Sub

        <Test()> _
        Public Sub TestCacheableIsEventAndIsNotEvent()
            Dim tPoco As Object = New PocoEvent()

            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.IsEvent, "Event")

            Dim tResult = tCachedInvoke.Invoke(tPoco)

            Assert.AreEqual(True, tResult)

            Dim tDynamic As Object = New ImpromptuDictionary()

            tDynamic.[Event] = Nothing

            Dim tResult2 = tCachedInvoke.Invoke(DirectCast(tDynamic, Object))

            Assert.AreEqual(False, tResult2)
        End Sub



        <Test()> _
        Public Sub TestPocoAddAssign()
            Dim tPoco = New PocoEvent()
            Dim tTest As Boolean = False

            Impromptu.InvokeAddAssign(tPoco, "Event", New EventHandler(Of EventArgs)(Function([object], args)
                                                                                         tTest = True

                                                                                     End Function))

            tPoco.OnEvent(Nothing, Nothing)

            Assert.AreEqual(True, tTest)

            Dim tPoco2 = New PropPoco() With { _
              .Prop2 = 3 _
            }

            Impromptu.InvokeAddAssign(tPoco2, "Prop2", 4)

            Assert.AreEqual(7L, tPoco2.Prop2)
        End Sub

        <Test()> _
        Public Sub TestCacheablePocoAddAssign()
            Dim tPoco = New PocoEvent()
            Dim tTest As Boolean = False

            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.AddAssign, "Event")

            tCachedInvoke.Invoke(tPoco, New EventHandler(Of EventArgs)(Function([object], args)
                                                                           tTest = True

                                                                       End Function))

            tPoco.OnEvent(Nothing, Nothing)

            Assert.AreEqual(True, tTest)

            Dim tPoco2 = New PropPoco() With { _
              .[Event] = 3 _
            }

            tCachedInvoke.Invoke(tPoco2, 4)

            Assert.AreEqual(7L, tPoco2.[Event])
        End Sub





        <Test()> _
        Public Sub TestDynamicAddAssign()
            Dim tDynamic = Build.NewObject(Prop2:=3, [Event]:=Nothing, OnEvent:=New ThisAction(Of Object, EventArgs)(Function(this, obj, args) this.[Event](obj, args)))
            Dim tTest As Boolean = False

            Impromptu.InvokeAddAssign(tDynamic, "Event", New EventHandler(Of EventArgs)(Function([object], args)
                                                                                            tTest = True

                                                                                        End Function))

            tDynamic.OnEvent(Nothing, Nothing)

            Assert.AreEqual(True, tTest)

            Impromptu.InvokeAddAssign(tDynamic, "Prop2", 4)

            Assert.AreEqual(7L, tDynamic.Prop2)
        End Sub

        <Test()> _
        Public Sub TestCacheableDynamicAddAssign()
            Dim tDynamic = Build.NewObject(Prop2:=3, [Event]:=Nothing, OnEvent:=New ThisAction(Of Object, EventArgs)(Function(this, obj, args) this.[Event](obj, args)))
            Dim tDynamic2 = Build.NewObject([Event]:=3)
            Dim tTest As Boolean = False

            Dim tCachedInvoke = New CacheableInvocation(InvocationKind.AddAssign, "Event")

            tCachedInvoke.Invoke(DirectCast(tDynamic, Object), New EventHandler(Of EventArgs)(Function([object], args)
                                                                                                  tTest = True

                                                                                              End Function))

            tDynamic.OnEvent(Nothing, Nothing)

            Assert.AreEqual(True, tTest)

            tCachedInvoke.Invoke(DirectCast(tDynamic2, Object), 4)

            Assert.AreEqual(7, tDynamic2.[Event])
        End Sub





        <Test()> _
        Public Sub TestDynamicMemberNamesExpando()
            Dim tExpando As ExpandoObject = Build(Of ExpandoObject).NewObject(One:=1)

            Assert.AreEqual("One", Impromptu.GetMemberNames(tExpando, dynamicOnly:=True).[Single]())
        End Sub

        <Test()> _
        Public Sub TestDynamicMemberNamesImpromput()
            Dim tDict As ImpromptuDictionary = Build.NewObject(Two:=2)

            Assert.AreEqual("Two", Impromptu.GetMemberNames(tDict, dynamicOnly:=True).[Single]())
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class

End Namespace