using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Window_Map : WindowBase
{
    [Header("Object")]
    public Transform mapObjectParent;
    public MapObject mapObject;
    public Image logoImage;

    [Header("Icon")]
    public Image iconMapImage;
    public Image iconPinImage;
    public Image iconCircleImage;

    [Header("FuelObject")]
    public Image fuelGauge;

    [Header("LetterList")]
    public Transform LetterObjectsParent;
    public List<LetterListObject> letterObjectList;
    //p
    //[Header("Map")]
    public Transform markerParent;
    //public MarkerObject 

    public void OpenMap()
    {
        List<UserInfo.ItemData> bagItemList = UserInfo.Instance.GetBagItemList();

        letterObjectList = new List<LetterListObject>();
        Debug.Log(bagItemList.Count);
        for (int i = 0; i < bagItemList.Count; i++)
        {
            if (bagItemList[i].Type == MSUtil.eItemType.Letter)
            {
                LetterListObject letterObject = ObjectFactory.Instance.ActivateObject<LetterListObject>();
                letterObject.transform.SetParent(LetterObjectsParent);
                letterObject.transform.localScale = Vector3.one;
                letterObject.SetLetterObject(DataManager.Instance.GetLetterData(bagItemList[i].ID));
            }
        }

        mapObject = ObjectFactory.Instance.ActivateObject<MapObject>();
        mapObject.transform.SetParent(mapObjectParent);
        mapObject.transform.localScale = Vector3.one;
        mapObject.SetData();

        //iconCircleImage.rectTransform.sizeDelta = Vector2.zero;
        //iconPinImage.rectTransform.anchoredPosition = new Vector2(10, 60);
        //logoImage.gameObject.SetActive(true);



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

    public bool startSelected = false;

    public Vector2 startPoint;

    public void SelectPoint(int id, Vector2 position)
    {
        if (startSelected)
        {
            NodeManager.Instance.CalculatingStart(startPoint, position);
            startPoint = position;
        }
        else
        {
            NodeManager.Instance.roadPositions = new List<Vector3>();
            startSelected = true;
            startPoint = position;
        }
    }
}
