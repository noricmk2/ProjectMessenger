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
        var ingameObject = ResourcesManager.Instantiate("IngameObject").GetComponent<IngameObject>();
        WindowBase.OpenWindow(WindowBase.eWINDOW.MainWindow, ingameObject.WindowParent, false);
        yield break;
    }

    public override IEnumerator Exit_C()
    {
        throw new System.NotImplementedException();
    }
}
