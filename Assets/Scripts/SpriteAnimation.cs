using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    private Dictionary<eCharacterState, List<Sprite>> m_AnimationDic = new Dictionary<eCharacterState, List<Sprite>>();
    private float AnimationSpeed = 0.1f;
    private Image m_TargetImage;
    private IEnumerator m_PlayCoruoutine;
    private eCharacterState m_CurrentState;
    private bool m_IsRepeat;

    private void Awake()
    {
        if (m_TargetImage == null)
            m_TargetImage = GetComponent<Image>();
    }

    public void Init(DataManager.CharacterData data)
    {
        var iter = data.SpriteCountDic.GetEnumerator();
        while (iter.MoveNext())
        {
            if (iter.Current.Value > 0)
                m_AnimationDic[iter.Current.Key] = data.GetSpriteList(iter.Current.Key);
        }
    }

    public void PlayAnimation(eCharacterState state, bool isRepeat)
    {
        m_CurrentState = state;
        m_IsRepeat = isRepeat;
        if (m_TargetImage != null && m_AnimationDic.ContainsKey(state))
        {
            if (m_PlayCoruoutine != null)
                StopCoroutine(m_PlayCoruoutine);
            else
                m_PlayCoruoutine = PlayAnimation_C();
            StartCoroutine(m_PlayCoruoutine);
        }
    }

    //TODO: 반복재생 관련 로직 수정
    IEnumerator PlayAnimation_C()
    {
        var spriteList = m_AnimationDic[m_CurrentState];
        if (spriteList == null || spriteList.Count == 0)
        {
            MSLog.LogError("no sprite list");
            yield break;
        }

        int idx = 0;
        bool playSet = true;
        while (m_IsRepeat || idx < spriteList.Count)
        {
            m_TargetImage.sprite = spriteList[idx];
            m_TargetImage.SetNativeSize();

            if (playSet)
                ++idx;
            else
                --idx;

            if (idx >= spriteList.Count && m_IsRepeat)
            {
                playSet = false;
                --idx;
            }
            if (idx < 0 && m_IsRepeat)
            {
                playSet = true;
                ++idx;
            }

            yield return new WaitForSeconds(AnimationSpeed);
        }
    }
}
