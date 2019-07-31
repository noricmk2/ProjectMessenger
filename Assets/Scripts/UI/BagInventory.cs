using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagInventory : MonoBehaviour
{
    #region Inspector
    public RecycleScroll InventoryScroll;
    public RectTransform BackGroundTrans;
    #endregion

    public void Init()
    {
        var dataList = UserInfo.Instance.GetBagItemList();
        InventoryScroll.Init(new List<IRecycleSlotData>(dataList.ToArray()), delegate ()
        {
            return ObjectFactory.Instance.ActivateObject<BagSlot>();
        });
    }

    public void Release()
    {
        InventoryScroll.Release();
    }
}
