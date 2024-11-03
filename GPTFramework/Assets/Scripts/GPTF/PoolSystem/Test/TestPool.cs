/// <summary>
/// TestPool 类用于测试对象池系统，包括对象的获取、回收和清理操作。
/// 该测试脚本会加载一个预制体并将其放入 Unity 对象池进行管理，并模拟从池中取出和归还对象的过程。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 加载并创建一个 Unity 对象池来管理 Capsule 预制体对象。
/// 2. 支持从对象池中获取对象，并模拟对象使用后的回收。
/// 3. 支持通过按键触发对象池的清理操作，根据条件清理无效对象。
/// 
/// 注意事项：
/// 1. 从对象池中取出的对象不能直接销毁！必须通过 Recycle() 方法将对象归还到对象池。
/// 2. 对象池的清理操作会根据传入的条件函数清理符合条件的对象。一般可以根据对象是否为 null 或者其他条件来清理。
/// 3. 调用 Recycle() 时，将对象的状态重置并回收到池中，否则对象会被销毁。
/// </remarks>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResourceModule;
using PoolModule;

public class TestPool : MonoBehaviour
{
    GameObject eapsulePrefab;

    void Start()
    {
        //// 创建一个通用对象池，用于管理Int类型对象
        //GenericObjectPoolFactory.Instance.CreatePool<int>(10, 50);

        // 加载Capsule预制体
        eapsulePrefab = ResourceManager.Instance.LoadResource<GameObject>("Prefabs/Objects/Capsule.prefab");

        // 创建一个Unity对象池，用于管理Capsule预制体
        UnityObjectPoolFactory.Instance.CreatePool(eapsulePrefab, "CapsulePool", 5, 10);
    }

    void Update()
    {
        // 从对象池中获取对象，并将其放置到随机位置
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var obj = UnityObjectPoolFactory.Instance.GetObject("CapsulePool");
            if (obj == null)
                return;

            obj.transform.position = new Vector3(RandomRangeFloat(), RandomRangeFloat(), RandomRangeFloat());

            // 模拟对象使用完后3秒后归还到对象池
            StartCoroutine(RecycleObject(obj));
        }

        // 清理对象池：根据条件（例如，obj == null）来清理
        if (Input.GetKeyDown(KeyCode.C))
        {
            UnityObjectPoolFactory.Instance.CleanupPool("CapsulePool", obj => obj == true);
        }
    }

    IEnumerator RecycleObject(GameObject obj)
    {
        // 等待3秒后，将对象归还到对象池
        yield return new WaitForSeconds(3);

        UnityObjectPoolFactory.Instance.RecycleObject("CapsulePool", obj);

        // !!!不能直接销毁对象池中取出的对象！必须要归还对象池
        // Destroy(obj);  // 销毁操作应通过对象池管理
    }

    // 生成一个-10到10之间的随机浮点数
    private float RandomRangeFloat()
    {
        return Random.Range(-10, 10);
    }
}
