using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MSUtil
{
    #region Enum
    public enum eStageTag
    {
        NONE,
        START = 1,
        PREDELV,
        ONDELVLESS,
        DAYEND,
    }

    public enum eChapterTag
    {
        PROLOGUE = 1,
        DAY1,
    }

    public enum eLanguage
    {
        Korean,
        English,
        Japanese,
        Length
    }

    public enum eCharacterState
    {
        NONE,
        IDLE,
        LAUGH,
        ANGRY,
        CONFUSE,
        SIGH,
        QUESTION,
        SURPRISE,
        LENGTH
    }

    public enum eChatPosition
    {
        Center,
        Left,
        Right,
    }

    public enum eCharacter
    {
        NIKA,
        LUCIA,
        RINTA,
        LESS,
        JACQUES,
        ARUE,
        LENGTH
    }

    public enum eTextEventTag
    {
        APR,
        DPR,
        DRK,
        CHO,
        HL,
        CNG,
        OVR,
        BACKDOWN,
        TERM,
        APRITEM,
        DPRITEM,
        MAILSORT,
        FONTBIG,
        FONTSML,
        FONTCOL,
        NOTIFY,
        LENGTH
    }

    public enum eLetterType
    {
        Event,
        Junk,
    }

    public enum eItemType
    {
        Letter,
        Item,
        Length
    }

    public enum eIngameState
    {
        Title = 0,
        MailSort,
        Map,
    }

    public enum eTransitionType
    {
        NONE,
        CIRCLE,
        NORMAL,
    }

    public enum eOverlapType
    {
        MAILSORT,
        MAILDROP,
    }

    public enum eChatType
    {
        NONE,
        NORMAL,
        EVENT,
        CHOICE,
    }

    public enum eEventEffect
    {
        NONE,
        SHAKE,
    }
    #endregion

    #region Func
    public static class Func
    {
        #region Extension Method
        public static void SetActive_Check(this GameObject gameObj, bool bActive)
        {
            if (gameObj.activeSelf != bActive)
            {
                gameObj.SetActive(bActive);
            }
        }

        public static void Init(this Transform transform, Transform parent)
        {
            transform.SetParent(parent);
            transform.localScale = new Vector3(1f, 1f, 1f);
            transform.localPosition = Vector3.zero;
        }
        #endregion

        public static Vector2 GetSizeByCorner(Vector3[] corners)
        {
            var result = new Vector2(Screen.width - (corners[0].x - corners[2].x), Screen.height - (corners[0].y - corners[1].y));
            return result;
        }

        public static bool InPercent(float percent)
        {
            return UnityEngine.Random.Range(0f, 100f) <= percent;
        }

        public static bool InPercent_100000(int percent)
        {
            return UnityEngine.Random.Range(0, 100001) <= percent;
        }

        public static int GetInt(string value, int defaultVal = 0)
        {
            if (value == null || string.IsNullOrEmpty(value))
                return defaultVal;
            int result = 0;
            if (int.TryParse(value, out result))
                return result;
            return defaultVal;
        }

        public static float GetFloat(string value, float defaultVal = 0)
        {
            if (value == null || string.IsNullOrEmpty(value))
                return defaultVal;
            float result = 0;
            if (float.TryParse(value, out result))
                return result;
            return defaultVal;
        }

        public static bool GetBool(string value, bool defaultVal = false)
        {
            if (value == null || string.IsNullOrEmpty(value))
                return defaultVal;
            bool result;

            if (bool.TryParse(value, out result))
                return result;

            return defaultVal;
        }

        public static T GetEnum<T>(string value) where T : struct
        {
            if (value == null || string.IsNullOrEmpty(value))
                return default(T);

            T result;
            try
            {
                result = (T)System.Enum.Parse(typeof(T), value);
            }
            catch (System.Exception e)
            {
                MSLog.LogError(e.ToString());
                result = default(T);
            }
            return result;
        }

        public static float Msec2sec(float milliSec)
        {
            return milliSec * 0.001f;
        }
    }
    #endregion

    #region Value
    public class ConstValue        //전역적으로 쓰이는 고정값
    {
        public static readonly Vector2 DEFULT_SCREEN_SIZE = new Vector2(1280f, 720f); // 기본 화면사이즈
        public static readonly string LAST_CHAPTER_SAVE_KEY = "LAST_CHAPTER"; // 최근 플레이 챕터 세이브 키값

        public static readonly string CHARACTER_GETTER_ID = "CHARACTER_NAME_";
        public static readonly string CHARACTER_NIKA = "nika"; //니카 캐릭터명;
        public static readonly int CHARACTER_NIKA_ID = 10001; //니카 캐릭터 ID;
        public static readonly string FIRST_DIALOUGE_ID = "DL1";

        public static readonly Vector2 BUBBLE_DEFAULT_POS = new Vector2(-300, 300);

        public static float CHARACTER_APPEAR_TIME = 0.5f;
        public static float CURSOR_BLINK_TIME = 0.5f;
        public static float BUBBLE_ANIMATION_TIME = 0.2f;
        public static float LONG_CLICK_TIME = 1.0f;
        public static float DEFAULT_ANIM_FPS = 14f;

        public const string SHEET_SAVE = "SheetSave";//시트 세이브 키
        public const string BUBBLE_SPRITE_NAME_1 = "ui_speech_bubble1";
        public const string BUBBLE_SPRITE_NAME_2 = "ui_speech_bubble2";
        public const string SELECT_TEXT = "TEXT_SELECT";
        public const string NOTIFY_TEXT = "TEXT_NOTIFY_";
    }

    public class ColorPalette        //컬러 상수값
    {
        public static readonly Color FADE_OUT_BLACK = new Color(0, 0, 0, 1);
        public static readonly Color FADE_IN_BLACK = new Color(0, 0, 0, 0);
        public static readonly Color BUTTON_HILIGHT_COLOR = new Color(94 / 255f, 94 / 255f, 94 / 255f, 146 / 255f);
        public static readonly Color CHARACTER_HILIGHT_COLOR = new Color(85 / 255f, 85 / 255f, 85 / 255f, 255 / 255f);
    }
    #endregion

    #region Sort
    public class Sort
    {
        public class LetterSort : IComparer<DataManager.LetterData>
        {
            private int m_Ascending = -1;

            public void SetAscending(bool ascend)
            {
                m_Ascending = ascend ? -1 : 1;
            }

            public bool IsAscending()
            {
                return m_Ascending == -1 ? true : false;
            }

            public int Compare(DataManager.LetterData x, DataManager.LetterData y)
            {
                if (x == y)
                    return 0;

                if (x.ID > y.ID)
                    return -m_Ascending;
                else if (x.ID < y.ID)
                    return m_Ascending;

                return 0;
            }
        }
    }
    #endregion
}
