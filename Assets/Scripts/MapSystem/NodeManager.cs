using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    public List<NodeObject> allNodeObjectList;
    private List<Vector3> nodeList;
    private List<NodeObject> roadList;

    public List<Vector3> roadPositions;
    public LineRenderer roadRenderer;
    public float totalDist;


    private void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            allNodeObjectList.Add(transform.GetChild(i).GetComponent<NodeObject>());
        }
    }

    public void DisplayNodes()
    {
        for (int i = 0; i < allNodeObjectList.Count; i++)
        {
            for (int j = 0; j < allNodeObjectList[i].nodeDataList.Count; j++)
            {
                Debug.Log(allNodeObjectList[i].nodeDataList[j].nodeObject);
                Debug.DrawLine(allNodeObjectList[i].transform.position, allNodeObjectList[i].nodeDataList[j].nodeObject.transform.position, Color.green, 2f);
            }
        }
    }

    //시작점에서 도로까지 이어주는 메소드
    public void CalculatingStart(Vector2 start, Vector2 endPos)
    {
        nodeList = new List<Vector3>();
        roadList = new List<NodeObject>();
        totalDist = 0;

        float tempDist = float.MaxValue;
        NodeObject tempNodeObject = null;
        float dist = 0;

        for (int i = 0; i < allNodeObjectList.Count; i++)
        {
            //dist = Vector2.Distance(start, allNodeObjectList[i].rectTransform.anchoredPosition) + Vector2.Distance(endPos, allNodeObjectList[i].rectTransform.anchoredPosition);
            dist = Vector2.Distance(start, allNodeObjectList[i].rectTransform.anchoredPosition);
            if (dist < tempDist)
            {
                tempDist = dist;
                tempNodeObject = allNodeObjectList[i];
            }
        }
        Debug.Log(tempNodeObject + " : " + tempNodeObject.currentNodeData.nodePosition + " : " + tempDist);

        roadList.Add(tempNodeObject);
        nodeList.Add(start);
        //nodeList.Add(tempNodeObject.rectTransform.anchoredPosition);
        CalculatingRoad(tempNodeObject, endPos);
    }

    //목적지까지의 길을 이어주는 재귀함수
    private void CalculatingRoad(NodeObject nodeObj, Vector2 endPos)
    {
        float tempDist = float.MaxValue;
        NodeObject tempNodeObject = null;
        float dist = 0;

        int nodeListCount = nodeObj.nodeDataList.Count;
        for (int i = 0; i < nodeListCount; i++)
        {
            dist = Vector2.Distance(nodeObj.nodeDataList[i].nodePosition, endPos);
            if (dist < tempDist)
            {
                tempDist = dist;
                tempNodeObject = nodeObj.nodeDataList[i].nodeObject;
            }
        }

        Debug.Log(tempNodeObject + " : " + tempNodeObject.currentNodeData.nodePosition + " : " + tempDist);

        float endDist = Vector2.Distance(nodeObj.currentNodeData.nodePosition, endPos);

        if (endDist < tempDist)
        {
            //nodeList.Add(endPos);
            DisplayRoad(endPos);
        }
        else
        {
            //nodeList.Add(Vector2.Lerp(nodeObj.rectTransform.anchoredPosition, tempNodeObject.rectTransform.anchoredPosition, 0.5f));
            roadList.Add(tempNodeObject);
            //nodeList.Add(tempNodeObject.rectTransform.anchoredPosition);
            CalculatingRoad(tempNodeObject, endPos);
        }
    }

    private void DisplayRoad(Vector2 endPos)
    {
        if (roadList.Count > 2)
        {
            for (int i = 0; i < roadList.Count; i++)
            {
                if (i == roadList.Count - 2)
                {
                    nodeList.Add(roadList[i].currentNodeData.nodePosition);
                    Vector2 nextPos = NearestPos(endPos, 
                        roadList[i].FindNextCenterPos(roadList[i + 1].currentNodeData.nodeID),
                        roadList[i + 1].currentNodeData.nodePosition);
                    nodeList.Add(nextPos);
                    break;
                }
                else if (i == 0)
                {
                    Vector2 nextPos = NearestPos(nodeList[0], roadList[i].currentNodeData.nodePosition,
                        roadList[i].FindNextCenterPos(roadList[i + 1].currentNodeData.nodeID),
                        roadList[i + 1].currentNodeData.nodePosition);
                    nodeList.Add(nextPos);
                }
                else
                {
                    nodeList.Add(roadList[i].currentNodeData.nodePosition);
                }
                //nodeList.Add();
            }
        }
        nodeList.Add(endPos);

        roadPositions.AddRange(nodeList);
        roadRenderer.positionCount = roadPositions.Count;
        roadRenderer.SetPositions(roadPositions.ToArray());
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
}
