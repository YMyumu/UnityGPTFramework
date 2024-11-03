using EventModule;
using UnityEngine;
using UnityEngine.UI;

namespace LocalizationModule
{
    public class LocalizedText : MonoBehaviour
    {
        [Tooltip("该字段是对应在 LocalizationDefine 中的键值")]
        public string localizationKey;

        // 固定文本部分，可以是直接的文本，不会随语言变化
        [Tooltip("固定在本地化文本前面的内容")]
        public string staticTextBefore;
        [Tooltip("固定在本地化文本后面的内容")]
        public string staticTextAfter; 

        private Text uiText;

        // 组件初始化时，绑定 UI 文本和本地化事件
        private void Start()
        {
            uiText = GetComponent<Text>();
            UpdateText(); // 初始化时立即更新文本
            //LocalizationManager.OnLanguageChanged += UpdateText; // 监听语言切换事件

            EventManager.Instance.Register(EventDefine.SWITCH_LANGUAGE, UpdateText);

        }

        // 更新文本内容为当前语言的本地化文本，并拼接固定部分
        private void UpdateText()
        {
            if (uiText != null)
            {
                // 从 LocalizationManager 获取对应键值的本地化文本
                string localizedText = LocalizationManager.Instance.GetLocalizedText(localizationKey);

                // 将固定文本部分和本地化文本拼接起来
                uiText.text = $"{staticTextBefore}{localizedText}{staticTextAfter}";
            }   
        }

        // 当该组件销毁时，取消事件监听，避免内存泄漏
        private void OnDestroy()
        {
            //LocalizationManager.OnLanguageChanged -= UpdateText;
            EventManager.Instance.Remove(EventDefine.SWITCH_LANGUAGE, UpdateText);

        }
    }

}
