namespace JAAM.RPC

open System
open System.Collections.Concurrent
open System.Threading.Tasks
open System.Collections
open Microsoft.FSharp.Collections
open System.Reflection
open System.Linq.Expressions

open JAAM.Collections

type public Input (name, dataType)=
    inherit Tuple<string,System.Type>(name,dataType)

type public Input<'T> (name)=
    inherit Input(name,typeof<'T>)

type public RPCService() = 
    inherit Object()
    member val handlers = ThreadCachedConcurrentDictionary<string,Delegate * RpcDefinition>()
    member val handlersBag = DictionaryBag<string>()
    member val handlersListBag = ListBag()
    member val contexts = ConcurrentDictionary<int,Object>()

    member this.Invoke (rpc:RpcRequest): RpcResponse =
            match rpc.Params with
//            | :? ICollection as p when p.Count = 2 ->                     
//                    let hasHandler, both = this.handlers2Param.TryGetCachedValue rpc.Method                    
//                    if hasHandler then
//                        let (handle, meta) = both                       
//                        let paramArray = p :?> array<System.Object>
//                        if paramArray.Length = meta.ParameterCount then
//                            try
//                                let results = handle paramArray.[0] paramArray.[1]
//                                new RpcResponse(results, rpc.Id)
//                            with
//                                | _ as ex -> 
//                                    new RpcResponse(new RpcException(-32603,"Internal Error", ex),rpc.Id)
//                        else new RpcResponse (new RpcException(-32602,"Invalid params","The number of Parameters could not be counted"),rpc.Id)
//                    else new RpcResponse(new RpcException(-32601, "Method not found", "The method does not exist / is not available."), rpc.Id )
//            | :? ICollection as p when p.Count = 1 ->                     
//                    let hasHandler, both = this.handlers1Param.TryGetCachedValue rpc.Method
//                    
//                    if hasHandler then
//                        let (handle, meta) = both                       
//                        let paramArray = p :?> array<System.Object>
//                        if paramArray.Length = meta.ParameterCount then
//                            try
//                                let results = handle (paramArray.[0])
//                                new RpcResponse(results, rpc.Id)
//                            with
//                                | _ as ex -> 
//                                    new RpcResponse(new RpcException(-32603,"Internal Error", ex),rpc.Id)
//                        else new RpcResponse (new RpcException(-32602,"Invalid params","The number of Parameters could not be counted"),rpc.Id)
//                    else new RpcResponse(new RpcException(-32601, "Method not found", "The method does not exist / is not available."), rpc.Id )                            
            | :? ICollection  as p ->                     
                    let hasHandler, (handle, meta) = this.handlers.TryGetCachedValue rpc.Method
                    if hasHandler = false then
                        new RpcResponse(new RpcException(-32601, "Method not found", "The method does not exist / is not available."), rpc.Id )
                    else                                                          
                        let paramArray = p :?> array<System.Object>
                        if paramArray.Length <> meta.ParameterCount then
                            new RpcResponse (new RpcException(-32602,"Invalid params","The number of Parameters could not be counted"),rpc.Id)
                        else
                            try
                                let results = handle.DynamicInvoke(paramArray)
                                new RpcResponse(results, rpc.Id)
                            with | _ as ex -> new RpcResponse(new RpcException(-32603,"Internal Error", ex),rpc.Id)                               
            | _  -> new RpcResponse (new RpcException(-32602,"Invalid params","The number of Parameters could not be counted"),rpc.Id)

    member this.Invoke (rpcs:seq<RpcRequest>) = rpcs |> Seq.map(this.Invoke)
    member this.InvokeAsyncFS (rpc:RpcRequest) = async {return this.Invoke(rpc) }
    member this.InvokeAsync (rpc:RpcRequest) = Async.StartAsTask(this.InvokeAsyncFS rpc)
    member this.InvokeBatch (rpcs:seq<RpcRequest>) =        
        rpcs
        |> Seq.map(this.InvokeAsyncFS) 
        |> Async.Parallel 
        |> Async.RunSynchronously
    
    

    member this.Invoke<'T1, 'T2, 'T3> (rpc:RpcRequest<'T1 * 'T2 * 'T3>) : RpcResponse =
        let hasboth, metaAndHandle = this.handlersBag.TryGetValue<RpcDefinition*('T1->'T2->'T3->Object)> rpc.Method
        if hasboth then
            try
                let (meta, handle) = metaAndHandle
                let p1,p2,p3 = rpc.Params
                let results = handle p1 p2 p3
                new RpcResponse(results, rpc.Id)
            with | _ as ex -> new RpcResponse(new RpcException(-32603,"Internal Error",ex),rpc.Id)
        else this.Invoke (RpcRequest(rpc.Method,rpc.Params:>Object,rpc.Id))                          // Fallback to the boxing/unboxing Invoke
    
    member this.Invoke<'T1, 'T2> (rpc:RpcRequest<'T1 * 'T2>) : RpcResponse =
        let hasboth, metaAndHandle = this.handlersBag.TryGetValue<RpcDefinition*('T1->'T2->Object)> rpc.Method
        if hasboth then
            try
                let (meta, handle) = metaAndHandle
                let p1,p2 = rpc.Params
                let results = handle p1 p2
                new RpcResponse(results, rpc.Id)
            with | _ as ex -> new RpcResponse(new RpcException(-32603,"Internal Error",ex),rpc.Id)
        else this.Invoke (RpcRequest(rpc.Method,rpc.Params:>Object,rpc.Id))                          // Fallback to the boxing/unboxing Invoke

    member this.Invoke<'T1> (rpc : RpcRequest<'T1>) : RpcResponse =
        let hasboth, metaAndHandle = this.handlersBag.TryGetValue<RpcDefinition*('T1->Object)> rpc.Method
        if hasboth then
            try
                let (meta, handle) = metaAndHandle
                let results = handle rpc.Params
                new RpcResponse(results, rpc.Id)
            with | _ as ex -> new RpcResponse(new RpcException(-32603,"Internal Error",ex),rpc.Id)
        else this.Invoke (RpcRequest(rpc.Method,rpc.Params:>Object,rpc.Id))                          // Fallback to the boxing/unboxing Invoke
    
        member this.Invoke<'T1, 'T2> (fnIdx, rpc:RpcRequest<'T1 * 'T2>) : RpcResponse = 
        let meta, handle = this.handlersListBag.TryGetValue<RpcDefinition*('T1->'T2->Object)> fnIdx
        try
            let p1,p2 = rpc.Params
            let results = handle p1 p2
            new RpcResponse(results, rpc.Id)
        with | _ as ex -> new RpcResponse(new RpcException(-32603,"Internal Error",ex),rpc.Id)

    member this.Invoke<'T1> (fnIdx, rpc:RpcRequest<'T1>) : RpcResponse = 
        let meta, handle = this.handlersListBag.TryGetValue<RpcDefinition*('T1->Object)> fnIdx
        try
            let results = handle rpc.Params
            new RpcResponse(results, rpc.Id)
        with | _ as ex -> new RpcResponse(new RpcException(-32603,"Internal Error",ex),rpc.Id)
            
    // creates a method that wraps the passed in method that takes object parameters and casts them to the real type
    // essentially given (fun ('a 'b 'c)->d) -> (fun (obj obj obj)->d) it does this by wrapping the passed in function with another one
    // with the same number of parameters but as objects, it then convertes the object parameters to the cooresponding type, and then invokes the origional
    // method.
    // TODO: need the non-instance * static version of this method
//    member this.DelegateCaster (p:seq<string * Type>) fn instance=      
//        // create helper method to convert the sequence of tuples to a parameter and convert expressions
//        let rec toCastExpression (s:List<string *Type>) = 
//            let toExpression j = Expression.Parameter(typeof<Object>,fst j)
//            let toCast pEx nameType = Expression.ConvertChecked(pEx,snd nameType)
//            match s |> List.length with   
//                     
//            | x when x < 2  -> [] // always ignore the last item as it is the return type
//            | x -> let px = (List.head s) |> toExpression       // take head item and create a parameter expression with it
//                   let exp1 = (List.head s) |> toCast px        // take head item and parameter expression can cast it to the correct type
//                   (px,exp1) :: ((List.tail s) |> toCastExpression) // concatenate the tuple of the (parameterExpression,CastExpression) with the rest                 
//        let castPs = p |> Seq.toList |> toCastExpression 
//        let converts = castPs |> Seq.map(fun e -> snd e :> Expression) // extract all convertExpressions
//        let parray2 = castPs |> Seq.map(fun e -> fst e )               // extract all parameterExpressions
//        let cal = Expression.Call(instance, fn, converts)            // create a function call expression that calls fn on the instance passing in the converted parameters
//        let call = Expression.Lambda(cal,parray2) // create a lambda that calls the call expressions, the lamda has the parameter expressions
//        call.Compile() // compile and return it

    //TODO: Make this method have a required parameter that defines (Add/Overwrite) so that they don't blame us for shooting themselves in the foot
    member this.AddToService ((methodName:string), (parameters: seq<string * Type>), (fn:Delegate)) : Unit =
        let meta = RpcDefinition (methodName, parameters)
        let v = (fn, meta)
        this.handlers.AddOrUpdate (methodName, v, (fun m f -> v))
        |> ignore

    member this.AddToService1Param ((methodName:string), (parameters: seq<string * Type>), (functionToWrap:('T->'TResult))) : Unit =
        let wrappedFn = fun x -> functionToWrap x :> Object
        let meta = RpcDefinition (methodName, parameters)
        this.handlersBag.TryAdd methodName (meta,wrappedFn) |> ignore  
        this.handlersListBag.TryAdd (meta,wrappedFn) |> ignore  

    member this.AddToService2Param ((methodName:string), (parameters: seq<string * Type>), (functionToWrap:('T1->'T2->'TResult))) : Unit =
        let wrappedasFS = fun (p1:'T1) (p2:'T2) -> functionToWrap p1 p2 :> Object
        let meta = RpcDefinition (methodName, parameters)
        this.handlersBag.TryAdd methodName (meta,wrappedasFS) |> ignore
        this.handlersListBag.TryAdd (meta,wrappedasFS) |> ignore  
    // C# Compatability Functions

    /// Used to add a function to the service
    /// methodName is the key to call the method
    /// parameters parameters should be the name and type of each input parameter
    /// functionToWrap is the function that you want to be called
    member this.AddToServiceCS ((methodName:string), (parameterName: String), (functionToWrap:System.Func<'T,'TResult>)) : Unit =
        let asFsharpParameters = [(parameterName, typeof<'T>)]// convert to f# tuple
        let wrappedFn = fun x -> functionToWrap.Invoke(x) :> Object // pass FunctionToWrap to a Func that returns a Func that invokes that function
        let meta = RpcDefinition (methodName, asFsharpParameters)
        this.handlersBag.TryAdd methodName (meta,wrappedFn) |> ignore
        this.handlersListBag.TryAdd (meta,wrappedFn) |> ignore  

    member this.AddToServiceCS ((methodName:string), (namedParameter1: String), (namedParameter2: String), (functionToWrap:System.Func<'T1,'T2,'T3>)) : Unit =
        let asFsharpParameters =  [(namedParameter1, typeof<'T1>);(namedParameter2, typeof<'T2>)]// convert to f# tuple
        let wrappedasFS = fun a b -> functionToWrap.Invoke(a, b) :> Object
        let meta = RpcDefinition (methodName, asFsharpParameters)
        this.handlersBag.TryAdd methodName (meta,wrappedasFS) |> ignore
        this.handlersListBag.TryAdd (meta,wrappedasFS) |> ignore  

    // END C# compatablility Functions

    //TODO: We need another signature that takes the object and a collection of attributes that does the mapping in the case where the type was not implemented by the user        
    member this.AddToService (serviceInstance:System.Object): Unit =
        let theType = serviceInstance.GetType()
        let jsonMethods = seq {for m in theType.GetMethods(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance) do
                                if m.GetCustomAttributes(typeof<RpcMethodAttribute>, false).Length > 0 then
                                    yield m}
        for jm in jsonMethods do
            let paras = Seq.append  (seq {for p in jm.GetParameters() do
                                            yield (p.Name, p.ParameterType)}) 
                                    [("returns", jm.ReturnType)]
            for attrib in jm.GetCustomAttributes(typeof<RpcMethodAttribute>,false) do
                let methodName = attrib :?> RpcMethodAttribute 
                                    |> fun x->x.MethodName
                let methodNameToUse = if methodName.Length>0 then
                                        methodName
                                      else
                                        jm.Name   
                                                     
                let newDelegate = Delegate.CreateDelegate(Expression.GetDelegateType( Seq.toArray (seq { for p in paras do yield snd p})), serviceInstance,jm)
                let exObj = Expression.Constant( serviceInstance )
                let len = paras |> Seq.length
               // let wrapped  = this.DelegateCaster paras jm exObj
                match len  with // -1 to account for result type data
                | 3 -> // Get the generic method from this class to pass the delegate to.
                    let ourRegisterMethod = this.GetType().GetMethods() 
                                                |> Seq.where (fun m -> m.Name = "AddToService2ParamCS")
                                                |> Seq.toList
                                                |> List.head 
                    let paramTypeArray = paras |> Seq.map (fun x-> snd x) |> Seq.toArray
                    let step2 = ourRegisterMethod.MakeGenericMethod( paramTypeArray )
                    try 
                        step2.Invoke (this, [| methodNameToUse; paras; newDelegate |])
                        |> ignore
                    with
                    | _ as ex -> ex |> ignore
                | 2 -> // Get the generic method from this class to pass the delegate to.
                    let ourRegisterMethod = this.GetType().GetMethods() 
                                                |> Seq.where (fun m -> m.Name = "AddToService1ParamCS")
                                                |> Seq.toList
                                                |> List.head 
                    let step2 = ourRegisterMethod.MakeGenericMethod( paras |> Seq.map (fun x-> snd x) |> Seq.toArray )
                    try 
                        step2.Invoke (this, [| methodNameToUse; paras; newDelegate |])
                        |> ignore
                    with
                    | _ as ex -> ex |> ignore
                    //this.AddToService1Param(methodNameToUse, paras, newDelegate)
                | _ -> this.AddToService (methodNameToUse, paras, newDelegate)        
       

        

        
