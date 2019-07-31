﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MSUtil;
using UnityEngine.EventSystems;
using TMPro;

public class MailSlot : RecycleSlotBase
{
    #region Inspector
    public RectTransform SlotTrans;
    public DraggableUI Drag;
    public TextMeshProUGUI Text;
    #endregion
    private Transform m_OrgParent;
    private CustomButton m_TargetBag;

    public DataManager.LetterData CurrentLetterData { get; private set; }

    private void Awake()
    {
        if (Drag == null)
            Drag = GetComponent<DraggableUI>();
    }

    public override void Init(RecycleScroll scroll, Transform parent, List<IRecycleSlotData> dataList)
    {
        base.Init(scroll, parent, dataList);
        Drag.Init(BeginDrag, OnDrag, EndDrag);
        var window = WindowBase.GetWindow<Window_Chat_Main>();
        m_TargetBag = window.DragTargetBag;
    }

    public void InitForClone(Transform parent, DataManager.LetterData data)
    {
        transform.Init(parent);
        CurrentLetterData = data;
        Text.text = CurrentLetterData.ID.ToString();
    }

    public override void Refresh(int idx)
    {
        base.Refresh(idx);
        CurrentLetterData = m_DataList[idx] as DataManager.LetterData;
        Text.text = CurrentLetterData.ID.ToString();
    }

    public void OnClickMail()
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

        var slotClone = ObjectFactory.Instance.ActivateObject<MailSlot>();
        slotClone.InitForClone(m_Scroll.transform.parent, CurrentLetterData);
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
        var targetTrans = m_TargetBag.transform as RectTransform;
        var image = m_TargetBag.ButtonImage;
        if (RectTransformUtility.RectangleContainsScreenPoint(targetTrans, eventData.position, UICamera.Instance.Camera))
        {
            image.material = ObjectFactory.Instance.SpriteOutlineMaterial;
        }
        else
        {
            image.material = image.defaultMaterial;
        }
    }

    public void EndDrag(DraggableUI target, PointerEventData eventData)
    {
        m_Scroll.enabled = true;

        var dragObj = target.GetDragObjectTrans();
        if (dragObj != null)
        {
            var slot = dragObj.GetComponent<MailSlot>();
            ObjectFactory.Instance.DeactivateObject(slot);

            var targetTrans = m_TargetBag.transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(targetTrans, eventData.position, UICamera.Instance.Camera))
            {
                UserInfo.Instance.AddLetter(CurrentLetterData);
                m_DataList.Remove(CurrentLetterData);
                ObjectFactory.Instance.DeactivateObject(this);
                m_Scroll.RefreshScroll(m_DataList);
            }
            else
            {
                transform.Init(m_OrgParent);
                m_Scroll.AddSlot(this);
                m_Scroll.RefreshScroll();
            }
        }
        m_TargetBag.ButtonImage.material = m_TargetBag.ButtonImage.defaultMaterial;
    }
}
