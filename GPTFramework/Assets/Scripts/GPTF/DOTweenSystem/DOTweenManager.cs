using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EventModule;

public class DOTweenManager : MonoSingleton<DOTweenManager>
{
    private void OnEnable()
    {
        // 注册暂停与运行事件
        EventManager.Instance.Register(EventDefine.PAUSE, PauseDOTween);
        EventManager.Instance.Register(EventDefine.CONTINUE, ContinueDOTween);
    }

    private void OnDisable()
    {
        // 移除暂停与运行事件
        EventManager.Instance.Remove(EventDefine.PAUSE, PauseDOTween);
        EventManager.Instance.Remove(EventDefine.CONTINUE, ContinueDOTween);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public static void PauseDOTween()
    {
        // 暂停所有带特定标签的动画
        DOTween.Pause(GlobalConstants.Tween_Pause_Affected);
    }

    public static void ContinueDOTween()
    {
        // 恢复所有带特定标签的动画
        DOTween.Play(GlobalConstants.Tween_Pause_Affected);
    }


}
