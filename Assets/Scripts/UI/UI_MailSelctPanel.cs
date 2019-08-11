using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MSUtil;
using System;
using DG.Tweening;

public class UI_MailSelctPanel : MonoBehaviour
{
    #region Inspector
    public RectTransform PanelContentTrans;
    public CustomButton StartButton;
    public CustomButton BagButton;
    public RecycleScroll MailSelectScroll;
    public GameObject Block;
    #endregion
    private float m_AnimTime = 0.3f;
    private Action m_OpenAction;
    private Action m_CloseAction;

    public void Init(Action openAction)
    {
        StartButton.Interactable = true;
        BagButton.Interactable = true;

        PanelContentTrans.DOKill();
        PanelContentTrans.localScale = Vector3.zero;
        PanelContentTrans.DOScale(Vector3.one, m_AnimTime).SetEase(Ease.InSine).OnComplete(() =>
        {
            if (openAction != null)
                openAction();

            MailSelectScroll.Init(new List<IRecycleSlotData>(UserInfo.Instance.GetChapterLetterList().ToArray()), delegate ()
            {
                return ObjectFactory.Instance.ActivateObject<LetterSlot>();
            });
        });
    }


    public void Close(Action closeAction)
    {
        DisableSelectPanel();
        Release();
        PanelContentTrans.DOKill();
        PanelContentTrans.DOScale(Vector3.zero, m_AnimTime).SetEase(Ease.OutSine).OnComplete(() =>
        {
            if (closeAction != null)
                closeAction();
        });
    }

    public void DisableSelectPanel()
    {
        StartButton.Interactable = false;
        BagButton.Interactable = false;
    }

    public void Release()
    {
        MailSelectScroll.Release();
    }
}
