using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window_Title : WindowBase
{
    public void OnClickStartButton()
    {
        UserInfo.Instance.SetGameData();
        MSSceneManager.Instance.EnterScene(SceneBase.eScene.CHAT);
    }
}
