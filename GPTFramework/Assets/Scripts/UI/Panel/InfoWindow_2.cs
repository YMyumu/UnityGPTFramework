using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIModule;

public class InfoWindow_2 : UIBasePanel
{
    public Button closeBtn, openPanel_3Btn;
    protected override void Initialize(ScreenParam param)
    {
        closeBtn.onClick.AddListener(CloseBtn);
        openPanel_3Btn.onClick.AddListener(OpenPanel_3Btn);
    }

    public override string GetPanelName()
    {
        return UIDefine.InfoWindow_2;
    }


    public void CloseBtn()
    {
        UIManager.Instance.ClosePanel(GetPanelName());
    }

    public void OpenPanel_3Btn()
    {
        UIManager.Instance.OpenPanel(UIDefine.Panel_3);
    }

}
