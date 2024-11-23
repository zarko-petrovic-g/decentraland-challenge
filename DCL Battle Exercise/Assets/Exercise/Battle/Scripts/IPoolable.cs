public interface IPoolable
{
    PoolableCategory Category { get; }
    void AcquireFromPool();
    void ReturnToPool();
}