using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MSUtil;

public class LetterListObject : MonoBehaviour, IPoolObjectBase
{
    public TextMeshProUGUI destinationText;
    public TextMeshProUGUI fromText;

    public Image portraitImage;

    private DataManager.LetterData letterData = null;
    private DataManager.CharacterData fromData = null;
    private DataManager.MapData_Point mapData = null;
    public Map_PointObject pointObject = null;

    public void SetLetterObject(DataManager.LetterData letter)
    {
        letterData = letter;
        fromData = DataManager.Instance.GetCharacterData(letterData.From);
        mapData = DataManager.Instance.GetMapData_Point(letterData.Destination);

        fromText.text = fromData.GetCharacterName();
        destinationText.text = TextManager.GetSystemText(mapData.Name);

        eCharacter characterType = DataManager.Instance.GetCharacterData(letterData.From).CharacterType;

        pointObject = IngameScene.instance.ingameObject.MapWindow.mapObject.GetPointObject(mapData);
        //var isPortrait = CharacterType == MSUtil.eCharacter.NIKA ? "portrait" : "stand";
        //var resourceName = CharacterType.ToString().ToLower() + "_" + isPortrait + "_" + state.ToString().ToLower() + "_" + i;
        //portraitImage.sprite = ObjectFactory.Instance.GetCharacterSprite(characterType, resourceName);
    }

    public void PopAction()
    {
        gameObject.SetActive(true);
    }

    public void PushAction()
    {
        gameObject.SetActive(false);
        transform.SetParent(null);
    }

    public void OnClickLetter()
    {
        Debug.Log("Letter : " + TextManager.GetSystemText(mapData.Name));

        IngameScene.instance.ingameObject.MapWindow.mapObject.HighlightPointObject(mapData);
    }
}
