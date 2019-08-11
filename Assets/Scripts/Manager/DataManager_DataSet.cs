using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;
using System.Text.RegularExpressions;
using System.Text;

public partial class DataManager : Singleton<DataManager>
{
    #region class
    public abstract class TableDataBase_String : IRecycleSlotData
    {
        public string ID { get; protected set; }
        public abstract void Parse(string[] values);
    }

    public abstract class TableDataBase_Int : IRecycleSlotData
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
                    for (int i = 1; i < split.Length; ++i)
                    {
                        eventData.Value += split[i];
                    }
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

        public string GetCharacterName()
        {
            var charData = Instance.GetCharacterData(CharacterID);
            return TextManager.GetSystemText(ConstValue.CHARACTER_GETTER_ID + charData.CharacterType.ToString().ToUpper());
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
        private string m_ChapterTitle;

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

        public ChapterTextData GetChapterTextData(eStageTag eventTag, string dialogueID)
        {
            var data = Instance.GetChapterTextData(eventTag, dialogueID);
            return data;
        }

        public List<ChapterTextData> GetAllChapterTextData()
        {
            var list = new List<ChapterTextData>();
            for (int i = 0; i < ChapterTextDataIDList.Count; ++i)
                list.Add(Instance.GetChapterTextData(ChapterTextDataIDList[i]));
            return list;
        }

        public List<LetterData> GetLetterList(int randomCount = 0)
        {
            var list = new List<LetterData>();

            for (int i = 0; i < LetterIDList.Count; ++i)
                list.Add(Instance.GetLetterData(LetterIDList[i]));

            list.AddRange(Instance.GetRandomLetterList(randomCount));

            return list;
        }

        public string GetChapterTitle()
        {
            if (string.IsNullOrEmpty(m_ChapterTitle))
                m_ChapterTitle = "Chapter : " + ChapterTag.ToString();
            return m_ChapterTitle;
        }
    }
    #endregion

    #region ChapterTextData
    public class ChapterTextData : TableDataBase_Int
    {
        const string STORY_TEXT = "TEXT_STORY_";
        public eChapterTag ChapterTag { get; private set; }
        public eStageTag StageTag { get; private set; }
        public string DialogueID { get; private set; }
        public Dictionary<int, string> TextIDDic { get; private set; }
        public string BGResourceName { get; private set; }

        public override void Parse(string[] values)
        {
            ID = Func.GetInt(values[0]);
            ChapterTag = Func.GetEnum<eChapterTag>(values[1]);
            StageTag = Func.GetEnum<eStageTag>(values[2]);
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
            BGResourceName = values[6];
        }

        public List<StoryTextData> GetAllTextData()
        {
            var list = new List<StoryTextData>();
            var iter = TextIDDic.GetEnumerator();
            while (iter.MoveNext())
                list.Add(Instance.GetStoryTextData(iter.Current.Value));
            return list;
        }

        public StoryTextData GetTextData(int idx)
        {
            if (TextIDDic.ContainsKey(idx))
                return Instance.GetStoryTextData(TextIDDic[idx]);
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

        public Sprite GetBackGroundSprite()
        {
            return ObjectFactory.Instance.GetBackGroundSprite(BGResourceName);
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

        public string GetCharacterName()
        {
            return TextManager.GetSystemText(ConstValue.CHARACTER_GETTER_ID + CharacterType.ToString().ToUpper());
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
        private string AddresseeText;
        private string AddressText;

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

        public string GetAddresseeText()
        {
            if (string.IsNullOrEmpty(AddresseeText))
            {
                var charData = Instance.GetCharacterData(To);
                var strBuilder = new StringBuilder(TextManager.GetSystemText("TEXT_ADDRESSEE"));
                strBuilder.Append(':');
                strBuilder.Append(charData.GetCharacterName());
                AddresseeText = strBuilder.ToString();
            }
            return AddresseeText;
        }

        public string GetAddressText()
        {
            if (string.IsNullOrEmpty(AddressText))
            {
                var mapData = Instance.GetMapData_Point(Destination);
                var strBuilder = new StringBuilder(TextManager.GetSystemText("TEXT_ADDRESS"));
                strBuilder.Append(":\n");
                strBuilder.Append(TextManager.GetSystemText(mapData.Name));
                AddressText = strBuilder.ToString();
            }
            return AddressText;
        }
    }
    #endregion

    #region MapData_Point
    public class MapData_Point : TableDataBase_Int
    {
        public string Name { get; private set; }
        public string Name_Short { get; private set; }
        public string Description { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }

        public override void Parse(string[] values)
        {
            ID = Func.GetInt(values[1]);
            Name = values[2];
            Name_Short = values[3];
            Description = values[4];
            PosX = Func.GetFloat(values[5]);
            PosY = Func.GetFloat(values[6]);
        }
    }
    #endregion
}
