namespace JAAM.RPC

open System

[<AllowNullLiteral>]
type RpcException (code:int,message:string,data:Object,innerException) =
    inherit System.ApplicationException(message,innerException)
    member val code = code with get,set
    member val data = data with get,set
    new (code, message, data) = RpcException(code, message, data, null)