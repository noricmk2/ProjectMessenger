using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool<T> where T : IPoolObjectBase
{
    public delegate T CreateAction();

    private readonly CreateAction m_CreateAction;
    private readonly Action<T> m_PushAction;
    private readonly Action<T> m_PopAction;

    private Stack<T> m_Pool = new Stack<T>();
    private List<T> m_RestoreList = new List<T>();

    public int Count
    {
        get { return m_Pool.Count; }
    }

    public ObjectPool(int count, CreateAction createAction, Action<T> pushAction, Action<T> popAction)
    {
        m_CreateAction = createAction;
        m_PushAction = pushAction;
        m_PopAction = popAction;

        for (int i = 0; i < count; ++i)
            Add();
    }

    public void Push(T pushObj)
    {
        if (m_Pool.Contains(pushObj))
            return;
        m_Pool.Push(pushObj);

        if (m_RestoreList.Contains(pushObj))
            m_RestoreList.Remove(pushObj);

        if (m_PushAction != null)
            m_PushAction(pushObj);
    }

    public T Pop()
    {
        if (m_Pool.Count == 0)
            Add();

        var retObj = m_Pool.Pop();
        if (m_PopAction != null)
            m_PopAction(retObj);
        m_RestoreList.Add(retObj);
        return retObj;
    }

    public T Peek()
    {
        if (m_Pool.Count == 0)
            Add();

        return m_Pool.Peek();
    }

    private void Add()
    {
        if (m_CreateAction != null)
        {
            var poolObj = m_CreateAction();
            Push(poolObj);
        }
    }

    public void Restore()
    {
        for (int i = 0; i < m_RestoreList.Count; ++i)
            Push(m_RestoreList[i]);
    }
}
