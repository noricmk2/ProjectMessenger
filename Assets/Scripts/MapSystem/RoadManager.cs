using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : Singleton<RoadManager>
{
    public List<RoadObject> allRoadObjectList;

    public Transform roadParent;

    private void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            allRoadObjectList.Add(transform.GetChild(i).GetComponent<RoadObject>());
        }
    }

    //길 시각화 켜기
    [ContextMenu("Road Visual On")]
    public void RoadVisualOn()
    {

    }

    //길 시각화 끄기
    [ContextMenu("Road Visual Off")]
    public void RoadVisualOff()
    {

    }
}
