/*
 * 文件名: Singleton.cs
 * 作者: 
 * 日期: 2024年08月29日
 * 描述: 实现一个通用的单例模式类Singleton，用于确保一个类在应用程序中只有一个实例。
 * 版本: 
 */

public class Singleton<T> where T : new()
{
    // 定义一个静态变量用于存储类的唯一实例
    private static T _instance;

    // 定义一个公共的静态属性，用于获取类的唯一实例
    public static T Instance
    {
        get
        {
            // 如果实例未被创建，则创建一个新实例
            if (_instance == null)
            {
                // 创建类T的新实例
                _instance = new T();

                // 如果T实现了IInitable接口，则调用其Init方法进行初始化
                (_instance as IInitable)?.Init();
            }

            // 返回已创建的实例
            return _instance;
        }
    }
}
