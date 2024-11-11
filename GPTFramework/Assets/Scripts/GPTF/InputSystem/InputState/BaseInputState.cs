/// <summary>
/// BaseInputState 是所有输入状态的基类，提供通用的按下、持续按下、抬起状态。
/// </summary>
public abstract class BaseInputState
{
    public bool IsPressed { get; set; }
    public bool IsHeld { get; set; }
    public bool IsReleased { get; set; }
}
