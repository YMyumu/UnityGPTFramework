/// <summary>
/// DelayedTaskList 类用于管理一组共享同一执行时间的延时任务。
/// 每个延时任务存储在 DelayedTaskData 中，而 DelayedTaskList 则将这些任务分组并统一管理。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. Time：表示该组任务的执行时间，所有任务在该时间点统一执行。
/// 2. DelayedTaskDataList：这是一个延时任务的数据列表，存储了所有将在相同时间执行的任务。
/// 3. CompareTo：用于比较两个 DelayedTaskList 的执行时间，便于在最小堆中排序管理。
/// 4. Dispose：释放当前任务列表中的资源，避免内存泄漏，特别是在任务被移除或不再使用时。
/// </remarks>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DelayedTaskModule
{
    public class DelayedTaskList : IComparable, IEnumerable<DelayedTaskData>, IPoolable
    {
        private bool _disposed = false;  // 标记对象是否已被释放，防止重复释放
        public long Time;  // 该任务列表的执行时间，所有任务将在该时间点统一触发
        public List<DelayedTaskData> DelayedTaskDataList;  // 存储该时间点将要执行的所有任务

        /// <summary>
        /// 比较两个 DelayedTaskList 的执行时间，用于堆排序。执行时间更早的列表优先。
        /// </summary>
        /// <param name="obj">另一个 DelayedTaskList 对象</param>
        /// <returns>返回比较结果，按执行时间从小到大排序</returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            return CompareTo((DelayedTaskList)obj);
        }

        /// <summary>
        /// 按时间戳比较两个任务列表的顺序，便于在堆结构中管理延时任务。
        /// </summary>
        /// <param name="obj">另一个 DelayedTaskList</param>
        /// <returns>比较结果，返回正数表示当前列表时间较晚，负数表示时间较早</returns>
        public int CompareTo(DelayedTaskList obj)
        {
            return Time.CompareTo(obj.Time);
        }

        /// <summary>
        /// 支持枚举任务列表中的延时任务，便于遍历操作。
        /// </summary>
        /// <returns>返回任务列表的枚举器</returns>
        IEnumerator<DelayedTaskData> IEnumerable<DelayedTaskData>.GetEnumerator()
        {
            return DelayedTaskDataList.GetEnumerator();
        }

        /// <summary>
        /// 支持枚举器，便于通过 foreach 等遍历延时任务。
        /// </summary>
        /// <returns>返回非泛型的枚举器</returns>
        public IEnumerator GetEnumerator()
        {
            return DelayedTaskDataList.GetEnumerator();
        }

        /// <summary>
        /// 释放任务列表中的资源。确保任务列表在不再使用时被清理，防止内存泄漏。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源的实际实现函数，用于在需要时手动或自动清理对象。
        /// </summary>
        /// <param name="disposing">指示是否手动调用 Dispose 方法</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                // 释放托管资源：清空延时任务数据列表
                DelayedTaskDataList.Clear();
                DelayedTaskDataList = null;
            }
            _disposed = true;  // 标记对象已被释放
        }

        public void Initialize(params object[] parameters)
        {
        }

        public void Reset()
        {
            // 释放托管资源：清空延时任务数据列表
            Time = 0;
            DelayedTaskDataList.Clear();
            DelayedTaskDataList = null;
        }

        /// <summary>
        /// 析构函数，在对象被垃圾回收时自动调用，确保资源被释放。
        /// </summary>
        ~DelayedTaskList()
        {
            Dispose(false);
        }
    }
}
