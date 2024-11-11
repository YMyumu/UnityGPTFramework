using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMModule;

/// <summary>
/// FSMTestIdleState 实现待机状态.
/// </summary>
public class FSMTestIdleState : IFSMState
{
    /// <summary>
    /// 返回当前状态的名称.
    /// </summary>
    public string GetState()
    {
        return TestFSMCharacterStates.Idle; // 返回 "Idle"
    }

    /// <summary>
    /// 进入状态时调用.
    /// </summary>
    public void OnEnter(FSMStateData data)
    {
        Debug.Log("Enter Idle State");
    }

    /// <summary>
    /// 离开状态时调用.
    /// </summary>
    public void OnExit()
    {
        Debug.Log("Exit Idle State");
    }

    /// <summary>
    /// 每帧更新状态.
    /// </summary>
    public void OnUpdate()
    {
        Debug.Log("Update Idle State");
    }
}
