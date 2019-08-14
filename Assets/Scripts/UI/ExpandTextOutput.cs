using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MSUtil;

public class ExpandTextOutput : MonoBehaviour
{
    #region Inspector
    public TextMeshProUGUI Text;
    #endregion
    private float TYPE_WRITE_SPEED = 0.07f;
    public delegate IEnumerator TextEventDelegate(SpeechBubble parent, DataManager.TextEventData data);

    private float m_DeltaTime = 0;
    private eTextFlag m_TextFlag;
    private string m_CurrentString;
    private TextEventDelegate m_EventAction;
    private System.Action m_TextEndAction;
    private Dictionary<int, DataManager.TextEventData> m_TextEventTagDic;
    private Coroutine m_TypeWriteCoroutine;
    private Coroutine m_CancelCoroutine;
    private SpeechBubble m_ParentBubble;
    private string m_PauseTarget;
    private float m_PauseTime;
    private float m_LastTerm;

    [System.Flags]
    public enum eTextFlag
    {
        TYPE_WRITE_EFFECT = 1,
        SHAKE_TEXT_EFFECT,
    }

    private void Awake()
    {
        if (Text == null)
            Text = GetComponent<TextMeshProUGUI>();
    }

    public void SetTextFlag(eTextFlag flag)
    {
        m_TextFlag = m_TextFlag | flag;
    }

    public void SetTypeWriteSpeed(float speed)
    {
        TYPE_WRITE_SPEED = speed;
    }

    public void SetPauseText(string target, float pauseTime)
    {
        m_PauseTarget = target;
        m_PauseTime = pauseTime;
    }

    public void SetLastTerm(float term)
    {
        m_LastTerm = term;
    }

    public void SetText(string text, System.Action textEndAction = null)
    {
        m_CurrentString = text;
        m_TextEndAction = textEndAction;
    }

    public void SetText(SpeechBubble parent, string content, Dictionary<int, DataManager.TextEventData> tagDic, 
        TextEventDelegate eventAction = null, System.Action textEndAction = null)
    {
        GameManager.IsPlayText = false;
        m_ParentBubble = parent;
        m_EventAction = eventAction;
        m_CurrentString = content;
        m_TextEventTagDic = tagDic;
        m_TextEndAction = textEndAction;
    }

    public void PlayText()
    {
        m_DeltaTime = 0;
        if (m_TypeWriteCoroutine != null)
            StopCoroutine(m_TypeWriteCoroutine);
        if ((m_TextFlag ^ eTextFlag.TYPE_WRITE_EFFECT) == eTextFlag.TYPE_WRITE_EFFECT)
        {
            m_TypeWriteCoroutine = StartCoroutine(TypeWrite_C());
        }
        else
        {
            Text.text = m_CurrentString;
            if (m_TextEventTagDic != null && m_EventAction != null && m_TextEventTagDic.ContainsKey(m_CurrentString.Length))
                m_EventAction(m_ParentBubble, m_TextEventTagDic[m_CurrentString.Length]);
        }
    }

    IEnumerator CancelTypeWrite_C()
    {
        GameManager.IsPlayText = false;
        if (m_CurrentString.Length > 0)
        {
            StopCoroutine(m_TypeWriteCoroutine);
            m_TypeWriteCoroutine = null;
            Text.text = m_CurrentString;

            if (m_TextEventTagDic != null && m_EventAction != null && m_TextEventTagDic.ContainsKey(m_CurrentString.Length))
                yield return m_EventAction(m_ParentBubble, m_TextEventTagDic[m_CurrentString.Length]);
            if (m_TextEndAction != null)
                m_TextEndAction();
        }
        SetPauseText(null, 0);
    }

    public void CancelTypeWrite()
    {
        if (m_CancelCoroutine != null)
            StopCoroutine(m_CancelCoroutine);
        m_CancelCoroutine = StartCoroutine(CancelTypeWrite_C());
    }

    IEnumerator TypeWrite_C()
    {
        GameManager.IsPlayText = true;
        int idx = 0;
        var tempStrBuilder = new System.Text.StringBuilder();
        var writeSpeed = TYPE_WRITE_SPEED;
        bool pauseText = false;
        while (idx < m_CurrentString.Length && m_CurrentString.Length > 0)
        {
            m_DeltaTime += Time.deltaTime;
            if (m_DeltaTime >= writeSpeed || (m_CurrentString[idx] == ' ' && !pauseText))
            {
                if (m_TextEventTagDic != null && m_EventAction != null)
                {
                    if (m_TextEventTagDic.ContainsKey(idx))
                        yield return m_EventAction(m_ParentBubble, m_TextEventTagDic[idx]);
                }

                if (m_CurrentString[idx] == '^')
                    tempStrBuilder.Append("\n");
                else
                {
                    tempStrBuilder.Append(m_CurrentString[idx]);
                    if (!string.IsNullOrEmpty(m_PauseTarget) && m_PauseTarget.Contains(m_CurrentString[idx].ToString()))
                    {
                        writeSpeed = m_PauseTime;
                        pauseText = true;
                    }
                    else
                    {
                        writeSpeed = TYPE_WRITE_SPEED;
                        pauseText = false;
                    }
                }
                ++idx;

                Text.text = tempStrBuilder.ToString();
                m_DeltaTime = 0;
            }
            yield return null;
        }
        if (m_TextEventTagDic != null && m_EventAction != null)
        {
            if (m_TextEventTagDic.ContainsKey(m_CurrentString.Length))
            {
                if (m_TextEventTagDic[m_CurrentString.Length].Tag == eTextEventTag.CHO)
                    Text.text = "";
                yield return m_EventAction(m_ParentBubble, m_TextEventTagDic[m_CurrentString.Length]);
            }
        }

        while (m_DeltaTime < m_LastTerm)
        {
            m_DeltaTime += Time.deltaTime;
            yield return null;
        }

        if (m_TextEndAction != null)
            m_TextEndAction();
        GameManager.IsPlayText = false;
        SetPauseText(null, 0);
    }

    public void ResetText()
    {
        m_TextFlag = 0;
        Text.text = string.Empty;
        SetPauseText(null, 0);
    }
}
