using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DataManager : Singleton<DataManager>
{
    #region class
    public abstract class TableDataBase_String
    {
        public string ID { get; protected set; }
        public abstract void Parse(string[] values);
    }

    public abstract class TableDataBase_Int
    {
        public int ID { get; protected set; }
        public abstract void Parse(string[] values);
    }
    #endregion

    #region TableData
    public class TextData : TableDataBase_String
    {
        public enum eLanguage
        {
            Korean,
            English,
            Japanese,
            Length
        }

        public string[] Tags { get; private set; }
        Dictionary<eLanguage, string> m_TextDic = new Dictionary<eLanguage, string>();

        public override void Parse(string[] values)
        {
            ID = values[0];
            Tags = values[1].Split(';');

            for (int i = 0; i < (int)eLanguage.Length; ++i)
                m_TextDic[(eLanguage)i] = values[2 + i];
        }

        public string GetText(eLanguage langType)
        {
            return m_TextDic[langType];
        }
    }
    #endregion

    #region ChapterData
    public class ChapterData : TableDataBase_String
    {
        public override void Parse(string[] values)
        {

        }
    }
    #endregion
}
