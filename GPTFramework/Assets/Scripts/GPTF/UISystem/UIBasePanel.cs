/// <summary>
/// UIBasePanel 类是所有 UI 面板的基类，提供了面板的生命周期管理、配置和状态控制功能。
/// 它定义了面板的初始化、显示、隐藏和关闭的基本逻辑。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 设置 UI 面板的配置（如是否为模态窗口、点击背景关闭等）。
/// 2. 提供打开、显示、刷新、隐藏和关闭等基本功能。
/// 3. 定义面板的生命周期方法，如初始化和运行时参数管理。
/// </remarks>
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


namespace UIModule
{
    public abstract class UIBasePanel : MonoBehaviour
    {
        private string _uiLayer; // 用于存储 UI 所在的层级
        private bool _isCloseSameLayer; // 是否关闭同层其他界面
        private bool _isFullPanel; // 是否是全屏界面
        private bool _isModernPanel; // 是否为模态窗口
        private bool _clickModernNeedClose; // 是否允许点击模态窗口背景关闭面板
        private bool _isInitialized = false; // 标记面板是否已经初始化

        public string UILayer => _uiLayer; // 公共只读属性，返回当前 UI 面板的层级
        public event Action OnClose; // 定义一个公开的回调，用于面板关闭时执行自定义逻辑

        /// <summary>
        /// 设置 UI 面板的配置，包括是否是模态窗口、点击背景关闭等选项。
        /// </summary>
        /// <param name="isModernPanel">是否为模态窗口。</param>
        /// <param name="clickModernNeedClose">点击模态背景是否关闭窗口。</param>
        /// <param name="layer">UI 所在的层级。</param>
        public void SetConfig(bool isFullPanel, bool isCloseSameLayer, bool isModernPanel, bool clickModernNeedClose, string layer)
        {
            _isCloseSameLayer = isCloseSameLayer; // 存储是否关闭同层其他界面
            _isFullPanel = isFullPanel;     // 存储是否是全屏界面
            _isModernPanel = isModernPanel; // 存储是否为模态窗口
            _clickModernNeedClose = clickModernNeedClose; // 存储点击背景关闭窗口的配置
            _uiLayer = layer; // 存储 UI 面板的层级信息
        }

        /// <summary>
        /// Open 方法用于打开并初始化界面，如果界面没有初始化则调用 Initialize 方法。
        /// </summary>
        /// <param name="param">ScreenParam 类型的参数对象，用于传递界面需要的数据。</param>
        public void Open(ScreenParam param = null)
        {
            if (!_isInitialized) // 检查界面是否已经初始化过
            {
                Initialize(param); // 调用初始化方法，传递参数
                _isInitialized = true; // 标记界面已经初始化
            }
            Show(); // 打开时调用 Show 方法，确保界面显示
        }

        /// <summary>
        /// 显示 UI 面板，并刷新面板的内容。
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true); // 将当前游戏对象设置为激活状态，从而显示 UI 面板
            Refresh(); // 调用 Refresh 方法刷新面板内容
        }

        /// <summary>
        /// 刷新 UI 面板内容，可以根据需要在子类中实现具体的刷新逻辑。
        /// </summary>
        public virtual void Refresh()
        {
            // 由子类实现具体的刷新逻辑
        }

        /// <summary>
        /// 隐藏 UI 面板。
        /// </summary>
        public virtual void Hide() => gameObject.SetActive(false); // 将当前游戏对象设置为非激活状态，从而隐藏 UI 面板

        /// <summary>
        /// 关闭 UI 面板 关闭界面逻辑必须走UIManager！！！！！ 对于界面本身的直接销毁 不应该直接调用
        /// </summary>
        public virtual void Close()
        {
            Destroy(gameObject);
        }

        // 由UIStackCeil来启动关闭事件
        public void OnCloseEvent()
        {
            OnClose?.Invoke();
        }


        // 打开界面时的动画携程
        public virtual IEnumerator PlayOpenAnimationCoroutine()
        {
            // 默认不做任何动画，直接返回
            yield break;
        }

        //关闭界面时的动画携程
        public virtual IEnumerator PlayCloseAnimationCoroutine()
        {
            // 默认不做任何动画，直接返回
            yield break;
        }


        /// <summary>
        /// 抽象方法，必须由子类实现，用于返回当前面板的名称。
        /// </summary>
        /// <returns>返回当前面板的名称字符串。</returns>
        public abstract string GetPanelName();

        /// <summary>
        /// 检查当前 UI 是否关闭同层其他界面，默认返回 false，子类可以重写此方法。
        /// </summary>
        /// <returns>如果是全屏面板，返回 true；否则返回 false。</returns>
        public virtual bool IsCloseSameLayer() => _isCloseSameLayer;

        /// <summary>
        /// 检查当前 UI 是否为全屏面板，默认返回 false，子类可以重写此方法。
        /// </summary>
        /// <returns>如果是全屏面板，返回 true；否则返回 false。</returns>
        public virtual bool IsFullPanel() => _isFullPanel;

        /// <summary>
        /// 检查当前 UI 是否为模态面板，默认返回 _isModernPanel。
        /// </summary>
        /// <returns>如果是模态面板，返回 true；否则返回 false。</returns>
        public virtual bool IsModernPanel() => _isModernPanel;

        /// <summary>
        /// 检查是否允许点击模态背景关闭面板，默认返回 _clickModernNeedClose。
        /// </summary>
        /// <returns>如果允许点击背景关闭，返回 true；否则返回 false。</returns>
        public virtual bool ClickModernNeedClosePanel() => _clickModernNeedClose;

        // 获取和重置界面运行时参数的虚方法，供子类实现
        /// <summary>
        /// 获取当前 UI 面板的运行时参数，默认返回 null，子类可以重写此方法。
        /// </summary>
        /// <returns>返回运行时参数的字典。</returns>
        public virtual Dictionary<int, object> GetPanelRuntimeParam() => new Dictionary<int, object>();

        /// <summary>
        /// 重置 UI 面板的运行时参数，子类可以根据需要实现具体逻辑。
        /// </summary>
        /// <param name="param">包含运行时参数的字典。</param>
        public virtual void ResetPanelRuntimeParam(Dictionary<int, object> param) { }

        /// <summary>
        /// 初始化 UI 面板，子类可以根据传入的参数设置初始状态。
        /// </summary>
        /// <param name="param">传递给面板的参数对象。</param>
        protected virtual void Initialize(ScreenParam param)
        {
            // 子类实现具体的初始化逻辑，如加载数据、设置 UI 元素等
        }
    }
}
