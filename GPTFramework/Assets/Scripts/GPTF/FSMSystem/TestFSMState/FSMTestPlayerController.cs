using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMModule;

/// <summary>
/// FSMTestPlayerController 控制器类，管理状态机。
/// </summary>
public class FSMTestPlayerController : MonoBehaviour
{
    private FSMSystem fsm; // 创建状态机对象

    private void Start()
    {
        // 初始化状态机
        fsm = new FSMSystem();

        // 添加状态
        fsm.AddState(TestFSMCharacterStates.Idle, new FSMTestIdleState());
        fsm.AddState(TestFSMCharacterStates.Move, new FSMTestMoveState());

        // 设置初始状态为 Idle
        fsm.SetInitialState(TestFSMCharacterStates.Idle);
    }

    /// <summary>
    /// 每帧更新状态机。
    /// </summary>
    public void Update()
    {
        // 每帧更新当前状态
        fsm.Update();

        // 获取当前状态
        string currentState = fsm.GetCurrentState(); // 获取当前状态的名称
        Debug.Log($"Current State: {currentState}");

        // 模拟按键输入来切换状态
        if (Input.GetKeyDown(KeyCode.Space)) // 示例：按空格键切换到移动状态
        {
            TestMoveFSMStateData moveData = new TestMoveFSMStateData(5.0f, new Vector3(1, 0, 0));
            fsm.ChangeState(TestFSMCharacterStates.Move, moveData); // 切换状态并传递数据
        }
    }
}
