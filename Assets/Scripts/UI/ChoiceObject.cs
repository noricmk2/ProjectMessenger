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
    private System.Action<string> m_ClickEvent;
    private string m_DialogueFlag;

    public void Init(Transform parent, string content, string flag, System.Action<string> clickEvent)
    {
        Text.text = "-" + content;
        m_ClickEvent = clickEvent;
        m_DialogueFlag = flag;
        transform.Init(parent);
        ChoiceButton.IsColorHilight = true;
    }

    public void OnClickButton()
    {
        if (m_ClickEvent != null)
            m_ClickEvent(m_DialogueFlag);
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
