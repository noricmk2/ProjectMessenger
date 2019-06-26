using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : Singleton<TextManager>
{
    public static string GetStoryText(string id)
    {
        return DataManager.Instance.GetStoryText(id, (MSUtil.eLanguage)UserInfo.Instance.UserData.CurrentLanguageType);
    }

    public static string GetSystemText(string id)
    {
        return DataManager.Instance.GetSystemText(id, (MSUtil.eLanguage)UserInfo.Instance.UserData.CurrentLanguageType);
    }
}
