using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

public class TileCharacter : TileBase
{
    public enum TCState
    {
        IDLE,
        MOVE,
    }

    public TCStateMachine StateMachine { get; private set; }
    private Dictionary<TCState, TCStateBase> m_StateDic = new Dictionary<TCState, TCStateBase>();

    public override void Init(TileMap parent)
    {
        TargetSprite.sprite = ObjectFactory.Instance.GetTileSprite(TileResourceName);
        TileMapParent = parent;

        if (StateMachine == null)
            StateMachine = new TCStateMachine();
        StateMachine.Init(this);

        m_StateDic.Clear();
        m_StateDic[TCState.IDLE] = new StateIdle();
        m_StateDic[TCState.MOVE] = new StateMove();
        SetState(TCState.IDLE);
    }

    public void SetState(TCState state)
    {
        if(m_StateDic.ContainsKey(state) && m_StateDic[state] != null)
            StateMachine.SetState(m_StateDic[state]);
    }

    private void Update()
    {
        TargetSprite.sortingOrder = -(int)(CachedTransform.localPosition.y * IsometricRange);
    }
}
