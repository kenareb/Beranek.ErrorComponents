namespace Beranek.ErrorComponents
{
    using System;
    public class FaultTolerantAction : ExceptionTolerantAction<Exception>
    {

        public FaultTolerantAction() : base()
        { }

        public FaultTolerantAction(Action act) : base(act)
        {}

        public FaultTolerantAction(Action act, int maxTries) : base (act, maxTries)
        {
        }

        public FaultTolerantAction(Action act, int maxTries, IRetryStrategy strategy) 
            : base(act, maxTries, strategy)
        {
        }
    }
}
