using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;
using UnityEngine.UI;

public class IngameObject : MonoBehaviour
{
    #region Inspector
    public Transform WindowParent;
    public Canvas IngameCanvas;
    public CanvasScaler Scaler;
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
        Scaler.matchWidthOrHeight = UICamera.Instance.HasPillarBox ? 1 : 0;

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
                AlwaysTopCanvas.Instance.SetFadeAnimation(1, true, eTransitionType.CIRCLE, () =>
                {
                    StartResult();
                });
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
        var roomObj = ResourcesManager.Instantiate<TileMap>("Prefab/IsometricTileMap");
        roomObj.transform.localPosition = new Vector3(4.07f, -1.75f);

        ResultWindow = WindowBase.OpenWindow(WindowBase.eWINDOW.ResultPhase, WindowParent, false) as Window_Result_Phase;
        ResultWindow.Init();
    }
}
