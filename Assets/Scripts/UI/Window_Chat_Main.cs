using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MSUtil;

public class Window_Chat_Main : WindowBase
{
    #region Inspector
    public RectTransform PortraitTrans;
    public RectTransform CharacterPosition;
    public RectTransform ChoiceParent;
    public SpeechBubble MainSpeechBubble;
    public SpriteAnimation PortraitSpriteAnimation;
    public Image BackGroundImage;
    public CustomButton MainTouch;
    #endregion
    private System.Action m_AfterOpenAction;
    private System.Action m_MainTouchAction;

    public void Init(System.Action afterOpenAction = null, System.Action mainTouchAction = null)
    {
        MainSpeechBubble.Init(PortraitSpriteAnimation);
        PortraitSpriteAnimation.Init(DataManager.Instance.GetCharacterData(eCharacter.NIKA));
        PortraitSpriteAnimation.SetAnimation(eCharacterState.NONE);
        m_AfterOpenAction = afterOpenAction;
        m_MainTouchAction = mainTouchAction;
        MainTouch.IsColorHilight = false;
    }

    protected override void AfterOpen()
    {
        base.AfterOpen();
        if (m_AfterOpenAction != null)
            m_AfterOpenAction();
    }

    public void OnMainTouch()
    {
        if(m_MainTouchAction != null)
            m_MainTouchAction();
    }

    public void OnClickBag()
    {

    }

    public void OnClickOption()
    {

    }

    public void OnClickQuit()
    {

    }
}
