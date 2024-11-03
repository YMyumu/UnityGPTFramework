/// <summary>
/// UIManager 类用于管理游戏中所有的 UI 界面和窗口。
/// 它负责处理 UI 的打开、关闭、层级管理、以及窗口的栈管理逻辑。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 管理 UI 界面的打开与关闭。
/// 2. 管理 UI 层级关系，处理窗口的层次和遮挡。
/// 3. 通过栈管理 UI 界面，支持全屏和非全屏窗口的显示逻辑。
/// 4. 支持界面的延迟销毁，避免资源浪费。
/// </remarks>
using ConfigModule; // 引入配置模块，用于加载 UI 配置信息
using ResourceModule; // 引入资源模块，处理资源加载逻辑
using DelayedTaskModule; // 引入延迟任务模块，用于处理延迟销毁等逻辑
using PoolModule; // 引入对象池模块，用于管理对象的重用
using System; // 引入系统命名空间
using System.Collections;
using System.Collections.Generic; // 引入集合库，用于处理字典和列表等集合
using UnityEngine; // 引入 Unity 的核心命名空间
using UnityEngine.UI; // 引入 Unity 的 UI 命名空间


namespace UIModule
{
    public class UIManager : MonoSingleton<UIManager>, IInitable // 单例模式
    {
        private static string _defaultUIName = UIDefine.Panel_1; // 默认初始界面名称

        // 字典存储打开的 UI 面板
        private Dictionary<string, UIBasePanel> _panelDic = new Dictionary<string, UIBasePanel>();
        private Dictionary<string, List<UIStackCeil>> _uiStacks = new Dictionary<string, List<UIStackCeil>>();
        private Dictionary<string, (string destroyTaskToken, GameObject panel)> _waitDestroyPanelDic = new Dictionary<string, (string, GameObject)>();

        public GameObject canvasRoot; // 画布根对象
        public Transform defaultLayerRoot, layer1Root, layer2Root, layer3Root, layer4Root; // 各层级的根节点

        public GameObject modalBackgroundPrefab; // 模态窗口背景的预制体
        private const float _waitDestroyTime = 20f; // 延迟销毁时间
        private GameObject _inputBlockerInstance; // 输入遮罩实例

        // 初始化方法
        public void Init()
        {
            // 实例化画布
            canvasRoot = Instantiate(ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/CanvasRoot.prefab"));
            InitializeLayerRoots();
            InitializeUIStacks();
            CreateInputBlocker(); // 创建输入遮罩
            DontDestroyOnLoad(canvasRoot); // 不在场景切换时销毁
            modalBackgroundPrefab = ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/Panel/ModernPanel.prefab"); // 加载模态窗口背景预制体
        }

        // 初始化各层级根节点
        private void InitializeLayerRoots()
        {
            defaultLayerRoot = canvasRoot.transform.GetChild(1);
            layer1Root = canvasRoot.transform.GetChild(2);
            layer2Root = canvasRoot.transform.GetChild(3);
            layer3Root = canvasRoot.transform.GetChild(4);
            layer4Root = canvasRoot.transform.GetChild(5);
        }

        // 初始化 UI 栈
        private void InitializeUIStacks()
        {
            _uiStacks.Add("Default", new List<UIStackCeil>());
            _uiStacks.Add("Layer1", new List<UIStackCeil>());
            _uiStacks.Add("Layer2", new List<UIStackCeil>());
            _uiStacks.Add("Layer3", new List<UIStackCeil>());
            _uiStacks.Add("Layer4", new List<UIStackCeil>());
        }

        // 打开指定名称的 UI 界面
        public void OpenPanel(string uiName, ScreenParam param = null, bool shouldCloseHigherLayers = false)
        {
            UIPanels uiConfig = LoadUIPanelConfig(uiName);
            if (uiConfig == null) return; // 找不到配置，退出

            HandleWaitingForDestruction(uiName); // 处理等待销毁的界面
            UIBasePanel panel = GetPanel(uiConfig); // 获取或实例化面板
            if (panel == null) return; // 获取失败，退出

            ConfigurePanel(panel, uiConfig); // 配置面板
            HandleFullPanel(panel, uiConfig, param, shouldCloseHigherLayers); // 处理全屏面板逻辑

            if (panel.IsModernPanel())
            {
                CreateModalBackground(panel, uiConfig.UILayer); // 创建模态背景
            }
        }

        // 加载 UI 面板配置
        private UIPanels LoadUIPanelConfig(string uiName)
        {
            UIPanels uiConfig = ConfigManager.Instance.GetConfig<UIPanelsCfg>().cfg
                .Find(config => config.UIName == uiName);

            if (uiConfig == null)
            {
                LogManager.LogError($"UI配置中未找到 {uiName} 的相关信息！");
            }
            return uiConfig;
        }

        // 配置面板
        private void ConfigurePanel(UIBasePanel panel, UIPanels uiConfig)
        {
            panel.SetConfig(uiConfig.IsFullPanel, uiConfig.IsCloseSameLayer, uiConfig.IsModernPanel, uiConfig.IsClickModernNeedClose, uiConfig.UILayer);
        }

        // 获取或实例化指定名称的 UI 界面
        private UIBasePanel GetPanel(UIPanels uiConfig)
        {
            if (_panelDic.TryGetValue(uiConfig.UIName, out var panel))
            {
                return panel; // 已存在的面板
            }

            GameObject objPanel = Instantiate(ResourceManager.Instance.LoadResource<GameObject>(uiConfig.Path));
            if (objPanel == null)
            {
                LogManager.LogError("Load Panel Error: " + uiConfig.UIName);
                return null; // 实例化失败，返回 null
            }

            objPanel.name = uiConfig.UIName;
            UIBasePanel basePanel = objPanel.GetComponent<UIBasePanel>();
            if (basePanel == null)
            {
                LogManager.LogError("Panel not has UIBasePanel Component: " + uiConfig.UIName);
            }
            else
            {
                _panelDic.Add(uiConfig.UIName, basePanel); // 将面板存储到字典
            }

            return basePanel; // 返回面板
        }

        // 处理等待销毁的面板
        private void HandleWaitingForDestruction(string uiName)
        {
            if (_waitDestroyPanelDic.TryGetValue(uiName, out var tuple))
            {
                DelayedTaskScheduler.Instance.RemoveDelayedTask(tuple.destroyTaskToken); // 取消延迟销毁
                _waitDestroyPanelDic.Remove(uiName); // 移除等待销毁字典
                _panelDic[uiName] = tuple.panel.GetComponent<UIBasePanel>(); // 恢复面板引用
            }
        }

        // 关闭指定名称的 UI 界面
        public void ClosePanel(string uiName)
        {
            StartCoroutine(UIStackCeilClose(uiName)); // 启动协程处理关闭逻辑
        }

        // 关闭面板的协程逻辑
        private IEnumerator UIStackCeilClose(string uiName)
        {
            foreach (var item in _uiStacks)
            {
                var _uiStack = item.Value;
                for (int i = _uiStack.Count - 1; i >= 0; i--)
                {
                    var stackCeil = _uiStack[i];
                    if (stackCeil.Panel.GetPanelName() != uiName) continue; // 不匹配，继续

                    yield return CloseStackCeil(stackCeil); // 处理关闭逻辑
                    break; // 退出循环
                }
            }
        }

        // 处理关闭面板的具体逻辑
        private IEnumerator CloseStackCeil(UIStackCeil stackCeil)
        {

            if (stackCeil.IsShowing)
            {
                yield return StartCoroutine(stackCeil.Close()); // 关闭面板
            }
            else
            {
                stackCeil.Hide(); // 隐藏面板
            }
            ScheduleDestruction(stackCeil); // 安排延迟销毁

        }

        // 安排面板的延迟销毁
        private void ScheduleDestruction(UIStackCeil stackCeil)
        {
            string destroyTaskToken = DelayedTaskScheduler.Instance.AddDelayedTask(_waitDestroyTime, () =>
            {
                Destroy(stackCeil.Panel.gameObject); // 销毁面板
                _waitDestroyPanelDic.Remove(stackCeil.Panel.GetPanelName()); // 从字典中移除
                _panelDic.Remove(stackCeil.Panel.GetPanelName()); // 移除面板引用
            });
            _waitDestroyPanelDic.Add(stackCeil.Panel.GetPanelName(), (destroyTaskToken, stackCeil.Panel.gameObject)); // 添加到等待销毁字典
        }

        // 创建输入遮罩
        private void CreateInputBlocker()
        {
            _inputBlockerInstance = Instantiate(ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/Panel/InputBlockerPrefab.prefab"), layer4Root);
            if (_inputBlockerInstance != null)
            {
                _inputBlockerInstance.GetComponent<CanvasGroup>().blocksRaycasts = false; // 禁止交互
            }
        }

        // 显示输入遮罩
        public void ShowInputBlocker()
        {
            if (_inputBlockerInstance != null)
            {
                _inputBlockerInstance.GetComponent<CanvasGroup>().blocksRaycasts = true; // 启用交互
            }
        }

        // 隐藏输入遮罩
        public void HideInputBlocker()
        {
            if (_inputBlockerInstance != null)
            {
                _inputBlockerInstance.GetComponent<CanvasGroup>().blocksRaycasts = false; // 禁止交互
            }
        }

        // 处理全屏面板逻辑（简化示例，具体逻辑根据项目需要实现）
        private void HandleFullPanel(UIBasePanel panel, UIPanels uiConfig, ScreenParam param, bool shouldCloseHigherLayers)
        {
            // 检查是否已存在相同的全屏界面
            UIStackCeil existingFullScreenPanel = _uiStacks[uiConfig.UILayer].Find(p => p.Panel == panel);
            if (existingFullScreenPanel != null)
            {
                StartCoroutine(existingFullScreenPanel.Show(param)); // 显示已存在的面板
            }
            else
            {
                CreatePanelStack(panel, uiConfig.UILayer, param); // 创建新的面板栈
                ResortPanel(uiConfig.UILayer); // 重新排列面板
            }
        }

        // 创建面板栈并显示
        private UIStackCeil CreatePanelStack(UIBasePanel panel, string layer, ScreenParam param)
        {
            UIStackCeil existingStackCeil = _uiStacks[layer].Find(stackCeil => stackCeil.Panel.GetPanelName() == panel.GetPanelName());
            if (existingStackCeil != null)
            {
                StartCoroutine(existingStackCeil.Show(param)); // 显示已存在的面板
                return existingStackCeil;
            }

            UIStackCeil uiStackCeil = GenericObjectPoolFactory.Instance.GetObject<UIStackCeil>(); // 获取对象池中的 UIStackCeil
            uiStackCeil.Panel = panel; // 赋值面板
            panel.transform.SetParent(GetLayerRoot(layer), false); // 设置面板的父节点为对应层级

            _uiStacks[layer].Add(uiStackCeil); // 添加到 UI 栈
            StartCoroutine(uiStackCeil.Show(param)); // 显示面板
            return uiStackCeil; // 返回创建的面板栈
        }

        // 获取指定层级的根节点
        private Transform GetLayerRoot(string layer)
        {
            return layer switch
            {
                "Layer1" => layer1Root,
                "Layer2" => layer2Root,
                "Layer3" => layer3Root,
                "Layer4" => layer4Root,
                _ => defaultLayerRoot
            };
        }

        // 创建模态窗口背景
        private GameObject CreateModalBackground(UIBasePanel panel, string layer)
        {
            // 实例化模态背景预制体
            GameObject modalBackground = Instantiate(modalBackgroundPrefab, GetLayerRoot(layer));
            modalBackground.GetComponent<ModernPanel>().BindPanel(panel); // 绑定当前面板
            return modalBackground; // 返回模态背景
        }

        // 重新排列同层级内的 UI 顺序
        private void ResortPanel(string layer)
        {
            int siblingIndex = 0; // 初始化顺序索引
            foreach (var uiStackCeil in _uiStacks[layer])
            {
                if (uiStackCeil.IsShowing && uiStackCeil.Panel.transform.parent == GetLayerRoot(layer))
                {
                    uiStackCeil.Panel.transform.SetSiblingIndex(siblingIndex++); // 设置面板的顺序
                }
            }
        }
    }
}
