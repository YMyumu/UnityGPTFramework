/// <summary>
/// GlobalConstants 类用于存储项目中使用的全局常量。
/// 这些常量在游戏运行期间不可修改，常用于固定的名称、路径和预定义的数值。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 提供不可修改的全局常量值，方便项目中统一使用。
/// 2. 防止硬编码，提升代码可读性和维护性。
/// </remarks>
public static class GlobalConstants
{
    /* 示例
    public const string PlayerTag = "Player";
    public const string EnemyTag = "Enemy";

    public const int MaxHealth = 100;
    public const float Gravity = 9.81f;

    // 资源路径
    public const string IconPath = "Textures/Icons/";

    */

    /// <summary>
    /// DOTween 的动画标签，受暂停影响的动画
    /// </summary>
    public const string Tween_Pause_Affected = "DOTweenAffectedByPause";

    /// <summary>
    /// DOTween 的动画标签，不受暂停影响的动画
    /// </summary>
    public const string Tween_Pause_Unaffected = "DOTweenUnaffectedByPause";

}
