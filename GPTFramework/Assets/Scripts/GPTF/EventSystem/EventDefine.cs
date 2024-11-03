/// <summary>
/// EventDefine 类用于定义所有的事件标识符（Key）。
/// 这些标识符作为事件的唯一标识符，用于在事件系统中进行事件的注册、移除和派发。
/// 通过使用这些标识符，可以确保在不同模块间共享同一事件名称，避免重复定义或冲突。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 定义并存储所有事件的唯一标识符（Key）。
/// </remarks>
/// 

namespace EventModule
{
    public static class EventDefine
    {
        #region 系统
        /// <summary>
        /// 程序暂停事件，在暂停时触发
        /// </summary>
        public static readonly string PAUSE = "PAUSE";

        /// <summary>
        /// 程序运行事件，在暂停后需要运行时触发
        /// </summary>
        public static readonly string CONTINUE = "CONTINUE";

        /// <summary>
        /// 切换语言
        /// </summary>
        public static readonly string SWITCH_LANGUAGE = "SWITCH_LANGUAGE";

        /// <summary>
        /// 场景加载前的事件
        /// </summary>
        public static readonly string BEFORE_SCENE_LOAD = "BEFORE_SCENE_LOAD";

        /// <summary>
        /// 场景加载后的事件
        /// </summary>
        public static readonly string AFTER_SCENE_LOAD = "AFTER_SCENE_LOAD";
        #endregion



        /// <summary>
        /// 示例事件标识符，用于测试或示例。
        /// </summary>
        public static readonly string TEST_EVENT = "TEST_EVENT";

        /// <summary>
        /// 玩家死亡事件，当玩家角色死亡时触发该事件。
        /// </summary>
        public static readonly string PLAYER_DIED = "PLAYER_DIED";

        /// <summary>
        /// 物品收集事件，当玩家或角色收集到某个物品时触发该事件。
        /// </summary>
        public static readonly string ITEM_COLLECTED = "ITEM_COLLECTED";

        // 可以在此处继续添加更多的事件标识符，根据项目需求进行扩展。

        /// <summary>
        /// 场景加载进度事件，用于UI更新进度条
        /// </summary>
        public static readonly string ON_SCENE_LOAD_PROGRESS_FOR_UIPANEL = "ON_SCENE_LOAD_PROGRESS_FOR_UIPANEL";




    }

}
