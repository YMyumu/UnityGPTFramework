using System;

/// <summary>
/// IPoolable 接口定义了对象池中对象的初始化、重置和释放方法。
/// 实现该接口的对象可以在从对象池获取时被初始化，在回收时释放所有资源。
/// </summary>
public interface IPoolable : IDisposable
{
    /// <summary>
    /// 初始化对象，在从对象池中获取对象时调用。
    /// </summary>
    /// <param name="parameters">初始化时传递的参数</param>
    void Initialize(params object[] parameters);

    /// <summary>
    /// 重置对象，在回收到对象池时调用，使对象恢复到初始状态。
    /// </summary>
    void Reset();
}
