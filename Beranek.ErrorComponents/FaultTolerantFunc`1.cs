namespace Beranek.ErrorComponents
{
    using System;
    public class FaultTolerantFunc<TResult>
    {
        private const int DefaultDelay = 1000;
        protected Func<TResult> MyFunc { get; set; }
        protected int MaxTries { get; set; } = 10;
        protected IRetryStrategy Strategy { get; set; }

        public FaultTolerantFunc()
        { }

        public FaultTolerantFunc(Func<TResult> fun)
        {
            MyFunc = fun;
            Strategy = new LinearRetryStrategy(DefaultDelay);
        }

        public FaultTolerantFunc(Func<TResult> fun, int maxTries)
        {
            MyFunc = fun;
            MaxTries = maxTries;
            Strategy = new LinearRetryStrategy(DefaultDelay);
        }

        public FaultTolerantFunc(Func<TResult> fun, int maxTries, IRetryStrategy strategy)
        {
            MyFunc = fun;
            MaxTries = maxTries;
            Strategy = strategy;
        }

        public TResult Invoke()
        {
            bool tryAgain = true;
            TResult result = default(TResult);

            using (var retryAction = new DelayedFunc<TResult>(MyFunc))
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

