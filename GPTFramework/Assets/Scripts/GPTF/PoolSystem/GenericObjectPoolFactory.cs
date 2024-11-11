using System;
using System.Collections.Generic;

namespace PoolModule
{
    /// <summary>
    /// GenericObjectPoolFactory 类用于管理多个对象池的实例。
    /// 可以为不同类型的对象创建和管理各自的对象池，支持对象池的创建、获取、回收和清理操作。
    /// </summary>
    public class GenericObjectPoolFactory : Singleton<GenericObjectPoolFactory>
    {
        private Dictionary<Type, object> _pools = new Dictionary<Type, object>(); // 用于存储不同类型的对象池

        /// <summary>
        /// 为指定类型创建对象池。如果对象池已经存在，则不创建新的池。
        /// </summary>
        /// <typeparam name="T">需要管理的对象类型，必须实现 IPoolable 接口。</typeparam>
        /// <param name="initialSize">初始对象池大小。</param>
        /// <param name="maxPoolSize">对象池的最大容量。</param>
        public void CreatePool<T>(int initialSize = 10, int maxPoolSize = 100) where T : class, IPoolable, new()
        {
            var type = typeof(T);
            if (!_pools.ContainsKey(type))
            {
                _pools[type] = new GenericObjectPool<T>(initialSize, maxPoolSize); // 创建并存储新对象池
            }
        }

        /// <summary>
        /// 获取指定类型对象池中的对象。
        /// 如果对象池不存在，则自动创建一个默认大小的对象池。
        /// </summary>
        /// <typeparam name="T">对象类型，必须实现 IPoolable 接口。</typeparam>
        /// <param name="parameters">传递给对象的初始化参数。</param>
        /// <returns>从对象池中获取的对象实例。</returns>
        public T GetObject<T>(params object[] parameters) where T : class, IPoolable, new()
        {
            var type = typeof(T);
            if (!_pools.TryGetValue(type, out var pool))
            {
                CreatePool<T>(10, 100); // 创建默认大小的对象池
                pool = _pools[type];
            }
            return ((GenericObjectPool<T>)pool).Get(parameters); // 获取对象并初始化
        }

        /// <summary>
        /// 将对象回收到指定类型的对象池中。
        /// </summary>
        /// <typeparam name="T">对象类型，必须实现 IPoolable 接口。</typeparam>
        /// <param name="obj">要回收的对象实例。</param>
        public void RecycleObject<T>(T obj) where T : class, IPoolable, new()
        {
            var type = typeof(T);
            if (_pools.TryGetValue(type, out var pool))
            {
                ((GenericObjectPool<T>)pool).Recycle(obj); // 将对象回收到池中
            }
            else
            {
                // 如果池不存在，则释放对象资源，避免内存泄漏
                obj.Dispose();
            }
        }

        /// <summary>
        /// 非泛型回收方法,将对象回收到对应类型对象池中,必须实现 IPoolable 接口
        /// 主要用于无法指定准确类型的回收
        /// </summary>
        /// <param name="obj">要回收的对象实例</param>
        public void RecycleObject(IPoolable obj)
        {
            var type = obj.GetType();
            if (_pools.TryGetValue(type, out var pool) )
            {
                ((IGenericObjectPool)obj).Recycle(obj); // 将对象回收到池中
            }
            else
            {
                // 如果池不存在，则释放对象资源，避免内存泄漏
                obj.Dispose();
            }
        }

        /// <summary>
        /// 清理指定类型对象池中的对象，根据条件判断是否移除。
        /// </summary>
        /// <typeparam name="T">对象类型，必须实现 IPoolable 接口。</typeparam>
        /// <param name="shouldCleanup">用于判断是否应清理对象的条件函数。</param>
        public void CleanupPool<T>(Func<T, bool> shouldCleanup) where T : class, IPoolable, new()
        {
            var type = typeof(T);
            if (_pools.TryGetValue(type, out var pool))
            {
                ((GenericObjectPool<T>)pool).Cleanup(shouldCleanup); // 清理符合条件的对象
            }
        }

        /// <summary>
        /// 清理并移除指定类型的对象池。
        /// </summary>
        /// <typeparam name="T">对象类型，必须实现 IPoolable 接口。</typeparam>
        public void RemovePool<T>() where T : class, IPoolable, new()
        {
            var type = typeof(T);
            if (_pools.TryGetValue(type, out var pool))
            {
                ((GenericObjectPool<T>)pool).CleanupAll(); // 清理所有对象
                _pools.Remove(type); // 从字典中移除对象池
            }
        }

        /// <summary>
        /// 清理并移除所有对象池，释放所有池中的对象。
        /// </summary>
        public void ClearAllPools()
        {
            // 遍历所有对象池，清空每个池中的所有对象
            foreach (var pool in _pools.Values)
            {
                var method = pool.GetType().GetMethod("CleanupAll");
                method?.Invoke(pool, null); // 调用每个池的 CleanupAll 方法
            }
            _pools.Clear(); // 清空字典，移除所有对象池
        }
    }
}
