using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MSUtil;
using DG.Tweening;

public class Window_Chat_Main : WindowBase
{
    #region Inspector
    public RectTransform PortraitTrans;
    public RectTransform CharacterPosition;
    public RectTransform ChoiceParent;
    public RectTransform MailBundlePosition;
    public RectTransform OutsidePosition;
    public CharacterObject MainCharacter;
    public Image BackGroundImage;
    public RecycleScroll BackLogScroll;
    public CustomButton MainTouch;
    public CustomButton BackLogButton;
    public CustomButton DragTargetBag;
    public UI_LetterInfoPanel LetterInfo;
    public UI_BagInventory PlayerBag;
    public UI_MailSelctPanel MailSelectPanel;
    public UI_CurrentPlace CurrentPlace;
    public UI_NotifyPanel NotifyPanel;
    public GameObject InventoryBlock;
    #endregion
    private System.Action m_AfterOpenAction;
    private System.Action m_MainTouchAction;
    private GameObject BackLogObject;
    private DataManager.ChapterTextData m_CurrentChapterTextData;

    private LetterBundle m_MailBundle;

    private void Awake()
    {
        BackLogObject = BackLogScroll.transform.parent.gameObject;
    }

    public void Init(DataManager.ChapterTextData data, System.Action afterOpenAction = null, System.Action mainTouchAction = null)
    {
        ReleaseWindow();
        m_CurrentChapterTextData = data;
        MainCharacter.Init(ConstValue.CHARACTER_NIKA_ID);
        m_AfterOpenAction = afterOpenAction;
        m_MainTouchAction = mainTouchAction;
        MainTouch.IsColorHilight = false;
        BackLogObject.SetActive_Check(false);
        MailSelectPanel.gameObject.SetActive_Check(false);
        BackLogButton.gameObject.SetActive_Check(true);
        PlayerBag.gameObject.SetActive_Check(false);
        LetterInfo.gameObject.SetActive_Check(false);
        NotifyPanel.gameObject.SetActive_Check(false);
    }

    private RecycleSlotBase ActiveSlot()
    {
        return ObjectFactory.Instance.ActivateObject<BackLogText>();
    }

    protected override void AfterOpen()
    {
        base.AfterOpen();

        CurrentPlace.gameObject.SetActive_Check(true);
        CurrentPlace.Init(m_CurrentChapterTextData.GetPlaceText(), () =>
        {
            if (m_AfterOpenAction != null)
                m_AfterOpenAction();
        });
    }

    public void SetMailBundle()
    {
        m_MailBundle = ObjectFactory.Instance.ActivateObject<LetterBundle>();
        //TODO: 편지 떨어지는 연출
        m_MailBundle.Init(this, OpenSelectMailPanel);
        m_MailBundle.transform.Init(MailBundlePosition);
    }

    public void CloseMailSelectPanel()
    {
        if (LetterInfo.gameObject.activeSelf || PlayerBag.gameObject.activeSelf)
        {
            if (LetterInfo.gameObject.activeSelf)
                CloseMailInfo();
            if (PlayerBag.gameObject.activeSelf)
                ClosePlayerBag();
            return;
        }

        //MailSelectPanel.Release();
        //MailSelectPanel.Close(() => MailSelectPanel.gameObject.SetActive_Check(false));
    }

    private void OpenSelectMailPanel()
    {
        MailSelectPanel.gameObject.SetActive_Check(true);
        MailSelectPanel.Init(() =>
        {
            ChatObject.Instance.CheckOverlapEvent(eOverlapType.MAILSORT);
        });
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

    public void OnClickBag()
    {
        PlayerBag.gameObject.SetActive_Check(true);
        InventoryBlock.SetActive_Check(!MailSelectPanel.gameObject.activeSelf);

        PlayerBag.DOKill();
        PlayerBag.BackGroundTrans.localScale = Vector3.zero;
        PlayerBag.BackGroundTrans.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            PlayerBag.Init();
        });
        if (MailSelectPanel.gameObject.activeSelf)
        {
            var targetTrans = MailSelectPanel.PanelContentTrans;
            targetTrans.DOKill();
            targetTrans.DOAnchorPosX(-135, 0.2f).SetEase(Ease.InSine);
        }
    }

    public void ClosePlayerBag()
    {
        PlayerBag.Release();
        PlayerBag.DOKill();
        PlayerBag.BackGroundTrans.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            InventoryBlock.SetActive_Check(false);
            PlayerBag.gameObject.SetActive_Check(false);
        });
        if (MailSelectPanel.gameObject.activeSelf)
        {
            var targetTrans = MailSelectPanel.PanelContentTrans;
            targetTrans.DOKill();
            targetTrans.DOAnchorPosX(0, 0.2f).SetEase(Ease.InSine);
        }
    }

    public void OnClickOption()
    {

    }

    public void OnClickQuit()
    {

    }

    public void OnClickStart()
    {
        UserInfo.Instance.SetCurrentIngameState(eIngameState.Map);
        MSSceneManager.Instance.EnterScene(SceneBase.eScene.INGAME);
    }

    public void OpenMailInfo(DataManager.LetterData data)
    {
        LetterInfo.gameObject.SetActive_Check(true);
        LetterInfo.Init(data);
        var rectTrans = LetterInfo.transform as RectTransform;
        rectTrans.DOAnchorPosY(0, 0.2f).SetEase(Ease.InSine);
    }

    public void CloseMailInfo()
    {
        var rectTrans = LetterInfo.transform as RectTransform;
        rectTrans.DOAnchorPosY(-160, 0.2f).SetEase(Ease.InSine).OnComplete(()=>
        {
            LetterInfo.gameObject.SetActive_Check(false);
        });
    }

    public void ReleaseWindow()
    {
        BackLogScroll.Release();
        MailSelectPanel.Release();
        if (m_MailBundle != null)
            ObjectFactory.Instance.DeactivateObject(m_MailBundle);
        PlayerBag.Release();
    }

    protected override void Close()
    {
        ReleaseWindow();
        base.Close();
    }
}
