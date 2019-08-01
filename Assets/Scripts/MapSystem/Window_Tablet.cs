using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Window_Tablet : WindowBase
{
    public static Window_Tablet instance;

    private void Awake()
    {
        instance = this;
    }
    //public RectTransform logoTransform;
    public RectTransform tabletTransform;
    public Image handImage;
    public RectTransform appStartFadeTransform;
    public List<RectTransform> appGridLayout;

    private bool m_start = false;

    private void Start()
    {
        tabletTransform.anchoredPosition = new Vector2(0, -1000);
    }

    public void OnClickStart()
    {
        if (!m_start)
        {
            tabletTransform.anchoredPosition = new Vector2(0, -1000);
            tabletTransform.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
            m_start = true;
        }
    }

    public void OnClickAppIcon(int id)
    {
        appStartFadeTransform.gameObject.SetActive(true);
        Sequence appStartSeq = DOTween.Sequence();
        //appStartSeq.Append(tabletTransform.DOSizeDelta(new Vector2(2560f, 1440f), 0.5f));
        //appStartSeq.Join(handImage.DOFade(0, 0.5f));

        switch (id)
        {
            case 0:
                //Window_Map.instance.logoImage
                appStartFadeTransform.position = appGridLayout[0].transform.position;
                appStartFadeTransform.sizeDelta = Vector2.zero;
                appStartSeq.Append(appStartFadeTransform.DOAnchorMin(Vector2.zero, 0.5f).SetEase(Ease.OutQuint));
                appStartSeq.Join(appStartFadeTransform.DOAnchorMax(Vector2.one, 0.5f).SetEase(Ease.OutQuint));
                appStartSeq.Join(appStartFadeTransform.DOSizeDelta(Vector2.zero, 0.5f).SetEase(Ease.OutQuint));
                appStartSeq.Join(appStartFadeTransform.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutQuint));
                //Window_Map.instance.logoImage.rectTransform.sizeDelta = Vector2.zero;
                appStartSeq.OnComplete(delegate ()
                {
                    //Window_Map.OpenWindow(eWINDOW.Map,  IngameScene.instance.ingame IngameObject.WindowParent, false);
                });
                break;
            default:
                break;
        }

        appStartSeq.Play();
    }
}
