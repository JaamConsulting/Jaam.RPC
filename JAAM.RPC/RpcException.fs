namespace JAAM.RPC

open System

[<AllowNullLiteral>]
type RpcException (code:int,message:string,data:Object) =
    inherit System.ApplicationException()
    member val code = code with get,set
    member val message = message with get,set
    member val data = data with get,set