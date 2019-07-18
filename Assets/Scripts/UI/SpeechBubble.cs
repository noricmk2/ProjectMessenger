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

    public CharacterObject ParentObject { get; private set; }
    public SpriteAnimation ParentAnimation { get; private set; }
    private Coroutine m_CursorCoroutine;

    public void Init(CharacterObject parent)
    {
        BubbleBG.enabled = false;
        ParentObject = parent;
        Text.Reset();
        TextCursor.DOKill();
        TextCursor.gameObject.SetActive_Check(false);
    }

    public void Init(SpriteAnimation parentAnimation)
    {
        ParentAnimation = parentAnimation;
        Text.Reset();
    }

    public void SetTextData(DataManager.StoryTextData data, ExpandTextOutput.TextEventDelegate textEvent)
    {
        SetCursor(false);
        Text.SetText(this, TextManager.GetStoryText(data.ID), data.GetEventTagDic(), textEvent, () => SetCursor(true));
        Text.PlayText();
    }

    public void SetCursor(bool active)
    {
        TextCursor.gameObject.SetActive_Check(active);
        if (m_CursorCoroutine != null)
            StopCoroutine(m_CursorCoroutine);
        if (active)
            m_CursorCoroutine = StartCoroutine(CursorBlink_C());
    }

    public void Release()
    {
        Text.Reset();
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
