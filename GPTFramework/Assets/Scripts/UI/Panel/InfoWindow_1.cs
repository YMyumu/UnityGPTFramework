using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIModule;

public class InfoWindow_1 : UIBasePanel
{
    public Button closeBtn, openSelectWindow_1Btn, oponSelectWindow_2Btn;
    protected override void Initialize(ScreenParam param)
    {
        closeBtn.onClick.AddListener(CloseBtn);
        openSelectWindow_1Btn.onClick.AddListener(OpenSelectWindow_1Btn);
        oponSelectWindow_2Btn.onClick.AddListener(OpenSelectWindow_2Btn);
    }

    public override string GetPanelName()
    {
        return UIDefine.InfoWindow_1;
    }


    public void CloseBtn()
    {
        UIManager.Instance.ClosePanel(GetPanelName());
    }

    public void OpenSelectWindow_1Btn()
    {
        UIManager.Instance.OpenPanel(UIDefine.SelectWindow_1);
    }
    public void OpenSelectWindow_2Btn()
    {
        UIManager.Instance.OpenPanel(UIDefine.SelectWindow_2);

    }
}
