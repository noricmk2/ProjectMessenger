using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using MSUtil;

public class UI_CurrentPlace : MonoBehaviour
{
    #region Inspector
    public ExpandTextOutput PlaceText;
    public GameObject Block;
    #endregion
    private RectTransform m_TextRectTrans;
    private Vector2 m_OrgPos;
    private Vector2 m_TargetPos = new Vector2(0, -50);
    private Vector2 m_OrgSizeDelta;
    private Vector2 m_TargetSizeDelta = new Vector2(400, 30);

    private void Awake()
    {
        m_TextRectTrans = PlaceText.transform as RectTransform;
        m_OrgPos = m_TextRectTrans.anchoredPosition;
        m_OrgSizeDelta = m_TextRectTrans.sizeDelta;
    }

    public void Init(string place, Action endAction = null)
    {
        m_TextRectTrans.anchoredPosition = m_OrgPos;
        m_TextRectTrans.sizeDelta = m_OrgSizeDelta;

        PlaceText.ResetText();
        PlaceText.SetLastTerm(0.5f);
        PlaceText.SetText(place, ()=>
        {
            m_TextRectTrans.DOKill();
            m_TextRectTrans.DOAnchorPos(m_TargetPos, 0.3f).SetEase(Ease.InSine);
            m_TextRectTrans.DOSizeDelta(m_TargetSizeDelta, 0.3f).SetEase(Ease.InSine).OnComplete(() =>
            {
                Block.SetActive_Check(false);
                if (endAction != null)
                    endAction();
            });
        });
        PlaceText.PlayText();
    }
}
