using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeData
{
    public int nodeID;
    public NodeObject nodeObject;
    public Vector2 centerPosition;
    public Vector2 nodePosition;
    public bool oneway;
    public bool construction;
}

