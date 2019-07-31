using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;
using UnityEngine.UI;
using TMPro;

public class BagSlot : RecycleSlotBase
{
    #region Inspector
    public RectTransform SlotTrans;
    public Image IconImage;
    public TextMeshProUGUI Text;
    #endregion
    public UserInfo.ItemData CurrentData { get; private set; }

    public override void Init(RecycleScroll scroll, Transform parent, List<IRecycleSlotData> dataList)
    {
        base.Init(scroll, parent, dataList);
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

    public override float GetHeight()
    {
        return SlotTrans.rect.height;
    }

    public override float GetWidth()
    {
        return SlotTrans.rect.width;
    }
}
