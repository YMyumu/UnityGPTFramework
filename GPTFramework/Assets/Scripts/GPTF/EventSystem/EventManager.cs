/// <summary>
/// EventManager 类是事件系统的核心管理器，
/// 负责所有事件的注册、移除和派发操作。
/// 提供无参、单参、双参和三参的事件管理。
/// 通过 EventManager 类，开发者可以方便地管理全局事件，
/// 以实现模块之间的解耦和事件驱动的编程模型。
/// 新增了递归派发保护，确保事件在回调中再次派发时不会进入无限递归。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 管理事件的注册、移除和派发。
/// 2. 提供无参、单参、双参和三参的事件管理。
/// 3. 管理事件的优先级和派发顺序，确保关键事件优先处理。
/// 4. 添加递归派发保护，防止事件回调中再次派发时出现无限递归。
/// 5. 清除所有注册的事件，释放资源。
/// </remarks>
using PoolModule;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EventModule
{
    public class EventManager : Singleton<EventManager>
    {
        /// <summary>
        /// 事件字典，用于存储事件名称与事件数据的映射。
        /// </summary>
        private Dictionary<string, EventDataBase> eventDictionary = new Dictionary<string, EventDataBase>();

        /// <summary>
        /// 注册单参数事件，将回调函数与事件名称关联。
        /// </summary>
        /// <typeparam name="T">事件参数的类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="action">回调函数</param>
        /// <param name="priority">事件优先级，默认为 0</param>
        public void Register<T>(string eventName, UnityAction<T> action, int priority = 0)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData<T> eventData)
                {
                    eventData.AddEvent(action, priority);
                }
            }
            else
            {
                var eventData = GenericObjectPoolFactory.Instance.GetObject<EventData<T>>();
                eventData.AddEvent(action, priority);
                eventDictionary[eventName] = eventData;
            }
        }

        /// <summary>
        /// 注册双参数事件，将回调函数与事件名称关联。
        /// </summary>
        /// <typeparam name="T1">第一个事件参数的类型</typeparam>
        /// <typeparam name="T2">第二个事件参数的类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="action">回调函数</param>
        /// <param name="priority">事件优先级，默认为 0</param>
        public void Register<T1, T2>(string eventName, UnityAction<T1, T2> action, int priority = 0)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData<T1, T2> eventData)
                {
                    eventData.AddEvent(action, priority);
                }
            }
            else
            {
                var eventData = GenericObjectPoolFactory.Instance.GetObject<EventData<T1, T2>>();
                eventData.AddEvent(action, priority);
                eventDictionary[eventName] = eventData;
            }
        }

        /// <summary>
        /// 注册三参数事件，将回调函数与事件名称关联。
        /// </summary>
        /// <typeparam name="T1">第一个事件参数的类型</typeparam>
        /// <typeparam name="T2">第二个事件参数的类型</typeparam>
        /// <typeparam name="T3">第三个事件参数的类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="action">回调函数</param>
        /// <param name="priority">事件优先级，默认为 0</param>
        public void Register<T1, T2, T3>(string eventName, UnityAction<T1, T2, T3> action, int priority = 0)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData<T1, T2, T3> eventData)
                {
                    eventData.AddEvent(action, priority);
                }
            }
            else
            {
                var eventData = GenericObjectPoolFactory.Instance.GetObject<EventData<T1, T2, T3>>();
                eventData.AddEvent(action, priority);
                eventDictionary[eventName] = eventData;
            }
        }

        /// <summary>
        /// 注册无参事件，将回调函数与事件名称关联。
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="action">回调函数</param>
        /// <param name="priority">事件优先级，默认为 0</param>
        public void Register(string eventName, UnityAction action, int priority = 0)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData eventData)
                {
                    eventData.AddEvent(action, priority);
                }
            }
            else
            {
                var eventData = GenericObjectPoolFactory.Instance.GetObject<EventData>();
                eventData.AddEvent(action, priority);
                eventDictionary[eventName] = eventData;
            }
        }

        /// <summary>
        /// 移除单参数事件的回调函数，并将其从对象池中移除。
        /// </summary>
        /// <typeparam name="T">事件参数的类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="action">要移除的回调函数</param>
        public void Remove<T>(string eventName, UnityAction<T> action)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData<T> eventData)
                {
                    eventData.RemoveEvent(action);
                    if (!eventData.HasEvents())
                    {
                        eventDictionary.Remove(eventName);
                        GenericObjectPoolFactory.Instance.RecycleObject(eventData);
                    }
                }
            }
        }

        /// <summary>
        /// 移除双参数事件的回调函数，并将其从对象池中移除。
        /// </summary>
        /// <typeparam name="T1">第一个事件参数的类型</typeparam>
        /// <typeparam name="T2">第二个事件参数的类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="action">要移除的回调函数</param>
        public void Remove<T1, T2>(string eventName, UnityAction<T1, T2> action)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData<T1, T2> eventData)
                {
                    eventData.RemoveEvent(action);
                    if (!eventData.HasEvents())
                    {
                        eventDictionary.Remove(eventName);
                        GenericObjectPoolFactory.Instance.RecycleObject(eventData);
                    }
                }
            }
        }

        /// <summary>
        /// 移除三参数事件的回调函数，并将其从对象池中移除。
        /// </summary>
        /// <typeparam name="T1">第一个事件参数的类型</typeparam>
        /// <typeparam name="T2">第二个事件参数的类型</typeparam>
        /// <typeparam name="T3">第三个事件参数的类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="action">要移除的回调函数</param>
        public void Remove<T1, T2, T3>(string eventName, UnityAction<T1, T2, T3> action)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData<T1, T2, T3> eventData)
                {
                    eventData.RemoveEvent(action);
                    if (!eventData.HasEvents())
                    {
                        eventDictionary.Remove(eventName);
                        GenericObjectPoolFactory.Instance.RecycleObject(eventData);
                    }
                }
            }
        }

        /// <summary>
        /// 移除无参事件的回调函数，并将其从对象池中移除。
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="action">要移除的回调函数</param>
        public void Remove(string eventName, UnityAction action)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData eventData)
                {
                    eventData.RemoveEvent(action);
                    if (!eventData.HasEvents())
                    {
                        eventDictionary.Remove(eventName);
                        GenericObjectPoolFactory.Instance.RecycleObject(eventData);
                    }
                }
            }
        }

        /// <summary>
        /// 派发单参数事件，调用所有已注册的回调函数。
        /// </summary>
        /// <typeparam name="T">事件参数的类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="data">事件参数</param>
        public void Dispatch<T>(string eventName, T data)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData<T> eventData)
                {
                    eventData.Dispatch(data);
                }
                else
                {
                    LogManager.LogWarning($"事件 {eventName} 没有找到匹配的回调函数。");
                }
            }
            else
            {
                LogManager.LogWarning($"事件 {eventName} 没有任何回调函数绑定。");
            }
        }

        /// <summary>
        /// 派发双参数事件，调用所有已注册的回调函数。
        /// </summary>
        /// <typeparam name="T1">第一个事件参数的类型</typeparam>
        /// <typeparam name="T2">第二个事件参数的类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="arg1">第一个事件参数</param>
        /// <param name="arg2">第二个事件参数</param>
        public void Dispatch<T1, T2>(string eventName, T1 arg1, T2 arg2)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData<T1, T2> eventData)
                {
                    eventData.Dispatch(arg1, arg2);
                }
                else
                {
                    LogManager.LogWarning($"事件 ({eventName} {arg1} {arg2}) 没有找到匹配的回调函数。");
                }
            }
            else
            {
                LogManager.LogWarning($"事件 {eventName} 没有任何回调函数绑定。");
            }
        }

        /// <summary>
        /// 派发三参数事件，调用所有已注册的回调函数。
        /// </summary>
        /// <typeparam name="T1">第一个事件参数的类型</typeparam>
        /// <typeparam name="T2">第二个事件参数的类型</typeparam>
        /// <typeparam name="T3">第三个事件参数的类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="arg1">第一个事件参数</param>
        /// <param name="arg2">第二个事件参数</param>
        /// <param name="arg3">第三个事件参数</param>
        public void Dispatch<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData<T1, T2, T3> eventData)
                {
                    eventData.Dispatch(arg1, arg2, arg3);
                }
                else
                {
                    LogManager.LogWarning($"事件 ({eventName} {arg1} {arg2} {arg3}) 没有找到匹配的回调函数。");
                }
            }
            else
            {
                LogManager.LogWarning($"事件 {eventName} 没有任何回调函数绑定。");
            }
        }

        /// <summary>
        /// 派发无参事件，调用所有已注册的回调函数。
        /// </summary>
        /// <param name="eventName">事件名称</param>
        public void Dispatch(string eventName)
        {
            if (eventDictionary.TryGetValue(eventName, out var eventDataBase))
            {
                if (eventDataBase is EventData eventData)
                {
                    eventData.Dispatch();
                }
                else
                {
                    LogManager.LogWarning($"事件 {eventName} 没有找到匹配的回调函数。");
                }
            }
            else
            {
                LogManager.LogWarning($"事件 {eventName} 没有任何回调函数绑定。");
            }
        }

        /// <summary>
        /// 清除所有注册的事件，释放事件管理器中的资源，并将事件数据回收到对象池。
        /// </summary>
        public void Clear()
        {
            int clearedCount = 0;

            // 先获取 eventDictionary 的键列表，避免在遍历时修改集合
            var keys = new List<string>(eventDictionary.Keys);

            // 遍历键列表并清理每个事件的数据
            foreach (var eventName in keys)
            {
                if (eventDictionary.TryGetValue(eventName, out var eventDataBase) && eventDataBase != null)
                {
                    // 移除所有回调函数
                    eventDataBase.RemoveEvents(this, eventName);

                    // 回收事件数据对象到对象池
                    GenericObjectPoolFactory.Instance.RecycleObject(eventDataBase);

                    clearedCount++;
                }
            }

            // 清空事件字典
            eventDictionary.Clear();

            // 记录日志，说明已清除的事件数量
            LogManager.LogInfo($"所有事件已清除，总计 {clearedCount} 个事件被回收到对象池。");
        }






    }
}
