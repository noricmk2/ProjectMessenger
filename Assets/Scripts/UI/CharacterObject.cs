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
    private bool m_IsEvent;

    public void Init(int characterID, Transform parent = null)
    {
        m_IsEvent = false;
        CharacterAnimation.gameObject.SetActive_Check(true);
        CurrentCharacterData = DataManager.Instance.GetCharacterData(characterID);
        CharacterActivate = false;
        if (CurrentCharacterData.CharacterType != eCharacter.NIKA)
        {
            transform.Init(parent);
            Bubble.Init(this);
            Bubble.BubbleBG.sprite = ObjectFactory.Instance.GetUISprite(ConstValue.BUBBLE_SPRITE_NAME_1);
            BubbleParent.anchoredPosition = ConstValue.BUBBLE_DEFAULT_POS;
            Bubble.transform.localScale = Vector3.zero;
        }
        else
            Bubble.Init(this, true);
        CharacterAnimation.Init(CurrentCharacterData);
    }

    public void Init(int characterID, Transform parent, Vector3 bubblePos)
    {
        Init(characterID, parent);
        BubbleParent.anchoredPosition = new Vector2(bubblePos.x, bubblePos.y);
        var resName = bubblePos.z == -1 ? ConstValue.BUBBLE_SPRITE_NAME_2 : ConstValue.BUBBLE_SPRITE_NAME_1;
        Bubble.BubbleBG.sprite = ObjectFactory.Instance.GetUISprite(resName);
    }

    public void InitForEvent(int characterID, Transform parent, Vector3 bubblePos)
    {
        m_IsEvent = true;
        transform.Init(parent);
        CharacterAnimation.gameObject.SetActive_Check(false);
        BubbleParent.anchoredPosition = new Vector2(bubblePos.x, bubblePos.y);
        CurrentCharacterData = DataManager.Instance.GetCharacterData(characterID);
        CharacterActivate = true;
        Bubble.Init(this, true);
        var resName = bubblePos.z == -1 ? ConstValue.BUBBLE_SPRITE_NAME_2 : ConstValue.BUBBLE_SPRITE_NAME_1;
        Bubble.BubbleBG.sprite = ObjectFactory.Instance.GetUISprite(resName);
    }

    public void SetFocus(bool focus, bool bubbleEnable = true, System.Action endAction = null)
    {
        Bubble.transform.DOKill();
        if (focus)
        {
            Bubble.SetActive(bubbleEnable);
            CharacterAnimation.SetColor(Color.white);
            if (m_IsEvent || CurrentCharacterData.CharacterType != eCharacter.NIKA)
            {
                Bubble.transform.localScale = Vector3.zero;
                Bubble.ExpandText.ResetText();
                Bubble.transform.DOScale(Vector3.one, ConstValue.BUBBLE_ANIMATION_TIME).SetEase(Ease.InSine).OnComplete(
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
            if (m_IsEvent || CurrentCharacterData.CharacterType != eCharacter.NIKA)
            {
                Bubble.transform.DOScale(Vector3.zero, ConstValue.BUBBLE_ANIMATION_TIME).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    Bubble.SetActive(false, true);
                });
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
