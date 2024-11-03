using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventModule;
using DelayedTaskModule;

namespace TimeModule
{
    public class TimeManager : MonoSingleton<TimeManager>
    {
        public float TimeScale { get; set; } = 1f;  // 自定义时间缩放
        public float UnscaledDeltaTime { get; private set; }    // 不缩放的DeltaTime
        public float DeltaTime { get; private set; }  // 自定义 DeltaTime
        public bool IsRunning { get; private set; }   // 是正常运行

        void Start()
        {

        }

        public void Update()
        {
            // 每帧更新不缩放的 UnscaledDeltaTime
            UnscaledDeltaTime = Time.unscaledDeltaTime;

            // 每帧更新自定义的 DeltaTime
            DeltaTime = Time.deltaTime * TimeScale;
        }


        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            IsRunning = false;
            TimeScale = 0;  // 停止时间流动
            EventManager.Instance.Dispatch(EventDefine.PAUSE);      // 通知其他系统 程序已经暂停
        }


        /// <summary>
        /// 恢复
        /// </summary>
        public void Continue()
        {
            IsRunning = true;
            TimeScale = 1;  // 恢复时间流动
            EventManager.Instance.Dispatch(EventDefine.CONTINUE);      // 通知其他系统 程序已经暂停
        }
    }
}
