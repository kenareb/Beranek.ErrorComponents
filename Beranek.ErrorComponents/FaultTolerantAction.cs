namespace Beranek.ErrorComponents
{
    using System;

    public class FaultTolerantAction
    {
        protected Action MyAction { get; set; }
        protected int Delay { get; set; } = 1000;
        protected int MaxTries { get; set; } = 10;
        protected IRetryStrategy Strategy { get; set; }

        public FaultTolerantAction()
        { }

        public FaultTolerantAction(Action act)
        {
            MyAction = act;
            Strategy = new LinearRetryStrategy(Delay);
        }

        public FaultTolerantAction(Action act, int delay)
        {
            MyAction = act;
            Delay = delay;
            Strategy = new LinearRetryStrategy(Delay);
        }

        public FaultTolerantAction(Action act, int delay, int maxTries)
        {
            MyAction = act;
            Delay = delay;
            MaxTries = maxTries;
            Strategy = new LinearRetryStrategy(Delay);
        }

        public FaultTolerantAction(Action act, int delay, int maxTries, IRetryStrategy strategy)
        {
            MyAction = act;
            MaxTries = maxTries;
            Delay = delay;
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
