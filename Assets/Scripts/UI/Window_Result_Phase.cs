using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;
using UnityEngine.UI;
using TMPro;

public class Window_Result_Phase : WindowBase
{
    #region Inspector
    public UI_ResultPanel DayResultPanel;
    #endregion

    private void Awake()
    {

    }

    public void Init()
    {

    }

    public void OnClickNextButton()
    {
        //임시
        UserInfo.Instance.SetNextChapter();
        //MSSceneManager.Instance.EnterScene(SceneBase.eScene.CHAT);
        Application.Quit();
    }
}
