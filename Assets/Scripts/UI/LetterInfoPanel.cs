using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterInfoPanel : MonoBehaviour
{
    #region Inspector
    public TextMeshProUGUI AdresseeText;
    public TextMeshProUGUI AdressText;
    public Image Portrait;
    #endregion
    private DataManager.LetterData m_CurrentData;

    public void Init(DataManager.LetterData data)
    {
        m_CurrentData = data;
        var charData = DataManager.Instance.GetCharacterData(m_CurrentData.To);
        AdresseeText.text = m_CurrentData.GetAddresseeText();
        AdressText.text = m_CurrentData.GetAddressText();
        //TODO:별도의 포트레이트 이미지 로드로 변경
        Portrait.sprite = charData.GetSpriteList(MSUtil.eCharacterState.IDLE)[0];
    }
}
