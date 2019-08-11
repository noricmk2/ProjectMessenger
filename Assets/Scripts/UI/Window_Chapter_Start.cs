using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Window_Chapter_Start : WindowBase
{
    #region Inspector
    public ExpandTextOutput ChapterText;
    #endregion

    public void Init(string chapter, System.Action endEvent)
    {
        ChapterText.ResetText();
        ChapterText.SetText(chapter, endEvent);
        ChapterText.SetTypeWriteSpeed(0.1f);
        ChapterText.SetLastTerm(0.7f);
        ChapterText.SetPauseText(":", 1.0f);
        ChapterText.PlayText();
    }
}
