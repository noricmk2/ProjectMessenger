using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MSUtil;
using TMPro;

public class Window_Map : WindowBase
{
    [Header("Object")]
    public Transform mapObjectParent;
    public MapObject mapObject;
    public Image logoImage;

    [Header("Icon")]
    public Image iconMapImage;
    public Image iconPinImage;
    public Image iconCircleImage;

    [Header("FuelObject")]
    public Image fuelGauge;
    public TextMeshProUGUI fuelText;

    [Header("LetterList")]
    public Transform LetterObjectsParent;
    public List<LetterListObject> letterObjectList;

    [Header("ETC")]
    public Transform startButtonLayout;
    public Transform markerParent;

    public CharacterObject MainCharacter;
    public GameObject chatTriggerObject;

    private DataManager.LetterData currentPointLetterData = null;

    public void OpenMap()
    {
        //MapObject Setting
        mapObject = ObjectFactory.Instance.ActivateObject<MapObject>();
        mapObject.transform.SetParent(mapObjectParent);
        mapObject.transform.localPosition = Vector3.zero;
        mapObject.transform.localScale = Vector3.one;
        mapObject.SetData();
        UserInfo.Instance.CurrentGameData.CurrentMapProgress++;

        List<UserInfo.ItemData> bagItemList = UserInfo.Instance.GetBagItemList();
        Dictionary<int, DataManager.LetterData> letterDataDic = new Dictionary<int, DataManager.LetterData>();

        letterObjectList = new List<LetterListObject>();
        for (int i = 0; i < bagItemList.Count; i++)
        {
            if (bagItemList[i].Type == eItemType.Letter)
            {
                //LetterListObject letterObject = ObjectFactory.Instance.ActivateObject<LetterListObject>();
                //letterObject.transform.SetParent(LetterObjectsParent);
                //letterObject.transform.localScale = Vector3.one;
                //letterObject.SetLetterObject(DataManager.Instance.GetLetterData(bagItemList[i].ID));
                //letterObjectList.Add(letterObject);

                DataManager.LetterData letterData = DataManager.Instance.GetLetterData(bagItemList[i].ID);
                letterDataDic.Add(letterData.Destination, letterData);
            }
        }

        for (int i = 0; i < mapObject.allPointList.Count; i++)
        {
            if (letterDataDic.ContainsKey(mapObject.allPointList[i].pointID))
            {
                mapObject.allPointList[i].SetPointLetterData(letterDataDic[mapObject.allPointList[i].pointID]);
            }
        }

        MainCharacter.Init(ConstValue.CHARACTER_NIKA_ID);
        MainCharacter.Bubble.SetActive(false);
        chatTriggerObject.SetActive(false);

        if (UserInfo.Instance.CurrentGameData.CurrentMapProgress > 1)
        {
            EndPhase();
        }
    }

    public void ArrivalPoint()
    {
        //도착
        chatTriggerObject.SetActive(true);
        MainCharacter.Bubble.SetActive(true);
        MainCharacter.CharacterAnimation.SetAnimation(eCharacterState.IDLE, true, ConstValue.DEFAULT_ANIM_FPS);

        currentPointLetterData = null;
        currentPointLetterData = SearchLetterData(UserInfo.Instance.CurrentGameData.CurrentMapProgress);

        if (currentPointLetterData == null)
        {
            MainCharacter.Bubble.SetText("여기 왜 왔지..");
            return;
        }
        else
        {
            MainCharacter.Bubble.SetText("목적지에 도착했다.");
        }

    }

    public void EnterPoint()
    {
        if (currentPointLetterData == null)
        {
            StartNextPoint();
            return;
        }

        if (currentPointLetterData.Stage == eStageTag.NONE)
        {
            if (currentPointLetterData.LetterType == eLetterType.Junk)
            {
                Debug.Log("찌라시 보상");
            }
            else
            {
                Debug.LogError("Stage Not Included Event LetterData");
            }
        }
        else
        {
            if (UserInfo.Instance.SetNextStage(currentPointLetterData.Stage) != null)
            {
                //이벤트 입장
                MSSceneManager.Instance.EnterScene(SceneBase.eScene.CHAT);
            }
            else
            {
                StartNextPoint();
            }
        }
    }

    private DataManager.LetterData SearchLetterData(int idx)
    {
        List<UserInfo.ItemData> bagItemList = UserInfo.Instance.GetBagItemList();
        for (int i = 0; i < bagItemList.Count; i++)
        {
            if (bagItemList[i].Type == eItemType.Letter)
            {
                DataManager.LetterData letterData = DataManager.Instance.GetLetterData(bagItemList[i].ID);
                if (letterData.Destination == mapObject.selectedPointList[idx].pointID)
                {
                    return letterData;
                }
            }
        }
        return null;
    }

    public void StartNextPoint()
    {
        //맵에 다시 오고, 다음 스테이지로 이동하는 부분
        UserInfo.Instance.CurrentGameData.CurrentMapProgress++;
        mapObject.PlayNode();
    }

    public void EndPhase()
    {
        chatTriggerObject.SetActive(true);
        MainCharacter.Bubble.SetText(TextManager.GetSystemText("INGAME_CHAPTER_END_DIALOGUE"));
        UserInfo.Instance.SetCurrentIngameState(eIngameState.Result);
    }

    //public void OnClickCheckPin()
    //{
    //    //출발지, 경유지, 도착지 선택
    //    mapObject.DisplayNodes();
    //}

    public void OnClickStart()
    {
        //시작
        mapObject.PlayNode();
    }

    public void OnClickChatEnd()
    {
        chatTriggerObject.SetActive(false);
        if (UserInfo.Instance.GetCurrentIngameState() == eIngameState.Result)
        {
            IngameScene.instance.ingameObject.StartResult();
            return;
        }
        EnterPoint();
    }

    public void OnClickResetRoad()
    {
        //경로 리셋
    }

    public void OnClickClose()
    {
        //게임 종료
    }

    public void OnClickOption()
    {
        //옵션
    }
}
