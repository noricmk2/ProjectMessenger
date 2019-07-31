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
    private BackLogTextData m_CurrentData;

    public override void Init(RecycleScroll scroll, Transform parent, List<IRecycleSlotData> dataList)
    {
        base.Init(scroll, parent, dataList);
    }

    public override void Refresh(int idx)
    {
        base.Refresh(idx);
        m_CurrentData = m_DataList[idx] as BackLogTextData;
        HeaderText.text = m_CurrentData.CharacterName;
        ContentText.text = m_CurrentData.Content.Replace('^', '\n');
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
