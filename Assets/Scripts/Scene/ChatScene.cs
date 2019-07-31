using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatScene : SceneBase
{
    private ChatObject m_ChatObject;

    public ChatScene() : base(eScene.CHAT)
    {

    }

    public override IEnumerator Enter_C()
    {
        m_ChatObject = ResourcesManager.Instantiate("Prefab/ChatObject").GetComponent<ChatObject>();
        ObjectFactory.Instance.CreateChatObjectPool(m_ChatObject.PoolParent);
        m_ChatObject.Init();
        yield break;
    }

    public override IEnumerator Exit_C()
    {
        if (m_ChatObject != null)
            m_ChatObject.Release();
        ObjectFactory.Instance.Release();
        yield break;
    }
}