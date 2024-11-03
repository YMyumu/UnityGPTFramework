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
    public class UIManager : MonoSingleton<UIManager>, IInitable // UIManager 继承 MonoSingleton 类，实现单例模式，确保只有一个 UIManager 实例
    {
        // 默认的初始界面名
        private static string _defaultUIName = UIDefine.Panel_1;
        public static string defaultUIName
        {
            get => _defaultUIName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _defaultUIName = value;
                }
            }
        }
        [SerializeField]
        public List<UIStackCeil> _uiStack = new List<UIStackCeil>(); // 列表，存储 UI 界面的栈，控制界面的打开顺序

        private Dictionary<string, int> _panelRefDic = new Dictionary<string, int>(); // 字典，记录每个 UI 界面的引用计数，用于管理资源
        private Dictionary<string, UIBasePanel> _panelDic = new Dictionary<string, UIBasePanel>(); // 字典，存储已经打开的 UI 面板，方便查找和管理
        private Dictionary<string, (string destroyTaskToken, GameObject panel)> _waitDestroyPanelDic = new Dictionary<string, (string, GameObject)>(); // 字典，存储等待销毁的面板及其销毁任务的令牌

        public GameObject canvasRoot;       // 画布根对象
        public Transform defaultLayerRoot, layer1Root, layer2Root, layer3Root, layer4Root; // 五个 Transform 变量，分别表示不同 UI 层级的根节点
        public GameObject modalBackgroundPrefab; // 模态窗口背景的预制体，用于生成模态窗口背景
        private const float _waitDestroyTime = 20f; // 常量，设置延迟销毁的时间，单位是秒

        private GameObject _inputBlockerInstance;   // 实例化后的遮罩对象

        // 全屏界面栈，存储当前激活的全屏界面，控制全屏窗口的显示顺序
        private Stack<UIStackCeil> _fullScreenStack = new Stack<UIStackCeil>();
        // 字典，记录每个全屏界面遮挡的窗口，方便恢复这些窗口的显示状态
        private Dictionary<UIStackCeil, List<UIStackCeil>> _coveredWindows = new Dictionary<UIStackCeil, List<UIStackCeil>>();

        /// <summary>
        /// 获取指定 UI 层级的根节点。
        /// </summary>
        /// <param name="layer">指定的 UI 层级。</param>
        /// <returns>返回指定层级的根节点 Transform。</returns>
        private Transform GetLayerRoot(string layer) => layer switch // 使用 switch 表达式，根据不同层级返回对应的根节点
        {
            "Layer1" => layer1Root, // 如果层级为 Layer1，返回 Layer1 的根节点
            "Layer2" => layer2Root, // 如果层级为 Layer2，返回 Layer2 的根节点
            "Layer3" => layer3Root, // 如果层级为 Layer3，返回 Layer3 的根节点
            "Layer4" => layer4Root, // 如果层级为 Layer3，返回 Layer3 的根节点
            _ => defaultLayerRoot // 默认返回 Default 层级的根节点
        };

        public void Init()
        {
            canvasRoot =  Instantiate(ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/CanvasRoot.prefab"));
            defaultLayerRoot = canvasRoot.transform.GetChild(1);
            layer1Root = canvasRoot.transform.GetChild(2);
            layer2Root = canvasRoot.transform.GetChild(3);
            layer3Root = canvasRoot.transform.GetChild(4);
            layer4Root = canvasRoot.transform.GetChild(5);
            modalBackgroundPrefab = ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/Panel/ModernPanel.prefab");


            // 初始化全局输入遮罩
            CreateInputBlocker();

            DontDestroyOnLoad(canvasRoot);

        }


        /// <summary>
        /// 打开指定名称的 UI 界面，处理层级和栈的管理逻辑。
        /// </summary>
        /// <param name="uiName">要打开的 UI 界面的名称。</param>
        /// <param name="param">传递给 UI 界面的参数对象，通常用于界面初始化。</param>
        /// <param name="shouldCloseHigherLayers">是否需要关闭或隐藏高层级的窗口。</param>
        public void OpenPanel(string uiName, ScreenParam param = null, bool shouldCloseHigherLayers = false)
        {
            // 从配置文件中查找指定 UI 名称的配置信息
            UIPanels uiConfig = ConfigManager.Instance.GetConfig<UIPanelsCfg>().cfg.Find(config => config.UIName == uiName);

            // 如果没有找到对应的 UI 配置信息，则输出错误信息并返回
            if (uiConfig == null)
            {
                LogManager.LogError($"UI配置中未找到 {uiName} 的相关信息！");
                return;
            }

            // 检查是否有等待销毁的缓存界面需要恢复
            if (_waitDestroyPanelDic.TryGetValue(uiName, out var tuple))
            {
                // 取消延迟销毁任务，防止界面被提前销毁
                DelayedTaskScheduler.Instance.RemoveDelayedTask(tuple.destroyTaskToken);

                // 从等待销毁的字典中移除该界面
                _waitDestroyPanelDic.Remove(uiName);

                // 将缓存的面板重新加入到 _panelDic 中管理
                _panelDic[uiName] = tuple.panel.GetComponent<UIBasePanel>();
            }

            // 获取或实例化指定的 UI 面板
            UIBasePanel panel = GetPanel(uiConfig);

            // 如果面板存在，进行下一步的逻辑处理
            if (panel != null)
            {
                // 设置面板的配置（模态、点击背景关闭、层级）
                panel.SetConfig(uiConfig.IsFullPanel, uiConfig.IsCloseSameLayer, uiConfig.IsModernPanel, uiConfig.IsClickModernNeedClose, uiConfig.UILayer);

                // **增加引用计数逻辑**
                if (_panelRefDic.ContainsKey(uiName)) // 检查字典中是否已经有该面板的记录
                {
                    //_panelRefDic[uiName]++; // 如果存在该面板，增加引用计数
                }
                else
                {
                    _panelRefDic[uiName] = 1; // 如果不存在，初始化引用计数为 1
                }


                // 判断是否是全屏面板
                if (panel.IsFullPanel())
                {
                    // 调用处理全屏面板打开的逻辑
                    HandleFullScreenPanel(panel, uiConfig, param, shouldCloseHigherLayers);
                }
                else
                {

                    // 调用处理非全屏面板的逻辑
                    ShowPanel(panel, uiConfig, param, shouldCloseHigherLayers);


                    // 如果面板是模态窗口，创建模态背景  模态窗口不入栈，所以没法通过自动排序来调整层级关系，只能等待窗口先创建完后，再创建模态界面，调整为窗口之下
                    if (panel.IsModernPanel())
                    {
                        CreateModalBackground(panel, uiConfig.UILayer); // 创建模态窗口背景
                    }
                }


            }
        }


        /// <summary>
        /// 关闭指定名称的 UI 界面。
        /// </summary>
        /// <param name="uiName">要关闭的 UI 界面名称。</param>
        public void ClosePanel(string uiName)
        {
            
            StartCoroutine(UIStackCeilClose(uiName));

            //如果UI栈没有显示的UI了，则打开默认UI
            if (_uiStack.Count == 0)
            {
                OpenPanel(defaultUIName);
            }

        }


        /// <summary>
        /// 由于有关闭界面的DOTween动画，所以不能直接在普通函数方法内执行下面的代码
        /// 需要走协程来调用，等待界面关闭动画完成后再走其他的代码
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        private IEnumerator UIStackCeilClose(string uiName)
        {
            // 从栈顶开始遍历 UI 栈，向下遍历
            for (int i = _uiStack.Count - 1; i >= 0; i--)
            {
                UIStackCeil stackCeil = _uiStack[i]; // 获取栈顶的界面

                // 如果不是要关闭的界面，跳过该迭代
                if (stackCeil.Panel.GetPanelName() != uiName)
                {
                    continue;
                }

                _panelRefDic[uiName]--; // 减少该界面的引用计数
                if (_panelRefDic[uiName] <= 0) // 如果引用计数为0，处理延迟销毁逻辑
                {
                    // 只有还显示的界面才需要等待界面关闭动画播完，已经是隐藏的直接走延迟销毁
                    if (stackCeil.IsShowing)
                        yield return StartCoroutine(stackCeil.Close());
                    else
                        stackCeil.Hide();

                    // 添加延迟销毁任务
                    string destroyTaskToken = DelayedTaskScheduler.Instance.AddDelayedTask(_waitDestroyTime, () =>
                    {
                        if (_waitDestroyPanelDic.ContainsKey(uiName))
                        {
                            Destroy(_waitDestroyPanelDic[uiName].panel); // 销毁界面
                            _waitDestroyPanelDic.Remove(uiName); // 从等待销毁字典中移除

                            // 移除引用计数和面板记录
                            _panelRefDic.Remove(uiName);
                            _panelDic.Remove(uiName);
                        }
                    });
                    _waitDestroyPanelDic.Add(uiName, (destroyTaskToken, stackCeil.Panel.gameObject)); // 将面板添加到等待销毁字典中
                }
                else
                {
                    stackCeil.Hide(); // 如果引用计数不为0，只隐藏界面
                }

                // 如果是全屏面板，调用关闭全屏面板的逻辑
                if (stackCeil.Panel.IsFullPanel())
                {
                    HandleFullScreenPanelClose(stackCeil);
                }

                // 直接移除并结束循环
                _uiStack.Remove(stackCeil); // 从栈中移除界面
                GenericObjectPoolFactory.Instance.RecycleObject(stackCeil); // 回收该对象，返回对象池中
                break; // 退出循环
            }
        }


        /// <summary>
        /// 处理全屏界面的打开逻辑，隐藏被遮挡的窗口。
        /// </summary>
        private void HandleFullScreenPanel(UIBasePanel panel, UIPanels uiConfig, ScreenParam param, bool shouldCloseHigherLayers)
        {
            List<UIStackCeil> windowsToHide = new List<UIStackCeil>();

            // 检查是否已存在相同的全屏界面
            UIStackCeil existingFullScreenPanel = _uiStack.Find(p => p.Panel == panel);

            // 在当前全屏界面之下的界面保持隐藏的窗口，不恢复显示，也不清理字典
            // 关闭该全屏界面之上的窗口，并将其从字典中移除
            CloseHigherLevelPanels(uiConfig.UILayer, panel);

            if (existingFullScreenPanel != null)
            {
                // 显示全屏界面
                StartCoroutine(existingFullScreenPanel.Show(param));
            }
            else
            {
                // 正常打开新的全屏界面
                if (_fullScreenStack.Count > 0)
                {
                    UIStackCeil previousFullScreenPanel = _fullScreenStack.Peek();
                    windowsToHide = GetWindowPanelsBetween(previousFullScreenPanel, panel);
                }
                else
                {
                    windowsToHide = GetWindowPanelsBetween(null, panel);
                }

                foreach (var stackCeil in windowsToHide)
                {
                    stackCeil.Hide();
                }

                // 创建面板栈并显示
                var uiStackCeil = CreatePanelStack(panel, uiConfig.UILayer, param);

                // 记录被当前全屏界面覆盖的窗口
                _coveredWindows[uiStackCeil] = windowsToHide;

                // 将当前全屏界面压入栈
                _fullScreenStack.Push(uiStackCeil);

                ResortPanel(uiConfig.UILayer);
            }
        }


        /// <summary>
        /// 处理全屏界面的关闭逻辑，恢复被遮挡的窗口。
        /// </summary>
        private void HandleFullScreenPanelClose(UIStackCeil closingFullScreenPanel)
        {
            // 弹出栈
            if (_fullScreenStack.Count > 0 && _fullScreenStack.Peek() == closingFullScreenPanel)
            {
                _fullScreenStack.Pop(); // 从全屏栈中移除当前关闭的全屏面板
            }

            // 恢复被当前全屏界面覆盖的窗口
            if (_coveredWindows.ContainsKey(closingFullScreenPanel))
            {
                foreach (var window in _coveredWindows[closingFullScreenPanel])
                {
                    StartCoroutine(window.Show());  // 启动 Show 协程恢复这些被覆盖的窗口

                }
                _coveredWindows.Remove(closingFullScreenPanel); // 从字典中移除
            }

            // 恢复之前的全屏界面
            if (_fullScreenStack.Count > 0)
            {
                StartCoroutine(_fullScreenStack.Peek().Show()); // 显示栈中上一个全屏界面
            }
        }


        /// <summary>
        /// 显示非全屏窗口的逻辑。
        /// </summary>
        private void ShowPanel(UIBasePanel panel, UIPanels uiConfig, ScreenParam param, bool shouldCloseHigherLayers)
        {
            //// 如果 shouldCloseHigherLayers 为 true，关闭所有高层界面，否则仅隐藏
            //CloseOrHideHigherLevelPanels(uiConfig.UILayer, shouldCloseHigherLayers);

            CloseHigherLevelPanels(uiConfig.UILayer, panel);

            // 如果界面是需要隐藏同层界面的
            if (panel.IsCloseSameLayer())
            {
                CloseOrHideSameLayerPanels(uiConfig.UILayer, false);
            }

            CreatePanelStack(panel, uiConfig.UILayer, param); // 创建面板栈并显示

            ResortPanel(uiConfig.UILayer);  // 调整该层级的窗口显示顺序
        }


        /// <summary>
        /// 获取或实例化指定名称的 UI 界面。
        /// </summary>
        private UIBasePanel GetPanel(UIPanels uiConfig)
        {
            if (_panelDic.TryGetValue(uiConfig.UIName, out var panel)) return panel; // 如果字典中已有面板，直接返回

            GameObject objPanel = Instantiate(ResourceManager.Instance.LoadResource<GameObject>(uiConfig.Path)); // 实例化面板预制体
            if (objPanel == null) // 如果未能加载面板，输出错误信息
            {
                LogManager.LogError("Load Panel Error: " + uiConfig.UIName);
                return null;
            }

            objPanel.name = uiConfig.UIName; // 设置面板的名称
            UIBasePanel basePanel = objPanel.GetComponent<UIBasePanel>(); // 获取 UIBasePanel 组件
            if (basePanel == null) LogManager.LogError("Panel not has UIBasePanel Component: " + uiConfig.UIName); // 如果未找到 UIBasePanel 组件，输出错误
            else _panelDic.Add(uiConfig.UIName, basePanel); // 将面板存储到字典中

            return basePanel; // 返回面板
        }


        /// <summary>
        /// 创建面板栈结构并显示。
        /// </summary>
        private UIStackCeil CreatePanelStack(UIBasePanel panel, string layer, ScreenParam param)
        {
            // 检查是否已经存在相同的面板
            UIStackCeil existingStackCeil = _uiStack.Find(stackCeil => stackCeil.Panel != null && stackCeil.Panel.GetPanelName() == panel.GetPanelName());
            if (existingStackCeil != null)
            {
                StartCoroutine(existingStackCeil.Show(param)); // 显示面板，并传递参数
                return existingStackCeil;
            }

            // 如果不存在相同的面板，创建新的栈元素
            UIStackCeil uiStackCeil = GenericObjectPoolFactory.Instance.GetObject<UIStackCeil>(); // 创建 UIStackCeil 实例，用于存储面板
            uiStackCeil.Panel = panel; // 将面板赋值给 UIStackCeil
            panel.transform.SetParent(GetLayerRoot(layer), false); // 将面板设置为指定层级的子节点

            _uiStack.Add(uiStackCeil); // 将面板添加到 UI 栈
            StartCoroutine(uiStackCeil.Show(param)); // 显示面板，并传递参数

            return uiStackCeil; // 返回 UIStackCeil 实例
        }


        /// <summary>
        /// 创建模态窗口背景并绑定关闭当前面板的逻辑。
        /// </summary>
        private GameObject CreateModalBackground(UIBasePanel panel, string layer)
        {
            UnityObjectPoolFactory.Instance.CreatePool(modalBackgroundPrefab, "ModernPanel", 1, 10);

            // 实例化模态背景预制体，并将其设置为与面板相同的父节点
            GameObject modalBackground = UnityObjectPoolFactory.Instance.GetObject("ModernPanel");

            modalBackground.transform.SetParent(GetLayerRoot(layer), false); // 将面板设置为指定层级的子节点

            // 获取或添加 ModernPanel 组件
            ModernPanel modernPanel = modalBackground.GetComponent<ModernPanel>();


            // 绑定当前面板到模态背景，并添加关闭逻辑
            modernPanel.BindPanel(panel);

            // **确保模态背景位于其他界面上方，但低于当前界面**
            int siblingIndex = panel.transform.GetSiblingIndex(); // 获取当前窗口的 sibling index
            modalBackground.transform.SetSiblingIndex(siblingIndex - 1 < 0 ? 0 : siblingIndex - 1); // 将模态背景插入当前窗口下方


            return modalBackground;
        }


        /// <summary>
        /// 关闭比当前面板更高的界面以及层级更高的界面
        /// </summary>
        /// <param name="targetLayer">目标层级</param>
        /// <param name="currentPanel">当前面板</param>
        private void CloseHigherLevelPanels(string targetLayer, UIBasePanel currentPanel)
        {
            List<string> layerOrder = new List<string> { "Default", "Layer1", "Layer2", "Layer3", "Layer4" }; // 层级顺序
            int targetLayerIndex = layerOrder.IndexOf(targetLayer);

            for (int i = _uiStack.Count - 1; i >= 0; i--)
            {
                UIStackCeil stackCeil = _uiStack[i];
                int panelLayerIndex = layerOrder.IndexOf(stackCeil.Panel.UILayer);

                // 如果面板层级是 Layer3，跳过关闭操作
                if (stackCeil.Panel.UILayer == "Layer3")
                {
                    continue; // Layer3 不会被关闭
                }

                // 检查当前栈中的面板是否为全屏，并且层级比当前面板低
                if (stackCeil.Panel.IsFullPanel() && stackCeil.Panel != currentPanel && panelLayerIndex <= targetLayerIndex)
                {
                    break; // 只在遇到比当前面板低的全屏界面时停止操作
                }

                // 关闭比当前层级高的窗口
                if (panelLayerIndex > targetLayerIndex)
                {
                    // 如果面板存在于 _coveredWindows 字典中，移除覆盖关系
                    if (_coveredWindows.ContainsKey(stackCeil))
                    {
                        _coveredWindows.Remove(stackCeil);
                    }

                    // 关闭该面板
                    ClosePanel(stackCeil.Panel.GetPanelName());
                }
            }
        }


        /// <summary>
        ///  关闭或者隐藏当前面板处于同一层级的其他面板。
        /// </summary>
        /// <param name="currentLayer">当前面板的层级。</param>
        private void CloseOrHideSameLayerPanels(string currentLayer, bool shouldCloseHigherLayers)
        {
            foreach (var stackCeil in _uiStack)
            {
                if (stackCeil.Panel.UILayer == currentLayer && stackCeil.IsShowing)
                {
                    if (shouldCloseHigherLayers)
                        stackCeil.Close();  // 关闭该面板
                    else
                        stackCeil.Hide();  // 隐藏该面板
                }
            }
        }


        /// <summary>
        /// 获取上一个全屏界面和当前全屏界面之间的窗口，且只获取当前层级以下的窗口，并包括上一个全屏界面。
        /// </summary>
        private List<UIStackCeil> GetWindowPanelsBetween(UIStackCeil previousFullScreenPanel, UIBasePanel currentPanel)
        {
            List<UIStackCeil> windowsToHide = new List<UIStackCeil>(); // 定义列表存储需要隐藏的窗口
            List<string> layerOrder = new List<string> { "Default", "Layer1", "Layer2", "Layer3", "Layer4" }; // 层级顺序列表

            // 获取当前面板所在层级的索引
            int currentPanelLayerIndex = layerOrder.IndexOf(currentPanel.UILayer);

            // 遍历 UI 栈，找到位于两个全屏界面之间且层级小于或等于当前面板的窗口
            for (int i = _uiStack.Count - 1; i >= 0; i--)
            {
                UIStackCeil stackCeil = _uiStack[i];

                // 获取当前栈中面板的层级索引
                int stackCeilLayerIndex = layerOrder.IndexOf(stackCeil.Panel.UILayer);

                // 如果找到了当前全屏界面，停止查找
                if (stackCeil.Panel == currentPanel)
                    break;

                // 在遇到上一个全屏界面时，将它加入待隐藏列表，然后停止查找
                if (previousFullScreenPanel != null && stackCeil == previousFullScreenPanel)
                {
                    windowsToHide.Add(stackCeil); // 加入上一个全屏面板
                    break; // 停止查找
                }

                // 只加入与当前层级相同或更低层级的窗口，并且它们不是全屏面板
                if (stackCeilLayerIndex <= currentPanelLayerIndex && !stackCeil.Panel.IsFullPanel())
                {
                    windowsToHide.Add(stackCeil); // 加入待隐藏列表
                }
            }

            return windowsToHide; // 返回需要隐藏的窗口列表
        }


        /// <summary>
        /// 重新排列同层级内的 UI 顺序。
        /// </summary>
        private void ResortPanel(string layer)
        {
            int siblingIndex = 0; // 初始化顺序索引
            foreach (var uiStackCeil in _uiStack) // 遍历 UI 栈
            {
                if (uiStackCeil.IsShowing && uiStackCeil.Panel.transform.parent == GetLayerRoot(layer)) // 如果面板处于显示状态并属于当前层级
                {
                    uiStackCeil.Panel.transform.SetSiblingIndex(siblingIndex++); // 设置面板的顺序
                }
            }
        }


        /// <summary>
        /// 创建输入遮罩
        /// </summary>
        private void CreateInputBlocker()
        {
            // 创建遮罩并附加到 UI 根节点（最顶层）
            _inputBlockerInstance = Instantiate(ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/Panel/InputBlockerPrefab.prefab"), layer4Root);

            if (_inputBlockerInstance != null)
            {
                // 初始化时禁用遮罩
                _inputBlockerInstance.GetComponent<CanvasGroup>().blocksRaycasts = false; // 禁止交互

                //_inputBlockerInstance.SetActive(false); // 显示遮罩

            }
        }

        /// <summary>
        /// 显示遮罩，屏蔽输入
        /// </summary>
        public void ShowInputBlocker()
        {
            if (_inputBlockerInstance != null)
            {
                //_inputBlockerInstance.SetActive(true); // 显示遮罩
                _inputBlockerInstance.GetComponent<CanvasGroup>().blocksRaycasts = true; // 启用交互，阻止点击
            }
        }

        /// <summary>
        /// 隐藏遮罩，恢复输入
        /// </summary>
        public void HideInputBlocker()
        {
            if (_inputBlockerInstance != null)
            {
                _inputBlockerInstance.GetComponent<CanvasGroup>().blocksRaycasts = false; // 禁用交互
                //_inputBlockerInstance.SetActive(false); // 隐藏遮罩
            }
        }
    }

}