/// <summary>
/// 用于泛型引用池统一回收的接口
/// </summary>
public interface IGenericObjectPool
{
    void Recycle(IPoolable obj);
}
