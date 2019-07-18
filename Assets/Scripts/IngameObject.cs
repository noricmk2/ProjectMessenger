using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameObject : MonoBehaviour
{
    #region Inspector
    public Transform WindowParent;
    public Canvas IngameCanvas;
    #endregion

    public void Init()
    {
        IngameCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        IngameCanvas.worldCamera = UICamera.Instance.Camera;
    }
}
