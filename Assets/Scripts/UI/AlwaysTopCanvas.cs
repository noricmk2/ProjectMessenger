using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using MSUtil;

public class AlwaysTopCanvas : Singleton<AlwaysTopCanvas>
{
    #region Inspector
    public Image FadeImage;
    #endregion
    public bool IsOnFade { get; set; }

    private void Awake()
    {
        FadeImage.color = new Color(0, 0, 0, 0);
        FadeImage.gameObject.SetActive_Check(false);
    }

    public void SetFadeAnimation(float time, bool reset, eTransitionType effectType, Action fadeEndAction = null)
    {
        IsOnFade = true;
        switch (effectType)
        {
            case eTransitionType.CIRCLE:
                UICamera.Instance.TransEffect.SetTransitionEffect(true, reset, effectType, 1, ()=>
                {
                    if (fadeEndAction != null)
                        fadeEndAction();
                });
                break;
            case eTransitionType.NORMAL:
                FadeImage.gameObject.SetActive_Check(true);
                var sequence = DOTween.Sequence();
                sequence.Append(FadeImage.DOColor(ColorPalette.FADE_OUT_BLACK, time)).AppendCallback(() =>
                {
                    if (fadeEndAction != null)
                        fadeEndAction();
                });
                if(reset)
                    sequence.Append(FadeImage.DOColor(ColorPalette.FADE_IN_BLACK, time));
                sequence.OnComplete(() =>
                {
                    IsOnFade = false;
                    FadeImage.gameObject.SetActive_Check(false);
                });
                break;
        }
    }
}
