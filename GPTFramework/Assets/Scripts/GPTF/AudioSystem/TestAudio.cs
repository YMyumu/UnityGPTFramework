using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AudioModule;
using ResourceModule;
using PoolModule;

public class TestAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 将委托绑定为通过 Resources.Load 加载 GameObject 预制件
        UnityObjectPoolFactory.Instance.LoadFuncDelegate = (string itemName) =>
        {
            // 从 Resources 文件夹加载指定名称的 GameObject 预制件
            return ResourceManager.Instance.LoadResource<GameObject>(ResourceManager.Instance.GetResourcePathByType<GameObject>(itemName));
        };
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.Instance.PlayAudio("欢迎使用山河智能旋挖钻机XR展示系统");
        }

        if (Input.GetMouseButtonDown(1))
        {
            AudioManager.Instance.PlayAudio("Item purchase 26");
        }

    }
}
