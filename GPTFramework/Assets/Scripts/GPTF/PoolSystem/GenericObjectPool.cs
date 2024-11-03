/// <summary>
/// GenericObjectPool 类用于管理非 Unity 对象的通用对象池。
/// 支持从对象池中获取对象和回收对象，同时提供对象池清理功能。
/// </summary>
using System;
using System.Collections.Generic;

namespace PoolModule
{
    public class GenericObjectPool<T>
    {
        private Queue<T> _objects;
        private int _maxPoolSize;
        private int _totalCount;
        private bool _implementsIPoolable;

        public GenericObjectPool(int initialSize = 10, int maxPoolSize = 100)
        {
            _maxPoolSize = maxPoolSize;
            _objects = new Queue<T>(initialSize);

            // 检查对象是否实现 IPoolable 接口
            _implementsIPoolable = typeof(IPoolable).IsAssignableFrom(typeof(T));
        }


        #region 动态扩展和固定大小两种从对象池中获取对象的方式
        /// <summary>
        /// 从对象池中获取对象。
        /// 动态大小
        /// </summary>
        /// <param name="parameters">初始化对象时传递的参数</param>
        /// <returns>获取的对象实例</returns>
        public T Get(params object[] parameters)
        {
            T obj = _objects.Count > 0 ? _objects.Dequeue() : CreateNewObject();

            if (_implementsIPoolable && obj is IPoolable poolableObj)
            {
                poolableObj.Initialize(parameters);
            }

            return obj;
        }

        ///// <summary>
        ///// 从对象池中获取对象。如果池中没有可用对象且未达到最大大小，则创建新对象。
        ///// 固定大小
        ///// </summary>
        ///// <param name="parameters">初始化对象时传递的参数</param>
        ///// <returns>获取的对象实例</returns>
        //public T Get(params object[] parameters)
        //{
        //    // 如果池中有对象，直接从队列中取出
        //    if (_objects.Count > 0)
        //    {
        //        T obj = _objects.Dequeue();
        //        InitializeObject(obj, parameters);
        //        return obj;
        //    }

        //    // 如果总对象数小于最大值，则创建新对象
        //    if (_totalCount < _maxPoolSize)
        //    {
        //        T obj = CreateNewObject();
        //        InitializeObject(obj, parameters);
        //        return obj;
        //    }

        //    // 如果对象池已满，返回 null
        //    return default;
        //}
        #endregion

        /// <summary>
        /// 回收对象到对象池。
        /// </summary>
        /// <param name="obj">需要回收的对象</param>
        public void Recycle(T obj)
        {
            if (_implementsIPoolable && obj is IPoolable poolableObj)
            {
                poolableObj.Reset();
            }

            if (_objects.Count < _maxPoolSize)
            {
                _objects.Enqueue(obj);
            }
            else
            {
                _totalCount--;
            }
        }

        /// <summary>
        /// 清理对象池。
        /// </summary>
        /// <param name="shouldCleanup">决定是否清理对象的条件函数</param>
        public void Cleanup(Func<T, bool> shouldCleanup)
        {
            int count = _objects.Count;
            for (int i = 0; i < count; i++)
            {
                T obj = _objects.Dequeue();
                if (shouldCleanup(obj))
                {
                    _totalCount--;
                }
                else
                {
                    _objects.Enqueue(obj);
                }
            }
        }

        /// <summary>
        /// 清理所有对象，移除池中所有对象。
        /// </summary>
        public void CleanupAll()
        {
            while (_objects.Count > 0)
            {
                T obj = _objects.Dequeue();
                if (obj is System.IDisposable disposable)
                {
                    disposable.Dispose();  // 如果对象实现了 IDisposable，则调用 Dispose
                }
            }
            _totalCount = 0;  // 重置对象计数
        }


        /// <summary>
        /// 创建新对象。
        /// </summary>
        /// <returns>创建的新对象实例</returns>
        private T CreateNewObject()
        {
            _totalCount++;
            return Activator.CreateInstance<T>();
        }
    }
}
