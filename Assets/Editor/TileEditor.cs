using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using MSUtil;
using System.IO;

public class IsometricEditor : EditorWindow
{
    private static string savePath = "";
    private string[] paintOptions = new string[] { "Nothing", "Paint", "Erase" };
    private int currentMode = 0;
    private GameObject currentTilePrefab;
    private GameObject parent;
    private string spriteName = "";
    private Vector2 gridSize = new Vector2(1, 1);
    private Vector2 tileSize = new Vector2(1, 1);
    private int selectedIdx = -1;
    private TileBase.TileType selectedType;
    private static GameObject[,] tileMap = new GameObject[50, 50];

    [MenuItem("LetterGirl/IsometricEditor")]
    static void Init()
    {
        // 생성되어있는 윈도우를 가져온다. 없으면 새로 생성한다. 싱글턴 구조인듯하다.
        var window = GetWindow<IsometricEditor>();
        window.Show();
        var grid = GameObject.Find("Grid");
        if (grid == null)
        {
            grid = new GameObject("Grid");
            var gridGizmo = grid.AddComponent<IsometricGrid>();
            gridGizmo.SetOption(-25, -25, 25, 25);
        }
    }

    private void OnGUI()
    {
        var atlas = Resources.Load<SpriteAtlas>("SpriteAtlas/atlas_tile");
        var sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);
        var tileSpriteList = new List<Sprite>(sprites);

        GUILayout.Label("PaintMode", EditorStyles.boldLabel);
        currentMode = EditorGUILayout.Popup(selectedIndex: currentMode, displayedOptions: paintOptions, GUILayout.MaxWidth(150));

        GUILayout.Label("TileList", EditorStyles.boldLabel);
        TileButtonCreate(tileSpriteList);

        GUILayout.Label("Other", EditorStyles.boldLabel);
        tileSize = EditorGUILayout.Vector2Field("TileSize", tileSize, GUILayout.MaxWidth(150));
        GUILayout.BeginHorizontal();
        savePath = "Assets/Resources/Prefab";
        savePath = EditorGUILayout.TextField("SavePath", savePath, GUILayout.MaxWidth(450));
        if (GUILayout.Button("Save", GUILayout.MaxWidth(80)))
        {
            if(parent != null)
                SaveTileMap();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Close", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Close"))
        {
            Reset();
            this.Close();
        }
    }

    private void Reset()
    {
        currentTilePrefab = null;
        parent = null;
        spriteName = "";
        gridSize = Vector2.one;
        tileSize = Vector2.one;
        selectedIdx = -1;
        selectedType = TileBase.TileType.FLOOR;
        tileMap = new GameObject[50, 50];
    }

    private void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (currentMode == 1 || currentMode == 2)
        {
            var mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            var tilePos = GetTilePos();

            DrawTileMapGizmo(tilePos);
            OnClickScene(tilePos);
            sceneView.Repaint();
        }
    }

    private Vector2Int GetTilePos()
    {
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        var mousePos = ray.origin;
        mousePos.z = 0;
        mousePos = Func.Iso2Cart(mousePos);

        return new Vector2Int(Mathf.FloorToInt(mousePos.x / gridSize.x), Mathf.FloorToInt(mousePos.y / gridSize.y));
    }

    private void DrawTileMapGizmo(Vector2Int tilePos)
    {
        var topLeft = new Vector3(tilePos.x, tilePos.y + gridSize.y);
        var topRight = tilePos + gridSize;
        var bottomLeft = new Vector3(tilePos.x, tilePos.y);
        var bottomRight = new Vector3(tilePos.x + gridSize.x, tilePos.y);

        topLeft = Func.Cart2Iso(topLeft);
        topRight = Func.Cart2Iso(topRight);
        bottomLeft = Func.Cart2Iso(bottomLeft);
        bottomRight = Func.Cart2Iso(bottomRight);

        Handles.color = Color.green;
        Vector3[] lines = { topLeft, topRight, topRight, bottomRight, bottomRight, bottomLeft, bottomLeft, topLeft };
        Handles.DrawLines(lines);
    }

    private void OnClickScene(Vector2Int tilePos)
    {
        if (Event.current.type == EventType.Layout)
            HandleUtility.AddDefaultControl(0); // Consume the event

        if (currentTilePrefab != null && Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            int tileX = (int)(tilePos.x + tileMap.GetLength(0) * 0.5f);
            int tileY = (int)(tilePos.y + tileMap.GetLength(1) * 0.5f);

            if (tileMap.GetLength(0) <= tileX || tileMap.GetLength(1) <= tileY
                || tileX < 0 || tileY < 0)
                return;

            switch (currentMode)
            {
                case 1:
                    {
                        if (tileMap[tileX, tileY] != null && 
                            (selectedType == TileBase.TileType.WALL || selectedType == TileBase.TileType.FLOOR))
                            return;

                        var atlas = Resources.Load<SpriteAtlas>("SpriteAtlas/atlas_tile");

                        if (parent == null)
                        {
                            parent = GameObject.Find("TileMap");
                            if (parent == null)
                            {
                                parent = new GameObject("TileMap");
                                parent.AddComponent<TileMap>();
                            }
                        }

                        GameObject gameObject = PrefabUtility.InstantiatePrefab(currentTilePrefab) as GameObject;
                        var cartPos = new Vector2(tilePos.x + gridSize.x * 0.5f, tilePos.y + gridSize.y * 0.5f);
                        gameObject.transform.localPosition = Func.Cart2Iso(cartPos);
                        gameObject.transform.SetParent(parent.transform);
                        gameObject.transform.localScale = tileSize;

                        var tile = gameObject.GetComponent<TileBase>();
                        tile.TargetSprite.sprite = atlas.GetSprite(spriteName);

                        if (selectedType == TileBase.TileType.FLOOR)
                            tile.TargetSprite.sortingOrder = -(int)(gameObject.transform.localPosition.y * 100) - 999;
                        else
                            tile.TargetSprite.sortingOrder = -(int)(gameObject.transform.localPosition.y * 100);

                        tile.SetType(selectedType);
                        tile.TileResourceName = spriteName;
                        tileMap[tileX, tileY] = gameObject;
                    }
                    break;
                case 2:
                    {
                        if (tileMap[tileX, tileY] != null)
                        {
                            DestroyImmediate(tileMap[tileX, tileY]);
                            tileMap[tileX, tileY] = null;
                        }
                    }
                    break;
            }
        }
    }

    private void SaveTileMap()
    {
        var asset = ScriptableObject.CreateInstance<TileMapData>();
        asset.MapInfo = new int[50 * 50];
        for (int i = 0; i < tileMap.GetLength(0); ++i)
        {
            for (int j = 0; j < tileMap.GetLength(1); ++j)
            {
                if (tileMap[i, j] != null)
                {
                    var tile = tileMap[i, j].GetComponent<TileBase>();
                    var type = tile.CurrentType;
                    if (type == TileBase.TileType.CHARACTER)
                        type = TileBase.TileType.FLOOR;
                    asset.MapInfo[i * tileMap.GetLength(0) + j] = (int)type;
                }
                else
                    asset.MapInfo[i * tileMap.GetLength(0) + j] = -1;
            }
        }

        asset.TileSize = Vector2.one;
        asset.MapSize = new Vector2Int(50, 50);

        if(!AssetDatabase.IsValidFolder("Assets/Resources/TileMapData"))
            AssetDatabase.CreateFolder("Assets/Resources", "TileMapData");
        AssetDatabase.CreateAsset(asset, "Assets/Resources/TileMapData/IsometricMap.asset");
        AssetDatabase.SaveAssets();

        PrefabUtility.SaveAsPrefabAsset(parent, savePath + "/IsometricTileMap.prefab");
    }

    private void TileButtonCreate(List<Sprite> spriteList)
    {
        var prefabs = Resources.LoadAll<GameObject>("Prefab/TileMap");
        var tilePrefabDic = new Dictionary<TileBase.TileType, GameObject>();
        for (int i = 0; i < prefabs.Length; ++i)
        {
            var type = Func.GetEnum<TileBase.TileType>(prefabs[i].name.ToUpper());
            tilePrefabDic[type] = prefabs[i];
        }

        var tileList = new List<GUIContent>();
        if (spriteList != null && spriteList.Count > 0)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                var texture = spriteList[i].texture;
                tileList.Add(new GUIContent(texture));
            }

            selectedIdx = GUILayout.SelectionGrid(selectedIdx, tileList.ToArray(), 6, GUILayout.MaxWidth(500), GUILayout.MaxHeight(70));
            if (selectedIdx >= 0)
            {
                selectedType = TileBase.TileType.FLOOR;
                if (spriteList[selectedIdx].name.Contains("floor"))
                    selectedType = TileBase.TileType.FLOOR;
                else if (spriteList[selectedIdx].name.Contains("wall"))
                    selectedType = TileBase.TileType.WALL;
                else if (spriteList[selectedIdx].name.Contains("object"))
                    selectedType = TileBase.TileType.OBJECT;
                else if (spriteList[selectedIdx].name.Contains("character"))
                    selectedType = TileBase.TileType.CHARACTER;
                currentTilePrefab = tilePrefabDic[selectedType];
                spriteName = spriteList[selectedIdx].name;
                spriteName = spriteName.Replace("(Clone)", "");
            }
        }
    }
}
