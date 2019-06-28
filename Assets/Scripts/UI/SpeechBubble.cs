using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour, IPoolObjectBase
{
    #region Inspector
    public RectTransform BubbleTrans;
    public Image BubbleBG; 
    public ExpandTextOutput Text;
    #endregion

    public void Init()
    {
        Text.Reset();
    }

    public void SetText(string content, float fontSize = 25)
    {
        Text.SetText(content, fontSize);
        Text.PlayText();
    }

    public void PushAction()
    {
        gameObject.SetActive(false);
    }

    public void PopAction()
    {
        gameObject.SetActive(true);
    }
}
