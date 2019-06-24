using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : Singleton<UserInfo>
{
    public class GameData
    {
        public string CurrentChapterData { get; private set; }
    }
}
