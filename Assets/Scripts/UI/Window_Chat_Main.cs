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
    public RectTransform[] CharacterPositions;
    public SpeechBubble MainSpeechBubble;
    public SpriteAnimation PortraitSpriteAnimation;
    #endregion
    private Dictionary<int, CharacterObject> m_CurrentCharacterDic = new Dictionary<int, CharacterObject>();

    public void Init()
    {
        m_CurrentCharacterDic.Clear();
        MainSpeechBubble.Init();
        PortraitSpriteAnimation.Init(DataManager.Instance.GetCharacterData(ConstValue.CHARACTER_NIKA));
        PortraitSpriteAnimation.PlayAnimation(eCharacterState.Idle, true);
    }

    public void SetActiveCharacter(int characterID, bool activate, bool focus = false, eChatPosition posType = eChatPosition.Center)
    {
        if (activate)
        {
            CharacterObject charObj = null;
            if (!m_CurrentCharacterDic.ContainsKey(characterID))
            {
                charObj = ObjectFactory.Instance.ActivateObject<CharacterObject>();
                charObj.transform.Init(CharacterPositions[(int)posType]);
            }
            else
                charObj = m_CurrentCharacterDic[characterID];

            if (charObj != null)
                charObj.SetFocus(focus);
        }
        else
        {
            if (m_CurrentCharacterDic[characterID])
            {
                ObjectFactory.Instance.DeactivateObject(m_CurrentCharacterDic[characterID]);
                m_CurrentCharacterDic.Remove(characterID);
            }
        }
    }

    public void SetCharacterDialouge(int id, string text)
    {
        var charTable = DataManager.Instance.GetCharacterData(id);
    }

    public void SetMainDialouge(string text)
    {
        MainSpeechBubble.SetText(text);
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
