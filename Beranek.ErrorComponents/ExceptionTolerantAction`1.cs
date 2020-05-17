namespace Beranek.ErrorComponents
{
    using System;

    public class ExceptionTolerantAction<TException>
        where TException : Exception
    {
        private const int DefaultDelay = 1000;

        protected Action MyAction { get; set; }

        protected int MaxTries { get; set; } = 10;

        protected IRetryStrategy Strategy { get; set; }

        private Func<TException, bool> _filter = (e) =>
        {
            return true;
        };

        public ExceptionTolerantAction()
        { }

        public ExceptionTolerantAction(Action act)
        {
            MyAction = act;
            Strategy = new LinearRetryStrategy(DefaultDelay);
        }

        public ExceptionTolerantAction(Action act, int maxTries)
        {
            MyAction = act;
            MaxTries = maxTries;
            Strategy = new LinearRetryStrategy(DefaultDelay);
        }

        public ExceptionTolerantAction(Action act, int maxTries, IRetryStrategy strategy)
        {
            MyAction = act;
            MaxTries = maxTries;
            Strategy = strategy;
        }

        [Obsolete("This method is replaced by 'RetryWhen' and will be removed soon.")]
        public ExceptionTolerantAction<TException> Filter(Func<TException, bool> predicate)
        {
            return RetryWhen(predicate);
        }

        public ExceptionTolerantAction<TException> RetryWhen(Func<TException,bool> predicate)
        {
            _filter = predicate;
            return this;
        }

        public ExceptionTolerantAction<TException> WithMaxRetries(int max)
        {
            MaxTries = max;
            return this;
        }

        public ExceptionTolerantAction<TException> WithStrategy(IRetryStrategy strategy)
        {
            Strategy = strategy;
            return this;
        }

        public ExceptionTolerantAction<TException> WithAction(Action act)
        {
            MyAction = act;
            return this;
        }

        public Action AsMethod()
        {
            return () => Invoke();
        }

        public bool Invoke()
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

        public TException Exception { get; private set; }

        [Obsolete("This method is replaced by 'AsMethod' and will be removed soon.")]
        public Action Method()
        {
            return AsMethod();
        }

        public int Tries { get; private set; }

        private bool IsCatchable(TException e)
        {
            return _filter(e);
        }
    }
}
