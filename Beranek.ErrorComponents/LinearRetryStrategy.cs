namespace Beranek.ErrorComponents
{
    using System;
    public class LinearRetryStrategy : IRetryStrategy
    {
        private int _currentDelay = 0;
        public int InitialDelay { get; private set; } = 500;

        public LinearRetryStrategy(int initialdelay)
        {
            InitialDelay = initialdelay;
        }

        public int GetNextDelay()
        {
            var delay = _currentDelay;

            _currentDelay = InitialDelay;

            return delay;
        }

    }
}
