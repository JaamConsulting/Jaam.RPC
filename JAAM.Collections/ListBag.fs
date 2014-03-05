namespace JAAM.Collections

open System.Collections
open System.Collections.Generic
open System.Threading
open System
/// A dictionary that stores multiple values 'Key'
/// But only one value per Key and typeof<TValue>
type ListBag()=
    let bag = List<Object>()
    // adds a key and value to a dictionary in the back specific to the values type

    member this.TryAdd<'TValue> (value:'TValue) = 
        bag.Add(value)
        true

    /// This is only for use from C# use the tick or double tick from f#
    member this.AddOrUpdate<'TValue> (key, (value:'TValue), (updateValueFactory: Func<_,'TValue,'TValue>)) = 
        let idx = bag.IndexOf (value :> Object)
        if idx >= 0 then
            bag.[idx] <- updateValueFactory.Invoke (key,bag.[idx] :?> 'TValue) 
        else
            bag.Add value
        
    /// returns a tuple of (Found * 'TValue) where the Key = (key * typeof<'TValue>)
    member this.TryGetValue<'TValue> (key) =            
        bag.[key] :?> 'TValue          // try to pull value out of inner dictionary
           
