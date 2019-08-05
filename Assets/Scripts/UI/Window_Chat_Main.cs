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
    public CharacterObject MainCharacter;
    public Image BackGroundImage;
    public CustomButton MainTouch;
    public RecycleScroll BackLogScroll;
    public RecycleScroll MailSelectScroll;
    public BagInventory PlayerBag;
    public CustomButton BackLogButton;
    public CustomButton DragTargetBag;
    public LetterInfoPanel LetterInfo;
    public GameObject InventoryBlock;
    #endregion
    private System.Action m_AfterOpenAction;
    private System.Action m_MainTouchAction;
    private GameObject BackLogObject;
    private GameObject MailSelectObject;
    private ChatObject m_Parent;

    private LetterBundle m_MailBundle;

    private void Awake()
    {
        BackLogObject = BackLogScroll.transform.parent.gameObject;
        MailSelectObject = MailSelectScroll.transform.parent.parent.gameObject;
    }

    public void Init(ChatObject parent, System.Action afterOpenAction = null, System.Action mainTouchAction = null)
    {
        m_Parent = parent;
        MainCharacter.Init(ConstValue.CHARACTER_NIKA_ID, MainCharacter.transform.parent);
        m_AfterOpenAction = afterOpenAction;
        m_MainTouchAction = mainTouchAction;
        MainTouch.IsColorHilight = false;
        BackLogObject.SetActive_Check(false);
        MailSelectObject.SetActive_Check(false);
        BackLogButton.gameObject.SetActive_Check(true);
        PlayerBag.gameObject.SetActive_Check(false);
        LetterInfo.gameObject.SetActive_Check(false);
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

        MailSelectScroll.Release();
        MailSelectObject.gameObject.SetActive_Check(false);
    }

    private void OpenSelectMailPanel()
    {
        MailSelectObject.gameObject.SetActive_Check(true);
        MailSelectScroll.Init(new List<IRecycleSlotData>(UserInfo.Instance.GetChapterLetterList().ToArray()), delegate ()
        {
            return ObjectFactory.Instance.ActivateObject<LetterSlot>();
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
            BackLogScroll.Init(new List<IRecycleSlotData>(m_Parent.LogTextList.ToArray()), ActiveSlot);
            BackLogObject.SetActive_Check(true);
        }
    }

    public void OnClickBag()
    {
        PlayerBag.gameObject.SetActive_Check(true);
        InventoryBlock.SetActive_Check(!MailSelectObject.activeSelf);

        PlayerBag.DOKill();
        PlayerBag.BackGroundTrans.localScale = Vector3.zero;
        PlayerBag.BackGroundTrans.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            PlayerBag.Init();
        });
        if (MailSelectObject.gameObject.activeSelf)
        {
            var targetTrans = MailSelectScroll.transform.parent as RectTransform;
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
        if (MailSelectObject.gameObject.activeSelf)
        {
            var targetTrans = MailSelectScroll.transform.parent as RectTransform;
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
        MailSelectScroll.Release();
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
