using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSetter : MonoBehaviour
{
    public string TextID;
    private TextMeshProUGUI Content;

    private void Awake()
    {
        Content = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Content.text = TextManager.GetSystemText(TextID);
    }
}
