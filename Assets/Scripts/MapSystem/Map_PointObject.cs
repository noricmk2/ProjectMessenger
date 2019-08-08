using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Map_PointObject : MonoBehaviour
{
    public int pointID;
    public TextMeshProUGUI pointName;
    public RectTransform rectTransform;

    private void Reset()
    {
        pointID = transform.GetSiblingIndex();
    }

    private void OnValidate()
    {
        pointID = transform.GetSiblingIndex();

    }

    public void SetPointData(DataManager.MapData_Point pointData)
    {
        pointID = pointData.ID;
        pointName.text = TextManager.GetSystemText(pointData.Name_Short);
        rectTransform.anchoredPosition = new Vector2(pointData.PosX, pointData.PosY);
    }

    public void OnClickPoint()
    {
        //MapManager.Instance.SelectPoint(id, );
        Debug.Log("Select Point : " + pointName.text);
        //MapManager.Instance.SelectPoint(id, pointList[id].anchoredPosition);

        IngameScene.instance.ingameObject.MapWindow.SelectPoint(pointID, rectTransform.anchoredPosition);
    }

    [ContextMenu("Vali")]
    public void ButtonValidating()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(new UnityEngine.Events.UnityAction(OnClickPoint));
    }
}
