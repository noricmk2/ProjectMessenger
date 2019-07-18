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

    private Vector3 startPosition;
    private Vector3 endPosition;


    private void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            allNodeObjectList.Add(transform.GetChild(i).GetComponent<NodeObject>());
        }
    }

    [ContextMenu("NodeUpdate")]
    public void NodeUpdate()
    {
        for (int i = 0; i < allNodeObjectList.Count; i++)
        {
            allNodeObjectList[i].NodeUpdate();
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
        startPosition = start;
        endPosition = endPos;

        nodeList = new List<Vector3>();
        roadList = new List<NodeObject>();
        totalDist = 0;

        float tempDist = float.MaxValue;
        NodeObject tempNodeObject = null;
        float dist = 0;

        //처음에는 가장 가까운 노드 찾아서 연결
        for (int i = 0; i < allNodeObjectList.Count; i++)
        {
            //dist = Vector2.Distance(start, allNodeObjectList[i].rectTransform.anchoredPosition) + Vector2.Distance(endPos, allNodeObjectList[i].rectTransform.anchoredPosition);
            dist = Vector2.Distance(startPosition, allNodeObjectList[i].rectTransform.anchoredPosition);
            if (dist < tempDist)
            {
                tempDist = dist;
                tempNodeObject = allNodeObjectList[i];
            }
        }
        Debug.Log(tempNodeObject + " : " + tempNodeObject.currentNodeData.nodePosition + " : " + tempDist);

        roadList.Add(tempNodeObject);
        nodeList.Add(startPosition);
        //nodeList.Add(tempNodeObject.rectTransform.anchoredPosition);
        CalculatingRoad(tempNodeObject);
    }

    //목적지까지의 길을 계산하는 재귀함수
    private void CalculatingRoad(NodeObject nodeObj)
    {
        float tempDist = float.MaxValue;
        NodeObject tempNodeObject = null;
        float dist = 0;

        int nodeListCount = nodeObj.nodeDataList.Count;
        //searchIgnoreID = new List<int>();
        //searchIgnoreID.Add(nodeObj.currentNodeData.nodeID);
        //nodeSpline = new List<NodeObject>();
        //nodeSpline.Add(nodeObj);
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

        Debug.Log(tempNodeObject + " : " + tempNodeObject.currentNodeData.nodePosition + " : " + tempDist);

        float endDist = Vector2.Distance(nodeObj.currentNodeData.nodePosition, endPosition);

        if (endDist < tempDist)
        {
            //nodeList.Add(endPos);
            DisplayRoad();
        }
        else
        {
            //nodeList.Add(Vector2.Lerp(nodeObj.rectTransform.anchoredPosition, tempNodeObject.rectTransform.anchoredPosition, 0.5f));
            roadList.Add(tempNodeObject);
            nodeList.Add(tempNodeObject.rectTransform.anchoredPosition);
            CalculatingRoad(tempNodeObject);
        }
    }

    //private List<int> searchIgnoreID = new List<int>();
    //private List<NodeObject> nodeSpline = new List<NodeObject>();

    ////목적지까지의 거리를 최저로 계산하는 메소드
    //private float CalculatingDistance(List<NodeObject> nodesList, List<int> ignoreIDList, NodeObject nodeObj, Vector2 endPos)
    //{
    //    float tempDist = float.MaxValue;
    //    float dist = 0;
    //    int nodeListCount = nodeObj.nodeDataList.Count;
    //    for (int i = 0; i < nodeListCount; i++)
    //    {
    //        if (ignoreIDList.Contains(nodeObj.nodeDataList[i].nodeID))
    //        {
    //            continue;
    //        }
    //        else
    //        {
    //            if (Vector2.Distance(nodeObj.currentNodeData.nodePosition, endPos) < Vector2.Distance(nodeObj.nodeDataList[i].nodePosition, endPos))
    //            {
    //                string NodeString = string.Empty;
    //                for (int j = 0; j < nodesList.Count; j++)
    //                {
    //                    NodeString += nodesList[j].gameObject.name;
    //                    NodeString += " -> ";
    //                }
    //                Debug.Log(NodeString);
    //                return Vector2.Distance(nodeObj.currentNodeData.nodePosition, endPos);
    //            }
    //            else
    //            {
    //                List<int> ignoreList = ignoreIDList;
    //                ignoreList.Add(nodeObj.currentNodeData.nodeID);
    //                List<NodeObject> nodenode = nodesList;
    //                nodenode.Add(nodeObj);
    //                //해당 노드를 선택한 후 부터의 거리 측정
    //                dist = CalculatingDistance(nodenode, ignoreList, nodeObj.nodeDataList[i].nodeObject, endPos);
    //                if (dist <= tempDist)
    //                {
    //                    tempDist = dist;
    //                }
    //            }
    //        }
    //        return tempDist;
    //    }
    //    return 0;
    //}

    //LineRenderer에 길 데이터를 넣어주는 메소드
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

                        if(zeroDist > lengthDist)
                            roadObjectRoadList.Reverse();
                        //if (roadList[i].nodeDataList[j].roadObject.startNode.currentNodeData.nodeID == roadList[i].nodeDataList[j].nodeID)
                        //{
                        //    roadObjectRoadList.Reverse();
                        //}
                        roadPositions.AddRange(roadObjectRoadList);
                        //for (int k = 0; k < roadList[i].nodeDataList[j].roadObject.roadList.Count; k++)
                        //{
                        //    roadPositions.Add(roadList[i].nodeDataList[j].roadObject.roadList[k]);
                        //}
                        //Debug.Log(roadList[i + 1].currentNodeData.nodeID + " :: " + roadList[i].nodeDataList[j].nodeID);
                        //Debug.Log(roadList[i].nodeDataList[j].roadObject.roadList.Count);
                        //roadPositions.AddRange(roadList[i].nodeDataList[j].roadObject.roadList);
                    }
                }
            }

            //if (i < roadList.Count - 1)
            //{
            //    //roadList[i + 1]
            //}
            //else
            //{

            //}
            //if (i == roadList.Count - 2)
            //{
            //    nodeList.Add(roadList[i].currentNodeData.nodePosition);
            //    Vector2 nextPos = NearestPos(endPos,
            //        roadList[i].FindNextCenterPos(roadList[i + 1].currentNodeData.nodeID),
            //        roadList[i + 1].currentNodeData.nodePosition);
            //    nodeList.Add(nextPos);
            //    break;
            //}
            //else if (i == 0)
            //{
            //    Vector2 nextPos = NearestPos(nodeList[0], roadList[i].currentNodeData.nodePosition,
            //        roadList[i].FindNextCenterPos(roadList[i + 1].currentNodeData.nodeID),
            //        roadList[i + 1].currentNodeData.nodePosition);
            //    nodeList.Add(nextPos);
            //}
            //else
            //{
            //    nodeList.Add(roadList[i].currentNodeData.nodePosition);
            //}
            //nodeList.Add();



            //roadList[i].currentNodeData.nodePosition
        }

        //nodeList.Add(endPos);

        //roadPositions.AddRange(nodeList);

        roadPositions.Add(endPosition);
        roadRenderer.positionCount = roadPositions.Count;
        roadRenderer.SetPositions(roadPositions.ToArray());

        //roadRenderer.Simplify(20);//경로 간소화
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
