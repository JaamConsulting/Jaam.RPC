namespace JAAM.Collections

open System.Collections.Concurrent
open System.Threading
open System

type ThreadCachedConcurrentDictionary<'TKey, 'TValue when 'TKey:equality> () =
    inherit ConcurrentDictionary<'TKey,'TValue>()
    let lastName = new ThreadLocal<'TKey>()
    let lastResult = new ThreadLocal<bool * 'TValue>()
    member this.TryGetCachedValue (key:'TKey) =
        if lastName.Value = key then
            lastResult.Value
        else
        let has, h = base.TryGetValue(key)
        if has then
            lastName.Value <- key
            let result = (true,h)
            lastResult.Value <- result
            result
        else
            (false, Unchecked.defaultof<'TValue>)