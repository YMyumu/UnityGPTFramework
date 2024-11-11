using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class LocalizationDefineGenerator : Editor
{
    [MenuItem("Tools/生成本地化与语言定义文件 Generate Language and LocalizationDefine Script")]
    public static void GenerateLocalizationAndLanguageDefine()
    {
        // 读取 Localization.json 文件的路径
        string jsonFilePath = Application.dataPath + "/Resources/Configs/Localization.json";
        // LocalizationDefine.cs 文件保存路径
        string outputPath = Application.dataPath + "/Scripts/GPTF/LocalizationSystem/LocalizationDefine.cs";

        // 检查 Localization.json 文件是否存在
        if (!File.Exists(jsonFilePath))
        {
            LogManager.LogError("Localization.json file not found at path: " + jsonFilePath);
            return;
        }

        // 读取 json 文件内容
        var json = File.ReadAllText(jsonFilePath);
        var jsonObject = JObject.Parse(json);

        // 使用简体中文作为基准来生成键值
        var chineseSimplifiedEntries = jsonObject["Chinese_Simplified"];

        // 创建并写入 LocalizationDefine.cs 文件
        using (StreamWriter sw = new StreamWriter(outputPath))
        {
            // 写入 LanguageDefine 类
            sw.WriteLine("using System.Collections;");
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using UnityEngine;");
            sw.WriteLine();
            sw.WriteLine("// 此文件由工具自动生成");
            sw.WriteLine("public class LanguageDefine");
            sw.WriteLine("{");

            // 遍历 jsonObject，生成每种语言的常量字段
            foreach (var language in jsonObject.Properties())
            {
                string languageKey = language.Name;
                sw.WriteLine($"    public const string {languageKey} = \"{languageKey}\";");
            }

            sw.WriteLine("}");
            sw.WriteLine(); // 添加换行符

            // 写入 LocalizationDefine 类
            sw.WriteLine("public class LocalizationDefine");
            sw.WriteLine("{");

            // 只生成中文简体的键值
            foreach (var item in chineseSimplifiedEntries)
            {
                string key = item["key"].ToString();
                sw.WriteLine($"    public const string {key} = \"{key}\";");
            }

            sw.WriteLine("}");

            LogManager.LogInfo("LanguageDefine and LocalizationDefine script has been generated successfully!");
        }

        // 通知 Unity 刷新 Assets 目录
        AssetDatabase.Refresh();
    }
}
