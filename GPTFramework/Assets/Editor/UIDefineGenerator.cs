using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class UIDefineGenerator : Editor
{
    [MenuItem("Tools/生成UI模块用的面板枚举类型文件 Generate UIDefine Script")]
    public static void GenerateUIDefine()
    {
        // 读取文件的路径
        string jsonFilePath = Application.dataPath + "/Resources/Configs/UIPanel.json";
        // 生成的UIDefine.cs文件保存路径
        string outputPath = Application.dataPath + "/Scripts/UI/UIDefine.cs";

        // 检查UIPanel.json文件是否存在
        if (!File.Exists(jsonFilePath))
        {
            LogManager.LogError("UIPanel.json file not found at path: " + jsonFilePath);
            return;
        }

        // 读取json文件内容
        var json = File.ReadAllText(jsonFilePath);
        var uiPanels = JObject.Parse(json)["UIPanels"];

        // 创建并写入UIDefine.cs文件
        using (StreamWriter sw = new StreamWriter(outputPath))
        {
            sw.WriteLine("using System.Collections;");
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using UnityEngine;");
            sw.WriteLine();
            sw.WriteLine("public class UIDefine");
            sw.WriteLine("{");

            foreach (var panel in uiPanels)
            {
                string uiname = panel["uiname"].ToString();
                sw.WriteLine($"    public const string {uiname} = \"{uiname}\";");
            }

            sw.WriteLine("}");
        }

        // 通知Unity刷新Assets目录
        LogManager.LogInfo("UIDefine.cs script has been generated successfully at: " + outputPath);
        AssetDatabase.Refresh();  // 刷新Unity编辑器，确保生成的脚本能够立即被加载
    }
}
