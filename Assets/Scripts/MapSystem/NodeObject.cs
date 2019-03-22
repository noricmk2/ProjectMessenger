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
    public void OnValidate()
    {
        if (nodeDataList == null)
            nodeDataList = new List<NodeData>();

        if (currentNodeData.nodeObject == null)
        {
            currentNodeData.nodeID = transform.GetSiblingIndex();
            currentNodeData.nodeObject = this;
            currentNodeData.nodePosition = rectTransform.anchoredPosition;
        }

        int listCount = nodeDataList.Count;
        for (int i = 0; i < listCount; i++)
        {
            NodeData data = nodeDataList[i];
            if (data.nodeObject != null)
            {
                data.nodeID = data.nodeObject.currentNodeData.nodeID;
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
