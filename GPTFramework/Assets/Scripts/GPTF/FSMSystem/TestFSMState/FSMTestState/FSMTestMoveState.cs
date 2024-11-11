using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMModule;

/// <summary>
/// 专门给 TestMoveState 所传入的数据类.
/// </summary>
public class TestMoveFSMStateData : FSMStateData
{
    public float Speed { get; set; } // 速度属性
    public Vector3 Direction { get; set; } // 方向属性

    /// <summary>
    /// 构造函数.
    /// </summary>
    public TestMoveFSMStateData(float speed, Vector3 direction)
    {
        Speed = speed; // 初始化速度
        Direction = direction; // 初始化方向
    }
}

/// <summary>
/// FSMTestMoveState 实现移动状态.
/// </summary>
public class FSMTestMoveState : IFSMState
{
    /// <summary>
    /// 返回当前状态的名称.
    /// </summary>
    public string GetState()
    {
        return TestFSMCharacterStates.Move; // 返回 "Move"
    }

    /// <summary>
    /// 进入状态时调用.
    /// </summary>
    public void OnEnter(FSMStateData data)
    {
        if (data is TestMoveFSMStateData moveData) // 确保数据类型安全
        {
            Debug.Log($"Enter Move State with Speed: {moveData.Speed} and Direction: {moveData.Direction}");
        }
    }

    /// <summary>
    /// 离开状态时调用.
    /// </summary>
    public void OnExit()
    {
        Debug.Log("Exit Move State");
    }

    /// <summary>
    /// 每帧更新状态.
    /// </summary>
    public void OnUpdate()
    {
        Debug.Log("Update Move State");
    }
}
