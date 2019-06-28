using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterObject : MonoBehaviour, IPoolObjectBase
{
    #region Inspector
    public Image CharacterImage;
    public RectTransform BubbleParent;
    public SpeechBubble Bubble;
    #endregion
    public DataManager.CharacterData CurrentCharacterData { get; private set; }

    public void Init(int characterID)
    {
        CurrentCharacterData = DataManager.Instance.GetCharacterData(characterID);
    }

    public void SetFocus(bool focusOn)
    {

    }

    public void PopAction()
    {
        gameObject.SetActive(true);
    }

    public void PushAction()
    {
        gameObject.SetActive(false);
    }
}
