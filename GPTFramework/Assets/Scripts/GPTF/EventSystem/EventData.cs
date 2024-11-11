/// <summary>
/// EventData 类用于管理具体的事件数据和操作逻辑。
/// 提供了无参、单参、双参和三参的事件管理功能，负责事件的添加、移除、派发等操作。
/// 该类通过维护一个事件队列来存储和排序事件回调函数，根据优先级对事件进行有序处理。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 管理事件的回调函数及其优先级。
/// 2. 提供无参、单参、双参和三参的事件管理。
/// 3. 通过优先级对事件回调进行排序，并按顺序派发。
/// </remarks>
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace EventModule
{
    /// <summary>
    /// 事件数据的基类，用于处理事件的基本状态和派发循环限制。
    /// 实现了 IPoolable 接口，以便在对象池中进行管理。
    /// </summary>
    public abstract class EventDataBase : IPoolable
    {
        private const int MaxDispatchLoopCount = 10; // 最大派发递归深度限制

        public bool IsDispatching = false; // 当前事件是否正在派发
        public int DispatchLoopCount = 0; // 当前的派发循环计数

        /// <summary>
        /// 初始化方法，在对象从池中获取时调用。
        /// </summary>
        public void Initialize(params object[] parameters)
        {
            OnGet(); // 重置派发状态
        }

        /// <summary>
        /// 重置方法，在对象回收到池中时调用。
        /// </summary>
        public virtual void Reset()
        {
            IsDispatching = false;
            DispatchLoopCount = 0;
        }

        /// <summary>
        /// 释放方法，用于彻底清理资源，避免内存泄漏。
        /// </summary>
        public void Dispose()
        {
            Reset(); // 重置状态
        }

        /// <summary>
        /// 重置派发状态。
        /// </summary>
        public void OnGet()
        {
            IsDispatching = false;
            DispatchLoopCount = 0;
        }

        /// <summary>
        /// 检查事件是否具有注册的监听器。
        /// </summary>
        /// <returns>如果有监听器返回 true，否则返回 false。</returns>
        public abstract bool HasEvents();

        /// <summary>
        /// 移除指定事件管理器中的事件。
        /// </summary>
        /// <param name="eventManager">事件管理器实例。</param>
        /// <param name="eventName">事件名称。</param>
        public abstract void RemoveEvents(EventManager eventManager, string eventName);

        /// <summary>
        /// 检查是否可以继续派发事件。
        /// 防止无限递归。
        /// </summary>
        protected bool CanDispatch()
        {
            if (DispatchLoopCount >= MaxDispatchLoopCount)
            {
                LogManager.LogError($"递归派发深度超出最大限制 {MaxDispatchLoopCount}，已停止事件派发以防止无限递归。");
                return false;
            }
            return true;
        }
    }


    /// <summary>
    /// 泛型事件数据类，用于管理单参事件的回调函数及派发。
    /// </summary>
    /// <typeparam name="T">事件的参数类型</typeparam>
    public class EventData<T> : EventDataBase
    {
        /// <summary>
        /// 事件队列，用于存储回调函数和优先级。
        /// </summary>
        private List<EventDataBody<T>> _eventQueue = new List<EventDataBody<T>>();

        /// <summary>
        /// 添加事件回调函数到队列，并根据优先级排序。
        /// </summary>
        /// <param name="action">事件的回调函数</param>
        /// <param name="priority">事件的优先级</param>
        public void AddEvent(UnityAction<T> action, int priority = 0)
        {
            var eventDataBody = EventDataBody<T>.Create(action, priority);
            if (_eventQueue.Count == 0)
                _eventQueue.Add(eventDataBody);
            else
            {
                // 按优先级插入回调函数，确保事件按照优先级顺序执行
                int index = _eventQueue.FindIndex(e => e.Priority < priority);
                if (index == -1)
                    _eventQueue.Add(eventDataBody);
                else
                    _eventQueue.Insert(index, eventDataBody);
            }
        }

        /// <summary>
        /// 移除指定的事件回调函数，并将其回收到对象池中。
        /// </summary>
        /// <param name="action">要移除的回调函数</param>
        public void RemoveEvent(UnityAction<T> action)
        {
            var eventDataBody = _eventQueue.Find(e => e.EventAction == action);
            if (eventDataBody != null)
            {
                eventDataBody.Recycle();
                _eventQueue.Remove(eventDataBody);
            }
        }

        /// <summary>
        /// 检查事件队列是否为空。
        /// </summary>
        /// <returns>返回 true 表示有事件需要处理，false 表示队列为空</returns>
        public override bool HasEvents()
        {
            return _eventQueue.Count > 0;
        }

        /// <summary>
        /// 从事件管理器中移除所有事件，并将回调函数回收到对象池。
        /// </summary>
        /// <param name="eventManager">事件管理器实例</param>
        /// <param name="eventName">要移除的事件名称</param>
        public override void RemoveEvents(EventManager eventManager, string eventName)
        {
            // 创建 _eventQueue 的副本，避免遍历时修改集合
            var eventQueueCopy = new List<EventDataBody<T>>(_eventQueue);

            foreach (var eventBody in eventQueueCopy)
            {
                eventManager.Remove(eventName, eventBody.EventAction);
                eventBody.Recycle();
            }

            _eventQueue.Clear();  // 清空原始队列
        }

        /// <summary>
        /// 派发事件，依次调用队列中的回调函数。
        /// </summary>
        /// <param name="data">事件参数</param>
        public void Dispatch(T data)
        {
            // 检查递归深度是否超过限制
            if (!CanDispatch()) return;

            DispatchLoopCount++;
            IsDispatching = true;

            foreach (var eventBody in _eventQueue)
            {
                eventBody.EventAction?.Invoke(data);
            }

            IsDispatching = false;
            DispatchLoopCount--;
        }

        public override void Reset()
        {
            base.Reset();
            _eventQueue.Clear();
        }

    }

    /// <summary>
    /// 泛型事件数据类，用于管理双参事件的回调函数及派发。
    /// </summary>
    /// <typeparam name="T1">第一个事件参数的类型</typeparam>
    /// <typeparam name="T2">第二个事件参数的类型</typeparam>
    public class EventData<T1, T2> : EventDataBase
    {
        private List<EventDataBody<T1, T2>> _eventQueue = new List<EventDataBody<T1, T2>>();

        public void AddEvent(UnityAction<T1, T2> action, int priority = 0)
        {
            var eventDataBody = EventDataBody<T1, T2>.Create(action, priority);
            if (_eventQueue.Count == 0)
                _eventQueue.Add(eventDataBody);
            else
            {
                int index = _eventQueue.FindIndex(e => e.Priority < priority);
                if (index == -1)
                    _eventQueue.Add(eventDataBody);
                else
                    _eventQueue.Insert(index, eventDataBody);
            }
        }

        public void RemoveEvent(UnityAction<T1, T2> action)
        {
            var eventDataBody = _eventQueue.Find(e => e.EventAction == action);
            if (eventDataBody != null)
            {
                eventDataBody.Recycle();
                _eventQueue.Remove(eventDataBody);
            }
        }

        public override bool HasEvents()
        {
            return _eventQueue.Count > 0;
        }

        public override void RemoveEvents(EventManager eventManager, string eventName)
        {
            var eventQueueCopy = new List<EventDataBody<T1, T2>>(_eventQueue);

            foreach (var eventBody in eventQueueCopy)
            {
                eventManager.Remove(eventName, eventBody.EventAction);
                eventBody.Recycle();
            }

            _eventQueue.Clear();
        }

        public void Dispatch(T1 arg1, T2 arg2)
        {
            if (!CanDispatch()) return;

            DispatchLoopCount++;
            IsDispatching = true;

            foreach (var eventBody in _eventQueue)
            {
                eventBody.EventAction?.Invoke(arg1, arg2);
            }

            IsDispatching = false;
            DispatchLoopCount--;
        }

        public override void Reset()
        {
            base.Reset();
            _eventQueue.Clear();
        }
    }

    /// <summary>
    /// 泛型事件数据类，用于管理三参事件的回调函数及派发。
    /// </summary>
    /// <typeparam name="T1">第一个事件参数的类型</typeparam>
    /// <typeparam name="T2">第二个事件参数的类型</typeparam>
    /// <typeparam name="T3">第三个事件参数的类型</typeparam>
    public class EventData<T1, T2, T3> : EventDataBase
    {
        private List<EventDataBody<T1, T2, T3>> _eventQueue = new List<EventDataBody<T1, T2, T3>>();

        public void AddEvent(UnityAction<T1, T2, T3> action, int priority = 0)
        {
            var eventDataBody = EventDataBody<T1, T2, T3>.Create(action, priority);
            if (_eventQueue.Count == 0)
                _eventQueue.Add(eventDataBody);
            else
            {
                int index = _eventQueue.FindIndex(e => e.Priority < priority);
                if (index == -1)
                    _eventQueue.Add(eventDataBody);
                else
                    _eventQueue.Insert(index, eventDataBody);
            }
        }

        public void RemoveEvent(UnityAction<T1, T2, T3> action)
        {
            var eventDataBody = _eventQueue.Find(e => e.EventAction == action);
            if (eventDataBody != null)
            {
                eventDataBody.Recycle();
                _eventQueue.Remove(eventDataBody);
            }
        }

        public override bool HasEvents()
        {
            return _eventQueue.Count > 0;
        }

        public override void RemoveEvents(EventManager eventManager, string eventName)
        {
            var eventQueueCopy = new List<EventDataBody<T1, T2, T3>>(_eventQueue);

            foreach (var eventBody in eventQueueCopy)
            {
                eventManager.Remove(eventName, eventBody.EventAction);
                eventBody.Recycle();
            }

            _eventQueue.Clear();
        }

        public void Dispatch(T1 arg1, T2 arg2, T3 arg3)
        {
            if (!CanDispatch()) return;

            DispatchLoopCount++;
            IsDispatching = true;

            foreach (var eventBody in _eventQueue)
            {
                eventBody.EventAction?.Invoke(arg1, arg2, arg3);
            }

            IsDispatching = false;
            DispatchLoopCount--;
        }

        public override void Reset()
        {
            base.Reset();
            _eventQueue.Clear();
        }
    }

    /// <summary>
    /// 无参事件数据类，用于管理无参事件的回调函数及派发。
    /// </summary>
    public class EventData : EventDataBase
    {
        private List<EventDataBody> _eventQueue = new List<EventDataBody>();

        public void AddEvent(UnityAction action, int priority = 0)
        {
            var eventDataBody = EventDataBody.Create(action, priority);
            if (_eventQueue.Count == 0)
                _eventQueue.Add(eventDataBody);
            else
            {
                int index = _eventQueue.FindIndex(e => e.Priority < priority);
                if (index == -1)
                    _eventQueue.Add(eventDataBody);
                else
                    _eventQueue.Insert(index, eventDataBody);
            }
        }

        public void RemoveEvent(UnityAction action)
        {
            var eventDataBody = _eventQueue.Find(e => e.EventAction == action);
            if (eventDataBody != null)
            {
                eventDataBody.Recycle();
                _eventQueue.Remove(eventDataBody);
            }
        }

        public override bool HasEvents()
        {
            return _eventQueue.Count > 0;
        }

        public override void RemoveEvents(EventManager eventManager, string eventName)
        {
            var eventQueueCopy = new List<EventDataBody>(_eventQueue);

            foreach (var eventBody in eventQueueCopy)
            {
                eventManager.Remove(eventName, eventBody.EventAction);
                eventBody.Recycle();
            }

            _eventQueue.Clear();
        }

        public void Dispatch()
        {
            if (!CanDispatch()) return;

            DispatchLoopCount++;
            IsDispatching = true;

            foreach (var eventBody in _eventQueue)
            {
                eventBody.EventAction?.Invoke();
            }

            IsDispatching = false;
            DispatchLoopCount--;
        }

        public override void Reset()
        {
            base.Reset();
            _eventQueue.Clear();
        }
    }
}
