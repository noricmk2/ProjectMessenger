using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCStateMachine
{
    private Coroutine m_UpdateCoroutine;
    public TileCharacter Target { get; private set; }
    public TCStateBase CurrentState { get; private set; }
    private Stack<TCStateBase> m_StateStack = new Stack<TCStateBase>();

    public void Init(TileCharacter parent)
    {
        Target = parent;
        m_StateStack.Clear();
        if (m_UpdateCoroutine != null)
            Target.StopCoroutine(m_UpdateCoroutine);
        m_UpdateCoroutine = Target.StartCoroutine(Update_C());
    }

    public void SetState(TCStateBase nextState)
    {
        if (nextState != CurrentState)
        {
            OnExitState();
            m_StateStack.Push(nextState);
        }
    }

    IEnumerator Update_C()
    {
        while (true)
        {
            CurrentState = GetCurrentState();
            if (CurrentState != null)
                CurrentState.Update(this);

            yield return null;
        }
    }

    private TCStateBase GetCurrentState()
    {
        if (m_StateStack.Count > 0)
            return m_StateStack.Peek();
        return null;
    }

    private void OnExitState()
    {
        if (m_StateStack.Count > 0)
        {
            var state = m_StateStack.Pop();
            state.OnExit(this);
        }
    }
}
