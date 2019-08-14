using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour, IPoolObjectBase
{
    [Header("Point")]
    public List<Map_PointObject> allPointList;
    public Transform pointParent;

    [Header("Node")]
    public List<NodeObject> allNodeList;
    public Transform nodeParent;

    [Header("Road")]
    public List<RoadObject> allRoadList;
    public Transform roadParent;

    public LineRenderer lineRenderer;
    public RectTransform playerMarker;

    #region RoadCalculate
    public bool startSelected = false;

    public List<Vector3> roadPositions;
    public float totalDist;

    private List<Vector3> nodeList;
    private List<NodeObject> roadList;

    private Vector3 startPosition;
    private Vector3 endPosition;

    [ContextMenu("InspectorRedload")]
    public void InspectorReload()
    {
        allPointList = new List<Map_PointObject>();
        for (int i = 0; i < pointParent.childCount; i++)
        {
            allPointList.Add(pointParent.GetChild(i).GetComponent<Map_PointObject>());
        }

        allNodeList = new List<NodeObject>();
        for (int i = 0; i < nodeParent.childCount; i++)
        {
            allNodeList.Add(roadParent.GetChild(i).GetComponent<NodeObject>());
        }

        allRoadList = new List<RoadObject>();
        for (int i = 0; i < roadParent.childCount; i++)
        {
            allRoadList.Add(roadParent.GetChild(i).GetComponent<RoadObject>());
        }
    }

    [ContextMenu("NodeUpdate")]
    public void NodeUpdate()
    {
        for (int i = 0; i < allNodeList.Count; i++)
        {
            allNodeList[i].NodeUpdate();
        }
    }

    public void SetData()
    {
        for (int i = 0; i < allPointList.Count; i++)
        {
            allPointList[i].SetPointData(DataManager.Instance.GetMapData_Point(i));
        }
    }

    public void DisplayNodes()
    {
        for (int i = 0; i < allNodeList.Count; i++)
        {
            for (int j = 0; j < allNodeList[i].nodeDataList.Count; j++)
            {
                Debug.Log(allNodeList[i].nodeDataList[j].nodeObject);
                Debug.DrawLine(allNodeList[i].transform.position, allNodeList[i].nodeDataList[j].nodeObject.transform.position, Color.red, 2f);
            }
        }
    }

    //시작점에서 도로까지 이어주는 메소드
    public void CalculatingStart(Vector2 start, Vector2 endPos)
    {
        startPosition = start;
        endPosition = endPos;

        nodeList = new List<Vector3>();
        roadList = new List<NodeObject>();
        totalDist = 0;

        float tempDist = float.MaxValue;
        NodeObject tempNodeObject = null;
        float dist = 0;

        //처음에는 가장 가까운 노드 찾아서 연결
        for (int i = 0; i < allNodeList.Count; i++)
        {
            dist = Vector2.Distance(startPosition, allNodeList[i].rectTransform.anchoredPosition);
            if (dist < tempDist)
            {
                tempDist = dist;
                tempNodeObject = allNodeList[i];
            }
        }
        Debug.Log(tempNodeObject + " : " + tempNodeObject.currentNodeData.nodePosition + " : " + tempDist);

        roadList.Add(tempNodeObject);
        nodeList.Add(startPosition);
        CalculatingRoad(tempNodeObject);
    }

    //목적지까지의 길을 계산하는 재귀함수
    private void CalculatingRoad(NodeObject nodeObj)
    {
        float tempDist = float.MaxValue;
        NodeObject tempNodeObject = null;
        float dist = 0;

        int nodeListCount = nodeObj.nodeDataList.Count;
        for (int i = 0; i < nodeListCount; i++)
        {
            //해당 노드를 선택한 후 부터의 거리 측정
            //dist = CalculatingDistance(nodeSpline, searchIgnoreID, nodeObj.nodeDataList[i].nodeObject, endPos);
            //해당 노드부터 목적지까지의 거리 측정
            dist = Vector2.Distance(nodeObj.nodeDataList[i].nodePosition, endPosition);
            //Debug.Log(dist);
            if (dist <= tempDist)
            {
                tempDist = dist;
                tempNodeObject = nodeObj.nodeDataList[i].nodeObject;
            }
        }

        //Debug.Log(tempNodeObject + " : " + tempNodeObject.currentNodeData.nodePosition + " : " + tempDist);

        float endDist = Vector2.Distance(nodeObj.currentNodeData.nodePosition, endPosition);

        if (endDist < tempDist)
        {
            DisplayRoad();
        }
        else
        {
            roadList.Add(tempNodeObject);
            nodeList.Add(tempNodeObject.rectTransform.anchoredPosition);
            CalculatingRoad(tempNodeObject);
        }
    }

    private void DisplayRoad()
    {
        roadPositions = new List<Vector3>();
        roadPositions.Add(startPosition);

        for (int i = 0; i < roadList.Count; i++)
        {
            if (i < roadList.Count - 1)
            {
                for (int j = 0; j < roadList[i].nodeDataList.Count; j++)
                {
                    if (roadList[i + 1].currentNodeData.nodeID == roadList[i].nodeDataList[j].nodeID)
                    {
                        List<Vector3> roadObjectRoadList = roadList[i].nodeDataList[j].roadObject.roadList;
                        float zeroDist = Vector2.Distance(roadList[i].currentNodeData.nodePosition, roadObjectRoadList[0]);
                        float lengthDist = Vector2.Distance(roadList[i].currentNodeData.nodePosition, roadObjectRoadList[roadObjectRoadList.Count - 1]);

                        if (zeroDist > lengthDist)
                            roadObjectRoadList.Reverse();
                        roadPositions.AddRange(roadObjectRoadList);
                    }
                }
            }
        }
        roadPositions.Add(endPosition);
        lineRenderer.positionCount = roadPositions.Count;
        lineRenderer.SetPositions(roadPositions.ToArray());
        lineRenderer.Simplify(0.3f);
    }

    private Vector2 NearestPos(Vector2 startPos, params Vector2[] positions)
    {
        string debug = string.Empty;

        Vector2 nextPos = startPos;
        float minDist = float.MaxValue;
        for (int i = 0; i < positions.Length; i++)
        {
            float dist = Vector2.Distance(positions[i], startPos);
            if (dist < minDist)
            {
                minDist = dist;
                nextPos = positions[i];
            }
        }

        return nextPos;
    }
    #endregion

    public Map_PointObject GetPointObject(DataManager.MapData_Point pointData)
    {
        for (int i = 0; i < allPointList.Count; i++)
        {
            if (pointData.ID == allPointList[i].pointID)
                return allPointList[i];
        }

        return null;
    }

    public Map_PointObject HighlightPointObject(DataManager.MapData_Point pointData)
    {
        for (int i = 0; i < allPointList.Count; i++)
        {
            allPointList[i].Highlight(pointData.ID == allPointList[i].pointID);
        }
        return null;
    }

    public void PopAction()
    {

    }

    public void PushAction()
    {

    }
}
