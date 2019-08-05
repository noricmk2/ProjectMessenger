using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window_Title : WindowBase
{
    public void OnClickStartButton()
    {
        if (AlwaysTopCanvas.Instance.IsOnFade)
            return;

        AlwaysTopCanvas.Instance.SetFadeAnimation(0.5f, false, MSUtil.eTransitionType.NORMAL, () =>
        {
            CloseWindow();
            IngameScene.instance.StartIngame();
        });
    }
}
