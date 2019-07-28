using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;
using System.Text.RegularExpressions;

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

    public struct TextEventData
    {
        public eTextEventTag Tag;
        public string Value;
    }

    public class StoryTextData : TextData
    {
        public int CharacterID { get; private set; }
        Dictionary<eLanguage, string> m_TextDic = new Dictionary<eLanguage, string>();
        Dictionary<int, TextEventData> m_EventTagDic = new Dictionary<int, TextEventData>();

        public override void Parse(string[] values)
        {
            ID = values[0];
            CharacterID = Func.GetInt(values[1]);
            Regex regex;
            if (values[2].Contains("[CHO"))
                regex = new Regex(@"\[(.+)\]");
            else
                regex = new Regex(@"\[(\w+)\]");
            var result = regex.Matches(values[2]);

            var iter = result.GetEnumerator();
            var prevIdx = 0;
            while (iter.MoveNext())
            {
                var match = iter.Current as Match;

                TextEventData eventData = new TextEventData();
                var matchStr = match.Groups[1].Value;
                if (matchStr.Contains("_"))
                {
                    var split = matchStr.Split('_');
                    eventData.Tag = Func.GetEnum<eTextEventTag>(split[0]);
                    eventData.Value = split[1];
                }
                else
                    eventData.Tag = Func.GetEnum<eTextEventTag>(match.Groups[1].Value);

                m_EventTagDic[match.Index - prevIdx] = eventData;
                prevIdx += match.Groups[0].Length;

                for (int i = 0; i < (int)eLanguage.Length; ++i)
                    values[2 + i] = values[2 + i].Replace(match.Groups[0].Value, "");
            }

            for (int i = 0; i < (int)eLanguage.Length; ++i)
                m_TextDic[(eLanguage)i] = values[2 + i];
        }

        public override string GetText(eLanguage langType)
        {
            return m_TextDic[langType];
        }

        public Dictionary<int, TextEventData> GetEventTagDic()
        {
            return m_EventTagDic;
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

        public ChapterTextData GetChapterTextData(eEventTag eventTag, string dialogueID)
        {
            var data = Instance.GetChapterTextData(eventTag, dialogueID);
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
        public string DialogueID { get; private set; }
        public Dictionary<int, string> TextIDDic { get; private set; }

        public override void Parse(string[] values)
        {
            ID = Func.GetInt(values[0]);
            ChapterTag = Func.GetEnum<eChapterTag>(values[1]);
            EventTag = Func.GetEnum<eEventTag>(values[2]);
            DialogueID = values[3];

            TextIDDic = new Dictionary<int, string>();
            var min = Func.GetInt(values[4]);
            var max = Func.GetInt(values[5]);
            int count = max - min + 1;
            var strBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < count; ++i)
            {
                strBuilder.Clear();
                strBuilder.Append(STORY_TEXT);
                strBuilder.Append(values[1]);
                strBuilder.Append("_");
                strBuilder.Append(values[2]);
                strBuilder.Append("_");
                strBuilder.Append(values[3]);
                strBuilder.Append("_");
                strBuilder.Append((min + i).ToString());
                TextIDDic[min + i] = strBuilder.ToString();
            }
        }

        public StoryTextData GetTextData(int idx)
        {
            if (TextIDDic.ContainsKey(idx))
                return DataManager.Instance.GetStoryTextData(TextIDDic[idx]);
            else
                MSLog.LogError("text id not exist:" + idx);

            return null;
        }

        public string GetText(int idx)
        {
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
        public eCharacter CharacterType { get; private set; }
        public Dictionary<eCharacterState, AnimationFrameData> FrameDataDic { get; private set; }

        public override void Parse(string[] values)
        {
            ID = Func.GetInt(values[0]);
            if (!ColorUtility.TryParseHtmlString(values[2], out textColor))
            {
                MSLog.LogError("invalid color:" + values[2]);
                textColor = Color.black;
            }
            CharacterType = Func.GetEnum<eCharacter>(values[3].ToUpper());

            FrameDataDic = new Dictionary<eCharacterState, AnimationFrameData>();
            for (int i = 1; i < (int)eCharacterState.LENGTH; ++i)
            {
                var count = Func.GetInt(values[(2 * i - 1) + 3]);
                var targetFrame = Func.GetFloat(values[(2 * i - 1) + 4]);
                FrameDataDic[(eCharacterState)i] = new AnimationFrameData(0, count, targetFrame);
            }
        }

        public List<Sprite> GetSpriteList(eCharacterState state)
        {
            List<Sprite> resultList = null;
            if (FrameDataDic.ContainsKey(state))
            {
                resultList = new List<Sprite>();

                for (int i = 1; i <= FrameDataDic[state].FrameCount; ++i)
                {
                    var isPortrait = CharacterType == eCharacter.NIKA ? "portrait" : "stand";
                    var resourceName = CharacterType.ToString().ToLower() + "_" + isPortrait + "_" + state.ToString().ToLower() + "_" + i;
                    resultList.Add(ObjectFactory.Instance.GetCharacterSprite(CharacterType, resourceName));
                }
            }
            return resultList;
        }
    }
    #endregion

    #region LetterData
    public class LetterData : TableDataBase_Int
    {
        public eLetterType LetterType { get; private set; }
        public int From { get; private set; }
        public int To { get; private set; }
        public int Destination { get; private set; }
        public int Reward { get; private set; }
        public int Flag { get; private set; }

        public override void Parse(string[] values)
        {
            ID = Func.GetInt(values[1]);
            LetterType = Func.GetEnum<eLetterType>(values[2]);
            From = Func.GetInt(values[3]);
            To = Func.GetInt(values[4]);
            Destination = Func.GetInt(values[5]);
            Reward = Func.GetInt(values[6]);
            Flag = Func.GetInt(values[7]);

        }
    }
    #endregion

    #region MapData_Point
    public class MapData_Point : TableDataBase_Int
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public override void Parse(string[] values)
        {
            ID = Func.GetInt(values[1]);
            Name = values[2];
            Description = values[3];
        }
    }
    #endregion
}
