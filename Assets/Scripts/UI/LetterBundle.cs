using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LetterBundle : MonoBehaviour, IPoolObjectBase
{
    private Window_Chat_Main m_Parent;
    private List<DataManager.LetterData> m_LetterList;
    private Action m_ClickAction;

    public void Init(Window_Chat_Main parent, Action clickAction) 
    {
        m_Parent = parent;
        m_ClickAction = clickAction;
    }

    public void OnClickButton()
    {
        if (m_ClickAction != null)
            m_ClickAction();
    }

    public void PopAction()
    {
        gameObject.SetActive(true);
    }

    public void PushAction()
    {
        gameObject.SetActive(false);
        transform.SetParent(ObjectFactory.Instance.ChatPoolParent);
    }
}
