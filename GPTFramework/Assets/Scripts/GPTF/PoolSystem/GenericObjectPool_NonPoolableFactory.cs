using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PoolModule
{
    /// <summary>
    /// GenericObjectPool_NonPoolableFactory 类用于管理多个不实现 IPoolable 接口类型的对象池。
    /// 可以为不同类型的对象创建和管理各自的对象池，支持对象池的创建、获取、回收和清理操作。
    /// </summary>
    public class GenericObjectPool_NonPoolableFactory : Singleton<GenericObjectPool_NonPoolableFactory>
    {
        private Dictionary<Type, object> _pools = new Dictionary<Type, object>(); // 用于存储不同类型的对象池

        /// <summary>
        /// 为指定类型创建非 IPoolable 的对象池。如果对象池已经存在，则不创建新的池。
        /// </summary>
        /// <typeparam name="T">需要管理的对象类型。</typeparam>
        /// <param name="initialSize">初始对象池大小。</param>
        /// <param name="maxPoolSize">对象池的最大容量。</param>
        public void CreatePool<T>(int initialSize = 10, int maxPoolSize = 100) where T : class, new()
        {
            var type = typeof(T);

            if (!IsAllowedType<T>())
            {
                throw new InvalidOperationException($"类型 {type.Name} 不允许在 NonPoolableObjectPoolFactory 中");
            }


            if (!_pools.ContainsKey(type))
            {
                // 创建非 IPoolable 类型的对象池
                _pools[type] = new GenericObjectPool_NonPoolable<T>(initialSize, maxPoolSize);
            }
        }

        /// <summary>
        /// 获取指定类型对象池中的对象。
        /// 如果对象池不存在，则自动创建一个默认大小的对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>从对象池中获取的对象实例。</returns>
        public T GetObject<T>() where T : class, new()
        {
            var type = typeof(T);

            if (!IsAllowedType<T>())
            {
                throw new InvalidOperationException($"类型 {type.Name} 不允许在 NonPoolableObjectPoolFactory 中");
            }


            if (!_pools.TryGetValue(type, out var pool))
            {
                CreatePool<T>(10, 100); // 创建默认大小的对象池
                pool = _pools[type];
            }

            if (pool is GenericObjectPool_NonPoolable<T> nonPoolablePool)
            {
                return nonPoolablePool.Get(); // 获取非 IPoolable 类型的对象
            }

            throw new InvalidOperationException($"No pool found for type {type}");
        }

        /// <summary>
        /// 将对象回收到指定类型的对象池中。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="obj">要回收的对象实例。</param>
        public void RecycleObject<T>(T obj) where T : class, new()
        {
            var type = typeof(T);
            if (_pools.TryGetValue(type, out var pool))
            {
                if (pool is GenericObjectPool_NonPoolable<T> nonPoolablePool)
                {
                    nonPoolablePool.Recycle(obj); // 回收非 IPoolable 类型的对象
                }
            }
        }

        /// <summary>
        /// 清理指定类型对象池中的对象，根据条件判断是否移除。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="shouldCleanup">用于判断是否应清理对象的条件函数。</param>
        public void CleanupPool<T>(Func<T, bool> shouldCleanup) where T : class, new()
        {
            var type = typeof(T);

            if (!IsAllowedType<T>())
            {
                throw new InvalidOperationException($"类型 {type.Name} 不允许在 NonPoolableObjectPoolFactory 中");
            }

            if (_pools.TryGetValue(type, out var pool))
            {
                if (pool is GenericObjectPool_NonPoolable<T> nonPoolablePool)
                {
                    nonPoolablePool.Cleanup(shouldCleanup); // 清理符合条件的非 IPoolable 对象
                }
            }
        }

        /// <summary>
        /// 判断类型 T 是否在允许的非 IPoolable 类型列表中。
        /// </summary>
        private bool IsAllowedType<T>()
        {
            var type = typeof(T);

            // 限制为特定的类型，支持数组类型和其他指定类型
            return type == typeof(Stopwatch) ||
                   type == typeof(StringBuilder) ||
                   type.IsArray || // 检查是否为数组类型
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Queue<>)) ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Stack<>));
        }

        /// <summary>
        /// 清理并移除指定类型的对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        public void RemovePool<T>() where T : class, new()
        {
            var type = typeof(T);

            if (!IsAllowedType<T>())
            {
                throw new InvalidOperationException($"类型 {type.Name} 不允许在 NonPoolableObjectPoolFactory 中");
            }


            if (_pools.TryGetValue(type, out var pool))
            {
                if (pool is GenericObjectPool_NonPoolable<T> nonPoolablePool)
                {
                    nonPoolablePool.CleanupAll(); // 清理非 IPoolable 类型的对象池
                }

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
