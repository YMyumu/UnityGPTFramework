using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UIStackCeil 用于封装UI栈中的界面对象，控制界面的显示、隐藏和关闭逻辑。
/// </summary>
/// 

namespace UIModule
{
    [System.Serializable]
    public class UIStackCeil : IPoolable
    {
        public UIBasePanel panel;
        public bool isShowing = false;          // 界面是正在显示的
        public bool isHideWaitClose = false;    // 是被隐藏等待关闭的
        private bool _isFirstShow = true;  // 判断是否是第一次打开

        private ScreenParam _param;
        private Dictionary<int, object> _panelParam = new Dictionary<int, object>();

        public IEnumerator Show(ScreenParam param = null)
        {

            isShowing = true;
            if (_isFirstShow)   // 第一次打开调用Open
            {
                UIManager.Instance.ShowInputBlocker();
                panel.Open(param);
                // 启动打开动画协程（默认无动画）并等待其完成
                yield return panel.StartCoroutine(panel.PlayOpenAnimationCoroutine());
                _isFirstShow = false;
                UIManager.Instance.HideInputBlocker();

            }
            else    // 后续调用Show
            {
                panel.Show();
                ResetPanelParam();

            }

        }

        public void Hide()
        {
            isShowing = false;
            SavePanelParam();
            panel.Hide();
        }


        // 不会实际关闭界面，只会调用界面的关闭动画，然后在隐藏界面
        public IEnumerator Close()
        {
            // 启动关闭动画协程（默认无动画）并等待其完成
            yield return panel.StartCoroutine(panel.PlayCloseAnimationCoroutine());

            isShowing = false;
            panel.OnCloseEvent();
            panel.Hide();
            //Panel.Close();
        }

        private void SavePanelParam() => _panelParam = panel.GetPanelRuntimeParam();
        private void ResetPanelParam() => panel.ResetPanelRuntimeParam(_panelParam);


        public void Initialize(params object[] parameters)
        {
            if (parameters != null && parameters.Length > 0) _param = parameters[0] as ScreenParam;
            isShowing = false;
            _panelParam.Clear();
        }

        public void Reset()
        {
            isShowing = false;
            _panelParam.Clear();
            panel = null;
            _param = null;
            _isFirstShow = true;  // 重置标志位
        }


        /// <summary>
        /// Dispose 方法用于释放资源，包括托管和非托管资源。
        /// 在对象生命周期结束时调用。
        /// </summary>
        public void Dispose()
        {
            // 清除引用类型字段，避免内存泄漏
            isShowing = false;
            _panelParam.Clear();
            panel = null;
            _param = null;
            _isFirstShow = true;  // 重置标志位

            // 如果有非托管资源，可在此释放
            // 例如：关闭文件句柄、释放数据库连接等
            // GC.SuppressFinalize(this);  // 禁用对象的终结器，避免二次释放
        }
    }

}
