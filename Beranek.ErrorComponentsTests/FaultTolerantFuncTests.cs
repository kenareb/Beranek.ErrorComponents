namespace Beranek.ErrorComponents.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Diagnostics;

    [TestClass()]
    public class FaultTolerantFuncTests
    {
        [TestMethod()]
        public void LinearRetryTest()
        {
            int invocationCounter = 0;
            var sw = new Stopwatch();

            var error = new Func<int>(() =>
            {
                invocationCounter++;
                throw new NotImplementedException();
            });

            var retry = new FaultTolerantFunc<int>(error, 5);

            sw.Start();
            var result = retry.Invoke();
            sw.Stop();

            Assert.AreEqual(5, invocationCounter);
            Assert.AreEqual(5, retry.Tries);
            Assert.IsNotNull(retry.Exception);
            Assert.IsTrue(sw.ElapsedMilliseconds >= 4000);
            Assert.IsTrue(sw.ElapsedMilliseconds < 4500);
            Assert.AreEqual(0, result);
        }

        [TestMethod()]
        public void ExponentialRetryTest()
        {
            int invocationCounter = 0;
            var sw = new Stopwatch();

            var error = new Func<bool>(() =>
            {
                invocationCounter++;
                throw new NotImplementedException();
            });

            var retry = new FaultTolerantFunc<bool>(error, 5, new ExponentialRetryStrategy(1000));

            sw.Start();
            retry.Invoke();
            sw.Stop();

            Assert.AreEqual(5, invocationCounter);
            Assert.AreEqual(5, retry.Tries);
            Assert.IsNotNull(retry.Exception);
            Assert.IsTrue(sw.ElapsedMilliseconds >= 15000);
            Assert.IsTrue(sw.ElapsedMilliseconds < 15500);
        }

        [TestMethod()]
        public void LinearRetryWithParameterTest()
        {
            var invocationCounter = 0;
            string data = string.Empty;
            var sw = new Stopwatch();

            var error = new Func<string, string>(s =>
            {
                invocationCounter++;
                data = s;
                throw new NotImplementedException();
            });

            var retry = new FaultTolerantFunc<string, string>(error, 5, new LinearRetryStrategy(1000));

            sw.Start();
            var success = retry.Invoke("myvalue");
            sw.Stop();

            Assert.AreEqual(5, invocationCounter);
            Assert.AreEqual(5, retry.Tries);
            Assert.IsNotNull(retry.Exception);
            Assert.IsTrue(sw.ElapsedMilliseconds >= 4000);
            Assert.IsTrue(sw.ElapsedMilliseconds < 4500);
            Assert.IsNull(success);
            Assert.AreEqual("myvalue", data);
        }


        [TestMethod()]
        public void ExponentialRetryWithParameterTest()
        {
            var invocationCounter = 0;
            string data = string.Empty;
            var sw = new Stopwatch();

            var error = new Func<string,string>(s =>
            {
                invocationCounter++;
                data = s;
                throw new NotImplementedException();
            });

            var retry = new FaultTolerantFunc<string, string>(error, 5, new ExponentialRetryStrategy(1000));

            sw.Start();
            var success = retry.Invoke("myvalue");
            sw.Stop();

            Assert.AreEqual(5, invocationCounter);
            Assert.AreEqual(5, retry.Tries);
            Assert.IsNotNull(retry.Exception);
            Assert.IsTrue(sw.ElapsedMilliseconds >= 15000);
            Assert.IsTrue(sw.ElapsedMilliseconds < 15500);
            Assert.IsNull(success);
            Assert.AreEqual("myvalue", data);
        }
    }
}