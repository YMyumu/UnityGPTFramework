/// <summary>
/// Initializer 类用于在游戏启动时按顺序初始化所有系统模块。
/// 该类通过访问每个管理器的单例实例来触发初始化过程，而不是手动遍历场景中的对象。
/// 由于单例系统中已经内置了在创建实例时自动调用初始化接口的逻辑，
/// 因此该初始化器的主要任务是确保按顺序访问这些单例实例以实现正确的初始化顺序。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 按顺序初始化核心系统、依赖系统和自定义模块，确保各个模块按照预期顺序进行初始化。
/// 2. 通过访问单例实例的方式，确保在必要时自动创建并初始化模块。
/// 3. 作为游戏启动时的初始化流程管理类，确保所有关键系统在游戏启动时已准备就绪。
/// </remarks>
using UnityEngine;
using System.Collections.Generic;

using ConfigModule;
using DelayedTaskModule;
using ResourceModule;
using EventModule;
using TimeModule;
using AudioModule;
using LocalizationModule;
using UIModule;
using SceneLoaderModule;


public class Initializer : MonoBehaviour
{
    // 存储所有实现了IInitable接口的模块实例（当前未使用，但可以在未来扩展时用到）
    private List<IInitable> initModules = new List<IInitable>();

    /// <summary>
    /// 在游戏启动时，初始化所有模块。此方法在游戏启动的最初阶段调用，确保按顺序初始化各个模块。
    /// </summary>
    void Awake()
    {
        // 初始化核心系统
        InitializeCoreSystems();

        // 初始化依赖系统
        InitializeDependentSystems();

        // 初始化自定义模块
        InitializeCustomModules();

    }

    /// <summary>
    /// 核心系统的初始化。
    /// 核心系统通常在最早阶段初始化，因为它们可能为其他系统提供基础服务。
    /// 这个方法负责手动控制核心系统的初始化顺序，确保按照预期的顺序执行。
    /// </summary>
    private void InitializeCoreSystems()
    {
        // 访问核心系统的实例，触发初始化
        var configManager = ConfigManager.Instance;    // 初始化配置管理器
        var resourceManager = ResourceManager.Instance; // 初始化资源管理器
        var eventManager = EventManager.Instance;      // 初始化事件管理器

        var timeManager = TimeManager.Instance;     // 初始化时间管理器
        var delayedTaskScheduler = DelayedTaskScheduler.Instance;       // 初始化延时调度器

    }

    /// <summary>
    /// 依赖系统的初始化。
    /// 依赖于核心系统的模块在核心系统初始化之后进行初始化。
    /// 这里可以扩展和初始化其他依赖于核心系统的模块。
    /// </summary>
    private void InitializeDependentSystems()
    {
        // 在此初始化依赖于核心系统的模块
        var dotweenManager = DOTweenManager.Instance;   // 初始化DOTween管理器
        var audioManger = AudioManager.Instance;        // 初始化音频管理器
        var localizationManager = LocalizationManager.Instance;     // 初始化本地化模块
        var uiManager = UIManager.Instance;     // 初始化UI管理器

        var sceneLoader = SceneLoader.Instance;     // 初始化场景加载器

    }

    /// <summary>
    /// 自定义模块的初始化。
    /// 根据项目需求，可能有一些自定义模块在上述系统初始化完成后再进行初始化。
    /// </summary>
    private void InitializeCustomModules()
    {
        // 在此初始化自定义模块
    }
}
