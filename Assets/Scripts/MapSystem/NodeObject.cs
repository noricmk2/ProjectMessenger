using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeObject : MonoBehaviour
{
    [ContextMenu("NodeConnect")]
    public void NodeConnect()
    {

    }

    public NodeData currentNodeData;
    public List<NodeData> nodeDataList;
    public List<RoadObject> roadList;

    private RectTransform m_rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (m_rectTransform == null)
                m_rectTransform = GetComponent<RectTransform>();
            return m_rectTransform;
        }
    }


    //자동으로 노드에 대한 데이터를 업데이트 해주는 곳
    #region DataUpdate 
    [ContextMenu("NodeUpdate")]
    public void NodeUpdate()
    {
        Debug.Log(this + ": Validating");
        if (nodeDataList == null)
            nodeDataList = new List<NodeData>();

        currentNodeData.nodeID = transform.GetSiblingIndex();
        currentNodeData.nodePosition = rectTransform.anchoredPosition;

        if (currentNodeData.nodeObject == null)
        {
            currentNodeData.nodeObject = this;
            currentNodeData.roadObject = null;
        }

        int listCount = nodeDataList.Count;

        for (int i = 0; i < listCount; i++)
        {
            NodeData data = nodeDataList[i];
            if (data.nodeObject != null)
            {
                //data.nodeID = data.nodeObject.currentNodeData.nodeID;
                data.nodeID = data.nodeObject.transform.GetSiblingIndex();
                data.nodePosition = data.nodeObject.rectTransform.anchoredPosition;
                data.centerPosition = Vector2.Lerp(currentNodeData.nodePosition, data.nodeObject.rectTransform.anchoredPosition, 0.5f);
                data.oneway = false;

                bool check = false;
                int nodeCount = data.nodeObject.nodeDataList.Count;
                for (int j = 0; j < nodeCount; j++)
                {
                    if (data.nodeObject.nodeDataList[j].nodeObject == this)
                    {
                        check = true;
                        break;
                    }
                }

                if (!check)
                {
                    data.nodeObject.nodeDataList.Add(currentNodeData);
                }
                else
                {
                    //Debug.Log();
                }

                for (int j = 0; j < nodeCount; j++)
                {
                    //if (currentNodeData.roadObject == null && nodeDataList[i].roadObject == null)
                    if (nodeDataList[i].roadObject == null && data.nodeObject.nodeDataList[j].roadObject == null && data.nodeObject.nodeDataList[j].nodeObject == this)
                    {
                        //Debug.Log(this);
                        //Debug.Log(this + " :: " + nodeDataList[i].nodeObject);

                        GameObject roadObject = new GameObject();

                        Transform parent = transform.parent.parent.GetChild(4);
                        roadObject.transform.SetParent(parent);
                        RoadObject road = roadObject.AddComponent<RoadObject>();
                        road.rectTransform = roadObject.AddComponent<RectTransform>();
                        road.CreateRoad(this, data.nodeObject);

                        nodeDataList[i].roadObject = road;
                        data.nodeObject.nodeDataList[j].roadObject = road;

                        Debug.Log(i + "  :: " + j + "  :: " + this + "  :: " + nodeDataList[i].roadObject + " :: " + data.nodeObject.nodeDataList[j].roadObject);
                    }
                }

                //if (data.roadObject == null)
                //{
                //    Debug.Log(RoadManager.Instance.roadParent);
                //    //GameObject roadObject = new GameObject();
                //    //RoadObject road = roadObject.AddComponent<RoadObject>();
                //    //road.startNode = this;
                //    //road.endNode = data.nodeObject;
                //}
                //else
                //{

                //}
            }
        }
    }

    public Vector2 FindNextCenterPos(int id)
    {
        int listCount = nodeDataList.Count;
        for (int i = 0; i < listCount; i++)
        {
            if (nodeDataList[i].nodeID == id)
            {
                return nodeDataList[i].centerPosition;
            }
        }

        return Vector2.zero;
    }

    //public void

    //public void OnDrawGizmos()
    //{
    //    if (nodeDataList == null)
    //        nodeDataList = new List<NodeData>();

    //    Gizmos.color = Color.green;
    //    int listCount = nodeDataList.Count;
    //    for (int i = 0; i < listCount; i++)
    //    {
    //        //curr
    //        if (nodeDataList[i].nodeObject != null)
    //        {
    //            Gizmos.DrawLine(transform.position, nodeDataList[i].nodeObject.transform.position);
    //        }
    //        //transform.position
    //        //nodeObjectList[i]
    //    }
    //}
    #endregion
}
