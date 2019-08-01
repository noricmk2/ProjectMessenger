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

    private Window_Map m_MapWindow;

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
        }

    }

    public void StartMap()
    {
        m_MapWindow = WindowBase.OpenWindow(WindowBase.eWINDOW.Map, WindowParent, false) as Window_Map;
        m_MapWindow.OpenMap();
    }
}
