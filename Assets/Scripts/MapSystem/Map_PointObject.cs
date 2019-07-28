using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map_PointObject : MonoBehaviour
{
    public int pointID;
    public Text pointName;

    private void Reset()
    {
        pointID = transform.GetSiblingIndex();
    }

    public void SetPointData(DataManager.MapData_Point pointData)
    {
        pointID = pointData.ID;
        pointName.text = pointData.Name;

    }

    public void OnClickPoint()
    {
        //MapManager.Instance.SelectPoint(id, );

        //Debug.Log("Select Point : " + id);
        //MapManager.Instance.SelectPoint(id, pointList[id].anchoredPosition);
    }
}
