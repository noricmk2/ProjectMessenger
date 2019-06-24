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
        FadeImage.DOColor(ColorPalette.FADE_OUT_BLACK, time).OnComplete(delegate ()
        {
            FadeImage.DOColor(ColorPalette.FADE_IN_BLACK, time).OnComplete(delegate ()
            {
                if (fadeEndAction != null)
                    fadeEndAction();
                FadeImage.gameObject.SetActive_Check(false);
            });
        });
    }
}
