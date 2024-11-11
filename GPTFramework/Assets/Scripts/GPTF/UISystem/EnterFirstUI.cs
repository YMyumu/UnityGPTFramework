using System;
using UnityEngine;

namespace UIModule
{
    public class EnterFirstUI : MonoBehaviour
    {
        private void Start()
        {
            UIManager.Instance.OpenPanel(UIDefine.Panel_1);
            
        }
    }

}