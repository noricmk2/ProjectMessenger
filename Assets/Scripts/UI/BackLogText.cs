using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MSUtil;

public class BackLogTextData : IRecycleSlotData
{
    public BackLogTextData(string name, string content)
    {
        CharacterName = name;
        Content = content;
    }

    public string CharacterName;
    public string Content;
}

public class BackLogText : RecycleSlotBase
{
    #region Inspector
    public TextMeshProUGUI HeaderText;
    public TextMeshProUGUI ContentText;
    public RectTransform SlotTrans;
    #endregion
    private List<IRecycleSlotData> m_DataList;
    private BackLogTextData m_CurrentData;

    public override void Init(Transform parent, List<IRecycleSlotData> dataList)
    {
        SlotTrans.Init(parent);
        m_DataList = dataList;
    }

    public override void Refresh(int idx)
    {
        m_CurrentData = m_DataList[idx] as BackLogTextData;
        HeaderText.text = m_CurrentData.CharacterName;
        ContentText.text = m_CurrentData.Content;
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
