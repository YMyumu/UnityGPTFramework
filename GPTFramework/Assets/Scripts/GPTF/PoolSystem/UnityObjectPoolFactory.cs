using System;
using System.Collections.Generic;
using UnityEngine;

namespace PoolModule
{
    /// <summary>
    /// UnityObjectPoolFactory 管理所有 UnityObjectPool 的实例，
    /// 根据名称获取对象池，并支持自动加载和清理功能。
    /// </summary>
    public class UnityObjectPoolFactory : Singleton<UnityObjectPoolFactory>
    {
        private readonly Dictionary<string, UnityObjectPool> _pools = new Dictionary<string, UnityObjectPool>();
        public Func<string, GameObject> LoadFuncDelegate; // 委托，用于加载预制体

        /// <summary>
        /// 创建新的对象池，如果已存在则不会重复创建。
        /// </summary>
        /// <param name="prefab">对象池管理的预制体</param>
        /// <param name="poolName">对象池的名称</param>
        /// <param name="initialSize">初始对象池大小</param>
        /// <param name="maxPoolSize">对象池的最大容量</param>
        public void CreatePool(GameObject prefab, string poolName, int initialSize, int maxPoolSize)
        {
            if (!_pools.ContainsKey(poolName))
            {
                _pools[poolName] = new UnityObjectPool(prefab, initialSize, maxPoolSize);
            }
        }

        /// <summary>
        /// 获取对象池中的对象。如果池不存在且提供了加载委托，则会自动加载并创建新池。
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <returns>从池中获取的对象实例</returns>
        public GameObject GetObject(string poolName)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                return pool.Get();
            }

            if (LoadFuncDelegate != null)
            {
                var prefab = LoadFuncDelegate(poolName);
                if (prefab != null) // 确保加载成功
                {
                    CreatePool(prefab, poolName, 0, 500); // 使用默认大小创建新池
                    return _pools[poolName].Get();
                }
            }
            return null;
        }

        /// <summary>
        /// 将对象回收到指定名称的对象池中。
        /// </summary>
        /// <param name="obj">要回收的对象</param>
        /// <param name="poolName">对象池名称</param>
        public void RecycleObject(string poolName, GameObject obj)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                pool.Recycle(obj);
            }
            else
            {
                GameObject.Destroy(obj); // 如果没有池则销毁对象
            }
        }

        /// <summary>
        /// 清理指定类型对象池中的对象，根据条件判断是否移除。
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="shouldCleanup">判断是否清理对象的条件函数</param>
        public void CleanupPool(string poolName, Func<GameObject, bool> shouldCleanup)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                Queue<GameObject> remainingObjects = new Queue<GameObject>();

                // 通过 Count 属性检查对象数量
                while (pool.Count > 0)
                {
                    var obj = pool.Get();
                    if (shouldCleanup(obj))
                    {
                        GameObject.Destroy(obj); // 符合条件则销毁
                    }
                    else
                    {
                        remainingObjects.Enqueue(obj); // 不符合条件则重新入池
                    }
                }
                while (remainingObjects.Count > 0)
                {
                    pool.Recycle(remainingObjects.Dequeue()); // 将未清理的对象重新放入池中
                }
            }
        }

        /// <summary>
        /// 清理并移除指定名称的对象池，销毁所有池中的对象。
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        public void RemovePool(string poolName)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                pool.CleanupAll();
                _pools.Remove(poolName); // 移除对象池的引用
            }
        }

        /// <summary>
        /// 清理并移除所有对象池，销毁所有池中的对象。
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in _pools.Values)
            {
                pool.CleanupAll();
            }
            _pools.Clear(); // 清空字典，移除所有对象池
        }
    }
}
