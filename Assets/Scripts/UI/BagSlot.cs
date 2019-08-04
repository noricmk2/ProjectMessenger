using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BagSlot : RecycleSlotBase
{
    #region Inspector
    public RectTransform SlotTrans;
    public Image IconImage;
    public TextMeshProUGUI Text;
    public DraggableUI Drag;
    #endregion
    public UserInfo.ItemData CurrentData { get; private set; }
    private Transform m_OrgParent;
    private RecycleScroll m_LetterTargetScroll;

    public override void Init(RecycleScroll scroll, Transform parent, List<IRecycleSlotData> dataList)
    {
        base.Init(scroll, parent, dataList);
        Drag.Init(BeginDrag, OnDrag, EndDrag);
        var window = WindowBase.GetWindow<Window_Chat_Main>();
        m_LetterTargetScroll = window.MailSelectScroll;
    }

    public void InitForClone(Transform parent, UserInfo.ItemData data)
    {
        CurrentData = data;
        transform.Init(parent);
        switch (CurrentData.Type)
        {
            case eItemType.Letter:
                {
                    IconImage.sprite = ObjectFactory.Instance.GetUISprite("icon_mail");
                    Text.text = CurrentData.ID.ToString();
                }
                break;
            case eItemType.Item:
                break;
        }
    }

    public override void Refresh(int idx)
    {
        base.Refresh(idx);
        CurrentData = m_DataList[idx] as UserInfo.ItemData;

        switch (CurrentData.Type)
        {
            case eItemType.Letter:
                {
                    IconImage.sprite = ObjectFactory.Instance.GetUISprite("icon_mail");
                    Text.text = CurrentData.ID.ToString();
                }
                break;
            case eItemType.Item:
                break;
        }
    }

    public void OnClick()
    {

    }

    public override float GetHeight()
    {
        return SlotTrans.rect.height;
    }

    public override float GetWidth()
    {
        return SlotTrans.rect.width;
    }

    public void BeginDrag(DraggableUI target, PointerEventData eventData)
    {
        m_Scroll.enabled = false;
        m_OrgParent = transform.parent;
        transform.Init(ObjectFactory.Instance.ChatPoolParent);
        m_Scroll.RemoveSlot(this);

        var slotClone = ObjectFactory.Instance.ActivateObject<BagSlot>();
        slotClone.InitForClone(m_Scroll.transform.parent, CurrentData);
        var pos = slotClone.transform.position;
        var touchPos = UICamera.Instance.Camera.ScreenToWorldPoint(eventData.position);
        pos.x = touchPos.x;
        pos.y = touchPos.y;
        slotClone.transform.position = pos;
        target.SetDragObjectTrans(slotClone.transform as RectTransform);
    }

    public void OnDrag(DraggableUI target, PointerEventData eventData)
    {
        var dragObj = target.GetDragObjectTrans();
    }

    public void EndDrag(DraggableUI target, PointerEventData eventData)
    {
        m_Scroll.enabled = true;

        var dragObj = target.GetDragObjectTrans();
        if (dragObj != null)
        {
            var slot = dragObj.GetComponent<BagSlot>();
            ObjectFactory.Instance.DeactivateObject(slot);
            switch (CurrentData.Type)
            {
                case eItemType.Letter:
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(m_LetterTargetScroll.viewport, eventData.position, UICamera.Instance.Camera))
                        {
                            var letter = DataManager.Instance.GetLetterData(CurrentData.ID);
                            UserInfo.Instance.RemoveInventoryLetter(letter);
                            UserInfo.Instance.AddChapterLetter(letter);
                            m_DataList.Remove(CurrentData);
                            ObjectFactory.Instance.DeactivateObject(this);
                            m_Scroll.RefreshScroll(m_DataList);
                            m_LetterTargetScroll.RefreshScroll(new List<IRecycleSlotData>(UserInfo.Instance.GetChapterLetterList()));
                        }
                        else
                        {
                            transform.Init(m_OrgParent);
                            m_Scroll.AddSlot(this);
                            m_Scroll.RefreshScroll();
                        }
                    }
                    break;
                case eItemType.Item:
                    {
                        transform.Init(m_OrgParent);
                        m_Scroll.AddSlot(this);
                        m_Scroll.RefreshScroll();
                    }
                    break;
            }
        }
    }
}
