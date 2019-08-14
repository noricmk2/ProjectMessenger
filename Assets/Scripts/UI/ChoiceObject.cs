using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MSUtil;

public class ChoiceObject : MonoBehaviour, IPoolObjectBase
{
    #region Inspector
    public Image BG;
    public TextMeshProUGUI Text;
    public CustomButton ChoiceButton;
    #endregion
    private System.Action<string, string> m_ClickEvent;
    private string m_DialogueFlag;
    private string m_Content;

    public void Init(Transform parent, string content, string flag, System.Action<string, string> clickEvent)
    {
        m_Content = ">" + content;
        Text.text = m_Content;
        m_ClickEvent = clickEvent;
        m_DialogueFlag = flag;
        transform.Init(parent);
        ChoiceButton.IsColorHilight = true;
    }

    public void OnClickButton()
    {
        if (m_ClickEvent != null)
            m_ClickEvent(m_Content, m_DialogueFlag);
    }

    public void PopAction()
    {
        gameObject.SetActive(true);
    }

    public void PushAction()
    {
        gameObject.SetActive(false);
        transform.SetParent(ObjectFactory.Instance.ChatPoolParent);
    }
}
