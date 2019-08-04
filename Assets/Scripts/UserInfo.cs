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
        public int[] ChapterLetterIDs;
    }

    public class ItemData : IRecycleSlotData
    {
        public eItemType Type;
        public int ID;
        public string Value;
    }

    public _UserData UserData { get; private set; }
    public _GameData CurrentGameData { get; private set; }

    private List<DataManager.LetterData> m_ChapterMailList = new List<DataManager.LetterData>();

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
            SetChapterMailList();
        }
    }

    public void SetNextChapter()
    {
        var chapterData = DataManager.Instance.GetChapterData(CurrentGameData.CurrentChapterID + 1);
        if (chapterData == null)
        {
            //TODO: 챕터 정보 없을시 처리
        }
        else
        {
            ++CurrentGameData.CurrentChapterID;
            CurrentGameData.CurrentChapterEvent = 1;
            CurrentGameData.CurrentIngameState = 0;
            SetChapterMailList();
        }
    }

    public List<DataManager.LetterData> GetChapterLetterList()
    {
        return new List<DataManager.LetterData>(m_ChapterMailList);
    }

    public void SetChapterMailList()
    {
        var curChapter = DataManager.Instance.GetChapterData((eChapterTag)CurrentGameData.CurrentChapterID);
        //TODO: 찌라시 정보 구성 공식 추가 
        m_ChapterMailList = curChapter.GetLetterList(5);

        var idList = new List<int>();
        for (int i = 0; i < m_ChapterMailList.Count; ++i)
            idList.Add(m_ChapterMailList[i].ID);

        CurrentGameData.ChapterLetterIDs = idList.ToArray();
    }

    public void AddChapterLetter(DataManager.LetterData data)
    {
        var list = new List<int>();
        if (CurrentGameData.ChapterLetterIDs != null)
            list.AddRange(CurrentGameData.ChapterLetterIDs);
        list.Add(data.ID);
        CurrentGameData.ChapterLetterIDs = list.ToArray();

        m_ChapterMailList.Add(data);
        m_ChapterMailList.Sort(new Sort.LetterSort());
    }

    public void RemoveChapterLetter(DataManager.LetterData data)
    {
        if (CurrentGameData.ChapterLetterIDs != null)
        {
            var list = new List<int>(CurrentGameData.ChapterLetterIDs);
            if (list.Contains(data.ID))
                list.Remove(data.ID);
            CurrentGameData.ChapterLetterIDs = list.ToArray();

            m_ChapterMailList.Remove(data);
            m_ChapterMailList.Sort(new Sort.LetterSort());
        }
    }

    public void AddInventoryLetter(DataManager.LetterData data)
    {
        var list = new List<int>();
        if (CurrentGameData.InventoryLetterIDs != null)
            list.AddRange(CurrentGameData.InventoryLetterIDs);
        list.Add(data.ID);
        CurrentGameData.InventoryLetterIDs = list.ToArray();
    }

    public void RemoveInventoryLetter(DataManager.LetterData data)
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
