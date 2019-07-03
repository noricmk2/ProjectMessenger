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
            CurrentGameData.CurrentChapterEvent = 0;
            CurrentGameData.Cash = 100;
        }
    }
}
