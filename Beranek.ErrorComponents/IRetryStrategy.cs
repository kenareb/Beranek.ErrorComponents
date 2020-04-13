namespace Beranek.ErrorComponents
{
    public interface IRetryStrategy
    {
        int GetNextDelay();
    }
}
