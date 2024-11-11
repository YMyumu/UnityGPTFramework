/// <summary>
/// UILayer 枚举定义了 UI 系统中的不同层级，用于区分不同 UI 界面的显示顺序和层次关系。
/// 各个层级之间存在遮挡关系，高层级的界面会遮挡低层级的界面。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 定义四个 UI 层级（默认层、层级1、层级2、层级3）。
/// 2. 每个层级代表不同的 UI 类型或显示优先级。
/// 3. 管理各层级的 UI 显示遮挡关系。
/// </remarks>
/// 

namespace UIModule
{
    public enum UILayer
    {
        /// <summary>
        /// Default 层级：用于主界面和全屏界面，通常是用户主要交互的界面。
        /// </summary>
        Default,

        /// <summary>
        /// Layer1 层级：用于非全屏的窗口，信息量较大，例如背包、商店等界面。
        /// </summary>
        Layer1,

        /// <summary>
        /// Layer2 层级：用于非全屏的小窗口，信息量较少的弹窗，例如确认或提示框。
        /// </summary>
        Layer2,

        /// <summary>
        /// Layer3 层级：最上层的 UI，用于提示、警告或报错等重要提示，不会被其他 UI 界面遮挡。
        /// </summary>
        Layer3
    }
}
