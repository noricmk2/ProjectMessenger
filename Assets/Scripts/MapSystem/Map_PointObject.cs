using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Map_PointObject : MonoBehaviour
{
    public int pointID;
    public Image pointIconImage;
    public TextMeshProUGUI pointName;
    public RectTransform rectTransform;

    public RectTransform highlightTransform;

    [NonSerialized]
    public DataManager.MapData_Point pointData;

    private void Reset()
    {
        pointID = transform.GetSiblingIndex();
    }

    private void OnValidate()
    {
        pointID = transform.GetSiblingIndex();

    }

    public void SetPointData(DataManager.MapData_Point data)
    {
        pointData = data;
        pointID = pointData.ID;
        pointName.text = TextManager.GetSystemText(pointData.Name_Short);
        rectTransform.anchoredPosition = pointData.Position;
        highlightTransform.gameObject.SetActive(false);
    }

    public void OnClickPoint()
    {

        //MapManager.Instance.SelectPoint(id, );
        Debug.Log("Select Point : " + pointName.text);
        //MapManager.Instance.SelectPoint(id, pointList[id].anchoredPosition);

        IngameScene.instance.ingameObject.MapWindow.mapObject.SelectPoint(this);
    }

    [ContextMenu("Vali")]
    public void ButtonValidating()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(new UnityEngine.Events.UnityAction(OnClickPoint));
    }

    public void Highlight(bool highlight)
    {
        highlightTransform.gameObject.SetActive(highlight);

        if (highlight)
        {
            highlightTransform.sizeDelta = Vector2.one * 60;
            highlightTransform.DOSizeDelta(Vector2.one * 25f, 0.1f);
        }
    }
}
