using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

public class ChatObject : MonoBehaviour
{
    #region Inspector
    public Transform WindowParent;
    #endregion
    private IEnumerator m_UpdateCouroutine = null;
    private Window_Chat_Main m_ChatWindow;
    private DataManager.ChapterData m_CurrentChapterData;
    private DataManager.ChapterTextData m_CurrentChapterTextData;
    private int m_CurrentEventIdx;

    public void Init()
    {
        //TODO:챕터 정보 불러오기
        Release();
        m_CurrentChapterData = DataManager.Instance.GetChapterData(UserInfo.Instance.CurrentGameData.CurrentChapterID);
        SetChapterEvent((eEventTag)UserInfo.Instance.CurrentGameData.CurrentChapterEvent);
        m_ChatWindow = WindowBase.OpenWindowWithFade(WindowBase.eWINDOW.ChatMain, WindowParent, true) as Window_Chat_Main;
        m_ChatWindow.Init(()=>
        {
            if (m_UpdateCouroutine == null)
                m_UpdateCouroutine = Update_C();
            StartCoroutine(m_UpdateCouroutine);
        });
    }

    int testIdx = 1;
    IEnumerator Update_C()
    {
        var textData = m_CurrentChapterTextData.GetTextData(m_CurrentEventIdx);
        m_ChatWindow.SetTextData(textData);
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ++testIdx;
                if (testIdx >= (int)eCharacterState.LENGTH)
                    testIdx = 0;
                m_ChatWindow.PortraitSpriteAnimation.SetAnimation((eCharacterState)testIdx, testIdx == 1 ? true : false);
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                m_ChatWindow.SetActiveCharacter(10004, true);
            }
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                ++m_CurrentEventIdx;
                textData = m_CurrentChapterTextData.GetTextData(m_CurrentEventIdx);
                if (textData != null)
                    m_ChatWindow.SetTextData(textData);
                else
                {

                }
            }
            yield return null;
        }
    }

    public void SetChapterEvent(eEventTag tag)
    {
        m_CurrentChapterTextData = m_CurrentChapterData.GetChapterTextData(tag);
    }

    public void Release()
    {
        if (m_UpdateCouroutine != null)
        {
            StopCoroutine(m_UpdateCouroutine);
            m_UpdateCouroutine = null;
        }
        m_CurrentEventIdx = 1;
    }
}
