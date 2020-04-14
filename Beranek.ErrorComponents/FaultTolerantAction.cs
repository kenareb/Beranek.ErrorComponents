namespace Beranek.ErrorComponents
{
    using System;

    public class FaultTolerantAction
    {
        private const int DefaultDelay = 1000;
        protected Action MyAction { get; set; }
        protected int MaxTries { get; set; } = 10;
        protected IRetryStrategy Strategy { get; set; }

        public FaultTolerantAction()
        { }

        public FaultTolerantAction(Action act)
        {
            MyAction = act;
            Strategy = new LinearRetryStrategy(DefaultDelay);
        }

        public FaultTolerantAction(Action act, int maxTries)
        {
            MyAction = act;
            MaxTries = maxTries;
            Strategy = new LinearRetryStrategy(DefaultDelay);
        }

        public FaultTolerantAction(Action act, int maxTries, IRetryStrategy strategy)
        {
            MyAction = act;
            MaxTries = maxTries;
            Strategy = strategy;
        }

        public bool Try()
        {
            bool tryAgain = true;
            bool result = false;

            using (var retryAction = new DelayedAction(MyAction))
            {
                while (tryAgain)
                {
                    Tries++;
                    
                    var awaiter = retryAction.RunWithDelayAsync(Strategy.GetNextDelay())
                        .ConfigureAwait(true)
                        .GetAwaiter();

                    try
                    {
                        result = awaiter.GetResult();
                    }
                    catch (Exception e)
                    {
                        Exception = e;
                    }

                    tryAgain = Tries < MaxTries;
                }

                retryAction.Cancel();
            }

            return result;
        }

        public Exception Exception { get; private set; }

        public int Tries { get; private set; }
    }
}
