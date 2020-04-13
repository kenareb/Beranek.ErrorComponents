namespace Beranek.ErrorComponents.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Beranek.ErrorComponents;
    using System;
    using System.Diagnostics;
    using System.Threading;

    [TestClass()]
    public class TimedActionTests
    {
        [TestMethod()]
        public void RunWithDelayTest()
        {
            var called = false;
            var sw = new Stopwatch();
            ManualResetEvent mre = new ManualResetEvent(false);

            Action act = new Action(() =>
            {
                called = true;
                sw.Stop();
                mre.Set();
            });

            var delayedAction = new TimedAction(act);
            sw.Start();
            delayedAction.RunWithDelay(2000);

            mre.WaitOne();

            Assert.IsTrue(called);
            Assert.IsTrue(sw.Elapsed.TotalMilliseconds > 2000);
        }

        [TestMethod()]
        public void CancelTest()
        {
            var called = false;
            var sw = new Stopwatch();
            ManualResetEvent mre = new ManualResetEvent(false);

            Action act = new Action(() =>
            {
                called = true;
                sw.Stop();
                mre.Set();
            });

            var delayedAction = new TimedAction(act);
            sw.Start();
            delayedAction.RunWithDelay(2000);
            delayedAction.Cancel();

            var signaled = mre.WaitOne(3000);

            Assert.IsFalse(called);
            Assert.IsFalse(signaled);
            Assert.IsTrue(sw.IsRunning);
        }
    }
}