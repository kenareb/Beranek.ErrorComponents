namespace Beranek.ErrorComponents
{
    using System;
    public class ExceptionTolerantFunction<TException, TResult>
            where TException : Exception
    {
        private const int DefaultDelay = 1000;

        protected Func<TResult> MyFunc { get; set; }

        protected int MaxTries { get; set; } = 10;

        protected IRetryStrategy Strategy { get; set; }

        private Func<TException, bool> _filter = (e) =>
        {
            return true;
        };

        public ExceptionTolerantFunction()
        { }

        public ExceptionTolerantFunction(Func<TResult> act)
        {
            MyFunc = act;
            Strategy = new LinearRetryStrategy(DefaultDelay);
        }

        public ExceptionTolerantFunction(Func<TResult> act, int maxTries)
        {
            MyFunc = act;
            MaxTries = maxTries;
            Strategy = new LinearRetryStrategy(DefaultDelay);
        }

        public ExceptionTolerantFunction(Func<TResult> act, int maxTries, IRetryStrategy strategy)
        {
            MyFunc = act;
            MaxTries = maxTries;
            Strategy = strategy;
        }

        public ExceptionTolerantFunction<TException, TResult> Filter(Func<TException, bool> predicate)
        {
            _filter = predicate;
            return this;
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
                    catch (TException e) when (IsCatchable(e))
                    {
                        Exception = e;
                    }

                    tryAgain = Tries < MaxTries;
                }

                retryAction.Cancel();
            }

            return result;
        }

        public Func<TResult> Method()
        {
            return Invoke;
        }

        public TException Exception { get; private set; }

        public int Tries { get; private set; }

        private bool IsCatchable(TException e)
        {
            return _filter(e);
        }
    }
}
