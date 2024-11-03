using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigModule
{
    public class TestConfig : MonoBehaviour
    {
        private void Start()
        {

            // 获取 Sheet1Cfg 的配置表数据
            var sheet1Cfg = ConfigManager.Instance.GetConfig<Sheet1Cfg>();
            if (sheet1Cfg != null)
            {
                foreach (var item in sheet1Cfg.cfg)
                {
                    LogManager.LogInfo($"Sheet1 - ID: {item.ID}, Name: {item.Name}, HP: {item.HP}");
                }
            }
        }
    }
}
