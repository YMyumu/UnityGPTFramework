using UnityEngine;
using InputModule;

/// <summary>
/// KeyboardInputHandler 处理键盘输入，将输入状态传递给 InputManager。
/// 包括按键状态和浮点值的处理。
/// </summary>
public class KeyboardInputHandler : IInputHandler
{
    public void ProcessInput()
    {
        // 处理前进键 W 的按下、长按和抬起状态
        bool isPressed = Input.GetKeyDown(KeyCode.W);
        bool isHeld = Input.GetKey(KeyCode.W);
        bool isReleased = Input.GetKeyUp(KeyCode.W);

        // 将状态存储到 InputManager 中
        InputManager.Instance.SetKeyState(KeyConstants.MoveForward, isPressed, isHeld, isReleased);
    }

    public void ProcessAxisInput()
    {
        // 处理水平和垂直轴向输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 更新水平轴状态
        InputManager.Instance.SetAxisState(KeyConstants.Horizontal, horizontal, horizontal != 0, horizontal != 0, horizontal == 0);

        // 更新垂直轴状态
        InputManager.Instance.SetAxisState(KeyConstants.Vertical, vertical, vertical != 0, vertical != 0, vertical == 0);
    }
}
