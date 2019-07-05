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
    #endregion
    //private Dictionary<int, CharacterObject> m_CurrentCharacterDic = new Dictionary<int, CharacterObject>();
    private System.Action m_AfterOpenAction;

    public void Init(System.Action afterOpenAction = null)
    {
        //m_CurrentCharacterDic.Clear();
        MainSpeechBubble.Init(PortraitSpriteAnimation);
        PortraitSpriteAnimation.Init(DataManager.Instance.GetCharacterData(eCharacter.NIKA));
        PortraitSpriteAnimation.SetAnimation(eCharacterState.NONE);
        m_AfterOpenAction = afterOpenAction;
    }

    protected override void AfterOpen()
    {
        base.AfterOpen();
        if (m_AfterOpenAction != null)
            m_AfterOpenAction();
    }

    //public void SetActiveCharacter(int characterID, bool activate, bool focus = false)
    //{
    //    if (activate)
    //    {
    //        CharacterObject charObj = null;
    //        if (!m_CurrentCharacterDic.ContainsKey(characterID))
    //            charObj = ObjectFactory.Instance.ActivateObject<CharacterObject>();
    //        else
    //            charObj = m_CurrentCharacterDic[characterID];
    //        charObj.Init(characterID, CharacterPosition);
    //        m_CurrentCharacterDic[characterID] = charObj;
    //    }
    //    else
    //    {
    //        if (m_CurrentCharacterDic[characterID])
    //        {
    //            ObjectFactory.Instance.DeactivateObject(m_CurrentCharacterDic[characterID]);
    //            m_CurrentCharacterDic.Remove(characterID);
    //        }
    //    }
    //}

    //public void SetTextData(DataManager.StoryTextData data)
    //{
    //    MainSpeechBubble.SetCursor(false);
    //    var iter = m_CurrentCharacterDic.GetEnumerator();
    //    while (iter.MoveNext())
    //        iter.Current.Value.Bubble.SetCursor(false);

    //    var charData = DataManager.Instance.GetCharacterData(data.CharacterID);
    //    if (charData.CharacterType == eCharacter.NIKA)
    //    {
    //        MainSpeechBubble.SetTextData(data);
    //        return;
    //    }

    //    if (!m_CurrentCharacterDic.ContainsKey(data.CharacterID))
    //        SetActiveCharacter(data.CharacterID, true);
    //    var charObj = m_CurrentCharacterDic[data.CharacterID];
    //    charObj.Bubble.SetTextData(data);
    //}

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
