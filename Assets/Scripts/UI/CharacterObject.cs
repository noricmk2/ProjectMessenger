using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MSUtil;

public class CharacterObject : MonoBehaviour, IPoolObjectBase
{
    #region Inspector
    public SpriteAnimation CharacterAnimation;
    public RectTransform BubbleParent;
    public SpeechBubble Bubble;
    #endregion
    public DataManager.CharacterData CurrentCharacterData { get; private set; }

    public void Init(int characterID, Transform parent)
    {
        transform.Init(parent);
        CurrentCharacterData = DataManager.Instance.GetCharacterData(characterID);
        Bubble.Init(this);
        CharacterAnimation.Init(CurrentCharacterData);
    }

    public void SetFocus(bool focusOn)
    {

    }

    public void Release()
    {
        Bubble.Release();
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
