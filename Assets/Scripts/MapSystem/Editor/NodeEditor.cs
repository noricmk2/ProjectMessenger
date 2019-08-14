using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(NodeObject))]
public class NodeEditor : Editor
{
    void OnSceneGUI()
    {
        NodeObject nodeObject = (NodeObject)target;
        if (nodeObject == null)
            return;

        Handles.color = Color.blue;
        Handles.Label(nodeObject.transform.position + Vector3.up * 0.1f, nodeObject.name);
    }
}
