using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using MSUtil;

public class AllwaysTopCanvas : Singleton<AllwaysTopCanvas>
{
    #region Inspector
    public Image FadeImage;
    #endregion

    private void Awake()
    {
        FadeImage.color = new Color(0, 0, 0, 0);
        FadeImage.gameObject.SetActive_Check(false);
    }

    public void SetFadeAnimation(float time, Action fadeEndAction = null)
    {
        FadeImage.gameObject.SetActive_Check(true);
        var sequence = DOTween.Sequence();
        sequence.Append(FadeImage.DOColor(ColorPalette.FADE_OUT_BLACK, time)).AppendCallback(()=>
        {
            if (fadeEndAction != null)
                fadeEndAction();
        });
        sequence.Append(FadeImage.DOColor(ColorPalette.FADE_IN_BLACK, time));
        sequence.OnComplete(() =>
        {
            FadeImage.gameObject.SetActive_Check(false);
        });
    }
}
