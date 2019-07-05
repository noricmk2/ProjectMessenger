using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;
using UnityEngine.UI;

public struct AnimationFrameData
{
    public int StartFrame;
    public int FrameCount;
}

public class SpriteAnimationData
{
    public List<Sprite> SpriteList;
    public AnimationFrameData FrameData;
}

public class SpriteAnimation : MonoBehaviour
{
    private float m_targetFPS = 14f;
    public Dictionary<eCharacterState, SpriteAnimationData> TotalAnimationDataDic = new Dictionary<eCharacterState, SpriteAnimationData>();
    public Image TargetImage { get; private set; }

    private Dictionary<eCharacterState, List<Sprite>> m_AnimationDic = new Dictionary<eCharacterState, List<Sprite>>();
    private Coroutine m_PlayCoruoutine;
    private eCharacterState m_CurrentState;
    private bool m_IsRepeat;
    private float m_FrameTime = 0;
    private Coroutine m_UpdateCoroutine;
    private void Awake()
    {
        if (TargetImage == null)
            TargetImage = GetComponent<Image>();
    }

    public void Init(DataManager.CharacterData data)
    {
        var iter = data.SpriteCountDic.GetEnumerator();

        while (iter.MoveNext())
        {
            if (iter.Current.Value > 0)
            {
                var animData = new SpriteAnimationData();
                animData.SpriteList = data.GetSpriteList(iter.Current.Key);
                animData.FrameData.StartFrame = 0;
                animData.FrameData.FrameCount = animData.SpriteList.Count;
                TotalAnimationDataDic[iter.Current.Key] = animData;
            }
        }
        TargetImage.sprite = TotalAnimationDataDic[eCharacterState.IDLE].SpriteList[0];
        TargetImage.SetNativeSize();
        SetAnimation(eCharacterState.NONE);
        m_FrameTime = 0;
    }

    public void SetAnimation(eCharacterState state, bool isRepeat = false, float fps = 0)
    {
        if (fps > 0)
            m_targetFPS = fps;
        else
            m_targetFPS = ConstValue.DEFAULT_ANIM_FPS;

        TargetImage.gameObject.SetActive_Check(true);
        if (!TotalAnimationDataDic.ContainsKey(state))
        {
            if (state == eCharacterState.NONE)
            {
                m_CurrentState = state;
                if (m_UpdateCoroutine != null)
                    StopCoroutine(m_UpdateCoroutine);
                TargetImage.gameObject.SetActive_Check(false);
                return;
            }
            MSLog.LogError("animation not exist:" + state);
            return;
        }

        m_FrameTime = 0;
        m_IsRepeat = isRepeat;
        if (m_CurrentState != state)
        {
            m_CurrentState = state;
            if (m_UpdateCoroutine != null)
                StopCoroutine(m_UpdateCoroutine);
            m_UpdateCoroutine = StartCoroutine(Update_C());
        }
    }

    IEnumerator Update_C()
    {
        var targetAnimData = TotalAnimationDataDic[m_CurrentState];
        int frameIdx = 0;
        TargetImage.sprite = targetAnimData.SpriteList[frameIdx];
        while (true)
        {
            m_FrameTime += Time.deltaTime;

            if (m_FrameTime > (1 / m_targetFPS))
            {
                frameIdx += Mathf.RoundToInt(m_FrameTime * m_targetFPS);

                if (frameIdx >= targetAnimData.FrameData.FrameCount)
                {
                    if (m_IsRepeat)
                        frameIdx = frameIdx % targetAnimData.FrameData.FrameCount;
                    else
                        yield break;
                }

                var animIdx = targetAnimData.FrameData.StartFrame + frameIdx;
                TargetImage.sprite = targetAnimData.SpriteList[animIdx];
                m_FrameTime = m_FrameTime % 1.0f / m_targetFPS;
            }
            yield return null;
        }
    }
}
