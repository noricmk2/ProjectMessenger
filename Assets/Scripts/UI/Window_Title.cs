using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window_Title : WindowBase
{
    public void OnClickStartButton()
    {
        MSSceneManager.Instance.EnterScene(SceneBase.eScene.CHAT);
    }
}
