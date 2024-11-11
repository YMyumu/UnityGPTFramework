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
        private Dictionary<string, List<UIStackCeil>> _uiStacksDict = new Dictionary<string, List<UIStackCeil>>();

        public List<UIStackCeil> _uiStacks0Look = new List<UIStackCeil>();
        public List<UIStackCeil> _uiStacks1Look = new List<UIStackCeil>();
        public List<UIStackCeil> _uiStacks2Look = new List<UIStackCeil>();
        public List<UIStackCeil> _uiStacks3Look = new List<UIStackCeil>();
        public List<UIStackCeil> _uiStacks4Look = new List<UIStackCeil>();


        private Dictionary<string, (string destroyTaskToken, UIStackCeil stackCeil)> _waitDestroyPanelDic = new Dictionary<string, (string, UIStackCeil)>();

        
        private Dictionary<string, List<UIStackCeil>> fullscreenPanelState = new Dictionary<string, List<UIStackCeil>>();



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
            // 字典键入的顺序很重要 关乎到按层级查找界面，需要满足从上到下
            _uiStacksDict.Add("Layer4", new List<UIStackCeil>());
            _uiStacksDict.Add("Layer3", new List<UIStackCeil>());
            _uiStacksDict.Add("Layer2", new List<UIStackCeil>());
            _uiStacksDict.Add("Layer1", new List<UIStackCeil>());
            _uiStacksDict.Add("Default", new List<UIStackCeil>());


            _uiStacks0Look = _uiStacksDict["Default"];
            _uiStacks1Look = _uiStacksDict["Layer1"];
            _uiStacks2Look = _uiStacksDict["Layer2"];
            _uiStacks3Look = _uiStacksDict["Layer3"];
            _uiStacks4Look = _uiStacksDict["Layer4"];


        }

        // 打开指定名称的 UI 界面
        public void OpenPanel(string uiName, ScreenParam param = null)
        {
            UIPanels uiConfig = LoadUIPanelConfig(uiName);
            if (uiConfig == null) return; // 找不到配置，退出




            UIBasePanel panel = GetPanel(uiConfig); // 获取或实例化面板
            if (panel == null) return; // 获取失败，退出

            ConfigurePanel(panel, uiConfig); // 配置面板


            if (panel.IsCloseSameLayer())       // 如果需要关闭同层其他界面   关闭走的协程 所以实际界面关闭肯定是靠后的，下方代码会快过界面关闭
            {
                CloseOtherPanelsInLayer(panel.UILayer);
            }


            if (panel.IsFullPanel())        //如果是全屏界面
            {
                // 获取当前全屏界面层级内到上一个全屏窗口（包括）所有的窗口界面
                fullscreenPanelState.Add(panel.GetPanelName(), GetPreviousFullScreenStackCeils(panel.UILayer, panel.IsCloseSameLayer()));

                // 找到了上一个全屏界面（包括）之间的所有窗口并隐藏
                if (fullscreenPanelState.TryGetValue(panel.GetPanelName(), out var uiStack))
                {
                    if (uiStack.Count > 0)
                    {
                        foreach (var item in uiStack)
                        {
                            item.Hide();
                        }

                    }
                }
            }

            // TOOD:这里需要做判断
            HandleWaitingForDestruction(uiName); // 处理等待销毁的界面

            // 检查是否已存在相同的全屏界面
            UIStackCeil existingFullScreenPanel = _uiStacksDict[uiConfig.UILayer].Find(p => p.panel == panel);
            if (existingFullScreenPanel != null)
            {
                StartCoroutine(existingFullScreenPanel.Show(param)); // 显示已存在的面板
            }
            else
            {
                CreatePanelStack(panel, uiConfig.UILayer, param); // 创建新的面板栈

                //ResortPanel(uiConfig.UILayer); // 重新排列面板
            }


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

        // 配置面板
        private void ConfigurePanel(UIBasePanel panel, UIPanels uiConfig)
        {
            panel.SetConfig(uiConfig.IsFullPanel, uiConfig.IsCloseSameLayer, uiConfig.IsModernPanel, uiConfig.IsClickModernNeedClose, uiConfig.UILayer);
        }

        // 创建面板栈并显示
        private UIStackCeil CreatePanelStack(UIBasePanel panel, string layer, ScreenParam param)
        {
            UIStackCeil existingStackCeil = _uiStacksDict[layer].Find(stackCeil => stackCeil.panel.GetPanelName() == panel.GetPanelName());
            if (existingStackCeil != null)
            {
                StartCoroutine(existingStackCeil.Show(param)); // 显示已存在的面板
                return existingStackCeil;
            }

            UIStackCeil uiStackCeil = GenericObjectPoolFactory.Instance.GetObject<UIStackCeil>(); // 获取对象池中的 UIStackCeil
            uiStackCeil.panel = panel; // 赋值面板
            panel.transform.SetParent(GetLayerRoot(layer), false); // 设置面板的父节点为对应层级

            _uiStacksDict[layer].Add(uiStackCeil); // 添加到 UI 栈
            StartCoroutine(uiStackCeil.Show(param)); // 显示面板
            return uiStackCeil; // 返回创建的面板栈
        }

        


        #region 关闭界面与销毁的逻辑

        // 关闭指定名称的 UI 界面
        public void ClosePanel(string uiName)
        {
            if (fullscreenPanelState.TryGetValue(uiName, out var uiStacks))
            {
                foreach (var item in uiStacks)
                {
                    StartCoroutine(item.Show()); // 显示已存在的面板
                }

                fullscreenPanelState.Remove(uiName);
            }


            StartCoroutine(UIStackCeilClose(uiName)); // 启动协程处理关闭逻辑
        }

        // 关闭面板的协程逻辑
        private IEnumerator UIStackCeilClose(string uiName)
        {
            foreach (var item in _uiStacksDict)
            {
                var _uiStacks = item.Value;
                for (int i = _uiStacks.Count - 1; i >= 0; i--)
                {
                    var stackCeil = _uiStacks[i];
                    if (stackCeil.panel.GetPanelName() != uiName) continue; // 不匹配，继续

                    yield return CloseStackCeil(stackCeil); // 处理关闭逻辑
                    break; // 退出循环
                }
            }
        }

        // 处理关闭面板的具体逻辑
        private IEnumerator CloseStackCeil(UIStackCeil stackCeil)
        {

            if (stackCeil.isShowing)
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
            stackCeil.isHideWaitClose = true;
            string destroyTaskToken = DelayedTaskScheduler.Instance.AddDelayedTask(_waitDestroyTime, () =>
            {
                Destroy(stackCeil.panel.gameObject); // 销毁面板
                _waitDestroyPanelDic.Remove(stackCeil.panel.GetPanelName()); // 从字典中移除
                _panelDic.Remove(stackCeil.panel.GetPanelName()); // 移除面板引用

                // 检查当前层的UI栈
                if (_uiStacksDict.TryGetValue(stackCeil.panel.UILayer, out var uiStacks))
                {
                    uiStacks.Remove(stackCeil);
                }
                // 回收
                GenericObjectPoolFactory.Instance.RecycleObject(stackCeil);

            });
            _waitDestroyPanelDic.Add(stackCeil.panel.GetPanelName(), (destroyTaskToken, stackCeil)); // 添加到等待销毁字典
            _uiStacksDict[stackCeil.panel.UILayer].Remove(stackCeil);

            if (IsAllLayersEmpty)
            {
                OpenPanel(_defaultUIName);
            }


        }

        // 重新打开了等待销毁的面板逻辑
        private void HandleWaitingForDestruction(string uiName)
        {
            if (_waitDestroyPanelDic.TryGetValue(uiName, out var tuple))
            {
                tuple.stackCeil.isHideWaitClose = false;
                DelayedTaskScheduler.Instance.RemoveDelayedTask(tuple.destroyTaskToken); // 取消延迟销毁

                _uiStacksDict[tuple.stackCeil.panel.UILayer].Add(tuple.stackCeil);
                _waitDestroyPanelDic.Remove(uiName); // 移除等待销毁字典
            }
        }



        #endregion



        #region 输入遮罩逻辑

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

        #endregion


        //// 处理面板逻辑
        //private void HandlePanel(UIBasePanel panel, UIPanels uiConfig, ScreenParam param)
        //{
        //    // 检查是否已存在相同的全屏界面
        //    UIStackCeil existingFullScreenPanel = _uiStacks[uiConfig.UILayer].Find(p => p.Panel == panel);
        //    if (existingFullScreenPanel != null)
        //    {
        //        StartCoroutine(existingFullScreenPanel.Show(param)); // 显示已存在的面板
        //    }
        //    else
        //    {
        //        CreatePanelStack(panel, uiConfig.UILayer, param); // 创建新的面板栈
        //        ResortPanel(uiConfig.UILayer); // 重新排列面板
        //    }
        //}






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
            foreach (var uiStackCeil in _uiStacksDict[layer])
            {
                if (uiStackCeil.isShowing && uiStackCeil.panel.transform.parent == GetLayerRoot(layer))
                {
                    uiStackCeil.panel.transform.SetSiblingIndex(siblingIndex++); // 设置面板的顺序
                }
            }
        }







        // 关闭当前层级中的所有其他界面
        public void CloseOtherPanelsInLayer(string currentLayer)
        {
            if (_uiStacksDict.TryGetValue(currentLayer, out var uiStacks))
            {
                List<string> paneNames = new List<string>();

                foreach (var stackCeil in uiStacks)
                {
                    if (stackCeil.isShowing)
                    {
                        paneNames.Add(stackCeil.panel.GetPanelName());
                    }
                }
                foreach (var name in paneNames)
                {
                    ClosePanel(name); // 关闭面板

                }
                paneNames.Clear();
                paneNames = null;
            }
        }




        // 获取当前层到上一个全屏界面的所有窗口，包括全屏界面的包装类 UIStackCeil
        public List<UIStackCeil> GetPreviousFullScreenStackCeils(string currentLayer, bool isCloseSameLayer)
        {
            List<UIStackCeil> stackCeilsToReturn = new List<UIStackCeil>();

            // 遍历当前层级及其下方层级
            bool foundFullScreen = false;

            foreach (var layer in _uiStacksDict.Keys)
            {
                // 只处理当前层及其下方层级   如果需要关闭同层 则不处理当前层
                if (UILayerIndex(layer) > UILayerIndex(currentLayer) || (isCloseSameLayer && UILayerIndex(layer) == UILayerIndex(currentLayer)))
                {

                    continue; // 跳过高于当前层的层级
                }

                // 检查当前层的UI栈
                if (_uiStacksDict.TryGetValue(layer, out var uiStacks))
                {
                    // 遍历当前层的UI栈，从顶部开始查找
                    foreach (var stackCeil in uiStacks)
                    {
                        // 先添加所有的窗口
                        stackCeilsToReturn.Add(stackCeil);

                        // 如果找到全屏界面，停止继续遍历
                        if (stackCeil.panel.IsFullPanel() && !foundFullScreen)
                        {
                            foundFullScreen = true; // 标记找到全屏界面
                            break; // 找到全屏界面后停止当前层级窗口遍历
                        }
                    }
                }

                // 如果找到全屏界面，跳出层级遍历
                if (foundFullScreen)
                {
                    break;
                }
            }

            return stackCeilsToReturn; // 返回所有找到的窗口和第一个全屏界面
        }


        // 属性：检查所有层级是否都为空（没有UI）排除层3 4
        public bool IsAllLayersEmpty
        {
            get
            {
                // 遍历所有层级，检查每个层级的UI栈是否为空
                foreach (var layer in _uiStacksDict.Keys)
                {
                    if (UILayerIndex(layer) == 3 || UILayerIndex(layer) == 4) continue;     // 如果是 3 4层就跳过

                    if (_uiStacksDict.TryGetValue(layer, out var uiStack) && uiStack.Count > 0)
                    {
                        // 如果有任何层级的UI栈不为空，返回 false
                        return false;
                    }
                }

                // 如果所有层级的UI栈都为空，返回 true
                return true;
            }
        }




        private int UILayerIndex(string UIlayer)
        {
            switch (UIlayer)
            {
                case "Default":
                    return 0;
                case "Layer1":
                    return 1;
                case "Layer2":
                    return 2;
                case "Layer3":
                    return 3;
                case "Layer4":
                    return 4;
            }
            return 0;
        }



    }
}
