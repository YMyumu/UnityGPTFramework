/// <summary>
/// IInputHandler 接口定义了输入设备的标准处理方法。
/// 所有输入设备（如键盘、鼠标、手柄）需要实现该接口来处理按键和轴向输入。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. ProcessInput 用于处理按键状态（按下、持续按下、抬起）。
/// 2. ProcessAxisInput 用于处理轴向输入（如水平、垂直方向的输入）。
/// </remarks>
namespace InputModule
{
    public interface IInputHandler
    {
        void ProcessInput();      // 处理按键状态
        void ProcessAxisInput();  // 处理轴向输入
    }

}
