Option Strict Off

Imports System.Collections
Imports System.Collections.Generic
Imports System.Dynamic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports NUnit.Framework
Imports ImpromptuInterface
Imports ImpromptuInterface.Dynamic

Namespace VBNET

    <TestFixture()> _
    Public Class ImpromptuDynamic
        Inherits Helper

        <Test()> _
        Public Sub DictionaryPropertyTest()

            Dim tNew As Object = New ImpromptuDictionary()
            tNew.Prop1 = "Test"
            tNew.Prop2 = 42L
            tNew.Prop3 = Guid.NewGuid()

            Dim tActsLike As ISimpeleClassProps = Impromptu.ActLike(Of ISimpeleClassProps)(tNew)

            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1)
            Assert.AreEqual(tNew.Prop2, tActsLike.Prop2)
            Assert.AreEqual(tNew.Prop3, tActsLike.Prop3)
        End Sub

        <Test()> _
        Public Sub DictionaryNullPropertyTest()

            Dim tNew As Object = New ImpromptuDictionary()


            Dim tActsLike As ISimpeleClassProps = Impromptu.ActLike(Of ISimpeleClassProps)(tNew)

            Assert.AreEqual(Nothing, tActsLike.Prop1)
            Assert.AreEqual(0, tActsLike.Prop2)
            Assert.AreEqual(New Guid(), tActsLike.Prop3)
        End Sub

        <Test()> _
        Public Sub GetterAnonTest()
            Dim tAnon = New With { _
             Key .Prop1 = "Test", _
             Key .Prop2 = 42L, _
             Key .Prop3 = Guid.NewGuid() _
            }

            Dim tTest As Object = New ImpromptuGet(tAnon)

            Assert.AreEqual(tAnon.Prop1, tTest.Prop1)
            Assert.AreEqual(tAnon.Prop2, tTest.Prop2)
            Assert.AreEqual(tAnon.Prop3, tTest.Prop3)
        End Sub

        <Test()> _
        Public Sub GetterVoidTest()
            Dim tPoco = New VoidMethodPoco()

            Dim tTest As Object = New ImpromptuGet(tPoco)

            tTest.Action()
        End Sub

        <Test()> _
        Public Sub GetterArrayTest()


            Dim tArray = New Integer() {1, 2, 3}

            Dim tTest As IStringIntIndexer = ImpromptuGet.Create(Of IStringIntIndexer)(tArray)

            Assert.AreEqual(tArray(2).ToString(), tTest(2))
        End Sub






        <Test()> _
        Public Sub GetterObjectTest()
            Dim tNew As Object = New ExpandoObject()
            tNew.Prop1 = "Test"
            tNew.Prop2 = 42L
            tNew.Prop3 = Guid.NewGuid()

            Dim tTest As Object = New ImpromptuGet(tNew)


            Assert.AreEqual(tNew.Prop1, tTest.Prop1)
            Assert.AreEqual(tNew.Prop2, tTest.Prop2)
            Assert.AreEqual(tNew.Prop3, tTest.Prop3)
        End Sub

        <Test()> _
        Public Sub ForwardAnonTest()
            Dim tAnon = New With { _
             Key .Prop1 = "Test", _
             Key .Prop2 = 42L, _
             Key .Prop3 = Guid.NewGuid() _
            }

            Dim tTest As Object = New TestForwarder(tAnon)

            Assert.AreEqual(tAnon.Prop1, tTest.Prop1)
            Assert.AreEqual(tAnon.Prop2, tTest.Prop2)
            Assert.AreEqual(tAnon.Prop3, tTest.Prop3)
        End Sub

        <Test()> _
        Public Sub ForwardVoidTest()
            Dim tPoco = New VoidMethodPoco()

            Dim tTest As Object = New TestForwarder(tPoco)

            tTest.Action()
        End Sub



        <Test()> _
        Public Sub ForwardObjectTest()
            Dim tNew As Object = New ExpandoObject()
            tNew.Prop1 = "Test"
            tNew.Prop2 = 42L
            tNew.Prop3 = Guid.NewGuid()

            Dim tTest As Object = New TestForwarder(tNew)


            Assert.AreEqual(tNew.Prop1, tTest.Prop1)
            Assert.AreEqual(tNew.Prop2, tTest.Prop2)
            Assert.AreEqual(tNew.Prop3, tTest.Prop3)
        End Sub

        <Test()> _
        Public Sub DictionaryMethodsTest()

            Dim tNew As Object = New ImpromptuDictionary()
            tNew.Action1 = New Action(AddressOf Assert.Fail)
            tNew.Action2 = New Action(Of Boolean)(AddressOf Assert.IsFalse)
            tNew.Action3 = New Func(Of String)(Function() "test")
            tNew.Action4 = New Func(Of Integer, String)(Function(arg) "test" & Convert.ToString(arg))


            Dim tActsLike As ISimpeleClassMeth2 = Impromptu.ActLike(Of ISimpeleClassMeth2)(tNew)



            AssertException(Of AssertionException)(AddressOf tActsLike.Action1)

            Assert.AreEqual("test", tActsLike.Action3())

            Assert.AreEqual("test4", tActsLike.Action4(4))
        End Sub

        <Test()> _
        Public Sub ForwardMethodsTest()

            Dim tNew As Object = New ImpromptuDictionary()
            tNew.Action1 = New Action(AddressOf Assert.Fail)
            tNew.Action2 = New Action(Of Boolean)(AddressOf Assert.IsFalse)
            tNew.Action3 = New Func(Of String)(Function() "test")
            tNew.Action4 = New Func(Of Integer, String)(Function(arg) "test" & Convert.ToString(arg))


            Dim tFwd As Object = New TestForwarder(tNew)



            AssertException(Of AssertionException)(Function() tFwd.Action1())
            AssertException(Of AssertionException)(Function() tFwd.Action2(True))

            Assert.AreEqual("test", tFwd.Action3())

            Assert.AreEqual("test4", tFwd.Action4(4))
        End Sub



        Private Shared Function TestOut(dummy As CallSite, [in] As Object, ByRef out As String) As Object
            out = TryCast([in], String)

            Return out IsNot Nothing
        End Function


        <Test()> _
        Public Sub DictionaryMethodsTestWithPropertyAccess()

            Dim tNew As Object = New ImpromptuDictionary()
            tNew.PropCat = "Cat-"
            tNew.Action1 = New Action(AddressOf Assert.Fail)
            tNew.Action2 = New Action(Of Boolean)(AddressOf Assert.IsFalse)
            tNew.Action3 = New ThisFunc(Of String)(Function(this) Convert.ToString(this.PropCat) & "test")

            Dim tActsLike As ISimpeleClassMeth = Impromptu.ActLike(Of ISimpeleClassMeth)(tNew)



            AssertException(Of AssertionException)(AddressOf tActsLike.Action1)

            Assert.AreEqual("Cat-test", tActsLike.Action3())


        End Sub

        <Test()> _
        Public Sub DictionaryNullMethodsTest()

            Dim tNew As Object = New ImpromptuDictionary()

            Dim tActsLike As ISimpleStringMethod = Impromptu.ActLike(Of ISimpleStringMethod)(tNew)

            Assert.AreEqual(False, tActsLike.StartsWith("Te"))



        End Sub


        <Test()> _
        Public Sub ObjectDictionaryWrappedTest()

            Dim tDictionary = New Dictionary(Of String, Object)() From { _
             {"Test1", 1}, _
             {"Test2", 2}, _
             {"TestD", New Dictionary(Of String, Object)() From { _
              {"TestA", "A"}, _
              {"TestB", "B"} _
             }} _
            }

            Dim tNew As Object = New ImpromptuDictionary(tDictionary)

            Assert.AreEqual(1, tNew.Test1)
            Assert.AreEqual(2, tNew.Test2)
            Assert.AreEqual("A", tNew.TestD.TestA)
            Assert.AreEqual("B", tNew.TestD.TestB)
        End Sub

        <Test()> _
        Public Sub InterfaceDictionaryWrappedTest()

            Dim tDictionary = New Dictionary(Of String, Object)() From { _
             {"Test1", 1}, _
             {"Test2", 2L}, _
             {"TestD", New Dictionary(Of String, Object)() From { _
              {"TestA", "A"}, _
              {"TestB", "B"} _
             }} _
            }

            Dim tObject As Object = ImpromptuDictionary.Create(Of IDynamicDict)(tDictionary)
            Dim tNotObject As Object = ImpromptuDictionary.Create(Of INonDynamicDict)(tDictionary)

            Assert.AreEqual(tObject, tNotObject)

            Assert.AreEqual(1, tObject.Test1)
            Assert.AreEqual(2L, tObject.Test2)
            Assert.AreEqual("A", tObject.TestD.TestA)
            Assert.AreEqual("B", tObject.TestD.TestB)

            Assert.AreEqual(1, tNotObject.Test1)
            Assert.AreEqual(2L, tNotObject.Test2)


            Assert.AreEqual(GetType(Dictionary(Of String, Object)), tNotObject.TestD.[GetType]())
            Assert.AreEqual(GetType(ImpromptuDictionary), tObject.TestD.[GetType]())
        End Sub

        <Test()> _
        Public Sub ObjectObjectEqualsTest()
            Dim tDictionary = New Dictionary(Of String, Object)() From { _
             {"Test1", 1}, _
             {"Test2", 2}, _
             {"TestD", New Dictionary(Of String, Object)() From { _
              {"TestA", "A"}, _
              {"TestB", "B"} _
             }} _
            }

            Dim tObject As Object = ImpromptuDictionary.Create(Of IDynamicDict)(tDictionary)
            Dim tNotObject As Object = ImpromptuDictionary.Create(Of INonDynamicDict)(tDictionary)

            Assert.AreEqual(tObject, tNotObject)

            Assert.AreEqual(tObject, tDictionary)

            Assert.AreEqual(tNotObject, tDictionary)
        End Sub

        <Test()> _
        Public Sub ObjectAnnonymousWrapper()
            Dim tData = New Dictionary(Of Integer, String)() From { _
             {1, "test"} _
            }
            Dim tDyn = ImpromptuGet.Create(New With { _
             Key .Test1 = 1, _
             Key .Test2 = "2", _
             Key .IsGreaterThan5 = [Return](Of Boolean).Arguments(Of Integer)(Function(it) it > 5), _
             Key .ClearData = ReturnVoid.Arguments(Function() tData.Clear()) _
            })

            Assert.AreEqual(1, tDyn.Test1)
            Assert.AreEqual("2", tDyn.Test2)
            Assert.AreEqual(True, tDyn.IsGreaterThan5(6))
            Assert.AreEqual(False, tDyn.IsGreaterThan5(4))

            Assert.AreEqual(1, tData.Count)
            tDyn.ClearData()
            Assert.AreEqual(0, tData.Count)

        End Sub

        <Test()> _
        Public Sub TestAnonInterface()
            Dim tInterface As ICollection = ImpromptuGet.Create(Of ICollection)(New With { _
             .CopyArray = ReturnVoid.Arguments(Of Array, Integer)(Function(ar, i) Enumerable.Range(1, 10)), _
             .Count = 10, _
             .IsSynchronized = False, _
             .SyncRoot = Me, _
             .GetEnumerator = [Return](Of IEnumerator).Arguments(Function() Enumerable.Range(1, 10).GetEnumerator()) _
            })

            Assert.AreEqual(10, tInterface.Count)
            Assert.AreEqual(False, tInterface.IsSynchronized)
            Assert.AreEqual(Me, tInterface.SyncRoot)
            Assert.AreEqual(True, tInterface.GetEnumerator().MoveNext())
        End Sub

        <Test()> _
        Public Sub TestBuilder()

            Dim NewD As Object = New ImpromptuBuilder(Of ExpandoObject)()


            Dim tExpandoNamedTest = NewD.Robot(LeftArm:="Rise", RightArm:="Clamp")

            Assert.AreEqual("Rise", tExpandoNamedTest.LeftArm)
            Assert.AreEqual("Clamp", tExpandoNamedTest.RightArm)
        End Sub

        <Test()> _
        Public Sub TestSetupOtherTypes()
            Dim [New] = Builder.[New]().Setup(Expando:=GetType(ExpandoObject), Dict:=GetType(ImpromptuDictionary))

            Dim tExpando = [New].Expando(LeftArm:="Rise", RightArm:="Clamp")

            Dim tDict = [New].Dict(LeftArm:="RiseD", RightArm:="ClampD")

            Assert.AreEqual("Rise", tExpando.LeftArm)
            Assert.AreEqual("Clamp", tExpando.RightArm)
            Assert.AreEqual(GetType(ExpandoObject), tExpando.[GetType]())

            Assert.AreEqual("RiseD", tDict.LeftArm)
            Assert.AreEqual("ClampD", tDict.RightArm)
            Assert.AreEqual(GetType(ImpromptuDictionary), tDict.[GetType]())

        End Sub


        'This test data is modified from MS-PL Clay project http://clay.codeplex.com
        <Test()> _
        Public Sub TestClayFactorySyntax()
            Dim [New] As Object = Builder.[New]()

            If True Then
                Dim person = [New].Person()
                person.FirstName = "Louis"
                person.LastName = "Dejardin"
                Assert.AreEqual("Louis", person.FirstName)
                Assert.AreEqual("Dejardin", person.LastName)
            End If
            If True Then
                Dim person = [New].Person()
                person("FirstName") = "Louis"
                person("LastName") = "Dejardin"
                Assert.AreEqual("Louis", person.FirstName)
                Assert.AreEqual("Dejardin", person.LastName)
            End If
            If True Then
                Dim person As Object = [New].Person(FirstName:="Bertrand", LastName:="Le Roy").Aliases("bleroy", "boudin")

                Assert.AreEqual("Bertrand", person.FirstName)
                Assert.AreEqual("Le Roy", person.LastName)
                Assert.AreEqual("boudin", Impromptu.InvokeGetIndex(person.Aliases, 1))
            End If

            If True Then
                Dim person = [New].Person().FirstName("Louis").LastName("Dejardin").Aliases(New String() {"Lou"})

                Assert.AreEqual("Louis", person.FirstName)
                Assert.AreEqual("Lou", Impromptu.InvokeGetIndex(person.Aliases, 0))
            End If

            If True Then
                Dim person = [New].Person(New With { _
                 Key .FirstName = "Louis", _
                 Key .LastName = "Dejardin" _
                })
                Assert.AreEqual("Louis", person.FirstName)
                Assert.AreEqual("Dejardin", person.LastName)
            End If

        End Sub

        <Test()> _
        Public Sub TestBuilderActLikeAnon()
            Dim [New] = Builder.[New]().ActLike(Of IBuilder)()

            Dim tNest = [New].Nester(New With { _
             Key .NameLevel1 = "Lvl1", _
             Key .Nested = [New].Nester2(New With { _
              Key .NameLevel2 = "Lvl2" _
             }) _
            })

            Assert.AreEqual("Lvl1", tNest.NameLevel1)
            Assert.AreEqual("Lvl2", tNest.Nested.NameLevel2)
        End Sub

        <Test()> _
        Public Sub TestBuilderActLikeNamed()
            Dim [New] = Builder.[New]().ActLike(Of IBuilder)()

            Dim tNest = [New].Nester(NameLevel1:="Lvl1", Nested:=[New].Nester2(NameLevel2:="Lvl2"))

            Assert.AreEqual("Lvl1", tNest.NameLevel1)
            Assert.AreEqual("Lvl2", tNest.Nested.NameLevel2)
        End Sub


        'This test data is modified from MS-PL Clay project http://clay.codeplex.com
        <Test()> _
        Public Sub TestFactoryListSyntax()
            Dim [New] As Object = Builder.[New]()

            'Test using Clay Syntax
            Dim people = [New].Array([New].Person().FirstName("Louis").LastName("Dejardin"), [New].Person().FirstName("Bertrand").LastName("Le Roy"))

            Assert.AreEqual("Dejardin", people(0).LastName)
            Assert.AreEqual("Le Roy", people(1).LastName)

            Dim people2 = New ImpromptuList() From { _
             [New].Robot(Name:="Bender"), _
             [New].Robot(Name:="RobotDevil") _
            }


            Assert.AreEqual("Bender", people2(0).Name)
            Assert.AreEqual("RobotDevil", people2(1).Name)

        End Sub

        <Test()> _
        Public Sub TestQuicListSyntax()
            Dim tList = Build.NewList("test", "one", "two")
            Assert.AreEqual("one", tList(1))
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function

    End Class
End Namespace