/// <summary>
/// EventDataBody 类用于封装事件的回调函数及其优先级信息。
/// 通过这个类，可以将事件的回调函数（即响应事件时执行的操作）与其对应的优先级绑定在一起。
/// 该类提供了无参、单参、双参和三参的不同版本，以适应不同类型的事件需求。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 存储事件的回调函数及其优先级信息。
/// 2. 提供无参、单参、双参和三参的事件回调函数。
/// 3. 对象池管理，用于减少频繁的对象创建和销毁。
/// </remarks>
using UnityEngine.Events;
using PoolModule;

namespace EventModule
{
    public class EventDataBody<T> : IPoolable
    {
        /// <summary>
        /// 事件的回调函数，用于处理事件触发后的具体操作。
        /// </summary>
        public UnityAction<T> EventAction;

        /// <summary>
        /// 事件的优先级，用于决定事件处理的顺序。
        /// 优先级数值越高，事件越早被处理。
        /// </summary>
        public int Priority;

        /// <summary>
        /// 默认构造函数，用于创建空的 EventDataBody 实例。
        /// </summary>
        public EventDataBody() { }

        /// <summary>
        /// 带参构造函数，用于创建包含指定回调函数和优先级的 EventDataBody 实例。
        /// </summary>
        /// <param name="action">事件的回调函数</param>
        /// <param name="priority">事件的优先级</param>
        public EventDataBody(UnityAction<T> action, int priority)
        {
            EventAction = action;
            Priority = priority;
        }

        /// <summary>
        /// 从对象池中获取 EventDataBody 实例。
        /// </summary>
        /// <param name="action">事件的回调函数</param>
        /// <param name="priority">事件的优先级</param>
        /// <returns>返回 EventDataBody 实例</returns>
        public static EventDataBody<T> Create(UnityAction<T> action, int priority)
        {
            var eventDataBody = GenericObjectPoolFactory.Instance.GetObject<EventDataBody<T>>();
            eventDataBody.EventAction = action;
            eventDataBody.Priority = priority;
            return eventDataBody;
        }

        /// <summary>
        /// 回收 EventDataBody 实例到对象池中。
        /// </summary>
        public void Recycle()
        {
            EventAction = null;
            GenericObjectPoolFactory.Instance.RecycleObject(this);
        }

        /// <summary>
        /// 实现 IPoolable 接口的初始化方法。
        /// </summary>
        public void Initialize(params object[] parameters)
        {
            if (parameters.Length > 0 && parameters[0] is UnityAction<T> action)
            {
                EventAction = action;
            }

            if (parameters.Length > 1 && parameters[1] is int priority)
            {
                Priority = priority;
            }
        }

        /// <summary>
        /// 实现 IPoolable 接口的重置方法，清除所有引用。
        /// </summary>
        public void Reset()
        {
            EventAction = null;
            Priority = 0;
        }

        /// <summary>
        /// 实现 IPoolable 接口的 Dispose 方法，用于彻底清理对象。
        /// </summary>
        public void Dispose()
        {
            Reset();
        }
    }

    public class EventDataBody<T1, T2> :IPoolable
    {
        /// <summary>
        /// 双参数事件的回调函数，用于处理事件触发后的具体操作。
        /// </summary>
        public UnityAction<T1, T2> EventAction;

        /// <summary>
        /// 事件的优先级，用于决定事件处理的顺序。
        /// 优先级数值越高，事件越早被处理。
        /// </summary>
        public int Priority;

        public EventDataBody() { }

        public EventDataBody(UnityAction<T1, T2> action, int priority)
        {
            EventAction = action;
            Priority = priority;
        }

        public static EventDataBody<T1, T2> Create(UnityAction<T1, T2> action, int priority)
        {
            var eventDataBody = GenericObjectPoolFactory.Instance.GetObject<EventDataBody<T1, T2>>();
            eventDataBody.EventAction = action;
            eventDataBody.Priority = priority;
            return eventDataBody;
        }

        public void Recycle()
        {
            EventAction = null;
            GenericObjectPoolFactory.Instance.RecycleObject(this);
        }

        /// <summary>
        /// 实现 IPoolable 接口的初始化方法。
        /// </summary>
        public void Initialize(params object[] parameters)
        {
            if (parameters.Length > 0 && parameters[0] is UnityAction<T1, T2> action)
            {
                EventAction = action;
            }

            if (parameters.Length > 1 && parameters[1] is int priority)
            {
                Priority = priority;
            }
        }

        /// <summary>
        /// 实现 IPoolable 接口的重置方法，清除所有引用。
        /// </summary>
        public void Reset()
        {
            EventAction = null;
            Priority = 0;
        }

        /// <summary>
        /// 实现 IPoolable 接口的 Dispose 方法，用于彻底清理对象。
        /// </summary>
        public void Dispose()
        {
            Reset();
        }

    }

    public class EventDataBody<T1, T2, T3> :IPoolable
    {
        /// <summary>
        /// 三参数事件的回调函数，用于处理事件触发后的具体操作。
        /// </summary>
        public UnityAction<T1, T2, T3> EventAction;

        /// <summary>
        /// 事件的优先级，用于决定事件处理的顺序。
        /// 优先级数值越高，事件越早被处理。
        /// </summary>
        public int Priority;

        public EventDataBody() { }

        public EventDataBody(UnityAction<T1, T2, T3> action, int priority)
        {
            EventAction = action;
            Priority = priority;
        }

        public static EventDataBody<T1, T2, T3> Create(UnityAction<T1, T2, T3> action, int priority)
        {
            var eventDataBody = GenericObjectPoolFactory.Instance.GetObject<EventDataBody<T1, T2, T3>>();
            eventDataBody.EventAction = action;
            eventDataBody.Priority = priority;
            return eventDataBody;
        }

        public void Recycle()
        {
            EventAction = null;
            GenericObjectPoolFactory.Instance.RecycleObject(this);
        }

        /// <summary>
        /// 实现 IPoolable 接口的初始化方法。
        /// </summary>
        public void Initialize(params object[] parameters)
        {
            if (parameters.Length > 0 && parameters[0] is UnityAction<T1, T2, T3> action)
            {
                EventAction = action;
            }

            if (parameters.Length > 1 && parameters[1] is int priority)
            {
                Priority = priority;
            }
        }

        /// <summary>
        /// 实现 IPoolable 接口的重置方法，清除所有引用。
        /// </summary>
        public void Reset()
        {
            EventAction = null;
            Priority = 0;
        }

        /// <summary>
        /// 实现 IPoolable 接口的 Dispose 方法，用于彻底清理对象。
        /// </summary>
        public void Dispose()
        {
            Reset();
        }
    }

    public class EventDataBody : IPoolable
    {
        /// <summary>
        /// 无参事件的回调函数，用于处理事件触发后的具体操作。
        /// </summary>
        public UnityAction EventAction;

        /// <summary>
        /// 事件的优先级，用于决定事件处理的顺序。
        /// 优先级数值越高，事件越早被处理。
        /// </summary>
        public int Priority;

        public EventDataBody() { }

        public EventDataBody(UnityAction action, int priority)
        {
            EventAction = action;
            Priority = priority;
        }

        public static EventDataBody Create(UnityAction action, int priority)
        {
            var eventDataBody = GenericObjectPoolFactory.Instance.GetObject<EventDataBody>();
            eventDataBody.EventAction = action;
            eventDataBody.Priority = priority;
            return eventDataBody;
        }

        public void Recycle()
        {
            EventAction = null;
            GenericObjectPoolFactory.Instance.RecycleObject(this);
        }

        /// <summary>
        /// 实现 IPoolable 接口的初始化方法。
        /// </summary>
        public void Initialize(params object[] parameters)
        {
            if (parameters.Length > 0 && parameters[0] is UnityAction action)
            {
                EventAction = action;
            }

            if (parameters.Length > 1 && parameters[1] is int priority)
            {
                Priority = priority;
            }
        }

        /// <summary>
        /// 实现 IPoolable 接口的重置方法，清除所有引用。
        /// </summary>
        public void Reset()
        {
            EventAction = null;
            Priority = 0;
        }

        /// <summary>
        /// 实现 IPoolable 接口的 Dispose 方法，用于彻底清理对象。
        /// </summary>
        public void Dispose()
        {
            Reset();
        }
    }
}
