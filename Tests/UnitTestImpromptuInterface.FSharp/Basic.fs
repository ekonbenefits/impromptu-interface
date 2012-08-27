// Learn more about F# at http://fsharp.net
#if SILVERLIGHT
namespace UnitTestImpromptuInterface.FSharp.Silverlight
#else
namespace UnitTestImpromptuInterface.FSharp
#endif

open NUnit.Framework
open Swensen.Unquote
open System
open System.Dynamic
open ImpromptuInterface.FSharp
open ImpromptuInterface
open ImpromptuInterface.Dynamic
open System.Collections.Generic
open UnitTestSupportLibrary
open System.Xml.Linq
open System.Numerics
open Microsoft.CSharp.RuntimeBinder


module Module1=


    [<TestFixture>] 
    type ``Basic Dynamic Operator Tests`` ()=
                       

        [<Test>] member basic.``Call method off of an object dynamically`` ()=
                        test <@ "HelloWorld"?Substring(0,5) = "Hello" @>


        [<Test>] member basic.``Test Expando Set and Get`` ()=
                        let ex1 = ExpandoObject()
                        ex1?Test<-"Hi";
                        test <@ ex1?Test = "Hi" @>;


        [<Test>] member basic.``Test Direct Invoke`` ()=   
                        test <@ !?Impromptu.Curry(dynStaticContext(typeof<string>))?Format("Test {0} {1}") (1,2) = "Test 1 2" @>;


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
                        test <@ ex1?TestDel(2) = 5 @>


        [<Test>] member basic.``Test FSharp Lambda 3 arg `` ()=
                        let dyn = (fun x y z -> x + y - z) :> obj
                        test <@ !?dyn (3,2,1) = 4 @>
                    

        [<Test>] member basic.``Test FSharp Lambda 4 arg`` ()=
                        let dyn = (fun x y z bbq -> x + y - z - bbq) :> obj  in
                        test <@ !?dyn (3, 2, 1, 5) = -1 @>;


        [<Test>] member basic.``Test FSharp Lambda 5 arg`` ()=
                        let unknownfunc = (fun x y z bbq etc -> x + y - z - bbq + etc) :> obj in
                        test <@ !?unknownfunc (3, 2, 1, 5, 9) = 8 @>;


        [<Test>] member basic.``Test Events`` ()=
                        let pocoObj = TestEvent()
                        let refBool = ref false
                        let myevent = EventHandler<EventArgs>(fun obj arg -> (refBool := true))
                    
                        //Add event dynamically
                        dynAddAssign(pocoObj)?Event <- myevent
                        pocoObj.OnEvent(null,null)
                        test <@ !refBool @>
                   
                        //Remove event dynamically
                        refBool :=false
                        dynSubtractAssign(pocoObj)?Event <- myevent
                        test <@ not !refBool@>

                   
        [<Test>] member basic.``Test NamedArgs`` ()=
                        let buildObj = !?Build<ExpandoObject>.NewObject (
                                                                            dynArg(1) ? One,
                                                                            dynArg(2) ? Two
                                                                        )
                        test <@ buildObj?One = 1 @>
                        test <@ buildObj?Two = 2 @>


        [<Test>] member basic.``Test dynamic Explicit Conversion`` ()=
                        let ele = XElement(XName.Get("Test"),"50")
                        test <@ ele >>?>> typeof<Int32> = 50 @>


        [<Test>] member basic.``Test dynamic Implicit Conversion`` ()=
                        let ele = 50
                        test <@ ele >?> typeof<decimal> = decimal(50) @>


        [<Test>] member basic.``Test Explicit Conversion`` ()=
                        let ele = XElement(XName.Get("Test"),"50")
                        test <@ ele |> dynExplicit  = 50 @>


        [<Test>] member basic.``Test Implicit Conversion`` ()=
                        let ele = 50
                        test <@ ele |> dynImplicit = decimal(50) @>


        [<Test>] member basic.``Test Implicit Conversion Fail`` ()=
                        let ele = XElement(XName.Get("Test"),"50")
                        raises<RuntimeBinderException> <@ dynImplicit(ele)  = 50 @>