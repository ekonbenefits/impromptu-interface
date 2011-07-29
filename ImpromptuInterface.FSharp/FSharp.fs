// 
//  Copyright 2011  Ekon Benefits
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
namespace ImpromptuInterface

///Module to add DLR dynamic invocation to FSharp through the dynamic operator (?)
module FSharp=

    open System
    open Microsoft.CSharp.RuntimeBinder
    open Microsoft.FSharp.Reflection
    open ImpromptuInterface
    open ImpromptuInterface.Dynamic
  
    ///Dynamic get property or method invocation
    let (?)  (target : obj) (name:string)  : 'TResult  = 
        let resultType = typeof<'TResult>
        let (|NoConversion| Conversion|) t = if t = typeof<obj> then NoConversion else Conversion

        if not (FSharpType.IsFunction resultType)
        then 
            let convert r = match resultType with
                                | NoConversion -> r
                                | _ -> Impromptu.InvokeConvert(r,resultType)

            Impromptu.InvokeGet(target, name) |> convert |> unbox
        else
            let lambda = fun arg ->
                               let argType,returnType = FSharpType.GetFunctionElements resultType

                               let argList = 
                                    match argType with
                                    | a when FSharpType.IsTuple(a) -> FSharpValue.GetTupleFields(arg)
                                    | a when a = typeof<unit> -> [| |]
                                    | _ -> [|arg|]

                               let invoker k = Invocation(k, InvokeMemberName(name,null)).Invoke(target,argList)

                               let (|Action|Func|) t = if t = typeof<unit> then Action else Func
                               let (|Invoke|InvokeMember|) n = if n = "_" then Invoke else InvokeMember
                               
                               let result =
                                    try
                                        match (returnType, name) with
                                        | (Action,Invoke) -> invoker(InvocationKind.InvokeAction)
                                        | (Action,InvokeMember) -> invoker(InvocationKind.InvokeMemberAction)
                                        | (Func, Invoke) -> invoker(InvocationKind.Invoke)
                                        | (Func, InvokeMember) -> invoker(InvocationKind.InvokeMember)
                                    with  //Last chance incase we are trying to invoke an fsharpfunc 
                                        |  :? Microsoft.CSharp.RuntimeBinder.RuntimeBinderException as e  -> 
                                            try
                                                let invokeName =InvokeMemberName("Invoke", null) //FSharpFunc Invoke
                                                let invokeContext t = InvokeContext(t,typeof<obj>) //Improve cache hits by using the same context
                                                let invokeFSharpFoldBack (a:obj) t  = Impromptu.InvokeMember(invokeContext(t),invokeName,a)
                                                let seed = match name with
                                                           |InvokeMember -> Impromptu.InvokeGet(target,name)
                                                           |Invoke->target
                                                List.foldBack invokeFSharpFoldBack (argList |> List.ofArray |> List.rev) seed
                                            with
                                                | :? Microsoft.CSharp.RuntimeBinder.RuntimeBinderException as e2 -> AggregateException(e,e2) |> raise

                               match returnType with
                               | Action | NoConversion -> result
                               | _ -> Impromptu.InvokeConvert(result, returnType)

            FSharpValue.MakeFunction(resultType,lambda) |> unbox<'TResult>

    ///Dynamic set property
    let (?<-) (target : obj) (name : string) (value : 'TValue) : unit =
        Impromptu.InvokeSet(target, name, value) |> ignore

    ///Prefix operator that allows direct dynamic invocation of the object
    let (!?) (target:obj) : 'TResult =
        target?``_``