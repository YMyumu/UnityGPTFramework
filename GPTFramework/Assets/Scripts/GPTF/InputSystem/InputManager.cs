using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InputManager 统一管理所有设备的输入，包括按键、浮点值和二维向量的状态。
/// 不同的输入类型使用不同的状态类（KeyInputState、AxisInputState、Vector2InputState）。
/// </summary>
/// 
namespace InputModule
{
    public class InputManager : MonoSingleton<InputManager>
    {

        // 分别存储按键、浮点值和二维向量的状态
        private Dictionary<string, BaseInputState> keyStateCache = new Dictionary<string, BaseInputState>();
        private Dictionary<string, BaseInputState> axisStateCache = new Dictionary<string, BaseInputState>();
        private Dictionary<string, BaseInputState> vector2StateCache = new Dictionary<string, BaseInputState>();


        // 设置按键的状态
        public void SetKeyState(string keyName, bool isPressed, bool isHeld, bool isReleased)
        {
            if (!keyStateCache.ContainsKey(keyName))
            {
                keyStateCache[keyName] = new KeyInputState();
            }
            var state = keyStateCache[keyName] as KeyInputState;
            state.IsPressed = isPressed;
            state.IsHeld = isHeld;
            state.IsReleased = isReleased;
        }

        // 设置浮点值的状态
        public void SetAxisState(string axisName, float axisValue, bool isPressed, bool isHeld, bool isReleased)
        {
            if (!axisStateCache.ContainsKey(axisName))
            {
                axisStateCache[axisName] = new AxisInputState();
            }
            var state = axisStateCache[axisName] as AxisInputState;
            state.AxisValue = axisValue;
            state.IsPressed = isPressed;
            state.IsHeld = isHeld;
            state.IsReleased = isReleased;
        }

        // 设置二维向量的状态
        public void SetVector2State(string vector2Name, Vector2 vector2Value, bool isPressed, bool isHeld, bool isReleased)
        {
            if (!vector2StateCache.ContainsKey(vector2Name))
            {
                vector2StateCache[vector2Name] = new Vector2InputState();
            }
            var state = vector2StateCache[vector2Name] as Vector2InputState;
            state.Vector2Value = vector2Value;
            state.IsPressed = isPressed;
            state.IsHeld = isHeld;
            state.IsReleased = isReleased;
        }

        // 获取按键的状态
        public KeyInputState GetKeyState(string keyName)
        {
            return keyStateCache.ContainsKey(keyName) ? keyStateCache[keyName] as KeyInputState : new KeyInputState();
        }

        // 获取浮点值的状态
        public AxisInputState GetAxisState(string axisName)
        {
            return axisStateCache.ContainsKey(axisName) ? axisStateCache[axisName] as AxisInputState : new AxisInputState();
        }

        // 获取二维向量的状态
        public Vector2InputState GetVector2State(string vector2Name)
        {
            return vector2StateCache.ContainsKey(vector2Name) ? vector2StateCache[vector2Name] as Vector2InputState : new Vector2InputState();
        }
    }

}
