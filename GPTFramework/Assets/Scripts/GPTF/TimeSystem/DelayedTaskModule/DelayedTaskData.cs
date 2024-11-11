/// <summary>
/// DelayedTaskData 类用于表示一个延时任务的数据结构。
/// 每个延时任务包含其预定执行时间、唯一标识符和具体的执行操作。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. Time：任务的预定执行时间，单位为毫秒。该值通常是一个时间戳。
/// 2. Token：任务的唯一标识符，用于区分不同的延时任务，特别是在需要删除特定任务时。
/// 3. Action：该任务的具体操作，当时间到达时将执行此操作。它是一个无参数的委托（Action）。
/// 4. EarlyRemoveCallback：当任务在其执行时间之前被删除时，会触发这个回调，提供提前删除的处理方式。
/// </remarks>
using System;

namespace DelayedTaskModule
{
    public class DelayedTaskData :IPoolable
    {
        /// <summary>
        /// 该任务预定的执行时间。这个时间是一个绝对时间戳，通常以毫秒为单位。
        /// 延时任务调度器会在到达该时间时执行该任务。
        /// </summary>
        public long Time;

        /// <summary>
        /// 任务的唯一标识符。使用 GUID 生成，确保每个任务在系统中都有唯一的标识。
        /// 通过这个 Token，任务可以被精确地管理（如删除、查找等）。
        /// </summary>
        public string Token;

        /// <summary>
        /// 任务的核心操作。当时间到达任务的预定时间时，调度器会调用此操作来执行任务。
        /// </summary>
        public Action Action;

        /// <summary>
        /// 如果任务在预定时间之前被移除，调度器会触发此回调。这个回调函数允许任务在被提前移除时进行自定义处理。
        /// </summary>
        public Action EarlyRemoveCallback;

        /// <summary>
        ///  是否是循环任务,如果是循环任务在第一次执行完后重新加入调度队列
        /// </summary>
        public bool IsLooping;  // 是否是循环任务

        /// <summary>
        /// 第一次执行后的循环间隔时间
        /// </summary>
        public long Interval;  // 循环间隔时间，单位为毫秒

        /// <summary>
        /// 是否受暂停影响
        /// </summary>
        public bool AffectedByPause;  // 是否受暂停影响

        /// <summary>
        /// 暂停时的时间差
        /// </summary>
        public long PauseTimeDifference;  // 记录暂停时的时间差

        public void Initialize(params object[] parameters)
        {
        }

        public void Reset()
        {
        }
        public void Dispose()
        {
        }

    }
}
