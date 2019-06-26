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

    public void Init()
    {
        m_CurrentChapterData = DataManager.Instance.GetChapterData(UserInfo.Instance.CurrentGameData.CurrentChapterID);



        StartCoroutine(Update_C());
    }

    IEnumerator Update_C()
    {
        while(true)
        {


            yield return null;
        }
    }

    protected override void Close()
    {
        StopCoroutine(Update_C());
        base.Close();
    }
}
