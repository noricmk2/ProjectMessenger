using UnityEngine;
using System.Collections;
using MSUtil;

public class IsometricGrid : MonoBehaviour
{
    // universal grid scale
    public float GridScale = 1f;

    // extents of the grid
    public int MinX = -100;
    public int MinY = -100;
    public int MaxX = 100;
    public int MaxY = 100;

    // nudges the whole grid rel
    public Vector3 gridOffset = Vector3.zero;

    // is this an XY or an XZ grid?
    public bool TopDownGrid = false;

    // choose a colour for the gizmos
    public int GizmoMajorLines = 5;
    public Color GizmoLineColor = new Color(1f, 0.4f, 0.3f, 1f);

    // rename + centre the gameobject upon first time dragging the script into the editor. 
    void Reset()
    {
        transform.position = Vector3.zero;
    }

    public void SetOption(int minX = 0, int minY = 0, int maxX = 0, int maxY = 0, float gridScale = 1f, bool isTop = false) 
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
        GridScale = gridScale;
        TopDownGrid = isTop;
    }

    // draw the grid :) 
    void OnDrawGizmos()
    {
        // orient to the gameobject, so you can rotate the grid independently if desired
        Gizmos.matrix = transform.localToWorldMatrix;

        // set colours
        Color dimColor = new Color(GizmoLineColor.r, GizmoLineColor.g, GizmoLineColor.b, 0.25f * GizmoLineColor.a);
        Color brightColor = Color.Lerp(Color.white, GizmoLineColor, 0.75f);

        // draw the horizontal lines
        for (int x = MinX; x < MaxX + 1; x++)
        {
            // find major lines
            Gizmos.color = (x % GizmoMajorLines == 0 ? GizmoLineColor : dimColor);
            if (x == 0)
                Gizmos.color = brightColor;

            Vector3 pos1 = Func.Cart2Iso(new Vector3(x, MinY, 0) * GridScale);
            Vector3 pos2 = Func.Cart2Iso(new Vector3(x, MaxY, 0) * GridScale);

            // convert to topdown/overhead units if necessary
            if (TopDownGrid)
            {
                pos1 = new Vector3(pos1.x, 0, pos1.y);
                pos2 = new Vector3(pos2.x, 0, pos2.y);
            }

            Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
        }

        // draw the vertical lines
        for (int y = MinY; y < MaxY + 1; y++)
        {
            // find major lines
            Gizmos.color = (y % GizmoMajorLines == 0 ? GizmoLineColor : dimColor);
            if (y == 0)
                Gizmos.color = brightColor;

            Vector3 pos1 = Func.Cart2Iso(new Vector3(MinX, y, 0) * GridScale);
            Vector3 pos2 = Func.Cart2Iso(new Vector3(MaxX, y, 0) * GridScale);

            // convert to topdown/overhead units if necessary
            if (TopDownGrid)
            {
                pos1 = new Vector3(pos1.x, 0, pos1.y);
                pos2 = new Vector3(pos2.x, 0, pos2.y);
            }

            Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
        }
    }
}