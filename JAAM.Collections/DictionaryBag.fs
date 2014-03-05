namespace JAAM.Collections

open System.Collections
open System.Collections.Generic
open System.Threading
open System
/// A dictionary that stores multiple values 'Key'
/// But only one value per Key and typeof<TValue>
type DictionaryBag<'TKey when 'TKey:equality> ()=
    let bag = Dictionary<System.Type,IDictionary>()
    // adds a key and value to a dictionary in the back specific to the values type

    member private this.innerBag<'TValue> () = 
        let typeof'TValue = typeof<'TValue>
        let innerCollection = 
                let hasValue, value = bag.TryGetValue(typeof'TValue)
                if hasValue then value
                else
                    let newSubBag = new Dictionary<'TKey,'TValue>() :>IDictionary
                    bag.Add(typeof'TValue, newSubBag)
                    newSubBag 
        innerCollection :?> Dictionary<'TKey,'TValue>

    member this.TryAdd<'TValue> (key:'TKey) (value:'TValue) = 
        let ic = this.innerBag<'TValue>()
        if ic.ContainsKey(key) then 
            false
        else 
            ic.Add(key,value) 
            true

    /// Uses Update factory
    member this.AddOrUpdate'<'TValue> (key:'TKey) (value:'TValue) (updateValueFactory: ('TKey -> 'TValue -> 'TValue)) = 
        let ic = this.innerBag<'TValue>()
        if ic.ContainsKey(key) then
            ic.[key] <- updateValueFactory key ic.[key]
        else
            ic.Add(key,value)
        ic.[key] 

    /// Uses Insert and Update factories
    member this.AddOrUpdate''<'TValue> (key:'TKey) (value: ('TKey -> 'TValue))  (updateValueFactory: ('TKey -> 'TValue -> 'TValue)) =
        let ic = this.innerBag<'TValue>()
        if ic.ContainsKey(key) then
            ic.[key] <- updateValueFactory key ic.[key]
        else
            ic.Add(key,value key)
        ic.[key]

    /// This is only for use from C# use the tick or double tick from f#
    member this.AddOrUpdate<'TValue> ((key:'TKey), (value:'TValue), (updateValueFactory: Func<'TKey,'TValue,'TValue>)) = 
        this.AddOrUpdate' key value (fun key' value' -> updateValueFactory.Invoke (key',value'))
        
    /// returns a tuple of (Found * 'TValue) where the Key = (key * typeof<'TValue>)
    member this.TryGetValue<'TValue> (key:'TKey) =            
        this.innerBag<'TValue>().TryGetValue key          // try to pull value out of inner dictionary
           
