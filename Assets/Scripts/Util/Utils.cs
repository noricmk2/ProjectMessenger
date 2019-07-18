using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MSUtil
{
    #region Enum
    public enum eEventTag
    {
        START,
        PREV_DELV,
    }

    public enum eChapterTag
    {
        PROLOGUE,
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
        APITEM,
        DPITEM,
        FONTBIG,
        FONTSML,
        FONTCOL,
        LENGTH
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
            transform.localPosition = new Vector3();
        }
        #endregion

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
            if(int.TryParse(value, out result))
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
    }
    #endregion

    #region Value
    public class ConstValue        //전역적으로 쓰이는 고정값
    {
        public static readonly Vector2 DEFULT_SCREEN_SIZE = new Vector2(1280f, 720f); // 기본 화면사이즈
        public static readonly string LAST_CHAPTER_SAVE_KEY = "LAST_CHAPTER"; // 최근 플레이 챕터 세이브 키값

        public static readonly string CHARACTER_NIKA = "nika"; //니카 캐릭터명;

        public static float CHARACTER_APPEAR_TIME = 0.5f;
        public static float CURSOR_BLINK_TIME = 0.5f;
        public static float BUBBLE_ANIMATION_TIME = 0.3f;
        public static float LONG_CLICK_TIME = 1.0f;
        public static float DEFAULT_ANIM_FPS = 14f;
    }

    public class ColorPalette        //컬러 상수값
    {
        public static readonly Color FADE_OUT_BLACK = new Color(0, 0, 0, 1); 
        public static readonly Color FADE_IN_BLACK = new Color(0, 0, 0, 0); 
        public static readonly Color BUTTON_HILIGHT_COLOR = new Color(94 / 255f, 94 / 255f, 94 / 255f, 146 / 255f); 
        public static readonly Color CHARACTER_HILIGHT_COLOR = new Color(85 / 255f, 85 / 255f, 85 / 255f, 255 / 255f); 
    }
    #endregion
}
