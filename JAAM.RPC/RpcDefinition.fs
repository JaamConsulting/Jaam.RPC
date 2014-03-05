namespace JAAM.RPC

open System
open System.Collections.Concurrent
open Microsoft.FSharp.Collections
open Microsoft.FSharp
open System.Collections.Generic

[<AllowNullLiteralAttribute>]
type RpcDefinition (methodName,parameters) =
    let _expectsRefException = parameters 
                                |> Seq.last 
                                |> snd
                                |> fun t -> t = typeof<RpcException>
    let _paramCount = parameters |> Seq.length |> (fun a-> if _expectsRefException then 
                                                                 a-2
                                                              else 
                                                                 a-1)
    member this.ParameterCount  = _paramCount
    member val Name:string = methodName with get
    member val Parameters:seq<string*Type> = parameters with get    
    member this.ExpectsRefException = _expectsRefException
         