using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UIModule;
using EventModule;
using SceneLoaderModule;

public class LoadingUIPanel : UIBasePanel
{
    [SerializeField] private Text TitleText;
    [SerializeField] private Text LodingSliderVelueText; // 显示进度百分比
    [SerializeField] private Slider progressBar;

    public override string GetPanelName()
    {
        return UIDefine.LoadingUIPanel;
    }

    protected override void Initialize(ScreenParam param)
    {
        EventManager.Instance.Register<float>(EventDefine.ON_SCENE_LOAD_PROGRESS_FOR_UIPANEL, UpdateProgress);
        EventManager.Instance.Register(EventDefine.AFTER_SCENE_LOAD, OnSceneLoaded);
    }


    private void OnDestroy()
    {
        EventManager.Instance.Remove<float>(EventDefine.ON_SCENE_LOAD_PROGRESS_FOR_UIPANEL, UpdateProgress);
        EventManager.Instance.Remove(EventDefine.AFTER_SCENE_LOAD, OnSceneLoaded);
    }


    public override void Refresh()
    {
        progressBar.value = 0f;
        LodingSliderVelueText.text = "0%";
    }

    public override IEnumerator PlayOpenAnimationCoroutine()
    {
        return base.PlayOpenAnimationCoroutine();
    }

    public override IEnumerator PlayCloseAnimationCoroutine()
    {
        return base.PlayCloseAnimationCoroutine();
    }

    // 更新进度条并显示百分比
    private void UpdateProgress(float progress)
    {
        progressBar.value = progress;
        LodingSliderVelueText.text = Mathf.RoundToInt(progress * 100) + "%"; // 显示百分比

        // 如果进度达到100%，通知场景管理器准备激活场景
        if (progress >= 1.0f)
        {
            SceneLoader.Instance.OnSceneUIReady();
        }
    }

    // 场景加载完成后隐藏加载界面
    private void OnSceneLoaded()
    {
        UIManager.Instance.ClosePanel(GetPanelName());
    }
}
