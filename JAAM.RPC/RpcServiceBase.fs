namespace JAAM.RPC

open System

type public RPCServiceBase() as this= 
    inherit Object()
    let myDispatcher = new RPCService()
    do
        myDispatcher.AddToService (this)
    member val Dispatcher = myDispatcher with get
