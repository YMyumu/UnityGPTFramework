using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public class ExportAssetsWithScripts : EditorWindow
{
    private string exportFilePath = "Assets/ExportedProjectStructure.txt"; // 导出文件路径

    [MenuItem("Tools/Export Project Files With Structure")]
    public static void ShowWindow()
    {
        GetWindow<ExportAssetsWithScripts>("Export Project Structure");
    }

    private void OnGUI()
    {
        GUILayout.Label("Export Project Files and Scripts", EditorStyles.boldLabel);

        exportFilePath = EditorGUILayout.TextField("Export File Path:", exportFilePath);

        if (GUILayout.Button("Export Project"))
        {
            ExportAllAssetsWithStructure(exportFilePath);
        }
    }

    private void ExportAllAssetsWithStructure(string outputFile)
    {
        // 创建 StringBuilder 用于存储合并的内容
        StringBuilder projectContents = new StringBuilder();

        // 获取 Assets 文件夹下的所有文件
        string[] allFiles = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);

        foreach (string file in allFiles)
        {
            // 获取相对于 Assets 文件夹的路径
            string relativePath = "Assets" + file.Replace(Application.dataPath, "").Replace("\\", "/");

            // 判断文件类型
            if (file.EndsWith(".cs"))
            {
                // 对于 .cs 脚本，读取并写入其内容
                string fileContent = File.ReadAllText(file);

                // 添加文件路径和内容
                projectContents.AppendLine($"// ---- Start of {relativePath} ----");
                projectContents.AppendLine(fileContent);
                projectContents.AppendLine($"// ---- End of {relativePath} ----\n");
            }
            else
            {
                // 对于非 .cs 文件，只记录文件路径
                projectContents.AppendLine($"// {relativePath}");
            }
        }

        // 将内容写入指定的导出文件
        File.WriteAllText(outputFile, projectContents.ToString());

        LogManager.LogInfo($"Project structure and scripts exported to: {outputFile}");
        AssetDatabase.Refresh();
    }
}
