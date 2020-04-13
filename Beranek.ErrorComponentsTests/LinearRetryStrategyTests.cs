namespace Beranek.ErrorComponents.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Beranek.ErrorComponents;

    [TestClass()]
    public class LinearRetryStrategyTests
    {
        [TestMethod()]
        public void GetNextDelayTest()
        {
            var strategy = new LinearRetryStrategy(1000);

            var delay01 = strategy.GetNextDelay();
            Assert.AreEqual(0, delay01);

            var delay02 = strategy.GetNextDelay();
            Assert.AreEqual(1000, delay02);

            var delay03 = strategy.GetNextDelay();
            Assert.AreEqual(1000, delay03);

            var delay04 = strategy.GetNextDelay();
            Assert.AreEqual(1000, delay04);

            var delay05 = strategy.GetNextDelay();
            Assert.AreEqual(1000, delay05);

            var delay06 = strategy.GetNextDelay();
            Assert.AreEqual(1000, delay06);
        }
    }
}