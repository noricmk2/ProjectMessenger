using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RoadObject : MonoBehaviour
{
    //public 
    public RectTransform rectTransform;
    public NodeObject startNode;
    public NodeObject endNode;
    public List<Vector3> roadList;
    public float totalDist;
    public AnimationCurve roadCurve;
    private float normalizeStep = 0.05f;

    //public void OnValidate()
    //{
    //    if (startNode != null && endNode != null)
    //    {
    //        transform.position = Vector2.Lerp(startNode.transform.position, endNode.transform.position, 0.5f);
    //    }
    //}

    public void CreateRoad(NodeObject start, NodeObject end)
    {
        startNode = start;
        endNode = end;

        rectTransform.anchoredPosition = Vector2.Lerp(startNode.rectTransform.anchoredPosition, endNode.rectTransform.anchoredPosition, 0.5f);
        rectTransform.sizeDelta = Vector2.one;
        rectTransform.localScale = Vector3.one;

        roadCurve = new AnimationCurve();
        roadCurve.AddKey(0, 1);
        roadCurve.AddKey(1, 1);
    }

    public void Update()
    {
        gameObject.name = startNode.gameObject.name + "<->" + endNode.gameObject.name;
        //Debug.DrawLine(startNode.rectTransform.anchoredPosition, endNode.rectTransform.anchoredPosition, Color.green);
        //Debug.DrawLine(startNode.transform.position, endNode.transform.position, Color.green);

        Vector2 prevPoint = Vector2.zero;
        Vector2 currentPoint = Vector2.zero;
        //for (int i = 0; i < roadCurve.length; i++)
        //{
        //    currentPoint = Vector2.Lerp(startNode.transform.position, endNode.transform.position, roadCurve[i].time);
        //    if (i == 0)
        //    {
        //        Debug.DrawLine(startNode.transform.position, currentPoint, Color.green);
        //    }
        //    else if (i == roadCurve.length - 1)
        //    {
        //        Debug.DrawLine(prevPoint, currentPoint, Color.green);
        //        Debug.DrawLine(currentPoint, endNode.transform.position, Color.green);
        //    }
        //    else
        //    {
        //        Debug.DrawLine(prevPoint, currentPoint, Color.green);
        //        Debug.Log(currentPoint);
        //    }
        //    prevPoint = currentPoint;
        //}
        roadList = new List<Vector3>();
        totalDist = 0;

        Vector3 startPos = startNode.transform.position.x < endNode.transform.position.x ? startNode.transform.position : endNode.transform.position;
        Vector3 endPos = startNode.transform.position.x > endNode.transform.position.x ? startNode.transform.position : endNode.transform.position;

        prevPoint = startPos;

        float CurveDirY = 1;

        if (startPos.y > endPos.y)
            CurveDirY = 1f;
        else
            CurveDirY = -1f;

        roadList.Add(WorldToScreen(startPos));

        for (float i = normalizeStep; i < roadCurve[roadCurve.length - 1].time; i += normalizeStep)
        {
            Vector3 LerpPos = Vector3.Lerp(startPos, endPos, i);
            float curveValue = (roadCurve.Evaluate(i) - 1);

            currentPoint = new Vector3(LerpPos.x + (roadCurve.Evaluate(i) - 1) * CurveDirY, LerpPos.y + (roadCurve.Evaluate(i) - 1), 0);
            totalDist += Vector2.Distance(prevPoint, currentPoint);
            roadList.Add(WorldToScreen(currentPoint));
            Debug.DrawLine(prevPoint, currentPoint, Color.green);
            prevPoint = currentPoint;
            //lastPos = GetPoint(i);
            //GL.Vertex3(lastPos.x, lastPos.y, lastPos.z);
        }

        currentPoint = endPos;
        Debug.DrawLine(prevPoint, currentPoint, Color.green);
        roadList.Add(WorldToScreen(currentPoint));
    }

    private Vector2 GetPoint(float step)
    {
        float t = step * (roadCurve.length - 1);

        int startIndex = (int)t;
        int endIndex = startIndex + 1;

        if (endIndex == roadCurve.length)
            endIndex = 0;

        Vector2 startPoint = Vector2.Lerp(startNode.transform.position, endNode.transform.position, roadCurve.keys[startIndex].time);
        Vector2 endPoint = Vector2.Lerp(startNode.transform.position, endNode.transform.position, roadCurve.keys[endIndex].time);

        //roadCurve.keys[startIndex].

        float localT = t - startIndex;
        float oneMinusLocalT = 1f - localT;
        return endPoint;
        //return oneMinusLocalT * oneMinusLocalT * oneMinusLocalT * startPoint +
        //           3f * oneMinusLocalT * oneMinusLocalT * localT * startPoint.followingControlPointPosition +
        //           3f * oneMinusLocalT * localT * localT * endPoint.precedingControlPointPosition +
        //           localT * localT * localT * endPoint;
    }

    private Vector3 WorldToScreen(Vector3 position)
    {
        Vector3 viewPort = Camera.main.WorldToViewportPoint(position);
        return new Vector3(1280f * viewPort.x - 640f, 720 * viewPort.y - 360f, 0);
    }

    //private Vector2 GetPosition(CurveSelection selection)
    //{
    //    Keyframe keyframe = selection.keyframe;
    //    Vector2 vector2_1 = new Vector2(keyframe.time, keyframe.value);
    //    float num = 50f;
    //    if (selection.type == CurveSelection.SelectionType.InTangent)
    //    {
    //        Vector2 vec = new Vector2(1f, keyframe.inTangent);
    //        if ((double)keyframe.inTangent == double.PositiveInfinity)
    //            vec = new Vector2(0.0f, -1f);
    //        Vector2 vector2_2 = this.NormalizeInViewSpace(vec);
    //        return vector2_1 - vector2_2 * num;
    //    }
    //    if (selection.type != CurveSelection.SelectionType.OutTangent)
    //        return vector2_1;
    //    Vector2 vec1 = new Vector2(1f, keyframe.outTangent);
    //    if ((double)keyframe.outTangent == double.PositiveInfinity)
    //        vec1 = new Vector2(0.0f, -1f);
    //    Vector2 vector2_3 = this.NormalizeInViewSpace(vec1);
    //    return vector2_1 + vector2_3 * num;
    //}
}
