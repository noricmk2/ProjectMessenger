using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window_Title : WindowBase
{
    public void OnClickStartButton()
    {
        IngameScene.instance.StartIngame();
    }
}
