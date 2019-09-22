using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MSUtil;

public abstract class TCStateBase
{
    public abstract void Update(TCStateMachine machine);
    public abstract void OnExit(TCStateMachine machine);
}

public class StateIdle : TCStateBase
{
    float time = 0;

    public override void Update(TCStateMachine machine)
    {
        if (time > 1.0f)
        {
            time = 0;
            machine.Target.SetState(TileCharacter.TCState.MOVE);
        }
        else
            time += Time.deltaTime;
    }

    public override void OnExit(TCStateMachine machine)
    {
    }
}

public class StateMove : TCStateBase
{
    float speed = 0.02f;
    float dx = 0, dy = 0;
    float time = 0;

    public override void Update(TCStateMachine machine)
    {
        if (time > 1.5f)
        {
            time = 0;
            dx = 0;
            dy = 0;
            machine.Target.SetState(TileCharacter.TCState.IDLE);
        }
        else
        {
            if (dx == 0 && dy == 0)
            {
                dx = Func.InPercent(50) ? 1 : 0;
                dy = Func.InPercent(50) ? 1 : 0;
                dx = Func.InPercent(50) ? -dx : dx;
                dy = Func.InPercent(50) ? -dy : dy;
            }
            else
            {
                var move = new Vector2(machine.Target.CartesianPos.x + (dx * speed), machine.Target.CartesianPos.y + (dy * speed));
                var spriteBound = machine.Target.TargetSprite.bounds;
                var topRight = move + new Vector2(spriteBound.extents.x, spriteBound.extents.y);
                var topLeft = move + new Vector2(-spriteBound.extents.x, spriteBound.extents.y);
                var bottomRight = move + new Vector2(spriteBound.extents.x, -spriteBound.extents.y);
                var bottomLeft = move + new Vector2(-spriteBound.extents.x, -spriteBound.extents.y);

                topRight = Func.Cart2Iso(topRight);
                topLeft = Func.Cart2Iso(topLeft);
                bottomRight = Func.Cart2Iso(bottomRight);
                bottomLeft = Func.Cart2Iso(bottomLeft);

                if (!machine.Target.TileMapParent.IsMoveTile(topRight) ||
                    !machine.Target.TileMapParent.IsMoveTile(topLeft) ||
                    !machine.Target.TileMapParent.IsMoveTile(bottomRight) ||
                    !machine.Target.TileMapParent.IsMoveTile(bottomLeft))
                {
                    time += Time.deltaTime;
                    return;
                }
                move = Func.Cart2Iso(move);
                machine.Target.CachedTransform.localPosition = move;
            }

            time += Time.deltaTime;
        }
    }

    public override void OnExit(TCStateMachine machine)
    {
    }
}
