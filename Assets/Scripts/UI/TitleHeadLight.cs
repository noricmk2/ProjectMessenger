using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleHeadLight : MonoBehaviour
{
    #region Inspector
    public float m_RotateTime = 1.0f;
    public float m_RotateWidth = 15;
    public bool RightFirst;
    #endregion
    private RectTransform m_RectTrans;
    private Vector3 m_OrgRotate;

    private void Awake()
    {
        m_RectTrans = transform as RectTransform;
        m_OrgRotate = m_RectTrans.localRotation.eulerAngles;
    }

    private void Start()
    {
        m_RectTrans.DOKill();

        var moveRight = new Vector3(0, 0, m_OrgRotate.z - m_RotateWidth);
        var moveLeft = new Vector3(0, 0, m_OrgRotate.z + m_RotateWidth);
        var sequence = DOTween.Sequence();

        if (RightFirst)
        {
            sequence.Append(m_RectTrans.DOLocalRotate(moveRight, m_RotateTime).SetEase(Ease.OutSine));
            sequence.Append(m_RectTrans.DOLocalRotate(moveLeft, m_RotateTime * 2).SetEase(Ease.InOutSine));
            sequence.Append(m_RectTrans.DOLocalRotate(m_OrgRotate, m_RotateTime).SetEase(Ease.InQuad));
        }
        else
        {
            sequence.Append(m_RectTrans.DOLocalRotate(moveLeft, m_RotateTime).SetEase(Ease.OutSine));
            sequence.Append(m_RectTrans.DOLocalRotate(moveRight, m_RotateTime * 2).SetEase(Ease.InOutSine));
            sequence.Append(m_RectTrans.DOLocalRotate(m_OrgRotate, m_RotateTime).SetEase(Ease.InQuad));
        }

        sequence.SetLoops(-1);
        sequence.Play();
    }
}
