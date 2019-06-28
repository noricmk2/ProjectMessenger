using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExpandTextOutput : MonoBehaviour
{
    #region Inspector
    public TextMeshProUGUI Text;
    #endregion
    private float TYPE_WRITE_SPEED = 0.1f;

    private eTextFlag m_TextFlag;
    private string m_CurrentString;

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

    public void SetText(string content, float fontSize = 0)
    {
        m_CurrentString = content;
        Text.fontSize = fontSize;
    }

    public void PlayText()
    {
        if ((m_TextFlag ^ eTextFlag.TYPE_WRITE_EFFECT) == eTextFlag.TYPE_WRITE_EFFECT)
        {
            StartCoroutine(TypeWrite_C());
        }
    }

    IEnumerator TypeWrite_C()
    {
        int idx = 0;
        var tempStrBuilder = new System.Text.StringBuilder();
        while (idx < m_CurrentString.Length && m_CurrentString.Length > 0)
        {
            tempStrBuilder.Append(m_CurrentString[idx]);
            ++idx;

            Text.text = tempStrBuilder.ToString();

            yield return new WaitForSeconds(TYPE_WRITE_SPEED);
        }
    }

    public void Reset()
    {
        m_TextFlag = 0;
        Text.text = string.Empty;
    }
}
