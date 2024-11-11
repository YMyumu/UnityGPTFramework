/// <summary>
/// EventPriority 类用于定义事件的优先级。
/// 优先级数值决定了事件处理的顺序，数值越大，优先级越高，事件将会更早被派发和处理。
/// 通过这种方式，可以控制不同类型事件的处理顺序，确保关键事件优先得到处理。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 定义并存储所有事件的优先级。
/// </remarks>
/// 

namespace EventModule
{
    public static class EventPriority
    {
        /// <summary>
        /// UI 相关事件的优先级，默认值为 0。
        /// UI 事件通常在渲染和物理计算之后处理，因此优先级较低。
        /// </summary>
        public static readonly int UI = 0;

        /// <summary>
        /// 物理相关事件的优先级，默认值为 -50。
        /// 物理事件通常需要在游戏逻辑之后处理，因此优先级较低。
        /// </summary>
        public static readonly int PHYSICS = -50;

        /// <summary>
        /// 游戏逻辑相关事件的优先级，默认值为 100。
        /// 游戏逻辑事件通常涉及到关键的游戏状态更新，因此优先级较高。
        /// </summary>
        public static readonly int GAME_LOGIC = 100;

        /// <summary>
        /// 数据更新相关事件的优先级，默认值为 1000。
        /// 数据更新事件通常在其他所有事件之后处理，因此优先级最高。
        /// </summary>
        public static readonly int DATA_UPDATE = 1000;

        // 可以根据项目的实际需求，继续添加或调整优先级数值，以控制事件处理顺序。
    }

}
