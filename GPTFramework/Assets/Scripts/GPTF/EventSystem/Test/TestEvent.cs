/*
移除事件：确保在对象销毁或不再需要响应事件时，移除事件的注册，防止内存泄漏。

优先级：在注册事件时，你可以指定优先级。优先级数值越高，回调函数越早被执行。
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EventModule;


public class TestEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 注册无参事件
        EventManager.Instance.Register(EventDefine.TEST_EVENT, TEST_EVENT);
        EventManager.Instance.Register(EventDefine.PLAYER_DIED, PLAYER_DIED, EventPriority.GAME_LOGIC);


        // 注册双参事件
        EventManager.Instance.Register<int, int>(EventDefine.ITEM_COLLECTED, ITEM_COLLECTED);



    }


    private void OnDestroy()
    {
        // 在对象销毁时移除事件注册，避免内存泄漏
        EventManager.Instance.Remove(EventDefine.TEST_EVENT, TEST_EVENT);
        EventManager.Instance.Remove(EventDefine.PLAYER_DIED, PLAYER_DIED);


        EventManager.Instance.Remove<int, int>(EventDefine.ITEM_COLLECTED, ITEM_COLLECTED);

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 派发无参事件
            EventManager.Instance.Dispatch(EventDefine.TEST_EVENT);

            // 派发双参事件
            EventManager.Instance.Dispatch<int, int>(EventDefine.ITEM_COLLECTED, 1001, 20);

            EventManager.Instance.Dispatch(EventDefine.PLAYER_DIED);
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            // 清除所有注册的事件
            EventManager.Instance.Clear();
        }

    }


    // 测试
    private void TEST_EVENT()
    {
        LogManager.LogInfo("测试事件调用成功");
    }


    // 得到物品
    private void ITEM_COLLECTED(int ID, int Num)
    {
        LogManager.LogInfo("物品ID:" + ID + "  " + "数量:" + Num);
    }

    // 角色死亡
    private void PLAYER_DIED()
    {
        LogManager.LogInfo("死亡");
    }


}
