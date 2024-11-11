using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIModule;
using DG.Tweening;

public class SelectWindow_1 : UIBasePanel
{
    public Button closeBtn;
    protected override void Initialize(ScreenParam param)
    {
        closeBtn.onClick.AddListener(CloseBtn);
    }

    public override string GetPanelName()
    {
        return UIDefine.SelectWindow_1;
    }

    public override void Refresh()
    {
        transform.localScale = Vector3.one;
    }

    public override IEnumerator PlayOpenAnimationCoroutine()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(0, 1.5f).From());

        yield return sequence.WaitForCompletion();
    }


    public override IEnumerator PlayCloseAnimationCoroutine()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(0, 1.5f));

        yield return sequence.WaitForCompletion();
    }

    public void CloseBtn()
    {
        UIManager.Instance.ClosePanel(GetPanelName());
    }
}
