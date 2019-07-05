using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameScene : SceneBase
{
    public IngameScene() : base(eScene.INGAME)
    {
        
    }

    public override IEnumerator Enter_C()
    {
        var ingameObject = ResourcesManager.Instantiate("Prefab/IngameObject").GetComponent<IngameObject>();

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
