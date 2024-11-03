/// <summary>
/// ConfigManager 类用于管理游戏中的配置表数据。
/// 它通过加载存储在 Resources/Configs 目录下的 JSON 文件，
/// 解析其中的配置表数据并将其存储在内存中，
/// 以便在游戏运行期间通过类型安全的方式访问这些配置数据。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 加载所有配置表数据并存储在字典中。
/// 2. 提供泛型方法 GetConfig<T>() 以类型安全的方式获取指定类型的配置表数据。
/// </remarks>

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace ConfigModule
{
    public class ConfigManager : MonoSingleton<ConfigManager>, IInitable
    {
        // 字典用于存储所有配置表数据，键为配置类型，值为配置对象实例
        private Dictionary<System.Type, object> configData = new Dictionary<System.Type, object>();

        // 初始化方法，用于加载所有配置表
        public void Init()
        {
            LoadConfigs(); // 调用加载配置表的方法
            LogManager.LogInfo("所有配置表加载完成！");
        }

        // 私有方法，用于加载配置表文件夹中的所有JSON文件
        private void LoadConfigs()
        {
            // 获取配置表文件夹路径
            string configFolderPath = Path.Combine(Application.dataPath, "Resources/Configs");

            // 获取所有JSON文件的路径
            string[] configFiles = Directory.GetFiles(configFolderPath, "*.json");

            // 遍历每个JSON文件
            foreach (string configFile in configFiles)
            {
                // 读取JSON文件内容
                string jsonContent = File.ReadAllText(configFile);

                // 解析JSON内容为JObject（一个键值对集合）
                var jsonObject = JObject.Parse(jsonContent);

                // 遍历每个属性（对应一个Sheet）
                foreach (var property in jsonObject.Properties())
                {
                    string sheetName = property.Name; // 获取Sheet的名称
                    string jsonSheetContent = property.Value.ToString(); // 获取Sheet内容的字符串表示形式

                    // 获取对应的配置类型，例如 "Sheet1Cfg"
                    var configType = System.Type.GetType(sheetName + "Cfg");
                    if (configType == null)
                    {
                        // 如果未找到对应的C#类，记录错误日志并跳过
                        LogManager.LogError($"未找到对应的C#类: {sheetName}Cfg");
                        continue;
                    }

                    // 通过反射获取配置类中的私有字段 "_cfg"
                    var cfgField = configType.GetField("_cfg", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (cfgField != null)
                    {
                        // 获取List的元素类型（即Sheet的类型，例如Sheet1）
                        var elementType = cfgField.FieldType.GetGenericArguments()[0];

                        // 创建一个List<elementType>类型的实例
                        var listType = typeof(List<>).MakeGenericType(elementType);

                        // 将JSON字符串反序列化为相应的List类型
                        var deserializedList = JsonConvert.DeserializeObject(jsonSheetContent, listType);

                        // 创建配置类的实例
                        var configObject = System.Activator.CreateInstance(configType, deserializedList);

                        // 将反序列化的List赋值给配置类的"_cfg"字段
                        cfgField.SetValue(configObject, deserializedList);

                        // 将配置类实例存储到字典中，类型为键，实例为值
                        configData[configType] = configObject;
                    }
                    else
                    {
                        // 如果未找到字段"_cfg"，记录错误日志
                        LogManager.LogError($"类 {configType} 中未找到字段 '_cfg'");
                    }
                }
            }
        }

        // 泛型方法，用于获取指定类型的配置表实例
        public T GetConfig<T>() where T : class
        {
            var type = typeof(T); // 获取泛型类型的实际类型
            if (configData.ContainsKey(type))
            {
                // 如果字典中存在该类型的配置表数据，返回其实例
                return configData[type] as T;
            }
            else
            {
                // 如果未找到该类型的配置表数据，记录错误日志并返回null
                LogManager.LogError($"未找到类型为 {type} 的配置表数据！");
                return null;
            }
        }
    }

}
