/// <summary>
/// UnityObjectPool 类用于管理 Unity 中的 GameObject 对象池。
/// 通过缓存和复用 GameObject 实例，减少频繁的实例化和销毁操作，提高游戏性能。
/// </summary>
using System.Collections.Generic;
using UnityEngine;


namespace PoolModule
{
    public class UnityObjectPool
    {
        private Queue<GameObject> _objects;  // 用于存储对象池中的对象
        private GameObject _prefab;          // 对象池管理的预制体
        private Transform _poolRoot;         // 对象池的根节点，回收的对象会挂载到该节点下
        private int _maxPoolSize;            // 对象池的最大大小
        private int _totalCount;             // 当前对象池中管理的对象总数
        private bool _implementsIPoolable;   // 缓存对象是否实现了 IPoolable 接口

        public UnityObjectPool(GameObject prefab, int initialSize = 10, int maxPoolSize = 100)
        {
            _prefab = prefab;
            _maxPoolSize = maxPoolSize;
            _objects = new Queue<GameObject>(initialSize);
            _poolRoot = new GameObject($"{_prefab.name}_PoolRoot").transform;  // 创建一个隐藏的根节点管理回收的对象
            _poolRoot.gameObject.SetActive(false);  // 隐藏根节点

            // 在初始化时缓存对象是否实现了 IPoolable 接口
            _implementsIPoolable = prefab.GetComponent<IPoolable>() != null;

            // 初始化对象池，创建指定数量的对象
            for (int i = 0; i < initialSize; i++)
            {
                var obj = GameObject.Instantiate(_prefab, _poolRoot);
                obj.SetActive(false);  // 初始化时将对象设置为非激活状态
                _objects.Enqueue(obj);  // 将对象放入对象池
                _totalCount++;
            }
        }


        #region 动态扩展和固定大小两种从对象池中获取对象的方式
            /// <summary>
            /// 从对象池中获取对象，并对其进行初始化
            /// 动态扩展
            /// </summary>
            /// <param name="parameters">初始化对象时传递的参数</param>
            /// <returns>从对象池中获取的 GameObject 实例</returns>
            public GameObject Get(params object[] parameters)
            {
                GameObject obj = _objects.Count > 0 ? _objects.Dequeue() : CreateNewObject();

                // 如果对象实现了 IPoolable，则初始化
                if (_implementsIPoolable)
                {
                    obj.GetComponent<IPoolable>().Initialize(parameters);
                }

                obj.SetActive(true);
                obj.transform.SetParent(null);
                return obj;
            }

            ///// <summary>
            ///// 从对象池中获取对象。如果池中没有可用对象且未达到最大大小，则创建新对象。
            ///// 固定大小
            ///// </summary>
            ///// <param name="parameters">初始化对象时传递的参数</param>
            ///// <returns>从对象池中获取的 GameObject 实例</returns>
            //public GameObject Get(params object[] parameters)
            //{
            //    // 如果池中有对象，直接从队列中取出
            //    if (_objects.Count > 0)
            //    {
            //        GameObject obj = _objects.Dequeue();
            //        InitializeObject(obj, parameters);
            //        return obj;
            //    }

            //    // 如果总对象数小于最大值，则创建新对象
            //    if (_totalCount < _maxPoolSize)
            //    {
            //        GameObject obj = CreateNewObject();
            //        InitializeObject(obj, parameters);
            //        return obj;
            //    }

            //    // 对象池已满，返回 null 或根据业务逻辑处理
            //    return null;
            //}
        #endregion



        /// <summary>
        /// 将对象回收到对象池中，并对其进行重置。
        /// </summary>
        /// <param name="obj">需要回收的 GameObject 实例</param>
        public void Recycle(GameObject obj)
        {
            // 如果对象实现了 IPoolable，则重置
            if (_implementsIPoolable)
            {
                obj.GetComponent<IPoolable>().Reset();
            }

            if (_objects.Count < _maxPoolSize)
            {
                obj.SetActive(false);  // 将对象设置为非激活状态
                obj.transform.SetParent(_poolRoot);  // 将对象放到隐藏根节点下
                _objects.Enqueue(obj);  // 回收到队列中
            }
            else
            {
                GameObject.Destroy(obj);  // 如果池满，销毁对象
                _totalCount--;
            }
        }

        /// <summary>
        /// 清理对象池，移除不再需要的对象。
        /// </summary>
        /// <param name="shouldCleanup">决定是否清理对象的条件函数</param>
        public void Cleanup(System.Func<GameObject, bool> shouldCleanup)
        {
            int count = _objects.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject obj = _objects.Dequeue();
                if (shouldCleanup(obj) || obj == null)
                {
                    if (obj != null)
                    {
                        GameObject.Destroy(obj);
                    }
                    _totalCount--;
                }
                else
                {
                    _objects.Enqueue(obj);
                }
            }
        }

        /// <summary>
        /// 清理所有对象，移除池中所有对象
        /// </summary>
        public void CleanupAll()
        {
            while (_objects.Count > 0)
            {
                GameObject obj = _objects.Dequeue();
                if (obj != null)
                {
                    GameObject.Destroy(obj);  // 销毁 GameObject
                }
            }
            _totalCount = 0;  // 重置对象计数
        }



        /// <summary>
        /// 创建一个新的 GameObject 实例，并将其放入对象池中。
        /// </summary>
        /// <returns>创建的新对象</returns>
        private GameObject CreateNewObject()
        {
            var obj = GameObject.Instantiate(_prefab, _poolRoot);
            obj.SetActive(false);
            _totalCount++;
            return obj;
        }
    }

}
