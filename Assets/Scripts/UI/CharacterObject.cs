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
            Bubble.BubbleBG.sprite = ObjectFactory.Instance.GetUISprite(ConstValue.DEFAULT_BUBBLE_SPRITE_NAME);
            SetBubbleRect(ConstValue.DEFAULT_BUBBLE_SPRITE_NAME);
            BubbleParent.anchoredPosition = ConstValue.BUBBLE_DEFAULT_POS;
            Bubble.transform.localScale = Vector3.zero;
        }
        else
            Bubble.Init(this, true);
        CharacterAnimation.Init(CurrentCharacterData);
    }

    public void Init(int characterID, Transform parent, Vector2 bubblePos, string bubbleRes)
    {
        Init(characterID, parent);
        BubbleParent.anchoredPosition = new Vector2(bubblePos.x, bubblePos.y);
        Bubble.BubbleBG.sprite = ObjectFactory.Instance.GetUISprite(bubbleRes);
        SetBubbleRect(bubbleRes);
    }

    public void InitForBubbleOnly(int characterID, Transform parent, Vector2 bubblePos, string bubbleRes)
    {
        m_IsEvent = true;
        transform.Init(parent);
        CharacterAnimation.gameObject.SetActive_Check(false);
        BubbleParent.anchoredPosition = new Vector2(bubblePos.x, bubblePos.y);
        CurrentCharacterData = DataManager.Instance.GetCharacterData(characterID);
        CharacterActivate = true;
        Bubble.Init(this, true);
        Bubble.BubbleBG.sprite = ObjectFactory.Instance.GetUISprite(bubbleRes);
        SetBubbleRect(bubbleRes);
    }

    private void SetBubbleRect(string bubbleName)
    {
        var rectTrans = Bubble.ExpandText.transform as RectTransform;
        var bubbleTrans = Bubble.transform as RectTransform;
        bubbleTrans.sizeDelta = ConstValue.BUBBLE_ORG_SIZE;
        if (bubbleName == ConstValue.OUTSIDE_BUBBLE_SPRITE_NAME)
        {
            rectTrans.anchoredPosition = ConstValue.LINE_OUTSIDE_RECT.position;
            rectTrans.sizeDelta = ConstValue.LINE_OUTSIDE_RECT.size;
        }
        else if (bubbleName == ConstValue.IMPACT_BUBBLE_SPRITE_NAME)
        {
            bubbleTrans.sizeDelta = ConstValue.BUBBLE_IMPACT_SIZE;
            rectTrans.anchoredPosition = ConstValue.LINE_IMPACT_RECT.position;
            rectTrans.sizeDelta = ConstValue.LINE_IMPACT_RECT.size;
        }
        else
        {
            rectTrans.anchoredPosition = ConstValue.LINE_ORG_RECT.position;
            rectTrans.sizeDelta = ConstValue.LINE_ORG_RECT.size;
        }
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
