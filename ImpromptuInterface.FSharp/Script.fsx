// This file is a script that can be executed with the F# Interactive.  
// It can be used to explore and test the library project.
// Note that script files will not be part of the project build.

#r "Microsoft.CSharp.dll"
#r "bin\Release\ImpromptuInterface.dll"

#load "FSharp.fs"
open ImpromptuInterface.FSharp
open ImpromptuInterface;

open System.Dynamic;
open System.Collections;
let e1 = ExpandoObject()
e1?Test <- "Hello World"
printfn "%s" e1?Test


let sub : string = e1?Test?Substring(0,5) 
printfn "%s" sub

let a = ArrayList()

a?Add("5")
printfn "%s" (a.Item(0).ToString())

let Static t = InvokeContext.CreateStatic.Invoke(t)

let c : obj =Impromptu.Curry(Static(typeof<string>))?Format("Test {0} {1}");
let test : string = c?``_``(1,2)
printfn "%s" test


