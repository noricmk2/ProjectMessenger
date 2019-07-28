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

    public void SetLetterObject(DataManager.LetterData letterData)
    {
        DataManager.CharacterData fromData = DataManager.Instance.GetCharacterData(letterData.From);

        //fromText.text = fromData.name
        //destinationText.text = letterData.Destination;

        eCharacter characterType = DataManager.Instance.GetCharacterData(letterData.From).CharacterType;

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
}
