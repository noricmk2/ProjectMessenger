using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

public class IngameObject : MonoBehaviour
{
    #region Inspector
    public Transform WindowParent;
    public Canvas IngameCanvas;
    public Transform PoolParent;
    #endregion

    [Header("Map")]
    [System.NonSerialized]
    public Window_Map MapWindow;
    public MapObject mapObject;

    [Header("Result")]
    public Window_Result_Phase ResultWindow;

    public void Init()
    {
        IngameCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        IngameCanvas.worldCamera = UICamera.Instance.Camera;

        switch (UserInfo.Instance.GetCurrentIngameState())
        {
            case eIngameState.Title:
                WindowBase.OpenWindow(WindowBase.eWINDOW.Title, WindowParent, false);
                GameManager.StartUpGame = true;
                break;
            case eIngameState.MailSort:
                //MailSort
                break;
            case eIngameState.Map:
                StartMap();
                break;
            case eIngameState.Result:
                StartResult();
                break;
        }

    }

    public void StartMap()
    {
        MapWindow = WindowBase.OpenWindow(WindowBase.eWINDOW.Map, WindowParent, false) as Window_Map;
        MapWindow.OpenMap();
    }

    public void StartResult()
    {
        ResultWindow = WindowBase.OpenWindow(WindowBase.eWINDOW.ResultPhase, WindowParent, false) as Window_Result_Phase;
        ResultWindow.Init();
    }
}
