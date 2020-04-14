using Microsoft.VisualStudio.TestTools.UnitTesting;
using Beranek.ErrorComponents;

namespace Beranek.ErrorComponents.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Diagnostics;

    [TestClass()]
    public class FaultTolerantActionTests
    {
        [TestMethod()]
        public void LinearRetryTest()
        {
            int invocationCounter = 0;
            var sw = new Stopwatch();

            var error = new Action(() =>
            {
                invocationCounter++;
                throw new NotImplementedException();
            });

            var retry = new FaultTolerantAction(error, 5);

            sw.Start();
            var success = retry.Invoke();
            sw.Stop();

            Assert.AreEqual(5, invocationCounter);
            Assert.AreEqual(5, retry.Tries);
            Assert.IsNotNull(retry.Exception);
            Assert.IsTrue(sw.ElapsedMilliseconds >= 4000);
            Assert.IsTrue(sw.ElapsedMilliseconds < 4500);
            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ExponentialRetryTest()
        {
            int invocationCounter = 0;
            var sw = new Stopwatch();

            var error = new Action(() =>
            {
                invocationCounter++;
                throw new NotImplementedException();
            });

            var retry = new FaultTolerantAction(error, 5, new ExponentialRetryStrategy(1000));

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

            var error = new Action<string>(s =>
            {
                invocationCounter++;
                data = s;
                throw new NotImplementedException();
            });

            var retry = new FaultTolerantAction<string>(error, 5, new LinearRetryStrategy(1000));
            retry.SetParameter("myvalue");

            sw.Start();
            var success = retry.Invoke();
            sw.Stop();

            Assert.AreEqual(5, invocationCounter);
            Assert.AreEqual(5, retry.Tries);
            Assert.IsNotNull(retry.Exception);
            Assert.IsTrue(sw.ElapsedMilliseconds >= 4000);
            Assert.IsTrue(sw.ElapsedMilliseconds < 4500);
            Assert.IsFalse(success);
            Assert.AreEqual("myvalue", data);
        }


        [TestMethod()]
        public void ExponentialRetryWithParameterTest()
        {
            var invocationCounter = 0;
            string data = string.Empty;
            var sw = new Stopwatch();

            var error = new Action<string>(s =>
            {
                invocationCounter++;
                data = s;
                throw new NotImplementedException();
            });

            var retry = new FaultTolerantAction<string>(error, 5, new ExponentialRetryStrategy(1000));
            retry.SetParameter("myvalue");

            sw.Start();
            var success = retry.Invoke();
            sw.Stop();

            Assert.AreEqual(5, invocationCounter);
            Assert.AreEqual(5, retry.Tries);
            Assert.IsNotNull(retry.Exception);
            Assert.IsTrue(sw.ElapsedMilliseconds >= 15000);
            Assert.IsTrue(sw.ElapsedMilliseconds < 15500); 
            Assert.IsFalse(success);
            Assert.AreEqual("myvalue", data);
        }
    }
}