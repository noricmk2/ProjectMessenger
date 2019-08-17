using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;
using DG.Tweening;
using System.Text.RegularExpressions;

public class ChatObject : MonoBehaviour
{
    #region Inspector
    public Transform WindowParent;
    public Canvas ChatCanvas;
    public Transform PoolParent;
    #endregion
    public static ChatObject Instance;

    private Coroutine m_UpdateCouroutine;
    private Window_Chat_Main m_ChatWindow;
    private Window_Chat_Event m_EventWindow;
    private DataManager.ChapterData m_CurrentChapterData;
    private DataManager.StageData m_CurrentStageData;
    private DataManager.ChapterTextData m_CurrentChapterTextData;
    private int m_CurrentTextIdx;
    private Dictionary<int, CharacterObject> m_CurrentCharacterDic = new Dictionary<int, CharacterObject>();
    private List<ChoiceObject> m_ChoiceList = new List<ChoiceObject>();
    private ItemObject m_ItemObject;
    private CharacterObject m_CurrentChatter;
    private eChatType m_CurrentChatType;
    private bool m_IsMainTouchOn;

    public List<BackLogTextData> LogTextList { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        //TODO:챕터 정보 불러오기
        if (LogTextList == null)
            LogTextList = new List<BackLogTextData>();
        LogTextList.Clear();

        ChatCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        ChatCanvas.worldCamera = UICamera.Instance.Camera;
        m_CurrentChapterData = DataManager.Instance.GetChapterData(UserInfo.Instance.CurrentGameData.CurrentChapterID);
        m_CurrentStageData = DataManager.Instance.GetStageData((eStageTag)UserInfo.Instance.CurrentGameData.CurrentChapterStage);
        m_CurrentChatType = eChatType.NONE;

        if (UserInfo.Instance.GetCurrentChapterStage() == eStageTag.START)
        {
            var window = WindowBase.OpenWindow(WindowBase.eWINDOW.ChapterStart, WindowParent, true) as Window_Chapter_Start;
            window.Init(m_CurrentChapterData.GetChapterTitle(), ()=>
            {
                SetChapterTextData(ConstValue.FIRST_DIALOUGE_ID);
                window.CloseWindow();
            });
        }
        else
        {
            SetChapterTextData(ConstValue.FIRST_DIALOUGE_ID);
        }
    }

    private void SetChatWindow()
    {
        Release(true);
        m_CurrentTextIdx = 0;

        switch (m_CurrentChatType)
        {
            case eChatType.NORMAL:
                {
                    m_ChatWindow = WindowBase.OpenWindowWithTransition(WindowBase.eWINDOW.ChatMain, m_CurrentChapterTextData.TransitionType, WindowParent, true) as Window_Chat_Main;
                    m_ChatWindow.Init(m_CurrentChapterTextData, () =>
                    {
#if UNITY_EDITOR || UNITY_STANDALONE
                        if (m_UpdateCouroutine != null)
                            StopCoroutine(m_UpdateCouroutine);
                        m_UpdateCouroutine = StartCoroutine(Update_C());
#endif
                        PlayNextText();
                    }, OnMainTouch);
                    m_CurrentCharacterDic[m_ChatWindow.MainCharacter.CurrentCharacterData.ID] = m_ChatWindow.MainCharacter;
                }
                break;
            case eChatType.EVENT:
                {
                    m_EventWindow = WindowBase.OpenWindowWithTransition(WindowBase.eWINDOW.ChatEvent, m_CurrentChapterTextData.TransitionType, WindowParent, true) as Window_Chat_Event;
                    m_EventWindow.Init(() =>
                    {
#if UNITY_EDITOR || UNITY_STANDALONE
                        if (m_UpdateCouroutine != null)
                            StopCoroutine(m_UpdateCouroutine);
                        m_UpdateCouroutine = StartCoroutine(Update_C());
#endif
                        PlayNextText();
                    }, OnMainTouch, m_CurrentChapterTextData.EnterEffectType);
                }
                break;
        }
        SetMainTouch(true);
    }

    IEnumerator Update_C()
    {
        while (true)
        {
            if (m_IsMainTouchOn && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            {
                OnMainTouch();
            }
            yield return null;
        }
    }

    private void OnMainTouch()
    {
        if (!AlwaysTopCanvas.Instance.IsOnFade && m_CurrentChatType != eChatType.CHOICE
            && m_CurrentChatter != null && m_CurrentChatter.CharacterActivate)
        {
            if (GameManager.IsPlayText)
            {
                m_CurrentChatter.Bubble.CancelTypeWrite();
            }
            else
            {
                PlayNextText();
            }
        }
    }

    private void SetTextData(DataManager.StoryTextData data)
    {
        var iter = m_CurrentCharacterDic.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Value.Bubble.SetCursor(false);
            iter.Current.Value.SetFocus(false);
        }

        bool charCreate = false;
        var charData = DataManager.Instance.GetCharacterData(data.CharacterID);
        if (!m_CurrentCharacterDic.ContainsKey(data.CharacterID))
        {
            ActiveCharacter(data.CharacterID, data.BubblePos, data.BubbleResource, true);
            charCreate = true;
        }
        m_CurrentChatter = m_CurrentCharacterDic[data.CharacterID];

        bool hasChoiceTag = false;
        var tagIter = data.GetEventTagDic().GetEnumerator();
        while (tagIter.MoveNext())
        {
            if (tagIter.Current.Value.Tag == eTextEventTag.CHO)
            {
                hasChoiceTag = true;
                break;
            }
        }

        if (charCreate && m_CurrentChatType != eChatType.EVENT)
            m_CurrentChatter.Bubble.SetTextData(data, TextEvent);
        else
        {
            bool normalSet = !(hasChoiceTag && m_CurrentChatType == eChatType.EVENT);
            m_CurrentChatter.SetFocus(true, normalSet, () =>
            {
                m_CurrentChatter.Bubble.SetTextData(data, TextEvent, normalSet);
            });
        }

        var logData = new BackLogTextData(data.GetCharacterName(), TextManager.GetStoryText(data.ID));
        if(!string.IsNullOrEmpty(logData.Content))
            LogTextList.Insert(0, logData);
    }

    public void CheckOverlapEvent(eOverlapType targetType)
    {
        var nextTextData = m_CurrentChapterTextData.GetTextData(m_CurrentTextIdx + 1);
        if (nextTextData != null)
        {
            var dic = nextTextData.GetEventTagDic();
            var iter = dic.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.Value.Tag == eTextEventTag.OVR)
                {
                    var overlapType = Func.GetEnum<eOverlapType>(iter.Current.Value.Value);
                    if(overlapType == targetType)
                        PlayNextText();
                }
            }
        }
    }

    public void PlayNextText()
    {
        if (m_ItemObject != null)
        {
            ObjectFactory.Instance.DeactivateObject(m_ItemObject);
            m_ItemObject = null;
        }

        ++m_CurrentTextIdx;
        var textData = m_CurrentChapterTextData.GetTextData(m_CurrentTextIdx);

        if (textData != null)
            SetTextData(textData);
        else
        {
            m_CurrentTextIdx = 1;
            var nextData = m_CurrentStageData.GetNextChapterTextData(m_CurrentChapterTextData);
            if (nextData != null)
                SetChapterTextData(nextData.DialogueID);
            else
            {
                //TODO: 스테이지 끝났을때의 처리
                SetMainTouch(false);
                switch (m_CurrentStageData.StageTag)
                {
                    case eStageTag.START:
                        UserInfo.Instance.SetNextStage(eStageTag.PREDELV);
                        Init();
                        break;
                    case eStageTag.PREDELV:
                        break;
                    case eStageTag.ONDELVLESS:
                        break;
                    case eStageTag.DAYEND:
                        break;
                }
            }
            return;
        }
    }

    public void ActiveCharacter(int characterID, Vector3 bubblePos, string bubbleRes = null, bool focus = false)
    {
        CharacterObject charObj = null;
        if (m_ChatWindow != null)
        {
            if (!m_CurrentCharacterDic.ContainsKey(characterID))
                charObj = ObjectFactory.Instance.ActivateObject<CharacterObject>();
            else
                charObj = m_CurrentCharacterDic[characterID];

            if (bubblePos == Vector3.zero)
                charObj.Init(characterID, m_ChatWindow.CharacterPosition);
            else if (bubblePos != Vector3.zero && characterID != ConstValue.NONE_CHARACTER_ID)
                charObj.Init(characterID, m_ChatWindow.CharacterPosition, bubblePos, bubbleRes);
            else
                charObj.InitForBubbleOnly(characterID, m_ChatWindow.OutsidePosition, bubblePos, bubbleRes);
        }
        else
        {
            if (!m_CurrentCharacterDic.ContainsKey(characterID))
                charObj = ObjectFactory.Instance.ActivateObject<CharacterObject>();
            else
                charObj = m_CurrentCharacterDic[characterID];

            charObj.InitForBubbleOnly(characterID, m_EventWindow.BackGroundImage.transform, bubblePos, bubbleRes);
        }
        m_CurrentCharacterDic[characterID] = charObj;
    }

    public void DeactiveCharacter(int characterID)
    {
        if (m_CurrentCharacterDic[characterID])
        {
            var character = m_CurrentCharacterDic[characterID];
            character.Release();
            ObjectFactory.Instance.DeactivateObject(character);
            m_CurrentCharacterDic.Remove(characterID);
        }
    }

    private IEnumerator TextEvent(SpeechBubble target, DataManager.TextEventData data)
    {
        var targetSpriteAnim = target.ParentObject.CharacterAnimation;

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

                    m_CurrentChatter.SetFocus(true, true, ()=>
                    {
                        target.ParentObject.CharacterActivate = true;
                    });
                    targetSpriteAnim.SetAnimation(eCharacterState.IDLE, true);
                }
                break;
            case eTextEventTag.DPR:
                {
                    target.ParentObject.CharacterActivate = false;
                    var sequence = DOTween.Sequence();
                    sequence.Append(targetSpriteAnim.TargetImage.DOColor(Color.black, ConstValue.CHARACTER_APPEAR_TIME));
                    sequence.Append(targetSpriteAnim.TargetImage.DOColor(ColorPalette.FADE_IN_BLACK, ConstValue.CHARACTER_APPEAR_TIME));
                    sequence.Play();
                    while (sequence.IsActive() && !sequence.IsComplete())
                    {
                        yield return null;
                    }
                    target.SetActive(false, true);
                    targetSpriteAnim.SetAnimation(eCharacterState.NONE);
                    if (target.ParentObject != null)
                        DeactiveCharacter(target.ParentObject.CurrentCharacterData.ID);
                    PlayNextText();
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
                    m_CurrentChatType = eChatType.CHOICE;
                    var split = data.Value.Split('^');
                    var targetParent = m_ChatWindow != null ? m_ChatWindow.ChoiceParent : m_EventWindow.ChoiceParent;
                    if (m_EventWindow != null)
                        m_EventWindow.ChoiceParent.gameObject.SetActive_Check(true);

                    for (int i = 0; i < split.Length; ++i)
                    {
                        var choiceObj = ObjectFactory.Instance.ActivateObject<ChoiceObject>();
                        var subSplit = split[i].Split(';');
                        choiceObj.Init(targetParent, subSplit[0], subSplit[1], ChoiceEvent);
                        m_ChoiceList.Add(choiceObj);
                    }
                    SetMainTouch(false);
                }
                break;
            case eTextEventTag.OVR:
                {
                    m_ChatWindow.MainCharacter.transform.SetAsLastSibling();
                }
                break;
            case eTextEventTag.BACKDOWN:
                {
                    m_ChatWindow.MainCharacter.transform.SetAsFirstSibling();
                }
                break;
            case eTextEventTag.TERM:
                {
                    var time = Regex.Replace(data.Value, @"\D", "");
                    var sequence = DOTween.Sequence();
                    sequence.Append(transform.DOScale(Vector3.one, Func.Msec2sec(Func.GetFloat(time))));
                    sequence.Play();
                    while (sequence.IsActive() && !sequence.IsComplete())
                    {
                        yield return null;
                    }

                    if(data.Value.Contains("BACKDOWN"))
                        m_ChatWindow.MainCharacter.transform.SetAsFirstSibling();
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
                m_ChatWindow.MainCharacter.CharacterAnimation.SetAnimation(eCharacterState.NONE);
                var iter = m_CurrentCharacterDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    if (iter.Current.Value.CurrentCharacterData.CharacterType != eCharacter.NIKA)
                    {
                        iter.Current.Value.CharacterAnimation.SetAnimation(eCharacterState.NONE);
                        iter.Current.Value.Bubble.SetActive(false, true);
                    }
                }
                break;
            case eTextEventTag.APRITEM:
                {
                    if (m_ItemObject == null)
                        m_ItemObject = ObjectFactory.Instance.ActivateObject<ItemObject>();
                    var resName = "icon_" + data.Value.ToLower();
                    m_ItemObject.Init(m_ChatWindow.BackGroundImage.transform.parent, resName);
                    var cameraSize = UICamera.Instance.Camera.orthographicSize;
                    var setY = UICamera.Instance.Camera.transform.position.y - cameraSize;
                    var pos = m_ItemObject.transform.position;
                    pos.y = setY;
                    m_ItemObject.transform.position = pos;
                    m_ItemObject.transform.DOMove(m_ChatWindow.BackGroundImage.transform.position, 0.3f).SetEase(Ease.OutBack);
                }
                break;
            case eTextEventTag.DPRITEM:
                {
                    if (m_ItemObject != null)
                    {
                        ObjectFactory.Instance.DeactivateObject(m_ItemObject);
                        m_ItemObject = null;
                    }
                }
                break;
            case eTextEventTag.MAILSORT:
                {
                    SetMainTouch(false);
                    m_ChatWindow.SetMailBundle();
                }
                break;
            case eTextEventTag.NOTIFY:
                {
                    if (m_ChatWindow != null)
                    {
                        var content = TextManager.GetSystemText(ConstValue.NOTIFY_TEXT + data.Value);
                        m_ChatWindow.NotifyPanel.gameObject.SetActive_Check(true);
                        m_ChatWindow.NotifyPanel.Init(content);
                    }
                }
                break;
            case eTextEventTag.SHAKE:
                {
                    var deltaTime = 0f;
                    var targetTime = Func.Msec2sec(Func.GetFloat(data.Value));
                    UICamera.Instance.CameraShake(WindowParent, 10f, targetTime, () =>
                    {
                    });
                    while (targetTime >= deltaTime)
                    {
                        deltaTime += Time.deltaTime;
                        yield return null;
                    }
                }
                break;
            case eTextEventTag.FLASH:
                {
                    var count = Func.GetInt(data.Value);
                    UICamera.Instance.Flash(count);
                }
                break;
            case eTextEventTag.TRANSITION:
                {
                    var deltaTime = 0f;
                    var targetTime = 0.5f;
                    AlwaysTopCanvas.Instance.SetFadeAnimation(targetTime, true, eTransitionType.NORMAL);
                    while (targetTime >= deltaTime)
                    {
                        deltaTime += Time.deltaTime;
                        yield return null;
                    }
                }
                break;
        }
    }

    public void SetMainTouch(bool active)
    {
        m_IsMainTouchOn = active;
        if(m_ChatWindow != null)
            m_ChatWindow.MainTouch.gameObject.SetActive_Check(active);
        else
            m_EventWindow.MainTouch.gameObject.SetActive_Check(active);
    }

    private void ChoiceEvent(string content, string flag)
    {
        m_CurrentChatType = m_ChatWindow != null ? eChatType.NORMAL : eChatType.EVENT;
        if (m_CurrentChatType == eChatType.EVENT)
            m_EventWindow.ChoiceParent.gameObject.SetActive_Check(false);

        SetChapterTextData(flag);

        var logData = new BackLogTextData(TextManager.GetSystemText(ConstValue.SELECT_TEXT), content);
        LogTextList.Insert(0, logData);

        for (int i = 0; i < m_ChoiceList.Count; ++i)
            ObjectFactory.Instance.DeactivateObject(m_ChoiceList[i]);
        SetMainTouch(true);
    }

    private void SetChapterTextData(string dialogueID)
    {
        m_CurrentChapterTextData = m_CurrentStageData.GetChapterTextData(dialogueID);

        if (m_CurrentChatType != m_CurrentChapterTextData.ChatType)
        {
            m_CurrentChatType = m_CurrentChapterTextData.ChatType;
            SetChatWindow();
        }
        else
        {
            m_CurrentChatType = m_CurrentChapterTextData.ChatType;
            m_CurrentTextIdx = 0;
            PlayNextText();
        }

        var targetImage = m_ChatWindow != null ? m_ChatWindow.BackGroundImage : m_EventWindow.BackGroundImage;
        if (targetImage.sprite.name != m_CurrentChapterTextData.BGResourceName)
            targetImage.sprite = m_CurrentChapterTextData.GetBackGroundSprite();
    }

    public void Release(bool closeWindow = false)
    {
        if (m_ItemObject != null)
        {
            ObjectFactory.Instance.DeactivateObject(m_ItemObject);
            m_ItemObject = null;
        }
        for (int i = 0; i < m_ChoiceList.Count; ++i)
            ObjectFactory.Instance.DeactivateObject(m_ChoiceList[i]);

        var iter = m_CurrentCharacterDic.GetEnumerator();
        while(iter.MoveNext())
        {
            if (iter.Current.Value.CurrentCharacterData.CharacterType != eCharacter.NIKA)
                ObjectFactory.Instance.DeactivateObject(iter.Current.Value);
        }
        m_CurrentCharacterDic.Clear();
        if (m_UpdateCouroutine != null)
        {
            StopCoroutine(m_UpdateCouroutine);
            m_UpdateCouroutine = null;
        }
        m_CurrentTextIdx = 1;
        m_CurrentChatter = null;
        if (closeWindow)
        {
            if (m_ChatWindow != null)
                m_ChatWindow.CloseWindow();
            if (m_EventWindow != null)
                m_EventWindow.CloseWindow();
        }
        m_ChatWindow = null;
        m_EventWindow = null;
    }
}
