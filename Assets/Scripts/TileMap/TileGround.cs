using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

public class TileGround : TileBase
{
    public override void Init(TileMap parent)
    {
        TileMapParent = parent;
        TargetSprite.sprite = ObjectFactory.Instance.GetTileSprite(TileResourceName);
    }
}
