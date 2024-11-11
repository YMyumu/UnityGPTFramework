using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PoolModule
{
    /// <summary>
    /// 通用对象池，用于管理不实现 IPoolable 接口的对象类型。
    /// 支持 List<T>、Dictionary<TKey, TValue> 等系统类型，
    /// 适合非 IPoolable 类型的高效对象管理。
    /// </summary>
    public class GenericObjectPool_NonPoolable<T> where T : class, new()
    {
        // 使用线程安全的队列以确保对象池的线程安全性
        private readonly ConcurrentQueue<T> _objects = new ConcurrentQueue<T>();
        private readonly int _maxPoolSize;  // 最大对象池大小限制
        private int _currentSize;           // 当前池中实际的对象数量

        public GenericObjectPool_NonPoolable(int initialSize, int maxPoolSize)
        {
            _maxPoolSize = maxPoolSize;
            _currentSize = 0;

            // 初始化对象池，预先创建指定数量的对象
            for (int i = 0; i < initialSize; i++)
            {
                _objects.Enqueue(CreateNewObject());
                _currentSize++;
            }
        }

        /// <summary>
        /// 从对象池中获取一个对象。如果池为空且未达到最大容量，则创建新对象。
        /// </summary>
        /// <returns>可用的对象实例</returns>
        public T Get()
        {
            // 尝试从池中取出一个对象
            if (_objects.TryDequeue(out T obj))
            {
                // 清除对象内容，为下次使用做准备
                ClearObject(obj);
                return obj;
            }

            // 如果池为空且尚未达到最大容量，创建新对象并增加计数
            if (_currentSize < _maxPoolSize)
            {
                _currentSize++;
                var newObj = CreateNewObject();
                return newObj;
            }

            // 如果池已满，返回 null，表明无法提供新对象
            return null;
        }

        /// <summary>
        /// 将对象回收到池中。如果池已满，则丢弃对象。
        /// </summary>
        /// <param name="obj">要回收的对象</param>
        public void Recycle(T obj)
        {
            // 清除对象内容以便重复使用
            ClearObject(obj);
            if (_objects.Count < _maxPoolSize)
            {
                // 将对象重新放入池中，但不增加 _currentSize，因为对象总数不变
                _objects.Enqueue(obj);
            }
            else
            {
                // 如果池已满，直接丢弃对象并减少计数
                _currentSize--;
            }
        }

        /// <summary>
        /// 根据指定条件清理对象池中的对象。
        /// 满足条件的对象将被释放（Dispose 或丢弃）。
        /// </summary>
        /// <param name="shouldCleanup">一个条件函数，返回 true 表示需要清理该对象</param>
        public void Cleanup(Func<T, bool> shouldCleanup)
        {
            int count = _objects.Count;

            for (int i = 0; i < count; i++)
            {
                if (_objects.TryDequeue(out T obj))
                {
                    if (shouldCleanup(obj))
                    {
                        // 条件满足时，直接减少计数，并丢弃该对象
                        _currentSize--;
                    }
                    else
                    {
                        // 如果不满足条件，将对象重新放回队列
                        _objects.Enqueue(obj);
                    }
                }
            }
        }

        /// <summary>
        /// 清空对象池中的所有对象。
        /// </summary>
        public void CleanupAll()
        {
            // 从对象池中清空所有对象
            while (_objects.TryDequeue(out T obj))
            {
                ClearObject(obj); // 清除对象内容，防止残留数据
            }
            _objects.Clear(); // 确保队列彻底清空
            _currentSize = 0; // 重置对象池计数
        }

        /// <summary>
        /// 创建一个新的对象实例，用于初始化池或池中对象不足时调用。
        /// </summary>
        private T CreateNewObject()
        {
            return new T();
        }

        /// <summary>
        /// 清除对象的内容。根据对象类型选择适当的清理方法。
        /// </summary>
        private void ClearObject(T obj)
        {
            // 如果对象是列表类型，调用 Clear() 方法清空列表内容
            if (obj is IList list)
            {
                list.Clear();
                return;
            }

            // 如果对象是字典类型，调用 Clear() 方法清空字典内容
            if (obj is IDictionary dictionary)
            {
                dictionary.Clear();
                return;
            }

            // 如果对象是数组类型，调用 Array.Clear 清空数组内容
            if (obj is Array array)
            {
                Array.Clear(array, 0, array.Length);
                return;
            }

            // 如果对象是 Stopwatch 类型，调用 Reset() 方法重置计时器
            if (obj is Stopwatch stopwatch)
            {
                stopwatch.Reset();
                return;
            }

            // 如果对象是 StringBuilder 类型，调用 Clear() 方法清空字符串内容
            if (obj is StringBuilder stringBuilder)
            {
                stringBuilder.Clear();
                return;
            }

            // 如果对象是 Queue 类型，调用 Clear() 方法清空队列
            if (obj is Queue<object> queue)
            {
                queue.Clear();
                return;
            }

            // 如果对象是 Stack 类型，调用 Clear() 方法清空堆栈
            if (obj is Stack<object> stack)
            {
                stack.Clear();
                return;
            }

            // 针对其他类型，可以在此处继续扩展特定清理逻辑
        }
    }
}
