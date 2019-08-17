using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MSUtil;
using TMPro;

public class UI_NotifyPanel : MonoBehaviour
{
    #region Inspector
    public TextMeshProUGUI Content;
    public RectTransform Panel;
    #endregion
    private float m_OrgYPos;
    private bool m_IsActive;

    private void Awake()
    {
        m_OrgYPos = Panel.anchoredPosition.y;
    }

    public void Init(string content)
    {
        m_IsActive = false;
        Content.text = content;
        Panel.DOKill();
        Panel.DOAnchorPosY(0, 0.2f).SetEase(Ease.InSine).OnComplete(() =>
        {
            m_IsActive = true;
        });
    }

    public void OnClickBlock()
    {
        if (m_IsActive)
        {
            Panel.DOKill();
            Panel.DOAnchorPosY(m_OrgYPos, 0.2f).SetEase(Ease.OutSine).OnComplete(()=> 
            {
                gameObject.SetActive_Check(false);
            });
        }
    }
}
