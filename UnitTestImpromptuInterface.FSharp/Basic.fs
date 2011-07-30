// Learn more about F# at http://fsharp.net
namespace UnitTestImpromptuInterface.FSharp.Tests

module Module1=

    open NUnit.Framework
    open Swensen.Unquote
    open System
    open System.Dynamic
    open ImpromptuInterface.FSharp
    open ImpromptuInterface
    open ImpromptuInterface.Dynamic
    open System.Collections.Generic
    open UnitTestSupportLibrary

    [<TestFixture>] 
    type ``Basic Dynamic Operator Tests`` ()=
        let Static t = InvokeContext.CreateStatic.Invoke(t)

        [<Test>] member basic.``Call method off of an object dynamically`` ()=
                        test <@ "HelloWorld"?Substring(0,5) = "Hello" @>

        [<Test>] member basic.``Test Expando Set and Get`` ()=
                        let ex1 = ExpandoObject()
                        ex1?Test<-"Hi";
                        test <@ ex1?Test = "Hi" @>;

        [<Test>] member basic.``Test Direct Invoke`` ()=   
                        test <@ !?Impromptu.Curry(Static(typeof<string>))?Format("Test {0} {1}") (1,2) = "Test 1 2" @>;

        [<Test>] member basic.``Test Void Method`` ()=   
                        let array = List<string>()
                        array?Add("1");
                        test <@ array.[0] = "1" @>;

        [<Test>] member basic.``Test SetAll`` ()=   
                        let e1 = ExpandoObject()
                        !?Impromptu.InvokeSetAll (e1, [("One",1);("Two",2)])
                        test <@e1?One = 1 @>
                        test <@e1?Two = 2 @>

        [<Test>] member basic.``Test ActLike`` ()=   
                        let e1 = ImpromptuDictionary()
                        let i1 = e1.ActLike<IComparable>()
                        test <@ i1.CompareTo(null) = 0 @>  //Static Test
                        test <@ e1?CompareTo(null) = 0 @>;  //Dynamic Test

        [<Test>] member basic.``Test Lambda methods`` ()=
                    let ex1 = ImpromptuDictionary()
                    ex1?TestLam<- (fun x -> 42 + x)
                    ex1?TestLam2<- (fun x y -> y+ 42 + x)
                    ex1?TestDel<- TestFuncs.Plus3
                    test <@ ex1?TestLam(1) = 43 @>
                    test <@ ex1?TestLam2(1, 2) = 45 @>
                    test <@ ex1?TestDel (2) = 5 @>

        [<Test>] member basic.``Test FSharp Lambda 3 arg `` ()=
                   let dyn = (fun x y z -> x + y - z) :> obj
                   test <@ !?dyn (3,2,1) = 4 @>
        
        [<Test>] member basic.``Test FSharp Lambda 4 arg`` ()=
                   let dyn = (fun x y z bbq -> x + y - z - bbq) :> obj  in
                   test <@ !?dyn (3,2,1,5) = -1 @>;

        [<Test>] member basic.``Test FSharp Lambda 5 arg`` ()=
                   let dyn = (fun x y z bbq etc -> x + y - z - bbq + etc) :> obj in
                   test <@ !?dyn (3,2,1,5, 9) = 8 @>;