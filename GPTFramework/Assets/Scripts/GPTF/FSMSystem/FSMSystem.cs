using System;
using System.Collections.Generic;
using System.Linq;


namespace FSMModule
{
    /// <summary>
    /// FSMSystem 是单线有限状态机类，管理状态的添加、切换和更新。
    /// </summary>
    public class FSMSystem
    {
        private Dictionary<string, IFSMState> _states = new Dictionary<string, IFSMState>(); // 存储所有状态
        private IFSMState _currentState; // 当前活动的状态
        private bool _isChangingState; // 状态切换锁，防止重复切换状态

        /// <summary>
        /// 添加状态到状态机。
        /// </summary>
        public void AddState(string stateName, IFSMState state)
        {
            // 检查状态名称是否已经存在，如果不存在，则添加
            if (!_states.ContainsKey(stateName))
            {
                _states.Add(stateName, state); // 将状态添加到字典中
            }
        }

        /// <summary>
        /// 设置初始状态。
        /// </summary>
        public void SetInitialState(string stateName)
        {
            // 尝试从字典中获取初始状态
            if (_states.TryGetValue(stateName, out var initialState))
            {
                _currentState = initialState; // 设置当前状态为初始状态
                _currentState.OnEnter(null); // 进入初始状态，没有数据传入
            }
            else
            {
                LogManager.LogWarning($"初始状态 '{stateName}' 不存在，初始状态设置失败."); // 输出警告
            }
        }

        /// <summary>
        /// 切换到新状态。
        /// </summary>
        public void ChangeState(string newStateName, FSMStateData data = null)
        {
            // 检查是否正在进行状态切换
            if (_isChangingState)
            {
                LogManager.LogWarning("状态已在改变，中止状态改变."); // 输出警告
                return; // 退出方法，防止重复状态切换
            }

            _isChangingState = true; // 设置锁，防止再次调用

            // 检查当前状态是否与新状态相同，如果不同则进行状态切换
            if (_currentState != null && _currentState.GetState() != newStateName)
            {
                _currentState.OnExit(); // 退出当前状态
            }

            // 尝试获取新状态
            if (_states.TryGetValue(newStateName, out var newState))
            {
                _currentState = newState; // 设置当前状态为新状态
                _currentState.OnEnter(data); // 进入新状态，传递数据
            }
            else
            {
                LogManager.LogWarning($"状态 '{newStateName}' 未找到. 切换状态失败."); // 输出警告，状态未找到
            }

            _isChangingState = false; // 解除锁，允许下一次状态切换
        }

        /// <summary>
        /// 每帧更新当前状态。
        /// </summary>
        public void Update()
        {
            _currentState?.OnUpdate(); // 调用当前状态的更新方法
        }

        /// <summary>
        /// 获取当前状态的名称。
        /// </summary>
        public string GetCurrentState()
        {
            return _currentState?.GetState(); // 返回当前状态的名称
        }
    }

}
