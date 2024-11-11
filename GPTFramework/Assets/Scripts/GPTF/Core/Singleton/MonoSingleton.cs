/*
 * 文件名: MonoSingleton.cs
 * 作者: 
 * 日期: 2024年08月29日
 * 描述: 实现一个MonoBehaviour的单例类MonoSingleton，用于Unity环境下确保一个组件在游戏中只有一个实例。
 * 版本: 
 */

using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    // 定义一个静态变量用于存储类的唯一实例
    private static T _instance = null;

    // 定义一个公共的静态属性，用于获取类的唯一实例
    public static T Instance
    {
        get
        {
            // 如果实例未被创建，尝试在场景中查找已有的实例
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                // 如果在场景中未找到实例，则创建一个新实例
                if (_instance == null)
                {
                    // 创建一个新的GameObject，并将该类组件附加到该对象上
                    GameObject obj = new GameObject(typeof(T).Name, new[] { typeof(T) });

                    // 确保对象在加载新场景时不会被销毁
                    DontDestroyOnLoad(obj);

                    // 获取附加到新创建的GameObject上的组件实例
                    _instance = obj.GetComponent<T>();

                    // 如果组件实现了IInitable接口，则调用其Init方法进行初始化
                    (_instance as IInitable)?.Init();
                }
                else
                {
                    // 如果在场景中已找到实例，输出警告信息
                    LogManager.LogWarning("实例已存在!");
                }
            }

            // 返回已创建的实例
            return _instance;
        }
    }

    /// <summary>
    /// 如果继承了MonoSingleton的类需要实现Awake方法，必须在Awake方法的最开始调用base.Awake()，
    /// 以确保单例实例的正确赋值。
    /// </summary>
    private void Awake()
    {
        // 将单例实例设置为当前对象
        _instance = this as T;

        // 确保当前对象在加载新场景时不会被销毁
        DontDestroyOnLoad(this);
    }
}
