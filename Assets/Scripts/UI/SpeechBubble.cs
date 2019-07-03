using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MSUtil;
using DG.Tweening;

public class SpeechBubble : MonoBehaviour, IPoolObjectBase
{
    #region Inspector
    public RectTransform BubbleTrans;
    public Image BubbleBG; 
    public ExpandTextOutput Text;
    public Image TextCursor;
    #endregion
    private float APPEAR_TIME = 0.5f;
    private float BLINK_TIME = 0.5f;

    private CharacterObject m_Parent;
    private SpriteAnimation m_ParentAnimation;
    private Coroutine m_CursorCoroutine;

    public void Init(CharacterObject parent)
    {
        BubbleBG.enabled = false;
        m_Parent = parent;
        Text.Reset();
        TextCursor.DOKill();
        TextCursor.gameObject.SetActive_Check(false);
    }

    public void Init(SpriteAnimation parentAnimation)
    {
        m_ParentAnimation = parentAnimation;
        Text.Reset();
    }

    public void SetTextData(DataManager.StoryTextData data)
    {
        SetCursor(false);
        Text.SetText(TextManager.GetStoryText(data.ID), data.GetEventTagDic(), TextEvent, () => SetCursor(true));
        Text.PlayText();
    }

    public IEnumerator TextEvent(DataManager.TextEventData data)
    {
        var targetSpriteAnim = m_Parent != null ? m_Parent.CharacterAnimation : m_ParentAnimation;

        switch (data.Tag)
        {
            case eTextEventTag.APR:
                targetSpriteAnim.TargetImage.color = new Color(0, 0, 0, 0);
                targetSpriteAnim.TargetImage.gameObject.SetActive_Check(true);
                var sequence = DOTween.Sequence();
                sequence.Append(targetSpriteAnim.TargetImage.DOColor(ColorPalette.FADE_OUT_BLACK, APPEAR_TIME));
                sequence.Append(targetSpriteAnim.TargetImage.DOColor(Color.white, APPEAR_TIME));
                sequence.Play();
                while (sequence.IsActive() && !sequence.IsComplete())
                {
                    yield return null;
                }
                BubbleBG.enabled = true;
                targetSpriteAnim.SetAnimation(eCharacterState.IDLE, true);
                MSLog.Log("appear");
                break;
            case eTextEventTag.DPR:
                MSLog.Log("disappear");
                break;
            case eTextEventTag.HL:
                MSLog.Log("hilight");
                break;
            case eTextEventTag.CNG:
                var animType = Func.GetEnum<eCharacterState>(data.Value);
                targetSpriteAnim.SetAnimation(animType, animType == eCharacterState.IDLE ? true : false);
                MSLog.Log("change");
                break;
            case eTextEventTag.FONTBIG:
                break;
            case eTextEventTag.FONTSML:
                break;
            case eTextEventTag.FONTCOL:
                break;
        }
    }

    public void SetCursor(bool active)
    {
        TextCursor.gameObject.SetActive_Check(active);
        if (m_CursorCoroutine != null)
            StopCoroutine(m_CursorCoroutine);
        if (active)
            m_CursorCoroutine = StartCoroutine(CursorBlink_C());
    }

    IEnumerator CursorBlink_C()
    {
        while (true)
        {
            TextCursor.gameObject.SetActive(!TextCursor.gameObject.activeSelf);
            yield return new WaitForSeconds(BLINK_TIME);
        }
    }

    public void PushAction()
    {
        gameObject.SetActive(false);
    }

    public void PopAction()
    {
        gameObject.SetActive(true);
    }
}
