module JAAM.Collections.TestFS
open Xunit
open JAAM.Collections


[<Fact>]
let DictionaryBag_Can_Add_Value ():unit= 
    let dict = DictionaryBag<string>()
    let key = "key"
    let value = "foo"
    let canAdd = dict.TryAdd key value
    Assert.True canAdd
    0|>ignore

[<Fact>]
let DictionaryBag_Can_Add_Value_And_Get_Back ():unit= 
    let dict = DictionaryBag<string>()
    let key = "key"
    let value = "Foo"
    let success = dict.TryAdd key value
    let found, outvalue = dict.TryGetValue<string> key

    Assert.True (success, "Adding Value")
    Assert.True (found, "Fetch Value")
    Assert.Equal<string> (value, outvalue)

    0|>ignore

[<Fact>]
let DictionaryBag_TryAdd_Wont_Overwrite_Value ():unit= 
    let dict = DictionaryBag<string>()
    let key = "key"
    let value1 = "Foo"
    let value2 = "Foo2"
    let added1 = dict.TryAdd key value1
    let added2 = dict.TryAdd key value2
    let found, outvalue = dict.TryGetValue<string> key

    Assert.True (added1, "Adding Value1")
    Assert.False (added2, "Adding Value2")
    Assert.True (found, "Fetch Value")
    Assert.Equal<string> (value1, outvalue) //make sure its the 1st value added that we retrieve

    0|>ignore

[<Fact>]
let DictionaryBag_TryAdd_Multiple_Values ():unit= 
    let dict = DictionaryBag<string>()
    let key = "key"
    let value1 = "Foo"
    let value2 = "Foo2"|> Seq.map (fun x -> x) |> Seq.toArray 
    let value3 = 1337
    let added1 = dict.TryAdd key value1
    let added2 = dict.TryAdd key value2
    let added3 = dict.TryAdd key value3

    Assert.True (added1, "Adding Value1")
    Assert.True (added2, "Adding Value2")
    Assert.True (added3, "Adding Value3")
 
    0|>ignore

[<Fact>]
let DictionaryBag_TryAdd_Multiple_Values_And_Retrieve ():unit= 
    let dict = DictionaryBag<string>()
    let key = "key"
    let value1 = "Foo"
    let value2 = "Foo2"|> Seq.map (fun x -> x) |> Seq.toArray 
    let value3 = 1337
    let added1 = dict.TryAdd key value1
    let added2 = dict.TryAdd key value2
    let added3 = dict.TryAdd key value3

    Assert.True (added1, "Adding Value1")
    Assert.True (added2, "Adding Value2")
    Assert.True (added3, "Adding Value3")
    
    let found1, outvalue1 = dict.TryGetValue<string> key
    let found2, outvalue2 = dict.TryGetValue<char[]> key
    let found3, outvalue3 = dict.TryGetValue<int> key

    Assert.True (found1, "Fetch Value1")
    Assert.True (found2, "Fetch Value2")
    Assert.True (found3, "Fetch Value3")
    Assert.Equal<string> (value1, outvalue1) //make sure its the string value added that we retrieve
    Assert.Equal<char[]> (value2, outvalue2) //make sure its the char{} value added that we retrieve
    Assert.Equal<int> (value3, outvalue3) //make sure its the int value added that we retrieve

    0|>ignore

[<Fact>]
let DictionaryBag_AddOrUpdate' ():unit= 
    let dict = DictionaryBag<string>()
    let key = "key"
    let value1 = "Foo"
    let value2 = "Foo2" 
   
    let added1 = dict.TryAdd  key value1
    let added2 = dict.AddOrUpdate'  key value2 (fun k v->value2)
    
    Assert.True (added1, "Adding Value1")
    Assert.Equal<string> (value2, added2)
  
    0|>ignore

