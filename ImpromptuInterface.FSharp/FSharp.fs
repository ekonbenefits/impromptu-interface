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

module FSharp=

    open System
    open Microsoft.CSharp.RuntimeBinder
    open Microsoft.FSharp.Reflection
    open ImpromptuInterface
    open ImpromptuInterface.Dynamic

  

    let (?)  (target : obj) (name:string)  : 'TResult  = 
        let resultType = typeof<'TResult>
        if not (FSharpType.IsFunction resultType)
        then 
            let convert r =Impromptu.InvokeConvert(r,resultType)
            unbox (convert(Impromptu.InvokeGet(target, name)))
        else
            let lambda = fun arg ->
                               let argType,returnType = FSharpType.GetFunctionElements resultType

                               let cSharpArgs = 
                                    match argType with
                                    | a when FSharpType.IsTuple(a) -> FSharpValue.GetTupleFields(arg)
                                    | a when a = typeof<unit> -> [| |]
                                    | _ -> [|arg|]

                               let invoker k = Invocation(k, InvokeMemberName(name,null)).Invoke(target,cSharpArgs)

                               let (|Action|Func|) t = if t = typeof<unit> then Action else Func
                               let (|Invoke|InvokeMember|) n = if n = "_" then Invoke else InvokeMember
                               let (|NoConversion| Conversion|) t = if t = typeof<obj> then NoConversion else Conversion

                               let result =
                                    match (returnType, name) with
                                    | (Action,Invoke) -> invoker(InvocationKind.InvokeAction)
                                    | (Action,InvokeMember) -> invoker(InvocationKind.InvokeMemberAction)
                                    | (Func, Invoke) -> invoker(InvocationKind.Invoke)
                                    | (Func, InvokeMember) -> invoker(InvocationKind.InvokeMember)

                               match returnType with
                               | Action | NoConversion -> result
                               | _ -> Impromptu.InvokeConvert(result, returnType)

            unbox<'TResult> (FSharpValue.MakeFunction(resultType,lambda))

    let (?<-) (target : obj) (name : string) (value : 'TValue) : unit =
        Impromptu.InvokeSet(target, name, value) |> ignore
