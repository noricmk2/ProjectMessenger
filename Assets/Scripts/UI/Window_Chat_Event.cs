using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MSUtil;
using DG.Tweening;

public class Window_Chat_Event : WindowBase
{
    #region Inspector
    public Image BackGroundImage;
    public RecycleScroll BackLogScroll;
    public CustomButton MainTouch;
    public CustomButton BackLogButton;
    public RectTransform ChoiceParent;
    #endregion
    private System.Action m_AfterOpenAction;
    private System.Action m_MainTouchAction;
    private GameObject BackLogObject;
    private eEnterEffect m_EnterEffect;

    private void Awake()
    {
        BackLogObject = BackLogScroll.transform.parent.gameObject;
    }

    public void Init(System.Action afterOpenAction = null, System.Action mainTouchAction = null, eEnterEffect effect = eEnterEffect.NONE)
    {
        ReleaseWindow();
        m_AfterOpenAction = afterOpenAction;
        m_MainTouchAction = mainTouchAction;
        m_EnterEffect = effect;
        ChoiceParent.gameObject.SetActive_Check(false);
    }

    protected override void AfterOpen()
    {
        //TODO:이벤트씬 오픈시 연출 및 예외처리
        base.AfterOpen();

        switch (m_EnterEffect)
        {
            case eEnterEffect.NONE:
                {
                    if (m_AfterOpenAction != null)
                        m_AfterOpenAction();
                }
                break;
            case eEnterEffect.SHAKE:
                {
                    UICamera.Instance.CameraShake(ChatObject.Instance.WindowParent, 10f, 0.5f, () =>
                    {
                        if (m_AfterOpenAction != null)
                            m_AfterOpenAction();
                    });
                }
                break;
        }
    }

    private RecycleSlotBase ActiveSlot()
    {
        return ObjectFactory.Instance.ActivateObject<BackLogText>();
    }

    public void OnMainTouch()
    {
        if (m_MainTouchAction != null)
            m_MainTouchAction();
    }

    public void OnClickBackLog()
    {
        if (BackLogObject.activeSelf)
        {
            BackLogScroll.Release();
            BackLogObject.SetActive_Check(false);
            BackLogButton.gameObject.SetActive_Check(true);
        }
        else
        {
            BackLogButton.gameObject.SetActive_Check(false);
            BackLogScroll.Init(new List<IRecycleSlotData>(ChatObject.Instance.LogTextList.ToArray()), ActiveSlot);
            BackLogObject.SetActive_Check(true);
        }
    }

    public void ReleaseWindow()
    {
        BackLogScroll.Release();
    }

    protected override void Close()
    {
        ReleaseWindow();
        base.Close();
    }
}
