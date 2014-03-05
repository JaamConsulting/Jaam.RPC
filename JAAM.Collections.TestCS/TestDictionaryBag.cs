using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace JAAM.Collections.TestCS
{
    [TestClass]
    public class TestDictionaryBag
    {
        [TestMethod]
        public void DictionaryBag_Can_Add_Value()
        {
            var dict = new DictionaryBag<string>();
            var key = "key";
            var value = "Foo";

            var canAdd = dict.TryAdd(key, value);
            Assert.IsTrue(canAdd);
        }

        [TestMethod]
        public void DictionaryBag_Can_Add_Value_And_Get_Back()
        {
            var dict = new DictionaryBag<string>();
            var key = "key";
            var value = "Foo";

            var added = dict.TryAdd(key, value);
            var outTuple = dict.TryGetValue<string>(key);

            Assert.IsTrue(added,"Adding Value");
            Assert.IsTrue(outTuple.Item1, "Retrieved Value");
            Assert.AreEqual(value, outTuple.Item2);
        }

        [TestMethod]
        public void DictionaryBag_TryAdd_Wont_Overwrite_Value()
        {
            var dict = new DictionaryBag<string>();
            var key = "key";
            var value1 = "Foo";
            var value2 = "Foo2";

            var added1 = dict.TryAdd(key, value1);
            var added2 = dict.TryAdd(key, value2);
            var outTuple = dict.TryGetValue<string>(key);

            Assert.IsTrue(added1, "Adding Value1");
            Assert.IsFalse(added2, "Adding Value2");

            Assert.IsTrue(outTuple.Item1, "Retrieved Value");
            Assert.AreEqual(value1, outTuple.Item2); // make sure its the 1st value added that we retrieve
        }

        [TestMethod]
        public void DictionaryBag_TryAdd_Multiple_Values()
        {
            var dict = new DictionaryBag<string>();
            var key = "key";
            var value1 = "Foo";
            var value2 = "Foo2".Select(x=>x).ToArray();
            var value3 = 1337;

            var added1 = dict.TryAdd(key, value1);
            var added2 = dict.TryAdd(key, value2);
            var added3 = dict.TryAdd(key, value3);

            Assert.IsTrue(added1, "Adding Value1");
            Assert.IsTrue(added2, "Adding Value2");
            Assert.IsTrue(added3, "Adding Value3");
        }

        [TestMethod]
        public void DictionaryBag_TryAdd_Multiple_Values_And_Retrieve()
        {
            var dict = new DictionaryBag<string>();
            var key = "key";
            var value1 = "Foo";
            var value2 = "Foo2".Select(x => x).ToArray();
            var value3 = 1337;

            var added1 = dict.TryAdd(key, value1);
            var added2 = dict.TryAdd(key, value2);
            var added3 = dict.TryAdd(key, value3);

            Assert.IsTrue(added1, "Adding Value1");
            Assert.IsTrue(added2, "Adding Value2");
            Assert.IsTrue(added3, "Adding Value3");

            var out1 = dict.TryGetValue<string>(key);
            var out2 = dict.TryGetValue<char[]>(key);
            var out3 = dict.TryGetValue<int>(key);

            Assert.IsTrue(out1.Item1, "Fetch Value1");
            Assert.IsTrue(out2.Item1, "Fetch Value2");
            Assert.IsTrue(out3.Item1, "Fetch Value3");

            Assert.AreEqual(value1, out1.Item2);
            Assert.AreEqual(value2, out2.Item2);
            Assert.AreEqual(value3, out3.Item2);
        }

        [TestMethod]
        public void DictionaryBag_AddOrUpdate()
        {
            var dict = new DictionaryBag<string>();
            var key = "key";
            var value1 = "Foo";
            var value2 = "Foo2";

            var added1 = dict.TryAdd(key, value1);
            var added2 = dict.AddOrUpdate(key, value2, (x,y)=>value2);

            Assert.IsTrue(added1, "Adding Value1");
            Assert.AreEqual(value2,added2);
        }
    }
}
