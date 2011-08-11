module Swensen.Unquote
open Microsoft.FSharp.Quotations
open NUnit.Framework

let test (x:Expr<bool>) : unit = Assert.Inconclusive("Unquote not supported on silverlight")

let raises<'TType> (x:Expr<bool>) : unit = Assert.Inconclusive("Unquote not supported on silverlight")
