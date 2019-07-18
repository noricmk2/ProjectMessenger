using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MSUtil;
using DG.Tweening;

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
        Bubble.transform.localScale = Vector3.zero;
        Bubble.Init(this);
        CharacterAnimation.Init(CurrentCharacterData);
    }

    public void SetFocus(bool focusOn)
    {
        Bubble.transform.DOKill();
        if (focusOn)
        {
            CharacterAnimation.SetColor(Color.white);
            Bubble.gameObject.SetActive_Check(true);
            Bubble.transform.DOScale(Vector3.one, ConstValue.BUBBLE_ANIMATION_TIME).SetEase(Ease.InOutBack);
        }
        else
        {
            CharacterAnimation.SetColor(ColorPalette.CHARACTER_HILIGHT_COLOR);
            Bubble.transform.DOScale(Vector3.zero, ConstValue.BUBBLE_ANIMATION_TIME).SetEase(Ease.InOutBack).OnComplete(() => Bubble.gameObject.SetActive_Check(false));
        }
    }

    public void Release()
    {
        Bubble.Release();
        SetFocus(false);
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
