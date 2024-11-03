/// <summary>
/// IObjectPool 接口定义了对象池的基本操作，包括获取对象、回收对象和清理对象。
/// 它是通用对象池和Unity对象池的基础接口，用于管理池内对象的生命周期。
/// </summary>
/// <typeparam name="T">对象池中管理的对象类型。</typeparam>
using System;
public interface IObjectPool<T>
{
    /// <summary>
    /// 从对象池中获取对象。
    /// </summary>
    /// <returns>从池中获取的对象实例。</returns>
    T Get();

    /// <summary>
    /// 将对象回收到对象池中。
    /// </summary>
    /// <param name="item">要回收的对象实例。</param>
    void Recycle(T item);

    /// <summary>
    /// 清理对象池中的对象，根据指定的条件移除不需要的对象。
    /// </summary>
    /// <param name="shouldCleanup">用于判断是否应移除对象的条件函数。</param>
    void Cleanup(Func<T, bool> shouldCleanup);
}
