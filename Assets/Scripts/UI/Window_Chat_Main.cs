using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Window_Chat_Main : WindowBase
{
    #region Inspector
    public RectTransform PortraitTrans;
    public TextMeshProUGUI DialougeText;
    #endregion
    private DataManager.ChapterData m_CurrentChapterData;

    public void Init(string chapterID)
    {
        m_CurrentChapterData = DataManager.Instance.GetChapterData(chapterID);
        if (m_CurrentChapterData != null)
        {

        }
    }
}
