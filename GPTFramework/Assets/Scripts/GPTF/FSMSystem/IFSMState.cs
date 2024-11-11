
namespace FSMModule
{
/// <summary>
/// IFSMState 定义了有限状态机状态接口，所有状态类都需要实现这个接口。
/// </summary>
public interface IFSMState
{
    void OnEnter(FSMStateData data); // 进入状态时调用，传入状态数据
    void OnUpdate(); // 每帧更新状态
    void OnExit(); // 离开状态时调用
    string GetState(); // 返回当前状态的名称
}

}
