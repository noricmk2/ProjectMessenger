using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

public partial class DataManager : Singleton<DataManager>
{
    const string SheetURL = "https://docs.google.com/spreadsheets/d/1Op6ruKuO1umBiKAt0YB2dTU-0xMqkYJOMSzy2CcwAUE/export?format=tsv&id=1Op6ruKuO1umBiKAt0YB2dTU-0xMqkYJOMSzy2CcwAUE&gid=";

    public enum eSheetType
    {
        SystemText,
        StoryText,
        Chapter,
        ChapterTextData,
        Character,
        Letter,
        Length
    }

    private Dictionary<eSheetType, string> m_SheetIDDic = new Dictionary<eSheetType, string>()
    {
        { eSheetType.SystemText , "0" },
        { eSheetType.StoryText , "1675975552" },
        { eSheetType.Chapter , "715858755" },
        { eSheetType.ChapterTextData , "754458555" },
        { eSheetType.Character , "651796308" },
        { eSheetType.Letter , "1623412517" },
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
                case eSheetType.SystemText:
                    ParseTable_String(m_SystemTextDic, www.text);
                    break;
                case eSheetType.StoryText:
                    ParseTable_String(m_StoryTextDic, www.text);
                    break;
                case eSheetType.Chapter:
                    ParseTable_Int(m_ChapterDataDic, www.text);
                    break;
                case eSheetType.ChapterTextData:
                    ParseTable_Int(m_ChapterTextDataDic, www.text);
                    break;
                case eSheetType.Character:
                    ParseTable_Int(m_CharacterDataDic, www.text);
                    break;
                case eSheetType.Letter:
                    ParseTable_Int(m_LetterDataDic, www.text, 2);
                    break;
            }
        }
        else
            MSLog.LogError(type.ToString() + "//" + www.error);
    }

    private void ParseTable_String<T>(Dictionary<string, T> dataDic, string content, int startIdx = 1) where T : TableDataBase_String, new()
    {
        dataDic.Clear();
        var lines = content.Split('\n');
        for (int i = startIdx; i < lines.Length; ++i)
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

    private void ParseTable_Int<T>(Dictionary<int, T> dataDic, string content, int startIdx = 1) where T : TableDataBase_Int, new()
    {
        dataDic.Clear();
        var lines = content.Split('\n');
        for (int i = startIdx; i < lines.Length; ++i)
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
    private Dictionary<string, StoryTextData> m_StoryTextDic = new Dictionary<string, StoryTextData>();

    public StoryTextData GetStoryTextData(string id)
    {
        if (m_StoryTextDic.ContainsKey(id))
            return m_StoryTextDic[id];
        return null;
    }

    public string GetStoryText(string id, eLanguage langType)
    {
        string result = "";
        if (m_StoryTextDic.ContainsKey(id))
            result = m_StoryTextDic[id].GetText(langType);
        return result;
    }
    #endregion

    #region SystemText
    private Dictionary<string, SystemTextData> m_SystemTextDic = new Dictionary<string, SystemTextData>();

    public string GetSystemText(string id, eLanguage langType)
    {
        string result = "";
        if (m_SystemTextDic.ContainsKey(id))
            result = m_SystemTextDic[id].GetText(langType);
        return result;
    }
    #endregion

    #region ChapterData
    private Dictionary<int, ChapterData> m_ChapterDataDic = new Dictionary<int, ChapterData>();

    public ChapterData GetChapterData(int id)
    {
        if (m_ChapterDataDic.ContainsKey(id))
            return m_ChapterDataDic[id];
        return null;
    }

    public ChapterData GetChapterData(eChapterTag chapter)
    {
        ChapterData data = null;
        var iter = m_ChapterDataDic.GetEnumerator();
        while (iter.MoveNext())
        {
            if (iter.Current.Value.ChapterTag == chapter)
            {
                data = iter.Current.Value;
                break;
            }
        }
        return data;
    }
    #endregion

    #region ChapterTextData
    private Dictionary<int, ChapterTextData> m_ChapterTextDataDic = new Dictionary<int, ChapterTextData>();

    public ChapterTextData GetChapterTextData(int id)
    {
        if (m_ChapterTextDataDic.ContainsKey(id))
            return m_ChapterTextDataDic[id];
        return null;
    }

    public ChapterTextData GetChapterTextData(eEventTag eventTag, string dialogueID)
    {
        ChapterTextData data = null;
        var iter = m_ChapterTextDataDic.GetEnumerator();
        while (iter.MoveNext())
        {
            if (iter.Current.Value.EventTag == eventTag && iter.Current.Value.DialogueID == dialogueID)
            {
                data = iter.Current.Value;
                break;
            }
        }
        return data;
    }
    #endregion

    #region CharacterData
    private Dictionary<int, CharacterData> m_CharacterDataDic = new Dictionary<int, CharacterData>();

    public CharacterData GetCharacterData(int id)
    {
        if (m_CharacterDataDic.ContainsKey(id))
            return m_CharacterDataDic[id];
        return null;
    }

    public CharacterData GetCharacterData(eCharacter character)
    {
        var iter = m_CharacterDataDic.GetEnumerator();
        while (iter.MoveNext())
        {
            if (iter.Current.Value.CharacterType == character)
                return iter.Current.Value;
        }
        return null;
    }
    #endregion

    #region LetterData
    private Dictionary<int, LetterData> m_LetterDataDic = new Dictionary<int, LetterData>();

    public LetterData GetLetterData(int id)
    {
        if (m_LetterDataDic.ContainsKey(id))
            return m_LetterDataDic[id];
        return null;
    }

    public List<LetterData> GetRandomLetterList(int count)
    {
        var list = new List<LetterData>();
        var iter = m_LetterDataDic.GetEnumerator();

        //TODO: 조건에 맞는 랜덤 편지 선별
        while (iter.MoveNext() && count > 0)
        {
            var letter = iter.Current.Value;
            if (letter.LetterType == eLetterType.Junk)
            {
                var rand = Random.Range(0, 2) == 0 ? true : false;
                if (rand)
                {
                    list.Add(letter);
                    --count;
                }
            }
        }

        return list;
    }
    #endregion

    #region MapData_Point
    private Dictionary<int, MapData_Point> m_MapData_PointDic = new Dictionary<int, MapData_Point>();

    public MapData_Point GetMapData_Point(int id)
    {
        if (m_MapData_PointDic.ContainsKey(id))
            return m_MapData_PointDic[id];
        return null;
    }
    #endregion
}
