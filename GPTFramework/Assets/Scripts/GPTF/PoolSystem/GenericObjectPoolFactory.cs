/// <summary>
/// GenericObjectPoolFactory 类用于管理多个 GenericObjectPool 实例，
/// 可以为不同类型的对象创建和管理通用对象池，支持对象池的统一创建、获取和清理操作。
/// 使用类型作为字典的键，确保类型安全的存取。
/// 自动在获取对象时创建尚不存在的对象池。
/// </summary>
using System;
using System.Collections.Generic;

namespace PoolModule
{
    public class GenericObjectPoolFactory : Singleton<GenericObjectPoolFactory>
    {
        
        // 存储所有创建的对象池，使用类型作为键
        private Dictionary<Type, object> _pools = new Dictionary<Type, object>();

        // 默认对象池大小
        private const int DefaultInitialSize = 10;
        private const int DefaultMaxPoolSize = 100;

        /// <summary>
        /// 创建对象池，如果对象池已经存在，则不创建新的池。
        /// </summary>
        /// <typeparam name="T">对象池管理的对象类型</typeparam>
        /// <param name="initialSize">初始创建的对象数量</param>
        /// <param name="maxPoolSize">对象池的最大大小</param>
        public void CreatePool<T>(int initialSize, int maxPoolSize)
        {
            var type = typeof(T); // 使用类型作为键
            if (!_pools.ContainsKey(type))
            {
                GenericObjectPool<T> pool = new GenericObjectPool<T>(initialSize, maxPoolSize);
                _pools[type] = pool;
            }
        }

        /// <summary>
        /// 获取对象池中的对象。如果对象池不存在，则自动创建一个默认大小的对象池。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="parameters">初始化对象时传递的参数</param>
        /// <returns>获取的对象实例</returns>
        public T GetObject<T>(params object[] parameters)
        {
            var type = typeof(T); // 使用类型作为键

            // 如果对象池不存在，创建一个默认大小的对象池
            if (!_pools.TryGetValue(type, out var pool))
            {
                CreatePool<T>(DefaultInitialSize, DefaultMaxPoolSize);
                pool = _pools[type];  // 重新获取创建的对象池
            }

            return ((GenericObjectPool<T>)pool).Get(parameters);
        }

        /// <summary>
        /// 回收对象到指定的对象池中。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">需要回收的对象</param>
        public void RecycleObject<T>(T obj)
        {
            var type = typeof(T); // 使用类型作为键
            if (_pools.TryGetValue(type, out var pool))
            {
                ((GenericObjectPool<T>)pool).Recycle(obj);
            }
        }

        /// <summary>
        /// 清理指定对象池。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="shouldCleanup">决定是否清理对象的条件函数</param>
        public void CleanupPool<T>(Func<T, bool> shouldCleanup)
        {
            var type = typeof(T); // 使用类型作为键
            if (_pools.TryGetValue(type, out var pool))
            {
                ((GenericObjectPool<T>)pool).Cleanup(shouldCleanup);
            }
        }

        /// <summary>
        /// 清理所有对象池。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="shouldCleanup">决定是否清理对象的条件函数</param>
        public void CleanupAllPools<T>(Func<T, bool> shouldCleanup)
        {
            foreach (var pool in _pools.Values)
            {
                if (pool is GenericObjectPool<T> genericPool)
                {
                    genericPool.Cleanup(shouldCleanup);
                }
            }
        }


        // 清理所有对象并移除指定的对象池
        public void RemovePool<T>()
        {
            var type = typeof(T); // 使用类型作为键
            if (_pools.TryGetValue(type, out var pool))
            {
                ((GenericObjectPool<T>)pool).CleanupAll();  // 清理池中的所有对象
                _pools.Remove(type);  // 从字典中移除这个对象池
            }
        }



    }

}
