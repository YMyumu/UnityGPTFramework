/*
手动导航到日志路径
你也可以手动导航到Application.persistentDataPath所指向的文件夹。具体位置会根据你的操作系统不同而有所不同：

Windows: Application.persistentDataPath通常位于C:\Users\<你的用户名>\AppData\LocalLow\<公司名称>\<项目名称>\。

macOS: Application.persistentDataPath通常位于/Users/<你的用户名>/Library/Application Support/<公司名称>/<项目名称>/。

Linux: Application.persistentDataPath通常位于/home/<你的用户名>/.config/unity3d/<公司名称>/<项目名称>/。

进入上述路径后，你会找到一个名为Logs的文件夹，里面包含了日志文件。

查看日志文件
打开Logs文件夹后，你就可以找到按照日期命名的日志文件（例如log_2024-09-01.txt），打开它即可查看记录的日志内容。

 */



using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestLog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LogManager.LogInfo("Game Started.");
        LogManager.LogWarning("This is a warning.");
        LogManager.LogError("An error has occurred.");

        LogManager.LogInfo("Log files are saved at: " + Path.Combine(Application.persistentDataPath, "Logs"));


    }
}
