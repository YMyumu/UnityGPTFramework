using System.Collections;
using System.Collections.Generic;
using UIModule;
using UnityEngine;
using UnityEngine.UI;
public class Hint_2 : UIBasePanel
{
    public Button closeBtn;
    public Button openInfoWindow_1Btn, openInfoWindow_2Btn, openPanel_2Btn, openPanel_3Btn, openPanel_4Btn, openHint_1Btn;

    protected override void Initialize(ScreenParam param)
    {
        closeBtn.onClick.AddListener(CloseBtn);

        openInfoWindow_1Btn.onClick.AddListener(OpenInfoWindow_1Btn);
        openInfoWindow_2Btn.onClick.AddListener(OpenInfoWindow_2Btn);
        openPanel_2Btn.onClick.AddListener(OpenPanel_2Btn);
        openPanel_3Btn.onClick.AddListener(OpenPanel_3Btn);
        openPanel_4Btn.onClick.AddListener(OpenPanel_4Btn);
        openHint_1Btn.onClick.AddListener(OpenHint_1Btn);

    }

    public override string GetPanelName()
    {
        return UIDefine.Hint_2;
    }


    public void CloseBtn()
    {
        UIManager.Instance.ClosePanel(GetPanelName());
    }

    public void OpenInfoWindow_1Btn()
    {
        UIManager.Instance.OpenPanel(UIDefine.InfoWindow_1);
    }

    public void OpenInfoWindow_2Btn()
    {
        UIManager.Instance.OpenPanel(UIDefine.InfoWindow_2);
    }

    public void OpenPanel_2Btn()
    {
        UIManager.Instance.OpenPanel(UIDefine.Panel_2);
    }

    public void OpenPanel_3Btn()
    {
        UIManager.Instance.OpenPanel(UIDefine.Panel_3);
    }
    
    public void OpenPanel_4Btn()
    {
        UIManager.Instance.OpenPanel(UIDefine.Panel_4);
    }

    public void OpenHint_1Btn()
    {
        UIManager.Instance.OpenPanel(UIDefine.Hint_1);
    }

}
