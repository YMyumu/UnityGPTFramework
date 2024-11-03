/// <summary>
/// UnityObjectPoolFactory 类用于管理多个 UnityObjectPool 实例。
/// 可以为不同的对象类型创建和管理各自的对象池，支持对象池的统一创建、获取、回收和清理操作。
/// </summary>
using System.Collections.Generic;
using UnityEngine;


namespace PoolModule
{
    public class UnityObjectPoolFactory : Singleton<UnityObjectPoolFactory>
    {
        // 委托，用于根据名字加载预制件  需要在合适的地方对委托进行绑定 在此对象池工厂内没有指定
        public System.Func<string, GameObject> LoadFuncDelegate;

        // 用于管理对象池的字典，使用名字作为键
        private Dictionary<string, UnityObjectPool> _pools = new Dictionary<string, UnityObjectPool>();

        /// <summary>
        /// 创建对象池，如果对象池已经存在，则不创建新的池。
        /// </summary>
        /// <param name="prefab">管理的 GameObject 预制体</param>
        /// <param name="poolName">对象池的名称</param>
        /// <param name="initialSize">初始创建的对象数量</param>
        /// <param name="maxPoolSize">对象池的最大大小</param>
        public void CreatePool(GameObject prefab, string poolName, int initialSize, int maxPoolSize)
        {
            if (!_pools.ContainsKey(poolName))
            {
                UnityObjectPool pool = new UnityObjectPool(prefab, initialSize, maxPoolSize);
                _pools[poolName] = pool;
            }
        }

        /// <summary>
        /// 获取对象池中的对象。
        /// </summary>
        /// <param name="poolName">对象池的名称</param>
        /// <param name="parameters">初始化对象时传递的参数</param>
        /// <returns>获取的 GameObject 对象</returns>
        public GameObject GetObject(string poolName, params object[] parameters)
        {
            GameObject result = null;

            // 检查对象池是否存在
            if (_pools.TryGetValue(poolName, out var pool))
            {
                result = pool.Get(); // 从对象池中获取对象
            }
            else if (LoadFuncDelegate != null)
            {
                // 使用委托加载预制件并创建对象池
                CreatePool(LoadFuncDelegate(poolName), poolName, 0, 500);
                result = _pools[poolName].Get();
            }

            return result;
        }

        /// <summary>
        /// 将对象回收到指定的对象池中。
        /// </summary>
        /// <param name="poolName">对象池的名称</param>
        /// <param name="obj">需要回收的对象</param>
        public void RecycleObject(string poolName, GameObject obj)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                pool.Recycle(obj);
            }
        }

        /// <summary>
        /// 清理指定对象池。
        /// </summary>
        /// <param name="poolName">对象池的名称</param>
        /// <param name="shouldCleanup">决定是否清理对象的条件函数</param>
        public void CleanupPool(string poolName, System.Func<GameObject, bool> shouldCleanup)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                pool.Cleanup(shouldCleanup);
            }
        }

        /// <summary>
        /// 清理所有对象池。
        /// </summary>
        /// <param name="shouldCleanup">决定是否清理对象的条件函数</param>
        public void CleanupAllPools(System.Func<GameObject, bool> shouldCleanup)
        {
            foreach (var pool in _pools.Values)
            {
                pool.Cleanup(shouldCleanup);
            }
        }

        // 清理所有对象并移除指定的对象池
        public void RemovePool(string poolName)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                pool.CleanupAll();  // 清理池中的所有对象
                _pools.Remove(poolName);  // 从字典中移除这个对象池
            }
        }
    }

}
