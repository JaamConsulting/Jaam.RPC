using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JAAM.RPC.TestCS
{
    [TestClass]
    public class TestRpcService
    {


        [TestMethod]
        public void TestAddToService1ParamCS()
        {
            const string testString = "how long is this?";
            var exptectedLength = testString.Length;
            const int testId = 29;
            const string testMethodName = "testMethod";
            var fun = new Func<string, int>(inputString => inputString.Length);

            // SETUP
            var rpcHost = new JAAM.RPC.RPCService();
            rpcHost.AddToServiceCS(testMethodName, "inputString", fun);
            
            // INVOKE
            var callResult = rpcHost.Invoke(new RpcRequest<string>(testMethodName, testString, testId));

            Assert.AreEqual(testId, callResult.Id); // make sure we got a result for the same ID we passed in
            Assert.AreEqual(exptectedLength, callResult.Result); // make sure the result we got matches the calculation
            Assert.IsNull(callResult.Error); // make sure we didn't error

        }

        [TestMethod]
        public void TestAddToService1ParamCS_ByIdx()
        {
            const string testString = "how long is this?";
            var exptectedLength = testString.Length;
            const int testId = 29;
            const string testMethodName = "testMethod";
            var fun = new Func<string, int>(inputString => inputString.Length);

            // SETUP
            var rpcHost = new JAAM.RPC.RPCService();
            rpcHost.AddToServiceCS(testMethodName, "inputString", fun);

            // INVOKE
            var callResult = rpcHost.Invoke(0,new RpcRequest<string>(testMethodName, testString, testId));

            Assert.AreEqual(testId, callResult.Id); // make sure we got a result for the same ID we passed in
            Assert.AreEqual(exptectedLength, callResult.Result); // make sure the result we got matches the calculation
            Assert.IsNull(callResult.Error); // make sure we didn't error

        }

        [TestMethod]
        public void TestAddToService2ParamCS()
        {
            const string testString = "how long is this?";
            const int testMul = 23;
            const int testId = 29;
            const string testMethodName = "testMethod";

            var fun = new Func<string, int, int>((inputString, mul) => inputString.Length * mul);
            var expectedResult = fun(testString, testMul);

            // SETUP
            var rpcHost = new JAAM.RPC.RPCService();
            rpcHost.AddToServiceCS(testMethodName, "inputString","mul", fun);

            // INVOKE
            var callResult = rpcHost.Invoke(new RpcRequest<string,int>(testMethodName, testString, testMul, testId));

            Assert.AreEqual(testId, callResult.Id); // make sure we got a result for the same ID we passed in
            Assert.AreEqual(expectedResult, callResult.Result); // make sure the result we got matches the calculation
            Assert.IsNull(callResult.Error); // make sure we didn't error

        }

        [TestMethod]
        public void TestAddToService2ParamCS_byIdx()
        {
            const string testString = "how long is this?";
            const int testMul = 23;
            const int testId = 29;
            const string testMethodName = "testMethod";

            var fun = new Func<string, int, int>((inputString, mul) => inputString.Length * mul);
            var expectedResult = fun(testString, testMul);

            // SETUP
            var rpcHost = new JAAM.RPC.RPCService();
            rpcHost.AddToServiceCS(testMethodName, "inputString", "mul", fun);

            // INVOKE
            var callResult = rpcHost.Invoke(0, new RpcRequest<string, int>(testMethodName, testString, testMul, testId));

            Assert.AreEqual(testId, callResult.Id); // make sure we got a result for the same ID we passed in
            Assert.AreEqual(expectedResult, callResult.Result); // make sure the result we got matches the calculation
            Assert.IsNull(callResult.Error); // make sure we didn't error

        }

        [TestMethod]
        public void TestAddToService2ParamCS1()
        {
            const string testString = "how long is this?";
            const int testMul = 23;
            const int testId = 29;
            const string testMethodName = "testMethod";
            var fun = new Func<string, int, int>((inputString, mul) => inputString.Length * mul);
            var expectedResult = fun(testString, testMul);

            // SETUP
            var rpcHost = new JAAM.RPC.RPCService();
            rpcHost.AddToServiceCS(testMethodName, "inputString", "mul", fun);

            // INVOKE
            var callResult = rpcHost.Invoke(new RpcRequest<string,int>(testMethodName, testString, testMul, testId));

            Assert.AreEqual(testId, callResult.Id); // make sure we got a result for the same ID we passed in
            Assert.AreEqual(expectedResult, callResult.Result); // make sure the result we got matches the calculation
            Assert.IsNull(callResult.Error); // make sure we didn't error

        }

        [TestMethod]
        public void TestAddToService2ParamCS1_byIdx()
        {
            const string testString = "how long is this?";
            const int testMul = 23;
            const int testId = 29;
            const string testMethodName = "testMethod";
            var fun = new Func<string, int, int>((inputString, mul) => inputString.Length * mul);
            var expectedResult = fun(testString, testMul);

            // SETUP
            var rpcHost = new JAAM.RPC.RPCService();
            rpcHost.AddToServiceCS(testMethodName, "inputString", "mul", fun);

            // INVOKE
            var callResult = rpcHost.Invoke(0, new RpcRequest<string, int>(testMethodName, testString, testMul, testId));

            Assert.AreEqual(testId, callResult.Id); // make sure we got a result for the same ID we passed in
            Assert.AreEqual(expectedResult, callResult.Result); // make sure the result we got matches the calculation
            Assert.IsNull(callResult.Error); // make sure we didn't error
        }

        [TestMethod]
        public void TestAddToService2ParamCSExpectedFailBecauseParameterTypesDoNotMatchDelegateSignature()
        {
            const string testString = "how long is this?";
            const int testMul = 23;
            const int testId = 29;
            const string testMethodName = "testMethod";
            var fun = new Func<string, int, int>((inputString, mul) => inputString.Length * mul);
            var expectedResult = fun(testString, testMul);

            // SETUP
            var rpcHost = new JAAM.RPC.RPCService();
            rpcHost.AddToServiceCS(testMethodName, "inputString", "mul", fun);

            // INVOKE
            var callResult = rpcHost.Invoke(new RpcRequest<int, string>(testMethodName, testMul, testString, testId));

            Assert.IsNotNull(callResult.Error); // make sure we didn't error
            Assert.AreEqual(testId, callResult.Id); // make sure we got a result for the same ID we passed in
            Assert.AreNotEqual(expectedResult, callResult.Result); // make sure the result we got matches the calculation
        }
    }
}
