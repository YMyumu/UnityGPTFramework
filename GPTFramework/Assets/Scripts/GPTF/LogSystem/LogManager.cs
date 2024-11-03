/// <summary>
/// LogManager 类用于管理游戏中的日志记录。
/// 它提供了一套统一的接口，用于记录信息、警告和错误日志，并可选择性地将日志保存到文件中。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 通过静态方法记录不同级别的日志（信息、警告、错误）。
/// 2. 日志可以输出到Unity控制台，且可以保存到本地文件中（文件保存在Application.persistentDataPath/Logs目录下）。
/// 3. 自动创建日志目录，确保日志文件存储路径存在。
/// 4. 允许在运行时动态控制是否将日志写入文件。
/// </remarks>

using UnityEngine;
using System.IO;

public static class LogManager
{
    /// <summary>
    /// 定义日志的级别，包括信息、警告和错误。
    /// </summary>
    private enum LogLevel
    {
        Info,       // 信息日志
        Warning,    // 警告日志
        Error       // 错误日志
    }

    /// <summary>
    /// 日志文件保存路径，默认为Application.persistentDataPath/Logs。
    /// </summary>
    private static string logFilePath = Path.Combine(Application.persistentDataPath, "Logs");

    /// <summary>
    /// 控制是否将日志写入文件。默认为true，日志会被写入文件。
    /// </summary>
    private static bool writeToFile = true;

    /// <summary>
    /// 静态构造函数，初始化日志管理器，确保日志目录存在。
    /// </summary>
    static LogManager()
    {
        // 创建日志目录（如果不存在）
        if (!Directory.Exists(logFilePath))
        {
            Directory.CreateDirectory(logFilePath);
        }
    }

    /// <summary>
    /// 打印信息日志，并根据设置决定是否写入文件。
    /// </summary>
    /// <param name="message">要记录的日志消息。</param>
    public static void LogInfo(string message)
    {
        Log(message, LogLevel.Info);
    }

    /// <summary>
    /// 打印警告日志，并根据设置决定是否写入文件。
    /// </summary>
    /// <param name="message">要记录的日志消息。</param>
    public static void LogWarning(string message)
    {
        Log(message, LogLevel.Warning);
    }

    /// <summary>
    /// 打印错误日志，并根据设置决定是否写入文件。
    /// </summary>
    /// <param name="message">要记录的日志消息。</param>
    public static void LogError(string message)
    {
        Log(message, LogLevel.Error);
    }

    /// <summary>
    /// 日志处理方法，根据日志级别打印到控制台，并根据设置决定是否写入文件。
    /// </summary>
    /// <param name="message">要记录的日志消息。</param>
    /// <param name="level">日志的级别（信息、警告或错误）。</param>
    private static void Log(string message, LogLevel level)
    {
        string logMessage = $"[{System.DateTime.Now}] [{level}] {message}";

        // 根据日志级别输出到Unity控制台
        switch (level)
        {
            case LogLevel.Info:
                Debug.Log(logMessage);
                break;
            case LogLevel.Warning:
                Debug.LogWarning(logMessage);
                break;
            case LogLevel.Error:
                Debug.LogError(logMessage);
                break;
        }

        // 如果设置为写入文件，则将日志写入文件
        if (writeToFile)
        {
            WriteLogToFile(logMessage);
        }
    }

    /// <summary>
    /// 将日志消息写入文件，文件名为当前日期，存储在日志路径下。
    /// </summary>
    /// <param name="message">要写入文件的日志消息。</param>
    private static void WriteLogToFile(string message)
    {
        string fileName = $"log_{System.DateTime.Now:yyyy-MM-dd}.txt";
        string fullPath = Path.Combine(logFilePath, fileName);

        using (StreamWriter writer = new StreamWriter(fullPath, true))
        {
            writer.WriteLine(message);
        }
    }
}
