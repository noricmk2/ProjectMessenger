using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MSUtil;

public class Map_PointObject : MonoBehaviour
{
    public int pointID;
    public Image pointIconImage;
    public TextMeshProUGUI pointName;
    public RectTransform rectTransform;

    public RectTransform highlightTransform;

    [Header("BubbleObject")]
    public GameObject bubbleObject;
    public Image bubbleIcon;
    public GameObject bubblePortraitObject;
    public Image bubblePortraitImage;

    [NonSerialized]
    public DataManager.MapData_Point pointData;

    [NonSerialized]
    public DataManager.LetterData letterData = null;

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
        letterData = null;
        bubbleObject.SetActive(false);
    }

    public void SetPointLetterData(DataManager.LetterData data)
    {
        letterData = data;
        bubbleObject.SetActive(true);
        

        switch (letterData.LetterType)
        {
            case eLetterType.Event:
                bubbleIcon.gameObject.SetActive(false);
                bubblePortraitObject.SetActive(true);
                eCharacter characterType = DataManager.Instance.GetCharacterData(letterData.From).CharacterType;
                //ObjectFactory.Instance.GetCharacterSprite(characterType, );
                bubblePortraitImage.sprite = null;
                break;
            case eLetterType.Junk:
                bubbleIcon.gameObject.SetActive(true);
                bubblePortraitObject.SetActive(false);
                bubblePortraitImage.sprite = null;
                break;
        }
    }

    public void OnClickPoint()
    {
        Debug.Log("Select Point : " + pointName.text);
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
