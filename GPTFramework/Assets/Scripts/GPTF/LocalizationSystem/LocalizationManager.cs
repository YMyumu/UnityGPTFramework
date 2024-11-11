using ConfigModule;
using EventModule;
using System.Collections.Generic;
using UnityEngine;


namespace LocalizationModule
{
    public class LocalizationManager : MonoSingleton<LocalizationManager>, IInitable
    {
        // 存储当前语言的本地化键值对
        private Dictionary<string, string> localizedText;

        // 当前语言
        private string currentLanguage;

        // 初始化语言管理器
        public void Init()
        {
            //// 获取上次保存的语言，没有保存则默认是英文
            //currentLanguage = PlayerPrefs.GetString("CurrentLanguage", LocalizationDefine.English);
            currentLanguage = LanguageDefine.Chinese_Simplified;

            // 加载当前语言的数据
            LoadLanguage(currentLanguage);
        }

        // 加载语言数据
        public void LoadLanguage(string languageKey)
        {
            // 初始化存储字典
            localizedText = new Dictionary<string, string>();

            if (languageKey == LocalizationDefine.Chinese_Simplified)
            {
                // 从配置管理器获取中文简体的键值对
                var config = ConfigManager.Instance.GetConfig<Chinese_SimplifiedCfg>();
                foreach (var item in config.cfg)
                {
                    localizedText[item.Key] = item.Text;
                }
            }
            else if (languageKey == LocalizationDefine.English)
            {
                // 从配置管理器获取英语的键值对
                var config = ConfigManager.Instance.GetConfig<EnglishCfg>();
                foreach (var item in config.cfg)
                {
                    localizedText[item.Key] = item.Text;
                }
            }
            else
            {
                LogManager.LogError("不存在的语言，加载失败！");
                return;
            }
            //// 保存当前语言设置到 PlayerPrefs
            //PlayerPrefs.SetString("CurrentLanguage", currentLanguage);

            LogManager.LogInfo("调用了切换语言的事件");
            //// 通知所有监听的UI更新文本
            //OnLanguageChanged?.Invoke();
            EventManager.Instance.Dispatch(EventDefine.SWITCH_LANGUAGE);
        }

        // 获取当前本地化文本
        public string GetLocalizedText(string key)
        {
            if (localizedText != null && localizedText.ContainsKey(key))
            {
                return localizedText[key];
            }

            LogManager.LogWarning($"Key '{key}' 未在本地化数据中找到");
            return key; // 返回键名作为默认文本
        }

        // 切换语言
        public void SwitchLanguage(string languagKey)
        {
            if (currentLanguage == languagKey)
                LogManager.LogInfo("已经是当前语言！");
            currentLanguage = languagKey;
            LoadLanguage(currentLanguage);
        }

        //// 语言切换事件，用于通知UI更新
        //public static event System.Action OnLanguageChanged;
    }

}
