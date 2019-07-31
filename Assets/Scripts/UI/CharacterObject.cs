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
    public bool CharacterActivate { get; set; }

    public void Init(int characterID, Transform parent)
    {
        transform.Init(parent);
        CurrentCharacterData = DataManager.Instance.GetCharacterData(characterID);
        CharacterActivate = false;
        Bubble.Init(this);
        if(CurrentCharacterData.CharacterType != eCharacter.NIKA)
            Bubble.transform.localScale = Vector3.zero;
        CharacterAnimation.Init(CurrentCharacterData);
    }

    public void SetFocus(bool focus, System.Action endAction = null)
    {
        Bubble.transform.DOKill();
        if (focus)
        {
            CharacterAnimation.SetColor(Color.white);
            if (CurrentCharacterData.CharacterType != eCharacter.NIKA)
            {
                Bubble.gameObject.SetActive_Check(true);
                Bubble.ExpandText.Reset();
                Bubble.transform.DOScale(Vector3.one, ConstValue.BUBBLE_ANIMATION_TIME).SetEase(Ease.InOutBack).OnComplete(
                    () =>
                    {
                        if (endAction != null)
                            endAction();
                    });
            }
            else
                if (endAction != null)
                endAction();
        }
        else
        {
            CharacterAnimation.SetColor(ColorPalette.CHARACTER_HILIGHT_COLOR);
            if (CurrentCharacterData.CharacterType != eCharacter.NIKA)
            {
                Bubble.transform.DOScale(Vector3.zero, ConstValue.BUBBLE_ANIMATION_TIME).SetEase(Ease.InOutBack).OnComplete(() => Bubble.gameObject.SetActive_Check(false));
                Bubble.gameObject.SetActive_Check(false);
            }
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
        transform.SetParent(ObjectFactory.Instance.ChatPoolParent);
    }
}
