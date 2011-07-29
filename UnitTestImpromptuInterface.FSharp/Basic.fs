// Learn more about F# at http://fsharp.net
namespace UnitTestImpromptuInterface.FSharp.Tests

module Module1=

    open NUnit.Framework
    open FsUnit
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

        [<Test>] member test.
         ``Call method off of an object dynamically`` ()=
            "HelloWorld"?Substring(0,5) |> should equal "Hello";

        [<Test>] member test.
         ``Test Expando Set and Get`` ()=
            let ex1 = ExpandoObject() in
            ex1?Test<-"Hi";
            ex1?Test |> should equal "Hi";

        [<Test>] member test.
         ``Test Direct Invoke`` ()=   
               !?Impromptu.Curry(Static(typeof<string>))?Format("Test {0} {1}") (1,2) |> should equal "Test 1 2";

        [<Test>] member test.
         ``Test Void Method`` ()=   
               let array = List<obj>() in
               array?Add("1");
               array.[0] |> should equal "1";

        [<Test>] member test.
         ``Test SetAll`` ()=   
               let e1 = ExpandoObject() in
               !?Impromptu.InvokeSetAll (e1, [("One",1);("Two",2)]);
               e1?One |> should equal 1;
               e1?Two |> should equal 2;

        [<Test>] member test.
         ``Test ActLike`` ()=   
               let e1 = ImpromptuDictionary() in
               let i1 = e1.ActLike<IComparable>() in
               i1.CompareTo(null) |> should equal 0;  //Static Test
               e1?CompareTo(null) |> should equal 0;  //Dynamic Test

        [<Test>] member test.
         ``Test Lambda methods`` ()=
            let ex1 = ImpromptuDictionary() in
            ex1?TestLam<- (fun x -> 42 + x);
            ex1?TestLam2<- (fun x y -> y+ 42 + x);
            ex1?TestDel<- TestFuncs.Plus3;
            ex1?TestLam(1) |> should equal 43;
            ex1?TestLam2(1, 2) |> should equal 45;
            ex1?TestDel (2) |> should equal 5;

        [<Test>] member test.
         ``Test FSharp Lambda 3 arg `` ()=
           let dyn = (fun x y z -> x + y - z) :> obj in
           !?dyn (3,2,1) |> should equal 4;
        
        [<Test>] member test.
         ``Test FSharp Lambda 4 arg`` ()=
           let dyn = (fun x y z bbq -> x + y - z - bbq) :> obj  in
           !?dyn (3,2,1,5) |> should equal -1;

        [<Test>] member test.
         ``Test FSharp Lambda 5 arg`` ()=
           let dyn = (fun x y z bbq etc -> x + y - z - bbq + etc) :> obj in
           !?dyn (3,2,1,5, 9) |> should equal 8;