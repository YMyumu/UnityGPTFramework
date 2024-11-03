/// <summary>
/// KeyConstants 类用于统一管理项目中所有输入按键和轴向输入的常量，
/// 避免在代码中硬编码特定按键值，方便统一修改和扩展。
/// </summary>
/// 

namespace InputModule
{
    public static class KeyConstants
    {
        public const string MoveForward = "MoveForward";
        public const string Jump = "Jump";
        public const string Fire = "Fire";
        public const string Horizontal = "Horizontal";
        public const string Vertical = "Vertical";
        public const string GamepadMove = "GamepadMove";
        // 根据项目需求，添加更多按键常量
    }

}
