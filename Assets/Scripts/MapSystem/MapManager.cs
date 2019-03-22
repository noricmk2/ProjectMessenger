using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    public bool startSelected = false;

    public Vector2 startPoint;

    public void SelectPoint(int id, Vector2 position)
    {
        if (startSelected)
        {
            NodeManager.Instance.CalculatingStart(startPoint, position);
            startPoint = position;
        }
        else
        {
            NodeManager.Instance.roadPositions = new List<Vector3>();
            startSelected = true;
            startPoint = position;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            startSelected = false;
        }
    }
}
