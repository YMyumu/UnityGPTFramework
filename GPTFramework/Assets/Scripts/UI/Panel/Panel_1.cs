using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIModule;
using LocalizationModule;
using DG.Tweening;
using SceneLoaderModule;
public class Panel_1 : UIBasePanel
{
    public Button openInfoWindow_1Btn, openInfoWindow_2Btn, openPanel_2Btn, 
        openPanel_3Btn, openHint_1Btn, openHint_2Btn, switchLanguageBtn_Chinese_Simplified, switchLanguageBtn_English,
        loaderTestScene_1Btn;


    protected override void Initialize(ScreenParam param)
    {
        openInfoWindow_1Btn.onClick.AddListener(OpenInfoWindow_1Btn);
        openInfoWindow_2Btn.onClick.AddListener(OpenInfoWindow_2Btn);
        openPanel_2Btn.onClick.AddListener(OpenPanel_2Btn);
        openPanel_3Btn.onClick.AddListener(OpenPanel_3Btn);
        openHint_1Btn.onClick.AddListener(OpenHint_1Btn);
        openHint_2Btn.onClick.AddListener(OpenHint_2Btn);

        switchLanguageBtn_Chinese_Simplified.onClick.AddListener(SwitchLanguageBtn_Chinese_Simplified);
        switchLanguageBtn_English.onClick.AddListener(SwitchLanguageBtn_English);

        loaderTestScene_1Btn.onClick.AddListener(LoaderTestScene_1Btn);
    }

    public override void Refresh()
    {
        base.Refresh();
    }

    public override void Close()
    {
        base.Close();
    }

    public override string GetPanelName()
    {
        return UIDefine.Panel_1;
    }


    public override IEnumerator PlayOpenAnimationCoroutine()
    {
        Sequence sequence = DOTween.Sequence();


        sequence.Append(openInfoWindow_1Btn.transform.DOScale(0, 1.5f).From());
        sequence.Append(openInfoWindow_2Btn.transform.DOScale(0, 1.5f).From());
        sequence.Append(openPanel_2Btn.transform.DOScale(0, 1.5f).From());
        sequence.Append(openPanel_3Btn.transform.DOScale(0, 1.5f).From());
        sequence.Join(openHint_1Btn.transform.DOScale(0, 1.5f).From());
        sequence.Join(openHint_2Btn.transform.DOScale(0, 1.5f).From());
        sequence.Play();

        yield return sequence.WaitForCompletion();
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

    public void OpenHint_1Btn()
    {
        UIManager.Instance.OpenPanel(UIDefine.Hint_1);
    }
    public void OpenHint_2Btn()
    {
        UIManager.Instance.OpenPanel(UIDefine.Hint_2);
    }

    public void SwitchLanguageBtn_Chinese_Simplified()
    {
        LocalizationManager.Instance.SwitchLanguage(LanguageDefine.Chinese_Simplified);
    }
    public void SwitchLanguageBtn_English()
    {
        LocalizationManager.Instance.SwitchLanguage(LanguageDefine.English);

    }

    public void LoaderTestScene_1Btn()
    {
        SceneLoader.Instance.LoadSceneAsync("TestScene_1");
    }

}
