using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using MSUtil;

public class TileMap : MonoBehaviour
{
    #region Inspector
    public TileMapData MapData;
    public GameObject TileParent;
    #endregion
    private TileBase[,] m_Tiles;

    private void Awake()
    {
        var scritable = Resources.Load<ScriptableObject>("TileMapData/IsometricMap");
        MapData = scritable as TileMapData;
        if (TileParent == null)
            TileParent = this.gameObject;
        var tileList = TileParent.GetComponentsInChildren<TileBase>();
        m_Tiles = new TileBase[MapData.MapSize.x, MapData.MapSize.y];
        for (int i = 0; i < tileList.Length; ++i)
        {
            var coord = Func.GetTileCoord(tileList[i].CachedTransform.position, MapData.TileSize.x);
            if(tileList[i].CurrentType != TileBase.TileType.CHARACTER)
                m_Tiles[(int)coord.x + (int)(MapData.MapSize.x * 0.5f), (int)coord.y + (int)(MapData.MapSize.y * 0.5f)] = tileList[i];
            tileList[i].Init(this);
        }
    }

    public bool IsMoveTile(Vector2 pos)
    {
        bool result = false;
        var coord = Func.GetTileCoord(pos, MapData.TileSize.x);
        var idx = new Vector2Int((int)coord.x + (int)(MapData.MapSize.x * 0.5f), (int)coord.y + (int)(MapData.MapSize.y * 0.5f));

        var tile = m_Tiles[idx.x, idx.y];
        if (tile != null)
            result = tile.CurrentType == TileBase.TileType.FLOOR ? true : false;
        return result;
    }
}
