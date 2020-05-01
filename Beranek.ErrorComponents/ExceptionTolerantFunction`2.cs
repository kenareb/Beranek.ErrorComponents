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

        [Obsolete("This method is replaced by 'RetryWhen' and will be removed soon.")]
        public ExceptionTolerantFunction<TException, TResult> Filter(Func<TException, bool> predicate)
        {
            return RetryWhen(predicate);
        }

        public ExceptionTolerantFunction<TException, TResult> RetryWhen(Func<TException, bool> predicate)
        {
            _filter = predicate;
            return this;
        }

        public ExceptionTolerantFunction<TException, TResult> WithMaxRetries(int max)
        {
            MaxTries = max;
            return this;
        }

        public ExceptionTolerantFunction<TException, TResult> WithStrategy(IRetryStrategy strategy)
        {
            Strategy = strategy;
            return this;
        }

        public ExceptionTolerantFunction<TException, TResult> WithFunction(Func<TResult> fun)
        {
            MyFunc = fun;
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

        [Obsolete("This method is replaced by 'AsMethod' and will be removed soon.")]
        public Func<TResult> Method()
        {
            return AsMethod();
        }

        public Func<TResult> AsMethod()
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
