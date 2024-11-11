using System;
using System.Collections.Concurrent;
using UnityEditor;

namespace PoolModule
{
    /// <summary>
    /// GenericObjectPool 类用于管理实现 IPoolable 接口的对象的通用对象池。
    /// 支持对象的获取、回收和清理，以便复用对象并释放资源，避免内存泄漏。
    /// </summary>
    /// <typeparam name="T">池中管理的对象类型，必须实现 IPoolable 接口。</typeparam>
    public class GenericObjectPool<T> : IGenericObjectPool where T : class, IPoolable, new()
    {
        private readonly ConcurrentQueue<T> _objects; // 用于存储对象池中的对象
        private int _maxPoolSize; // 对象池的最大大小
        private int _currentSize; // 当前对象池的总计数

        /// <summary>
        /// 构造函数，初始化对象池并创建初始对象。
        /// </summary>
        /// <param name="initialSize">对象池初始化时创建的对象数量。</param>
        /// <param name="maxPoolSize">对象池允许存储的最大对象数量。</param>
        public GenericObjectPool(int initialSize, int maxPoolSize)
        {
            _maxPoolSize = maxPoolSize;
            _objects = new ConcurrentQueue<T>();

            // 根据初始大小创建对象并放入池中
            for (int i = 0; i < initialSize; i++)
            {
                _objects.Enqueue(CreateNewObject());
            }
        }

        /// <summary>
        /// 从对象池中获取对象。
        /// 如果对象池为空且未达到最大容量，则创建新对象。
        /// 调用 Initialize 方法初始化对象。
        /// </summary>
        /// <param name="parameters">可选的初始化参数。</param>
        /// <returns>从对象池中获取的对象实例。</returns>
        public T Get(params object[] parameters)
        {
            if (_objects.TryDequeue(out T obj))
            {
                obj.Initialize(parameters);
                return obj;
            }

            if (_currentSize < _maxPoolSize)
            {
                _currentSize++;
                var newObj = CreateNewObject();
                newObj.Initialize(parameters);
                return newObj;
            }

            return null;
        }

        /// <summary>
        /// 将对象回收到对象池中。
        /// 调用 Reset 方法重置对象状态，如果池已满则调用 Dispose 释放对象。
        /// </summary>
        /// <param name="obj">需要回收的对象。</param>
        public void Recycle(T obj)
        {
            obj.Reset();

            if (_objects.Count < _maxPoolSize)
            {
                _objects.Enqueue(obj);
            }
            else
            {
                obj.Dispose(); // 池已满，彻底释放资源
                _currentSize--;
            }
        }

        /// <summary>
        /// 统一将对象回收到对象池中。
        /// 调用 Reset 方法重置对象状态，如果池已满则调用 Dispose 释放对象。
        /// </summary>
        /// <param name="obj"></param>
        public void Recycle(IPoolable obj)
        {
            if (obj is T typedObj)
            {
                typedObj.Reset();

                if (_objects.Count < _maxPoolSize)
                {
                    _currentSize++;
                    _objects.Enqueue(typedObj);
                }
                else
                {
                    obj.Dispose(); // 池已满，彻底释放资源
                }
            }
        }

        /// <summary>
        /// 根据指定的条件清理对象池中的对象。
        /// 满足条件的对象将调用 Dispose 方法释放。
        /// </summary>
        /// <param name="shouldCleanup">一个条件函数，返回 true 表示需要清理该对象。</param>
        public void Cleanup(Func<T, bool> shouldCleanup)
        {
            int count = _objects.Count;

            for (int i = 0; i < count; i++)
            {
                if (_objects.TryDequeue(out T obj))
                {
                    if (shouldCleanup(obj))
                    {
                        obj.Dispose(); // 清理符合条件的对象并释放资源
                        _currentSize--;
                    }
                    else
                    {
                        _objects.Enqueue(obj);
                    }
                }
            }
        }

        /// <summary>
        /// 清理对象池中所有对象，移除所有对象并重置计数。
        /// 调用 Dispose 释放每个对象的资源。
        /// </summary>
        public void CleanupAll()
        {
            while (_objects.TryDequeue(out T obj))
            {
                obj.Dispose(); // 释放对象资源
            }
            _currentSize = 0;
        }

        /// <summary>
        /// 创建一个新的对象实例。
        /// </summary>
        /// <returns>新创建的对象。</returns>
        private T CreateNewObject()
        {
            return new T();
        }

    }
}
