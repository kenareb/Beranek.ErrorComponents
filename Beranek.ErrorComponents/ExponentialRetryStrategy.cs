namespace Beranek.ErrorComponents
{
    public class ExponentialRetryStrategy : IRetryStrategy
    {
        private int _currentDelay = 0;

        public int InitialDelay { get; private set; } = 500;

        public ExponentialRetryStrategy(int initialdelay)
        {
            InitialDelay = initialdelay;
        }

        public int GetNextDelay()
        {
            var delay = _currentDelay;

            _currentDelay = _currentDelay > 0
                ? _currentDelay + _currentDelay
                : InitialDelay;

            return delay;
        }

    }
}
