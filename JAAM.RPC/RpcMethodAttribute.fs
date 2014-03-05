namespace JAAM.RPC

open System

/// <summary>
/// Use to expose a method to the Rpc Engine.
/// </summary>
[<Sealed>]
[<AttributeUsageAttribute(AttributeTargets.Method, Inherited = false, AllowMultiple = true)>]
type RpcMethodAttribute (methodName:string) = 
    inherit Attribute()  
    let mutable _Name = methodName
    new() = RpcMethodAttribute("")
    
    member this.MethodName with get() = _Name
