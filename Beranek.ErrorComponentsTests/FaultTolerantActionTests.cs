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

            var retry = new FaultTolerantAction(error, 1000, 5);

            sw.Start();
            var success = retry.Try();
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

            var retry = new FaultTolerantAction(error, 1000, 5, new ExponentialRetryStrategy(1000));

            sw.Start();
            retry.Try();
            sw.Stop();

            Assert.AreEqual(5, invocationCounter);
            Assert.AreEqual(5, retry.Tries);
            Assert.IsNotNull(retry.Exception);
            Assert.IsTrue(sw.ElapsedMilliseconds >= 15000);
            Assert.IsTrue(sw.ElapsedMilliseconds < 15500);
        }
    }
}