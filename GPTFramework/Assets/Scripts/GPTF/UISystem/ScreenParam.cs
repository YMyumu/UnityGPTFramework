/// <summary>
/// ScreenParam 类用于在界面之间传递参数。
/// 它包含子界面的索引和任意类型的数据，供界面初始化或交互时使用。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 传递子界面索引，用于界面管理逻辑。
/// 2. 传递通用数据对象，可用于任何界面需要的参数。
/// </remarks>

namespace UIModule
{
    public class ScreenParam
    {
        public int subIndex; // 子界面索引，用于标识当前打开的子界面或逻辑模块
        public object data; // 通用数据对象，可以存储任意类型的数据，用于界面间传递复杂数据
    }
}
