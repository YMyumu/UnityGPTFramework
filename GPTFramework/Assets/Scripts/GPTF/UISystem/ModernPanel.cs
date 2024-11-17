/// <summary>
/// ModernPanel 类用于管理模态窗口的显示和关闭逻辑。
/// 它通过绑定到指定的 UI 界面，并处理点击关闭按钮的操作。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 绑定 UI 界面到模态窗口。
/// 2. 管理模态窗口的显示与关闭逻辑。
/// 3. 实现 IDisposable 接口，用于释放相关资源。
/// </remarks>
using System;
using UnityEngine;
using UnityEngine.UI;
using PoolModule;

namespace UIModule
{
    public class ModernPanel : MonoBehaviour, IDisposable // 继承 MonoBehaviour 以使该脚本能附加到 Unity 对象，并实现 IDisposable 接口来处理资源释放
    {
        private Button btnClose; // 私有 Button 类型变量，存储模态窗口的关闭按钮
        private UIBasePanel _panel; // 私有 UIBasePanel 类型变量，引用当前绑定的 UI 面板
        private RectTransform _rectTransform;

        /// <summary>
        /// Awake 方法是 Unity 的生命周期方法之一，表示脚本启动时调用。
        /// 在这里我们获取并存储关闭按钮的引用。
        /// </summary>
        private void Awake()
        {
            btnClose = GetComponent<Button>(); // 获取当前游戏对象上的 Button 组件并将其存储在 btnClose 变量中
            _rectTransform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// 绑定 UI 面板到当前模态窗口，并为关闭按钮添加关闭面板的逻辑。
        /// </summary>
        /// <param name="panel">传入的 UIBasePanel 对象，表示要绑定的 UI 面板。</param>
        public void BindPanel(UIBasePanel panel)
        {
            _panel = panel; // 将传入的 UI 面板对象赋值给本地的 _panel 变量，绑定模态窗口与 UI 面板
            if (btnClose) // 检查关闭按钮是否存在，避免空引用错误
            {
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;

                // 设置左、右、顶部、底部的值
                _rectTransform.offsetMin = new Vector2(-20, -20); // 设置左和底部
                _rectTransform.offsetMax = new Vector2(20, 20); // 设置右和顶部


                btnClose.onClick.RemoveAllListeners(); // 移除所有之前的监听事件，确保不会重复绑定

                // 只有配置里可以点击模态界面能关闭才绑定关闭逻辑
                btnClose.onClick.AddListener(() => {
                    UIManager.Instance.ClosePanel(_panel.GetPanelName());// 为关闭按钮添加点击事件
                });

                _panel.OnClose += Close;
            }
        }

        public void Close()
        {
            _panel.OnClose -= Close;
            Dispose();
            UnityObjectPoolFactory.Instance.RecycleObject("ModernPanel", gameObject);

        }

        /// <summary>
        /// 实现 IDisposable 接口的 Dispose 方法，用于释放资源。
        /// 在这里，我们将清除对绑定面板和按钮事件的引用，防止内存泄漏。
        /// </summary>
        public void Dispose()
        {
            if (_panel != null) // 检查是否有绑定的面板
            {
                _panel = null; // 解除面板绑定，清除引用
            }
            if (btnClose != null) // 检查是否有绑定的关闭按钮
            {
                btnClose.onClick.RemoveAllListeners(); // 移除关闭按钮的所有事件监听器，防止内存泄漏
            }
        }

        ~ModernPanel()
        {
            Dispose();
        }
    }
}
