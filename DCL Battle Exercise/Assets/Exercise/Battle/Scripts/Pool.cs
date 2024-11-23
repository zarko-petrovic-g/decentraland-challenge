using System.Collections.Generic;

public class Pool
{
    private readonly Dictionary<PoolableCategory, Queue<IPoolable>> pool =
        new Dictionary<PoolableCategory, Queue<IPoolable>>();

    public Pool(CategoryData[] categoryData)
    {
        foreach(CategoryData data in categoryData)
        {
            var objects = new Queue<IPoolable>();

            foreach(IPoolable poolable in data.Objects)
            {
                poolable.ReturnToPool();
                objects.Enqueue(poolable);
            }

            pool.Add(data.Category, objects);
        }
    }

    public void Return(IPoolable obj)
    {
        obj.ReturnToPool();
        pool[obj.Category].Enqueue(obj);
    }

    public IPoolable Get(PoolableCategory category)
    {
        // we don't need to check if the pool is empty
        // as everything else, this pool is designed for 0 garbage
        // so the expectation is that everything is pre-pooled
        // and if it's not, it's a bug

        IPoolable obj = pool[category].Dequeue();
        obj.AcquireFromPool();
        return obj;
    }

    public class CategoryData
    {
        public PoolableCategory Category;
        public IEnumerable<IPoolable> Objects;
    }
}