namespace JAAM.Collections

open System.Collections
open System.Collections.Concurrent
open System.Threading
open System
/// A dictionary that stores multiple values 'Key'
/// But only one value per Key and typeof<TValue>
type ConcurrentDictionaryBag<'TKey when 'TKey:equality> ()=
    let bag = ConcurrentDictionary<System.Type,IDictionary>()
    // adds a key and value to a dictionary in the back specific to the values type

    let lastName = new ThreadLocal<'TKey>()         // Cache last Key requested
    let lastResult = new ThreadLocal<Object>()      // Cache last value requested
    member private this.ic<'TValue> () = 
        let innerCollection = bag.GetOrAdd(typeof<'TValue>, (fun (t':System.Type) -> new ConcurrentDictionary<'TKey,'TValue>() :> IDictionary )) 
        innerCollection :?> ConcurrentDictionary<'TKey,'TValue>

    member this.TryAdd<'TValue> (key:'TKey) (value:'TValue) = 
        let ic = this.ic<'TValue>()
        ic.TryAdd(key,value) 

    /// Uses Update factory
    member this.AddOrUpdate'<'TValue> (key:'TKey) (value:'TValue) (updateValueFactory: ('TKey -> 'TValue -> 'TValue)) = 
        let ic = this.ic<'TValue>()
        ic.AddOrUpdate(key,value,updateValueFactory) 

    /// Uses Insert and Update factories
    member this.AddOrUpdate''<'TValue> (key:'TKey) (value: ('TKey -> 'TValue))  (updateValueFactory: ('TKey -> 'TValue -> 'TValue)) =
        let ic = this.ic<'TValue>()
        ic.AddOrUpdate(key,value,updateValueFactory)

    /// This is only for use from C# use the tick or double tick from f#
    member this.AddOrUpdate<'TValue> ((key:'TKey), (value:'TValue), (updateValueFactory: Func<'TKey,'TValue,'TValue>)) = 
        this.AddOrUpdate' key value (fun key' value' -> updateValueFactory.Invoke (key',value'))
        
    /// returns a tuple of (Found * 'TValue) where the Key = (key * typeof<'TValue>)
    member this.TryGetValue<'TValue> (key:'TKey) =
        if lastName.Value = key && lastResult.Value :? 'TValue then (true, lastResult.Value :?> 'TValue) // return from threadCache if matches
        else
            let found, innerCollection = bag.TryGetValue(typeof<'TValue>)    // missed threadCache, pull inner dictionary
            if found then
                let ic = innerCollection :?> ConcurrentDictionary<'TKey,'TValue> // Cast as the type we know it has to be
                let f, o = ic.TryGetValue key           // try to pull value out of inner dictionary
                if f then                               // if we found our value, store the results in the thread cache
                    lastName.Value <- key               // store key in threadCache
                    lastResult.Value <- o               // store value in threadCache
                (f,o)                                   // return what we found
            else
                (false, Unchecked.defaultof<'TValue>)   // if we didn't have an inner collection, then return (false and default<T>)
