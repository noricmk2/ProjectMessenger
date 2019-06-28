using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : Singleton<TextManager>
{
    public static string GetStoryText(string id)
    {
        var result = DataManager.Instance.GetStoryText(id, (MSUtil.eLanguage)UserInfo.Instance.UserData.CurrentLanguageType);

        if (string.IsNullOrEmpty(result))
            MSLog.LogError("no text content:" + id);

        return result;
    }

    public static string GetSystemText(string id)
    {
        var result = DataManager.Instance.GetSystemText(id, (MSUtil.eLanguage)UserInfo.Instance.UserData.CurrentLanguageType);

        if (string.IsNullOrEmpty(result))
            MSLog.LogError("no text content:" + id);

        return result;
    }
}
