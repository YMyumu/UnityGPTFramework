/// <summary>
/// CharacterController 类用于处理角色的移动和输入操作。
/// 它使用 InputManager 提供的按键状态和轴向输入来控制角色的行为。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 获取按键状态来判断角色的动作（如前进、后退等）。
/// 2. 组合水平和垂直的轴向输入为二维向量，并控制角色的移动。
/// </remarks>

using UnityEngine;
using InputModule;

public class TestInputManagerCharacterController : MonoBehaviour
{
    void Update()
    {
        // 获取前进键的状态
        KeyInputState moveForwardState = InputManager.Instance.GetKeyState(KeyConstants.MoveForward);
        if (moveForwardState.IsPressed)
        {
            Debug.Log("角色开始前进");
        }
        if (moveForwardState.IsHeld)
        {
            Debug.Log("角色持续前进");
        }
        if (moveForwardState.IsReleased)
        {
            Debug.Log("角色停止前进");
        }

        // 获取水平轴的状态
        AxisInputState horizontalState = InputManager.Instance.GetAxisState(KeyConstants.Horizontal);
        Debug.Log($"水平轴值: {horizontalState.AxisValue}");

        // 获取二维向量的状态（假设这是手柄的摇杆输入）
        Vector2InputState vector2State = InputManager.Instance.GetVector2State(KeyConstants.GamepadMove);
        Debug.Log($"摇杆输入方向: {vector2State.Vector2Value}");
    }
}
