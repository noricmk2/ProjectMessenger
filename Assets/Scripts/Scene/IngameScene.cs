using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

public class IngameScene : SceneBase
{
    public static IngameScene instance;

    [NonSerialized]
    public IngameObject ingameObject = null;

    public IngameScene() : base(eScene.INGAME)
    {
        instance = this;
    }

    public void StartIngame()
    {
        UserInfo.Instance.SetGameData();

        switch (UserInfo.Instance.GetCurrentIngameState())
        {
            case eIngameState.Title:
            case eIngameState.MailSort:
                //채팅씬으로 가야할 때
                UserInfo.Instance.SetCurrentIngameState(eIngameState.MailSort);
                MSSceneManager.Instance.EnterScene(eScene.CHAT);
                break;
            case eIngameState.Map:
                ingameObject.StartMap();
                break;
        }
    }

    public override IEnumerator Enter_C()
    {
        ingameObject = ResourcesManager.Instantiate("Prefab/IngameObject").GetComponent<IngameObject>();
        ObjectFactory.Instance.CreateIngameObjectPool(ingameObject.PoolParent);
        ingameObject.Init();
        yield break;
    }

    public override IEnumerator Exit_C()
    {
        ObjectFactory.Instance.Release();
        yield break;
    }
}
