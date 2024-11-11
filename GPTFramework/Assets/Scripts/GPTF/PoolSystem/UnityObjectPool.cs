using System.Collections.Generic;
using UnityEngine;

namespace PoolModule
{
    /// <summary>
    /// UnityObjectPool 用于管理 Unity GameObject 类型的对象池。
    /// 支持对象的回收、重用，并限制最大池大小以控制内存使用。
    /// </summary>
    public class UnityObjectPool
    {
        private readonly Queue<GameObject> _objects = new Queue<GameObject>(); // 存储对象的队列
        private readonly GameObject _prefab;                                   // 对象池管理的预制体
        private readonly int _maxPoolSize;                                     // 对象池最大容量
        private Transform _poolRoot;                                           // 对象池的根节点，隐藏对象
        private int _currentSize;                                              // 追踪池中对象的数量

        /// <summary>
        /// 初始化对象池。
        /// </summary>
        /// <param name="prefab">对象池中的对象原型</param>
        /// <param name="initialSize">初始对象池大小</param>
        /// <param name="maxPoolSize">对象池的最大容量</param>
        public UnityObjectPool(GameObject prefab, int initialSize, int maxPoolSize)
        {
            _prefab = prefab;
            _maxPoolSize = maxPoolSize;
            _poolRoot = new GameObject($"{prefab.name}_Pool").transform;
            _poolRoot.gameObject.SetActive(false); // 隐藏对象池根节点

            // 预加载对象到池中
            for (int i = 0; i < initialSize; i++)
            {
                _objects.Enqueue(CreateNewObject());
                _currentSize++; // 更新当前对象数量
            }
        }

        /// <summary>
        /// 获取对象池中当前对象的数量。
        /// </summary>
        public int Count => _currentSize;

        /// <summary>
        /// 从对象池中获取一个对象。如果池为空，则创建新对象。
        /// </summary>
        /// <param name="onActivate">可选的初始化回调，用于在对象激活时执行自定义逻辑</param>
        /// <returns>从对象池中获取的对象</returns>
        public GameObject Get(System.Action<GameObject> onActivate = null)
        {
            GameObject obj;
            if (_currentSize > 0)
            {
                obj = _objects.Dequeue();
                _currentSize--; // 更新当前对象数量
            }
            else
            {
                obj = CreateNewObject();
            }

            obj.SetActive(true);
            obj.transform.SetParent(null); // 从对象池根节点分离
            onActivate?.Invoke(obj); // 执行自定义初始化逻辑
            return obj;
        }

        /// <summary>
        /// 将对象回收到对象池中。如果池已满，则销毁该对象。
        /// </summary>
        /// <param name="obj">需要回收的对象</param>
        public void Recycle(GameObject obj)
        {
            if (_currentSize >= _maxPoolSize)
            {
                Object.Destroy(obj); // 销毁对象以防止池无限增长
                return;
            }

            obj.SetActive(false);
            obj.transform.SetParent(_poolRoot); // 重新挂到隐藏的根节点下
            obj.transform.localPosition = Vector3.zero; // 重置位置
            obj.transform.localRotation = Quaternion.identity; // 重置旋转
            _objects.Enqueue(obj); // 将对象放回队列中
            _currentSize++; // 更新当前对象数量
        }

        /// <summary>
        /// 清空对象池，销毁所有池中的对象。
        /// </summary>
        public void CleanupAll()
        {
            while (_currentSize > 0)
            {
                GameObject obj = _objects.Dequeue();
                Object.Destroy(obj);
                _currentSize--; // 更新当前对象数量
            }
        }

        /// <summary>
        /// 创建新的对象实例，并将其设置为非激活状态。
        /// </summary>
        private GameObject CreateNewObject()
        {
            GameObject obj = Object.Instantiate(_prefab);
            obj.SetActive(false);
            obj.transform.SetParent(_poolRoot); // 将对象放入隐藏的根节点下
            return obj;
        }
    }
}
