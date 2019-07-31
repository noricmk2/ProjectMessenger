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
    public ExpandTextOutput ExpandText;
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
        ExpandText.Reset();
        TextCursor.DOKill();
        TextCursor.gameObject.SetActive_Check(false);
    }

    public void Init(SpriteAnimation parentAnimation)
    {
        ParentAnimation = parentAnimation;
        ExpandText.Reset();
    }

    public void SetTextData(DataManager.StoryTextData data, ExpandTextOutput.TextEventDelegate textTagEvent)
    {
        SetCursor(false);
        ExpandText.SetText(this, TextManager.GetStoryText(data.ID), data.GetEventTagDic(), textTagEvent, () =>
        {
            SetCursor(true);
        });
        ExpandText.PlayText();
    }

    public void SetCursor(bool active)
    {
        TextCursor.gameObject.SetActive_Check(active);
        if (m_CursorCoroutine != null)
            StopCoroutine(m_CursorCoroutine);
        if (active)
            m_CursorCoroutine = StartCoroutine(CursorBlink_C());
    }

    public void CancelTypeWrite()
    {
        ExpandText.CancelTypeWrite();
        transform.DOKill();
        transform.localScale = Vector3.one;
    }

    public void Release()
    {
        ExpandText.Reset();
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
        transform.SetParent(ObjectFactory.Instance.ChatPoolParent);
    }
}
