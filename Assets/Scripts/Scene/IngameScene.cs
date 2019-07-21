using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameScene : SceneBase
{
    public static IngameScene instance;

    public IngameScene() : base(eScene.INGAME)
    {
        instance = this;
    }

    public void StartIngame()
    {
        UserInfo.Instance.SetGameData();

        //if()
        //채팅씬으로 가야할 때
        //MSSceneManager.Instance.EnterScene(SceneBase.eScene.CHAT);

        //채팅 아닐 때
    }

    public override IEnumerator Enter_C()
    {
        var ingameObject = ResourcesManager.Instantiate("Prefab/IngameObject").GetComponent<IngameObject>();
        ingameObject.Init();
        //if (!GameManager.StartUpGame)
        {
            WindowBase.OpenWindow(WindowBase.eWINDOW.Title, ingameObject.WindowParent, false);
            GameManager.StartUpGame = true;
        }
        //else
        {
            //WindowBase.OpenWindow(WindowBase.eWINDOW.MainWindow, ingameObject.WindowParent, false);
        }
        yield break;
    }

    public override IEnumerator Exit_C()
    {
        yield break;
    }
}
