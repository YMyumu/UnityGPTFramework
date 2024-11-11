using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

using Debug = UnityEngine.Debug;
using DG.Tweening;
using TimeModule;
using DelayedTaskModule;
using PoolModule;


public class TimeTest : MonoBehaviour
{
    public Transform box; 


    // Start is called before the first frame update
    void Start()
    {
        box.DOMove(new Vector3(5, 5, 5), 5f).SetLoops(-1, LoopType.Yoyo).SetId(GlobalConstants.Tween_Pause_Affected);

        DelayedTaskScheduler.Instance.AddDelayedTask(3, () => { LogManager.LogInfo("这是不延迟的任务，3秒后执行"); });
        DelayedTaskScheduler.Instance.AddDelayedTask(4, () => { Debug.Log("这是不延迟的任务，4秒后执行"); });
        DelayedTaskScheduler.Instance.AddDelayedTask(5, () => { Debug.Log("这是不延迟的任务，5秒后执行"); });
        DelayedTaskScheduler.Instance.AddDelayedTask(6, () => { Debug.Log("这是不延迟的任务，6秒后执行"); });


        Stopwatch stopwatch = GenericObjectPool_NonPoolableFactory.Instance.GetObject<Stopwatch>();
        stopwatch.Restart();
        DelayedTaskScheduler.Instance.AddDelayedTask(3, () =>
        {
            LogManager.LogInfo(box.name);
            stopwatch.Stop();
            LogManager.LogInfo("这是受暂停影响的任务");
            LogManager.LogInfo($"{3}秒后了,执行了对应方法。实际过去了{stopwatch.ElapsedMilliseconds / 1000.0f}秒");

        }, null, true);


        DelayedTaskScheduler.Instance.AddDelayedTask(4, () => { Debug.Log("这是受暂停影响的任务"); }, null, true);
        DelayedTaskScheduler.Instance.AddDelayedTask(5, () => { Debug.Log("这是受暂停影响的任务"); }, null, true);
        DelayedTaskScheduler.Instance.AddDelayedTask(6, () => { Debug.Log("这是受暂停影响的任务"); }, null, true);


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TimeManager.Instance.Pause();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            TimeManager.Instance.Continue();
        }



    }
}
