using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

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

    #region TextData
    public abstract class TextData : TableDataBase_String
    {
        public abstract string GetText(eLanguage langType);
    }

    public class StoryTextData : TextData
    {
        public string[] Tags { get; private set; }
        public int CharacterID { get; private set; }
        Dictionary<eLanguage, string> m_TextDic = new Dictionary<eLanguage, string>();

        public override void Parse(string[] values)
        {
            ID = values[0];
            Tags = values[1].Split(';');
            CharacterID = Func.GetInt(values[2]);

            for (int i = 0; i < (int)eLanguage.Length; ++i)
                m_TextDic[(eLanguage)i] = values[3 + i];
        }

        public override string GetText(eLanguage langType)
        {
            return m_TextDic[langType];
        }
    }

    public class SystemTextData : TextData
    {
        Dictionary<eLanguage, string> m_TextDic = new Dictionary<eLanguage, string>();

        public override void Parse(string[] values)
        {
            ID = values[0];
            for (int i = 0; i < (int)eLanguage.Length; ++i)
                m_TextDic[(eLanguage)i] = values[1 + i];
        }

        public override string GetText(eLanguage langType)
        {
            return m_TextDic[langType];
        }
    }
    #endregion

    #region ChapterData
    public class ChapterData : TableDataBase_Int
    {
        public eChapterTag ChapterTag { get; private set; }
        public List<int> ChapterTextDataIDList { get; private set; }
        public List<int> LetterIDList { get; private set; }

        public override void Parse(string[] values)
        {
            ID = Func.GetInt(values[0]);
            ChapterTag = Func.GetEnum<eChapterTag>(values[1]);

            var split = values[2].Split(';');
            ChapterTextDataIDList = new List<int>();
            for (int i = 0; i < split.Length; ++i)
                ChapterTextDataIDList.Add(Func.GetInt(split[i]));

            split = values[3].Split(';');
            LetterIDList = new List<int>();
            for (int i = 0; i < split.Length; ++i)
                LetterIDList.Add(Func.GetInt(split[i]));
        }

        public ChapterTextData GetChapterTextData(eEventTag eventTag)
        {
            var data = Instance.GetChapterTextData(eventTag);
            return data;
        }
    }
    #endregion

    #region ChapterTextData
    public class ChapterTextData : TableDataBase_Int
    {
        const string STORY_TEXT = "TEXT_STORY_";
        public eChapterTag ChapterTag { get; private set; }
        public eEventTag EventTag { get; private set; }
        public Dictionary<int, string> TextIDDic { get; private set; }

        public override void Parse(string[] values)
        {
            ID = Func.GetInt(values[0]);
            ChapterTag = Func.GetEnum<eChapterTag>(values[1]);
            EventTag = Func.GetEnum<eEventTag>(values[2]);

            TextIDDic = new Dictionary<int, string>();
            var min = Func.GetInt(values[3]);
            var max = Func.GetInt(values[4]);
            int count = max - min + 1;
            var strBuilder = new System.Text.StringBuilder();
            for (int i=0; i < count; ++i)
            {
                strBuilder.Clear();
                strBuilder.Append(STORY_TEXT);
                strBuilder.Append(values[1]);
                strBuilder.Append("_");
                strBuilder.Append(values[2]);
                strBuilder.Append("_");
                strBuilder.Append((min + i).ToString());
                TextIDDic[min + i] = strBuilder.ToString();
            }
        }

        public string GetText(int idx)
        {
            var id = "";

            if (TextIDDic.ContainsKey(idx))
                return TextManager.GetStoryText(TextIDDic[idx]);
            else
                MSLog.LogError("text id not exist:" + idx);

            return "";
        }
    }
    #endregion

    #region CharacterData
    public class CharacterData : TableDataBase_Int
    {
        private Color textColor;
        public Color TextColor
        {
            get { return textColor; }
            private set { textColor = value; }
        }
        public string CharacterName { get; private set; }
        public Dictionary<eCharacterState, int> SpriteCountDic { get; private set; }

        public override void Parse(string[] values)
        {
            ID = Func.GetInt(values[0]);
            if (!ColorUtility.TryParseHtmlString(values[2], out textColor))
            {
                MSLog.LogError("invalid color:" + values[2]);
                textColor = Color.black;
            }
            CharacterName = values[3];

            SpriteCountDic = new Dictionary<eCharacterState, int>();
            SpriteCountDic[eCharacterState.Idle] = Func.GetInt(values[4]);
            SpriteCountDic[eCharacterState.Laugh] = Func.GetInt(values[5]);
            SpriteCountDic[eCharacterState.Angry] = Func.GetInt(values[6]);
            SpriteCountDic[eCharacterState.Confuse] = Func.GetInt(values[7]);
            SpriteCountDic[eCharacterState.Sigh] = Func.GetInt(values[8]);
        }

        public List<Sprite> GetSpriteList(eCharacterState state)
        {
            List<Sprite> resultList = null;
            if (SpriteCountDic.ContainsKey(state))
            {
                resultList = new List<Sprite>();

                for (int i = 1; i <= SpriteCountDic[state]; ++i)
                {
                    var isPortrait = CharacterName == "nika" ? "portrait" : "stand";
                    var resourceName = CharacterName + "_" + isPortrait + "_" + state.ToString().ToLower() + "_" + i;
                    resultList.Add(ObjectFactory.Instance.GetCharacterSprite(resourceName));
                }
            }
            return resultList;
        }
    }
    #endregion
}
