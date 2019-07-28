using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Window_Map : WindowBase
{
    public static Window_Map instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Object")]
    public GameObject mapObject;
    public Image logoImage;

    [Header("Icon")]
    public Image iconMapImage;
    public Image iconPinImage;
    public Image iconCircleImage;

    //[Header("LetterList")]
    //p

    public Transform LetterObjectsParent;
    //[Header("Map")]
    public Transform markerParent;
    //public MarkerObject 

    public void OpenMap()
    {
        iconCircleImage.rectTransform.sizeDelta = Vector2.zero;
        iconPinImage.rectTransform.anchoredPosition = new Vector2(10, 60);
        logoImage.gameObject.SetActive(true);

        //Sequence iconSeq = DOTween.Sequence();
        //iconSeq.Append(logoImage.rectTransform.DOAnchorMin(Vector2.zero, 0.5f).SetEase(Ease.OutQuint));
        //iconSeq.Join(logoImage.rectTransform.DOAnchorMax(Vector2.one, 0.5f).SetEase(Ease.OutQuint));
        //iconSeq.Join(logoImage.rectTransform.DOSizeDelta(Vector2.zero, 0.5f).SetEase(Ease.OutQuint));
        //iconSeq.Join(logoImage.rectTransform.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutQuint));

        //iconSeq.Append(Window_Tablet.instance.tabletTransform.DOSizeDelta(new Vector2(2560f, 1440f), 0.5f));
        //iconSeq.Join(Window_Tablet.instance.handImage.DOFade(0, 0.5f));
        //iconSeq.AppendCallback(delegate ()
        //{
        //    Window_Tablet.instance.appStartFadeTransform.gameObject.SetActive(false);
        //});
        ////iconSeq.AppendInterval(0.2f);
        //iconSeq.Append(iconMapImage.rectTransform.DOSizeDelta(Vector2.one * 100, 0.2f));
        //iconSeq.Join(iconMapImage.DOFade(1, 0.2f));
        //iconSeq.AppendInterval(0.1f);
        //iconSeq.Append(iconPinImage.rectTransform.DOAnchorPosY(30f, 0.2f));
        //iconSeq.Join(iconPinImage.DOFade(1, 0.2f));
        //iconSeq.Append(iconCircleImage.rectTransform.DOSizeDelta(new Vector2(18, 8), 0.2f));
        //iconSeq.Join(iconCircleImage.DOFade(1, 0.2f));
        //iconSeq.AppendCallback(delegate ()
        //{
        //    mapObject.SetActive(true);
        //});
        //iconSeq.AppendInterval(2f);
        //iconSeq.Append(iconMapImage.DOFade(0, 0.5f));
        //iconSeq.Join(iconPinImage.DOFade(0, 0.5f));
        //iconSeq.Join(iconCircleImage.DOFade(0, 0.5f));
        //iconSeq.Join(logoImage.DOFade(0, 0.5f));
        //iconSeq.Play();
        //iconSeq.OnComplete(delegate ()
        //{
        //    logoImage.gameObject.SetActive(false);
        //});
    }

    public void OnClickCheckPin()
    {
        //출발지, 경유지, 도착지 선택
        NodeManager.Instance.DisplayNodes();
    }

    public void SelectPoint(int id)
    {

    }
}
