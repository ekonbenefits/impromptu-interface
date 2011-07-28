// Learn more about F# at http://fsharp.net
namespace UnitTestImpromptuInterface.FSharp.Tests

module Module1=

    open NUnit.Framework
    open FsUnit
    open System.Dynamic
    open ImpromptuInterface.FSharp
    open ImpromptuInterface
    open System.Collections.Generic

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
               let format = Impromptu.Curry(Static(typeof<string>))?Format("Test {0} {1}") in
               format?``_``(1,2) |> should equal "Test 1 2";

        [<Test>] member test.
         ``Test Void Method`` ()=   
               let array = List<obj>() in
               array?Add("1");
               array.[0] |> should equal "1";
