using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIModule;

public class Panel_2 : UIBasePanel
{
    public Button closeBtn;
    protected override void Initialize(ScreenParam param)
    {
        closeBtn.onClick.AddListener(CloseBtn);
    }

    public override string GetPanelName()
    {
        return UIDefine.Panel_2;
    }


    public void CloseBtn()
    {
        UIManager.Instance.ClosePanel(GetPanelName());
    }

}
