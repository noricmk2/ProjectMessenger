using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatScene : SceneBase
{
    public ChatScene() : base(eScene.CHAT)
    {

    }

    public override IEnumerator Enter_C()
    {
        var chatObj = ResourcesManager.Instantiate("Prefab/ChatObject").GetComponent<ChatObject>();
        var chatWindow = WindowBase.OpenWindowWithFade(WindowBase.eWINDOW.ChatMain, chatObj.WindowParent, true) as Window_Chat_Main;
        chatWindow.Init("1");
        yield break;
    }

    public override IEnumerator Exit_C()
    {
        throw new System.NotImplementedException();
    }
}