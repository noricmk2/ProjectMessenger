using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DataManager : Singleton<DataManager>
{
    const string SheetURL = "https://docs.google.com/spreadsheets/d/1Op6ruKuO1umBiKAt0YB2dTU-0xMqkYJOMSzy2CcwAUE/export?format=tsv&id=1Op6ruKuO1umBiKAt0YB2dTU-0xMqkYJOMSzy2CcwAUE&gid=";

    public enum eSheetType
    {
        StoryText,
        Chapter,
        Length
    }

    private Dictionary<eSheetType, string> m_SheetIDDic = new Dictionary<eSheetType, string>()
    {
        { eSheetType.StoryText , "1675975552" },
        { eSheetType.Chapter , "715858755" },
    };

    public Coroutine LoadFromGoogleSheet(eSheetType type)
    {
        return StartCoroutine(LoadFromGoogleSheet_C(type));
    }

    private IEnumerator LoadFromGoogleSheet_C(eSheetType type)
    {
        WWW www = new WWW(SheetURL + m_SheetIDDic[type]);
        yield return www;

        while (www.isDone == false)
            yield return null;

        if (www.error == null)
        {
            switch (type)
            {
                case eSheetType.StoryText:
                    ParseTable_String(m_StoryTextDic, www.text);
                    break;
                case eSheetType.Chapter:
                    ParseTable_String(m_ChapterDataDic, www.text);
                    break;
            }
        }
        else
            MSLog.LogError(type.ToString() + "//" + www.error);
    }

    private void ParseTable_String<T>(Dictionary<string, T> dataDic, string content) where T : TableDataBase_String, new()
    {
        dataDic.Clear();
        var lines = content.Split('\n');
        for (int i = 1; i < lines.Length; ++i)
        {
            var tabs = lines[i].Split('\t');
            for (int j = 0; j < tabs.Length; ++j)
                tabs[j] = tabs[j].Trim();

            var tableData = new T();
            try
            {
                tableData.Parse(tabs);
            }
            catch (System.Exception e)
            {
                MSLog.LogError("error with " + typeof(T) + "/line:" + i + "/" + e.Message);
                continue;
            }
            if (dataDic.ContainsKey(tableData.ID))
            {
                MSLog.LogError("Overlap ID:" + tableData.ID);
                continue;
            }
            dataDic[tableData.ID] = tableData;
        }
    }

    private void ParseTable_Int<T>(Dictionary<int, T> dataDic, string content) where T : TableDataBase_Int, new()
    {
        dataDic.Clear();
        var lines = content.Split('\n');
        for (int i = 1; i < lines.Length; ++i)
        {
            var tabs = lines[i].Split('\t');
            for (int j = 0; j < tabs.Length; ++j)
                tabs[j] = tabs[j].Trim();

            var tableData = new T();
            try
            {
                tableData.Parse(tabs);
            }
            catch (System.Exception e)
            {
                MSLog.LogError("error with " + typeof(T) + "/line:" + i + "/" + e.Message);
                continue;
            }
            if (dataDic.ContainsKey(tableData.ID))
            {
                MSLog.LogError("Overlap ID:" + tableData.ID);
                continue;
            }
            dataDic[tableData.ID] = tableData;
        }
    }


    #region StoryText
    private Dictionary<string, TextData> m_StoryTextDic = new Dictionary<string, TextData>();

    public string GetText(string id, TextData.eLanguage langType)
    {
        string result = "";
        if (m_StoryTextDic.ContainsKey(id))
            result = m_StoryTextDic[id].GetText(langType);
        return result;
    }
    #endregion

    #region ChapterData
    private Dictionary<string, ChapterData> m_ChapterDataDic = new Dictionary<string, ChapterData>();

    public ChapterData GetChapterData(string id)
    {
        if (m_ChapterDataDic.ContainsKey(id))
            return m_ChapterDataDic[id];
        return null;
    }
    #endregion
}
