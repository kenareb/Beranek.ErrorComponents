using Microsoft.VisualStudio.TestTools.UnitTesting;
using Beranek.ErrorComponents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Beranek.ErrorComponents.Tests
{
    [TestClass()]
    public class DelayedFuncTests
    {
        [TestMethod()]
        public async Task RunWithoutParamTest()
        {
            var called = false;
            var sw = new Stopwatch();

            Func<string> fun = new Func<string>(() =>
            {
                called = true;
                sw.Stop();
                return "Received!";
            });

            var delayedFunc = new DelayedFunc<string>(fun);
            sw.Start();
            var result = await delayedFunc.RunWithDelayAsync(2000);

            Assert.IsTrue(called);
            Assert.AreEqual("Received!", result);
            Assert.IsTrue(sw.Elapsed.TotalMilliseconds > 2000);
        }

        [TestMethod()]
        public async Task RunWithDelayTest()
        {
            var called = false;
            var sw = new Stopwatch();

            Func<string, string> fun = new Func<string, string>((s) =>
            {
                called = true;
                sw.Stop();
                return "Received: " + s;
            });

            var delayedFunc = new DelayedFunc<string, string>(fun);
            sw.Start();
            delayedFunc.SetParameter("Test");
            var result = await delayedFunc.RunWithDelayAsync(2000);

            Assert.IsTrue(called);
            Assert.AreEqual("Received: Test", result);
            Assert.IsTrue(sw.Elapsed.TotalMilliseconds > 2000);
        }
    }
}