using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;
using DG.Tweening;

public class ChatObject : MonoBehaviour
{
    #region Inspector
    public Transform WindowParent;
    public Canvas ChatCanvas;
    #endregion
    private IEnumerator m_UpdateCouroutine = null;
    private Window_Chat_Main m_ChatWindow;
    private DataManager.ChapterData m_CurrentChapterData;
    private DataManager.ChapterTextData m_CurrentChapterTextData;
    private int m_CurrentEventIdx;
    private Dictionary<int, CharacterObject> m_CurrentCharacterDic = new Dictionary<int, CharacterObject>();
    private List<ChoiceObject> m_ChoiceList = new List<ChoiceObject>();
    private ItemObject m_ItemObject;

    private bool m_IsChoice = false;

    public void Init()
    {
        //TODO:챕터 정보 불러오기
        ChatCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        ChatCanvas.worldCamera = UICamera.Instance.Camera;
        Release();
        m_CurrentChapterData = DataManager.Instance.GetChapterData(UserInfo.Instance.CurrentGameData.CurrentChapterID);
        SetChapterDialogue((eEventTag)UserInfo.Instance.CurrentGameData.CurrentChapterEvent, "DL1");
        m_ChatWindow = WindowBase.OpenWindowWithFade(WindowBase.eWINDOW.ChatMain, WindowParent, true) as Window_Chat_Main;
        m_ChatWindow.Init(()=>
        {
            if (m_UpdateCouroutine == null)
                m_UpdateCouroutine = Update_C();
            StartCoroutine(m_UpdateCouroutine);
        }, OnMainTouch);
    }

    int testIdx = 1;
    IEnumerator Update_C()
    {
        var textData = m_CurrentChapterTextData.GetTextData(m_CurrentEventIdx);
        SetTextData(textData);
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ++testIdx;
                if (testIdx >= (int)eCharacterState.LENGTH)
                    testIdx = 0;
                m_ChatWindow.PortraitSpriteAnimation.SetAnimation((eCharacterState)testIdx, testIdx == 1 ? true : false);
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                SetActiveCharacter(10004, true);
            }
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                OnMainTouch();
            }
            yield return null;
        }
    }

    public void OnMainTouch()
    {
        if (!m_IsChoice && !GameManager.IsPlayText)
        {
            ++m_CurrentEventIdx;
            var textData = m_CurrentChapterTextData.GetTextData(m_CurrentEventIdx);

            if (textData != null)
                SetTextData(textData);
            else
            {
                //임시
                MSSceneManager.Instance.EnterScene(SceneBase.eScene.INTRO);
            }
        }
    }

    private void SetTextData(DataManager.StoryTextData data)
    {
        m_ChatWindow.MainSpeechBubble.SetCursor(false);
        m_ChatWindow.PortraitSpriteAnimation.SetColor(ColorPalette.CHARACTER_HILIGHT_COLOR);
        var iter = m_CurrentCharacterDic.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Value.Bubble.SetCursor(false);
            iter.Current.Value.SetFocus(false);
        }

        var charData = DataManager.Instance.GetCharacterData(data.CharacterID);
        if (charData.CharacterType == eCharacter.NIKA)
        {
            m_ChatWindow.MainSpeechBubble.SetTextData(data, TextEvent);
            m_ChatWindow.PortraitSpriteAnimation.SetColor(Color.white);
            return;
        }

        if (!m_CurrentCharacterDic.ContainsKey(data.CharacterID))
            SetActiveCharacter(data.CharacterID, true);
        var charObj = m_CurrentCharacterDic[data.CharacterID];
        charObj.SetFocus(true);
        charObj.Bubble.SetTextData(data, TextEvent);
    }

    public void SetActiveCharacter(int characterID, bool activate, bool focus = false)
    {
        if (activate)
        {
            CharacterObject charObj = null;
            if (!m_CurrentCharacterDic.ContainsKey(characterID))
                charObj = ObjectFactory.Instance.ActivateObject<CharacterObject>();
            else
                charObj = m_CurrentCharacterDic[characterID];
            charObj.Init(characterID, m_ChatWindow.CharacterPosition);
            m_CurrentCharacterDic[characterID] = charObj;
        }
        else
        {
            if (m_CurrentCharacterDic[characterID])
            {
                var character = m_CurrentCharacterDic[characterID];
                character.Release();
                ObjectFactory.Instance.DeactivateObject(character);
                m_CurrentCharacterDic.Remove(characterID);
            }
        }
    }

    public IEnumerator TextEvent(SpeechBubble target, DataManager.TextEventData data)
    {
        var targetSpriteAnim = target.ParentObject != null ? target.ParentObject.CharacterAnimation : target.ParentAnimation;

        switch (data.Tag)
        {
            case eTextEventTag.APR:
                {
                    targetSpriteAnim.TargetImage.color = new Color(0, 0, 0, 0);
                    targetSpriteAnim.TargetImage.gameObject.SetActive_Check(true);
                    var sequence = DOTween.Sequence();
                    sequence.Append(targetSpriteAnim.TargetImage.DOColor(ColorPalette.FADE_OUT_BLACK, ConstValue.CHARACTER_APPEAR_TIME));
                    sequence.Append(targetSpriteAnim.TargetImage.DOColor(Color.white, ConstValue.CHARACTER_APPEAR_TIME));
                    sequence.Play();
                    while (sequence.IsActive() && !sequence.IsComplete())
                    {
                        yield return null;
                    }
                    target.BubbleBG.enabled = true;
                    targetSpriteAnim.SetAnimation(eCharacterState.IDLE, true);
                }
                break;
            case eTextEventTag.DPR:
                {
                    var sequence = DOTween.Sequence();
                    sequence.Append(targetSpriteAnim.TargetImage.DOColor(Color.black, ConstValue.CHARACTER_APPEAR_TIME));
                    sequence.Append(targetSpriteAnim.TargetImage.DOColor(ColorPalette.FADE_IN_BLACK, ConstValue.CHARACTER_APPEAR_TIME));
                    sequence.Play();
                    while (sequence.IsActive() && !sequence.IsComplete())
                    {
                        yield return null;
                    }
                    target.BubbleBG.enabled = false;
                    targetSpriteAnim.SetAnimation(eCharacterState.NONE);
                    if (target.ParentObject != null)
                        SetActiveCharacter(target.ParentObject.CurrentCharacterData.ID, false);
                    ++m_CurrentEventIdx;
                    var textData = m_CurrentChapterTextData.GetTextData(m_CurrentEventIdx);

                    if (textData != null)
                        SetTextData(textData);
                    else
                    {
                        //임시
                        MSSceneManager.Instance.EnterScene(SceneBase.eScene.INTRO);
                    }
                }
                break;
            case eTextEventTag.HL:
                break;
            case eTextEventTag.CNG:
                var animType = Func.GetEnum<eCharacterState>(data.Value);
                targetSpriteAnim.SetAnimation(animType, animType == eCharacterState.IDLE ? true : false);
                break;
            case eTextEventTag.CHO:
                {
                    m_IsChoice = true;
                    var split = data.Value.Split('^');
                    for (int i = 0; i < split.Length; ++i)
                    {
                        var cObj = ObjectFactory.Instance.ActivateObject<ChoiceObject>();
                        var subSplit = split[i].Split(';');
                        cObj.Init(m_ChatWindow.ChoiceParent, subSplit[0], subSplit[1], ChoiceEvent);
                        m_ChoiceList.Add(cObj);
                    }
                    m_ChatWindow.MainTouch.gameObject.SetActive_Check(false);
                }
                break;
            case eTextEventTag.FONTBIG:
                break;
            case eTextEventTag.FONTSML:
                break;
            case eTextEventTag.FONTCOL:
                break;
            case eTextEventTag.DRK:
                m_ChatWindow.BackGroundImage.DOColor(ColorPalette.FADE_OUT_BLACK, ConstValue.CHARACTER_APPEAR_TIME);
                m_ChatWindow.PortraitSpriteAnimation.SetAnimation(eCharacterState.NONE);
                var iter = m_CurrentCharacterDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    iter.Current.Value.CharacterAnimation.SetAnimation(eCharacterState.NONE);
                    iter.Current.Value.Bubble.gameObject.SetActive_Check(false);
                }
                break;
            case eTextEventTag.APITEM:
                {
                    if (m_ItemObject == null)
                        m_ItemObject = ObjectFactory.Instance.ActivateObject<ItemObject>();
                    var resName = "icon_" + data.Value.ToLower();
                    m_ItemObject.Init(m_ChatWindow.BackGroundImage.transform, resName);
                    var cameraSize = UICamera.Instance.Camera.orthographicSize;
                    var setY = UICamera.Instance.Camera.transform.position.y - cameraSize;
                    var pos = m_ItemObject.transform.position;
                    pos.y = setY;
                    m_ItemObject.transform.position = pos;
                    m_ItemObject.transform.DOMove(m_ChatWindow.BackGroundImage.transform.position, 0.3f).SetEase(Ease.OutBack);
                }
                break;
            case eTextEventTag.DPITEM:
                {
                    if (m_ItemObject != null)
                    {
                        ObjectFactory.Instance.DeactivateObject(m_ItemObject);
                        m_ItemObject = null;
                    }
                }
                break;
        }
    }

    public void ChoiceEvent(string flag)
    {
        m_IsChoice = false;
        SetChapterDialogue((eEventTag)UserInfo.Instance.CurrentGameData.CurrentChapterEvent, flag);
        for (int i = 0; i < m_ChoiceList.Count; ++i)
            ObjectFactory.Instance.DeactivateObject(m_ChoiceList[i]);
        m_CurrentEventIdx = 1;
        var textData = m_CurrentChapterTextData.GetTextData(m_CurrentEventIdx);
        if (textData != null)
            SetTextData(textData);
        m_ChatWindow.MainTouch.gameObject.SetActive_Check(true);
    }

    public void SetChapterDialogue(eEventTag tag, string dialogueID)
    {
        UserInfo.Instance.CurrentGameData.CurrentChapterEvent = (int)tag;
        m_CurrentChapterTextData = m_CurrentChapterData.GetChapterTextData(tag, dialogueID);
    }

    public void Release()
    {
        if (m_ItemObject != null)
        {
            ObjectFactory.Instance.DeactivateObject(m_ItemObject);
            m_ItemObject = null;
        }
        for (int i = 0; i < m_ChoiceList.Count; ++i)
            ObjectFactory.Instance.DeactivateObject(m_ChoiceList[i]);
        m_CurrentCharacterDic.Clear();
        if (m_UpdateCouroutine != null)
        {
            StopCoroutine(m_UpdateCouroutine);
            m_UpdateCouroutine = null;
        }
        m_CurrentEventIdx = 1;
    }
}
