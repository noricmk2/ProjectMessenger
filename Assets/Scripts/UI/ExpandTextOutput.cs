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
    private SpeechBubble m_ParentBubble;

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

    public void SetText(SpeechBubble parent, string content, Dictionary<int, DataManager.TextEventData> tagDic, 
        TextEventDelegate eventAction = null, System.Action textEndAction = null)
    {
        m_ParentBubble = parent;
        m_EventAction = eventAction;
        m_CurrentString = content;
        m_TextEventTagDic = tagDic;
        m_TextEndAction = textEndAction;
    }

    public void PlayText()
    {
        GameManager.IsPlayText = false;
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

    public void CancelTypeWrite()
    {
        if (m_CurrentString.Length > 0)
        {
            StopCoroutine(m_TypeWriteCoroutine);
            m_TypeWriteCoroutine = null;
            var content = m_CurrentString.Replace('^', '\n');
            Text.text = content;
            if (m_TextEventTagDic != null && m_EventAction != null && m_TextEventTagDic.ContainsKey(m_CurrentString.Length))
                m_EventAction(m_ParentBubble, m_TextEventTagDic[m_CurrentString.Length]);
            if (m_TextEndAction != null)
                m_TextEndAction();
            GameManager.IsPlayText = false;
        }
    }

    IEnumerator TypeWrite_C()
    {
        GameManager.IsPlayText = true;
        int idx = 0;
        var tempStrBuilder = new System.Text.StringBuilder();
        while (idx < m_CurrentString.Length && m_CurrentString.Length > 0)
        {
            m_DeltaTime += Time.deltaTime;
            if (m_DeltaTime >= TYPE_WRITE_SPEED || m_CurrentString[idx] == ' ')
            {
                if (m_TextEventTagDic != null && m_EventAction != null)
                {
                    if (m_TextEventTagDic.ContainsKey(idx))
                        yield return m_EventAction(m_ParentBubble, m_TextEventTagDic[idx]);
                }

                if (m_CurrentString[idx] == '^')
                    tempStrBuilder.Append("\n");
                else
                    tempStrBuilder.Append(m_CurrentString[idx]);
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
        if (m_TextEndAction != null)
            m_TextEndAction();
        GameManager.IsPlayText = false;
    }

    public void Reset()
    {
        GameManager.IsPlayText = false;
        m_TextFlag = 0;
        Text.text = string.Empty;
    }
}
