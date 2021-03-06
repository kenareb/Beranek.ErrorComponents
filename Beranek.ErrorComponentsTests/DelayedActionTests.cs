﻿namespace Beranek.ErrorComponents.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Beranek.ErrorComponents;
    using System;
    using System.Threading;
    using System.Diagnostics;

    [TestClass()]
    public class DelayedActionTests
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

            var delayedTask = new DelayedAction(act);
            sw.Start();
            delayedTask.RunWithDelayAsync(2000);

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

            var delayedAction = new DelayedAction(act);
            sw.Start();

            // dont await the next call, so we can call "Cancel"
            delayedAction.RunWithDelayAsync(2000);
            delayedAction.Cancel();

            // Set timeout to 3 seconds, so we dont wait forever.
            var signaled = mre.WaitOne(3000);

            Assert.IsFalse(called);
            Assert.IsFalse(signaled);
            Assert.IsTrue(sw.IsRunning);
        }

        /*
        [TestMethod()]
        public void SetParameterTest()
        {
            string myValue = string.Empty;
            int counter = 0;

            var act = new Action<string>(s =>
            {
                if (counter++ < 2)
                {
                    throw new InvalidOperationException();
                }

                myValue = s;
            });



            Assert.AreEqual("data", myValue);
        }
        */
    }
}