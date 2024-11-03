/// <summary>
/// DelayedTaskScheduler 类是整个延时任务调度系统的核心类，负责调度和管理所有延时任务。
/// 通过时间戳和最小堆的方式确保任务按时间顺序执行。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. AddDelayedTask：向调度器添加一个延时任务，根据给定的时间戳，在到达时间时执行任务,可选择循环或者不循环。
/// 2. RemoveDelayedTask：移除一个已注册的延时任务，防止任务到时间时被执行。
/// 3. UpdateTime：更新当前时间并调度到达时间的任务，确保任务按时执行。
/// 4. Dispose：释放调度器中的所有任务和资源，防止内存泄漏。
/// </remarks>
using System;
using System.Collections.Generic;
using LDataStruct;  // 使用自定义的堆结构
using UnityEngine;
using PoolModule;
using EventModule;

namespace DelayedTaskModule
{
    public class DelayedTaskScheduler : MonoSingleton<DelayedTaskScheduler>, IDisposable
    {
        /// <summary>
        /// 任务字典，存储所有的延时任务，以任务的唯一 Token 作为键。
        /// 用于快速查找和删除特定任务。
        /// </summary>
        private Dictionary<string, DelayedTaskData> _taskDic = new Dictionary<string, DelayedTaskData>();

        /// <summary>
        /// 任务列表，在暂停时存储需要暂停的任务
        /// </summary>
        private List<DelayedTaskData> _pausedTasks = new List<DelayedTaskData>();  // 存储暂停的任务

        /// <summary>
        /// 任务列表字典，按时间戳将任务分组存储。时间戳作为键，任务列表作为值。
        /// 用于在任务时间到达时，批量执行同一时间的任务。
        /// </summary>
        private Dictionary<long, DelayedTaskList> _delayedTaskDict = new Dictionary<long, DelayedTaskList>();

        /// <summary>
        /// 任务队列，通过最小堆管理所有任务列表，确保任务按时间顺序被调度执行。
        /// 最小堆使得每次取出的任务都是时间最早的。
        /// </summary>
        private Heap<DelayedTaskList> _delayedTaskQueue = new Heap<DelayedTaskList>(10, HeapType.MinHeap);

        private bool _disposed = false;  // 标记调度器是否已被释放

        [SerializeField] private long CurrentTime;  // 当前系统时间戳，单位为毫秒

        #region 时间事件管理

        /// <summary>
        /// 添加一个延时任务，指定任务将在某个时间点执行，可以设置是否循环，以及每次循环执行等待的时间。
        /// 该方法根据给定的秒数，任务添加到延时任务队列中。
        /// </summary>
        /// <param name="_time">任务的执行时间，秒数</param>
        /// <param name="action">任务到达时间时执行的具体操作</param>
        /// <param name="earlyRemoveCallback">任务被提前移除时执行的回调操作</param>
        /// <param name="affectedByPause">是否受暂停影响</param>
        /// <param name="isLooping">是否循环</param>
        /// <param name="interval">循环执行的等待时间</param>
        /// <returns>返回任务的唯一标识符，用于以后移除任务</returns>
        public string AddDelayedTask(float time, Action action, Action earlyRemoveCallback = null, bool affectedByPause = false, bool isLooping = false, long interval = 0)
        {
            // 获得时间戳
            long timestamp = TimerUtil.GetLaterMilliSecondsBySecond(time);
            return AddDelayedTaskWithTimestamp(timestamp, action, earlyRemoveCallback, affectedByPause, isLooping, interval);


        }

        /// <summary>
        /// 添加一个延时任务，指定任务将在某个时间点执行，可以设置是否循环，以及每次循环执行等待的时间。
        /// 该方法根据给定的时间戳，任务添加到延时任务队列中。
        /// </summary>
        /// <param name="timestamp">任务的执行时间戳</param>
        /// <param name="action">任务到达时间时执行的具体操作</param>
        /// <param name="earlyRemoveCallback">任务被提前移除时执行的回调操作</param>
        /// <param name="affectedByPause">是否受暂停影响</param>
        /// <param name="isLooping">是否循环</param>
        /// <param name="interval">循环执行的等待时间</param>
        /// <returns>返回任务的唯一标识符，用于以后移除任务</returns>
        private string AddDelayedTaskWithTimestamp(long timestamp, Action action, Action earlyRemoveCallback = null, bool affectedByPause = false, bool isLooping = false, long interval = 0)
        {
            if (timestamp  < CurrentTime)
            {
                // 如果给定的执行时间已过去，记录错误并返回 null
                LogManager.LogError($"The time is pass. Time is {timestamp } CurrentTime is {CurrentTime}");
                return null;
            }

            // 检查是否已有相同时间的任务列表
            if (!_delayedTaskDict.TryGetValue(timestamp , out var delayedTaskList))
            {
                // 如果没有，则从对象池中获取新的任务列表，并初始化
                delayedTaskList = GenericObjectPoolFactory.Instance.GetObject<DelayedTaskList>();
                delayedTaskList.Time = timestamp ;
                delayedTaskList.DelayedTaskDataList = GenericObjectPool_NonPoolableFactory.Instance.GetObject<List<DelayedTaskData>>();
                delayedTaskList.DelayedTaskDataList.Clear();  // 确保列表为空
                _delayedTaskQueue.Insert(delayedTaskList);  // 将任务列表插入最小堆中
                _delayedTaskDict.Add(timestamp , delayedTaskList);  // 记录到字典中
            }

            // 生成任务的唯一标识符，用于以后查找或移除任务
            string token = Guid.NewGuid().ToString();
            var delayedTaskData = GenericObjectPoolFactory.Instance.GetObject<DelayedTaskData>();
            delayedTaskData.Time = timestamp ;
            delayedTaskData.Action = action;
            delayedTaskData.Token = token;
            delayedTaskData.EarlyRemoveCallback = earlyRemoveCallback;
            delayedTaskData.IsLooping = isLooping;  // 是否循环
            delayedTaskData.Interval = interval;    // 循环间隔
            delayedTaskData.AffectedByPause = affectedByPause;  // 设置是否受暂停影响


            delayedTaskList.DelayedTaskDataList.Add(delayedTaskData);  // 将任务添加到对应时间的任务列表中
            _taskDic.Add(token, delayedTaskData);  // 将任务存入任务字典，便于查找和管理
            return token;
        }


        /// <summary>
        /// 移除指定的延时任务，防止任务在到达执行时间时被执行。
        /// </summary>
        /// <param name="token">要移除的任务的唯一标识符</param>
        /// <returns>返回是否成功移除任务</returns>
        public bool RemoveDelayedTask(string token)
        {
            _taskDic.TryGetValue(token, out var delayedTaskData);
            if (delayedTaskData == null)
                return false;  // 如果找不到任务，返回失败
            _taskDic.Remove(token);

            if (_delayedTaskDict.TryGetValue(delayedTaskData.Time, out var delayedTaskList))
            {
                // 从任务列表中移除该任务
                bool removeSuccess = delayedTaskList.DelayedTaskDataList.Remove(delayedTaskData);
                if (removeSuccess)
                    delayedTaskData.EarlyRemoveCallback?.Invoke();  // 如果任务有移除回调，执行回调
                if (delayedTaskList.DelayedTaskDataList.Count == 0)
                {
                    // 如果任务列表为空，删除并回收资源
                    _delayedTaskDict.Remove(delayedTaskData.Time);
                    if (_delayedTaskQueue.Delete(delayedTaskList))
                    {
                        GenericObjectPool_NonPoolableFactory.Instance.RecycleObject(delayedTaskList.DelayedTaskDataList);
                        GenericObjectPoolFactory.Instance.RecycleObject(delayedTaskList);
                        GenericObjectPoolFactory.Instance.RecycleObject(delayedTaskData);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 更新时间戳，并根据当前时间执行所有已到时间的任务。
        /// 该方法会遍历最小堆中的任务列表，按时间顺序执行任务。
        /// </summary>
        /// <param name="time">当前时间戳，单位为毫秒</param>
        public void UpdateTime(long time)
        {
            CurrentTime = time;

            // 遍历任务队列，处理已到期的任务
            while (_delayedTaskQueue.Count > 0 && _delayedTaskQueue.GetHead().Time <= time)
            {
                long targetTime = _delayedTaskQueue.GetHead().Time;
                _delayedTaskDict.Remove(targetTime);  // 移除已经到期的任务列表
                var delayedTaskList = _delayedTaskQueue.DeleteHead();  // 获取当前执行的任务列表

                foreach (DelayedTaskData delayedTaskData in delayedTaskList.DelayedTaskDataList)
                {
                    // 执行任务
                    delayedTaskData.Action?.Invoke();
                    _taskDic.Remove(delayedTaskData.Token);  // 从任务字典中移除已执行的任务

                    // 如果是循环任务，需要重新添加到调度器中 再回收任务对象
                    if (delayedTaskData.IsLooping)
                    {
                        long nextExecutionTime = delayedTaskData.Interval;
                        Action taskAction = delayedTaskData.Action;
                        Action earlyRemoveCallback = delayedTaskData.EarlyRemoveCallback;
                        bool affectedByPause = delayedTaskData.AffectedByPause;
                        long interval = delayedTaskData.Interval;

                        GenericObjectPoolFactory.Instance.RecycleObject(delayedTaskData);

                        // 重新添加到延时任务调度中
                        AddDelayedTask(nextExecutionTime, taskAction, earlyRemoveCallback, affectedByPause, true, interval);
                    }
                    else
                    {
                        // 直接回收非循环任务对象
                        GenericObjectPoolFactory.Instance.RecycleObject(delayedTaskData);
                    }
                    
                }

                // 清理已完成的任务列表
                delayedTaskList.DelayedTaskDataList.Clear();
                GenericObjectPool_NonPoolableFactory.Instance.RecycleObject(delayedTaskList.DelayedTaskDataList);
                GenericObjectPoolFactory.Instance.RecycleObject(delayedTaskList);
            }
        }




        /// <summary>
        /// 暂停时的操作
        /// </summary>
        public void Pause()
        {
            for (int i = 1; i <= _delayedTaskQueue.Count; i++)
            {
                var taskList = _delayedTaskQueue.GetItemAt(i);
                for (int j = 0; j < taskList.DelayedTaskDataList.Count; j++)
                {
                    var task = taskList.DelayedTaskDataList[j];
                    if (task.AffectedByPause)
                    {
                        // 记录暂停时的时间差
                        task.PauseTimeDifference = task.Time - CurrentTime;

                        // 创建一个新的 DelayedTaskData 对象并复制属性
                        var copiedTask = new DelayedTaskData
                        {
                            Time = task.Time,
                            Action = task.Action,
                            Token = task.Token,
                            EarlyRemoveCallback = task.EarlyRemoveCallback,
                            IsLooping = task.IsLooping,
                            Interval = task.Interval,
                            AffectedByPause = task.AffectedByPause,
                            PauseTimeDifference = task.PauseTimeDifference
                        };

                        // 将新的任务对象保存到暂停任务列表中
                        _pausedTasks.Add(copiedTask);

                        // 再移除原任务，确保调度器不再执行它
                        RemoveDelayedTask(task.Token);
                    }
                }
            }
        }

        /// <summary>
        /// 暂停后恢复的操作
        /// </summary>
        public void Continue()
        {
            foreach (var task in _pausedTasks)
            {

                // 计算新的触发时间，补偿暂停期间的时间
                long newTime = CurrentTime + task.PauseTimeDifference;

                // 使用 AddDelayedTaskWithTimestamp 重新添加任务
                AddDelayedTaskWithTimestamp(newTime, task.Action, task.EarlyRemoveCallback, task.AffectedByPause, task.IsLooping, task.Interval);

            }

            // 清空暂停任务列表
            _pausedTasks.Clear();
        }

        #endregion

        #region Mono方法与测试的设置时间代码

        private void OnEnable()
        {
            EventManager.Instance.Register(EventDefine.PAUSE, Pause);
            EventManager.Instance.Register(EventDefine.CONTINUE, Continue);
        }

        private void OnDisable()
        {
            EventManager.Instance.Remove(EventDefine.PAUSE, Pause);
            EventManager.Instance.Remove(EventDefine.CONTINUE, Continue);
        }

        private void Awake()
        {
            UpdateTime(TimerUtil.GetTimeStamp(true));  // 初始化时更新时间为当前时间
        }

        /// <summary>
        /// 每帧更新一次，检查当前时间并执行到时的任务。
        /// </summary>
        public void Update()
        {
            UpdateTime(TimerUtil.GetTimeStamp(true));  // 每帧更新时间
        }

        private void OnDestroy()
        {
            Dispose();  // 当对象销毁时，清理资源
        }

        #endregion

        #region Dispose

        /// <summary>
        /// 释放资源，清理调度器中的所有任务和内存。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _taskDic.Clear();  // 清空所有任务
                    _delayedTaskDict.Clear();  // 清空任务列表
                    _delayedTaskQueue?.Dispose();  // 清理最小堆
                }

                _disposed = true;  // 标记对象已被释放
            }
        }

        /// <summary>
        /// 析构函数，确保对象被垃圾回收时也会释放资源。
        /// </summary>
        ~DelayedTaskScheduler()
        {
            Dispose(false);
        }

        #endregion
    }
}
