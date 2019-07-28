using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MSUtil;

public class Window_Chat_Main : WindowBase
{
    #region Inspector
    public RectTransform PortraitTrans;
    public RectTransform CharacterPosition;
    public RectTransform ChoiceParent;
    public CharacterObject MainCharacter;
    public Image BackGroundImage;
    public CustomButton MainTouch;
    public RecycleScroll BackLogScroll;
    public CustomButton BackLogButton;
    #endregion
    private System.Action m_AfterOpenAction;
    private System.Action m_MainTouchAction;
    private GameObject BackLogObject;
    private ChatObject m_Parent;

    private void Awake()
    {
        BackLogObject = BackLogScroll.transform.parent.gameObject;
    }

    public void Init(ChatObject parent, System.Action afterOpenAction = null, System.Action mainTouchAction = null)
    {
        m_Parent = parent;
        MainCharacter.Init(ConstValue.CHARACTER_NIKA_ID, MainCharacter.transform.parent);
        m_AfterOpenAction = afterOpenAction;
        m_MainTouchAction = mainTouchAction;
        MainTouch.IsColorHilight = false;
        BackLogObject.SetActive_Check(false);
        BackLogButton.gameObject.SetActive_Check(true);
    }

    private RecycleSlotBase ActiveSlot()
    {
        return ObjectFactory.Instance.ActivateObject<BackLogText>();
    }

    protected override void AfterOpen()
    {
        base.AfterOpen();
        if (m_AfterOpenAction != null)
            m_AfterOpenAction();
    }

    public void OnMainTouch()
    {
        if(m_MainTouchAction != null)
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
            BackLogScroll.Init(new List<IRecycleSlotData>(m_Parent.LogTextList.ToArray()), ActiveSlot);
            BackLogObject.SetActive_Check(true);
        }
    }

    public void OnClickBag()
    {

    }

    public void OnClickOption()
    {

    }

    public void OnClickQuit()
    {

    }
}
