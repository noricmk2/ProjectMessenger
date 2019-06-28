using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatObject : MonoBehaviour
{
    #region Inspector
    public Transform WindowParent;
    #endregion
    private IEnumerator m_UpdateCouroutine = null;
    private Window_Chat_Main m_ChatWindow;
    private DataManager.ChapterData m_CurrentChapterData;
    private DataManager.ChapterTextData m_CurrentChapterTextData;

    public void Init()
    {
        m_CurrentChapterData = DataManager.Instance.GetChapterData(UserInfo.Instance.CurrentGameData.CurrentChapterID);
        m_CurrentChapterTextData = m_CurrentChapterData.GetChapterTextData(MSUtil.eEventTag.START);
        m_ChatWindow = WindowBase.OpenWindowWithFade(WindowBase.eWINDOW.ChatMain, WindowParent, true) as Window_Chat_Main;
        m_ChatWindow.Init();
        if(m_UpdateCouroutine == null)
            m_UpdateCouroutine = Update_C();
        StartCoroutine(m_UpdateCouroutine);
    }

    IEnumerator Update_C()
    {
        while (true)
        {
            if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                m_ChatWindow.SetActiveCharacter(10001, true, true);
                m_ChatWindow.SetMainDialouge(m_CurrentChapterTextData.GetText(1));
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                m_ChatWindow.SetActiveCharacter(10001, false);
            }
            yield return null;
        }
    }

    public void Release()
    {
        if (m_UpdateCouroutine != null)
        {
            StopCoroutine(m_UpdateCouroutine);
            m_UpdateCouroutine = null;
        }
    }
}
