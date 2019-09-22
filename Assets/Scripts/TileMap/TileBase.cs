using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

public abstract class TileBase : MonoBehaviour
{
    public enum TileType
    {
        FLOOR,
        WALL,
        CHARACTER,
        OBJECT,
    }

    #region Inspector
    public SpriteRenderer TargetSprite;
    public string TileResourceName;
    public TileType CurrentType;
    #endregion
    public Vector2 CartesianPos { get { return Func.Iso2Cart(CachedTransform.localPosition); } }
    public TileMap TileMapParent { get; protected set; }
    protected Transform m_CachedTransform;
    public Transform CachedTransform
    {
        get
        {
            if (m_CachedTransform == null)
                m_CachedTransform = transform;
            return m_CachedTransform;
        }
    }
    protected const int IsometricRange = 100;

    public abstract void Init(TileMap parent);

    public void SetType(TileType type)
    {
        CurrentType = type;
    }
}
