/// <summary>
/// TimerUtil 类是一个时间工具类，提供与时间戳相关的各种静态方法。
/// 它用于获取当前时间的时间戳，支持秒级和毫秒级别的精度。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. GetTimeStamp：返回当前时间的时间戳，支持选择返回秒或毫秒级别的时间戳。
/// 2. GetLaterMilliSecondsBySecond：通过给定的秒数，返回未来的毫秒级时间戳，常用于设置延时任务的执行时间。
/// </remarks>
using System;

namespace DelayedTaskModule
{
    public static class TimerUtil
    {
        // Unix纪元时间 1970年1月1日
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 获取当前时间戳，可以选择返回秒级或毫秒级的时间戳。
        /// </summary>
        /// <param name="isMillisecond">如果为 true，返回毫秒级时间戳；否则返回秒级时间戳。</param>
        /// <returns>当前时间戳，单位为秒或毫秒。</returns>
        public static long GetTimeStamp(bool isMillisecond = false)
        {
            // 计算自 Unix 纪元以来的时间差，根据参数返回秒或毫秒
            return isMillisecond ? (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds : (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// 通过给定的秒数，计算未来的毫秒级时间戳。
        /// </summary>
        /// <param name="time">延迟时间，单位为秒。</param>
        /// <returns>未来的毫秒级时间戳。</returns>
        public static long GetLaterMilliSecondsBySecond(double time)
        {
            // 通过给定的秒数，计算当前时间的毫秒时间戳加上延迟秒数后的时间戳
            return (long)TimeSpan.FromMilliseconds(GetTimeStamp(true)).Add(TimeSpan.FromSeconds(time))
                .TotalMilliseconds;
        }
    }
}
