using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

public class UserInfo : Singleton<UserInfo>
{
    [System.Serializable]
    public class _UserData
    {
        public int CurrentLanguageType;
    }

    [System.Serializable]
    public class _GameData
    {
        public int CurrentChapterID;
        public int CurrentChapterEvent;
        public int Cash;
        public int[] InventoryLetterIDs;
        public int CurrentIngameState;
    }

    public class ItemData : IRecycleSlotData
    {
        public eItemType Type;
        public int ID;
        public string Value;
    }

    public _UserData UserData { get; private set; }
    public _GameData CurrentGameData { get; private set; }

    public void InitUserInfo()
    {
        UserData = SaveAndLoadManager.Load<_UserData>();
        if (UserData == null)
        {
            //TODO:첫 실행시 유저 정보 초기화
            UserData = new _UserData();
            UserData.CurrentLanguageType = 0;
            SaveAndLoadManager.Save(UserData);
        }
    }

    public void SetGameData()
    {
        var lastChapter = PlayerPrefs.GetInt(ConstValue.LAST_CHAPTER_SAVE_KEY, 0);
        CurrentGameData = SaveAndLoadManager.Load<_GameData>(lastChapter);

        if (CurrentGameData == null)
        {
            //TODO:첫 실행시 게임 정보 초기화
            CurrentGameData = new _GameData();
            CurrentGameData.CurrentChapterID = 1;
            CurrentGameData.CurrentChapterEvent = 1;
            CurrentGameData.Cash = 100;
            CurrentGameData.CurrentIngameState = 0;
        }
    }

    public List<DataManager.LetterData> GetChapterMailList(int chapter = -1)
    {
        //TODO:해당 챕터 리스트 가져오기
        if (chapter >= 0)
        {
            return null;
        }
        else
        {
            var curChapter = DataManager.Instance.GetChapterData((eChapterTag)CurrentGameData.CurrentChapterID);
            return curChapter.GetLetterList(5);
        }
    }

    public void AddLetter(DataManager.LetterData data)
    {
        var list = new List<int>();
        if (CurrentGameData.InventoryLetterIDs != null)
            list.AddRange(CurrentGameData.InventoryLetterIDs);
        list.Add(data.ID);
        CurrentGameData.InventoryLetterIDs = list.ToArray();
    }

    public void RemoveLetter(DataManager.LetterData data)
    {
        if (CurrentGameData.InventoryLetterIDs != null)
        {
            var list = new List<int>(CurrentGameData.InventoryLetterIDs);
            if (list.Contains(data.ID))
                list.Remove(data.ID);
            CurrentGameData.InventoryLetterIDs = list.ToArray();
        }
    }

    public List<ItemData> GetBagItemList()
    {
        var list = new List<ItemData>();

        if (CurrentGameData.InventoryLetterIDs != null)
        {
            for (int i = 0; i < CurrentGameData.InventoryLetterIDs.Length; ++i)
            {
                var item = new ItemData();
                item.Type = eItemType.Letter;
                item.ID = CurrentGameData.InventoryLetterIDs[i];
                list.Add(item);
            }
        }

        return list;
    }

    public void SetCurrentIngameState(eIngameState state)
    {
        if (CurrentGameData != null)
            CurrentGameData.CurrentIngameState = (int)state;
    }

    public eIngameState GetCurrentIngameState()
    {
        if (CurrentGameData != null)
            return (eIngameState)CurrentGameData.CurrentIngameState;
        else
            return eIngameState.Title;

    }
}
