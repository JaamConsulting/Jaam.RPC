namespace JAAM.RPC

open System


type RpcRequest<'T> (method':string, params':'T, id:int) =
    member this.Method = method'
    member this.Params:'T = params'
    member this.Id = id

type RpcRequest<'T1,'T2> (method':string, arg1:'T1, arg2:'T2, id:int) =
    inherit RpcRequest<'T1*'T2>(method',(arg1,arg2),id)

type RpcRequest<'T1,'T2,'T3> (method':string, arg1:'T1, arg2:'T2,arg3:'T3, id:int) =
    inherit RpcRequest<'T1*'T2*'T3>(method',(arg1,arg2,arg3),id)
    
[<Struct>]
type RpcRequest (method':string, params':Object, id:int) =
    member this.Method = method'
    member this.Params = params'
    member this.Id = id

[<Struct>]
type RpcResponse (result:Object, error:RpcException, id:int) =
    new(error,id)  = RpcResponse (null, error,id)
    new(result,id) = RpcResponse (result,null,id)
    member this.Result = result  
    member this.Error = error
    member this.Id = id