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

    private IEnumerator m_UpdateCouroutine = null;
    private Window_Chat_Main m_ChatWindow;
    private DataManager.ChapterData m_CurrentChapterData;
    private DataManager.ChapterTextData m_CurrentChapterTextData;
    private int m_CurrentEventIdx;
    private Dictionary<int, CharacterObject> m_CurrentCharacterDic = new Dictionary<int, CharacterObject>();
    private List<ChoiceObject> m_ChoiceList = new List<ChoiceObject>();
    private ItemObject m_ItemObject;
    private CharacterObject m_CurrentChatter;

    public List<BackLogTextData> LogTextList { get; private set; }

    private bool m_IsChoice = false;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        //TODO:챕터 정보 불러오기
        if(LogTextList == null)
            LogTextList = new List<BackLogTextData>();
        ChatCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        ChatCanvas.worldCamera = UICamera.Instance.Camera;

        m_CurrentChapterData = DataManager.Instance.GetChapterData(UserInfo.Instance.CurrentGameData.CurrentChapterID);
        //if (UserInfo.Instance.GetCurrentChapterStage() == eStageTag.START)
        //{
        //    var window = WindowBase.OpenWindow(WindowBase.eWINDOW.ChapterStart, WindowParent, true) as Window_Chapter_Start;
        //    window.Init(m_CurrentChapterData.GetChapterTitle(), ()=>
        //    {
        //        OpenMainChatWindow();
        //    });
        //}
        //else
        {
            OpenMainChatWindow();
        }
    }

    public void OpenMainChatWindow()
    {
        m_CurrentChatter = null;
        m_ChatWindow = WindowBase.OpenWindowWithFade(WindowBase.eWINDOW.ChatMain, WindowParent, true) as Window_Chat_Main;
        Release();
        SetMainTouch(true);

        m_ChatWindow.Init(() =>
        {
            if (m_UpdateCouroutine == null)
                m_UpdateCouroutine = Update_C();
            StartCoroutine(m_UpdateCouroutine);
        }, OnMainTouch);
        m_CurrentCharacterDic[m_ChatWindow.MainCharacter.CurrentCharacterData.ID] = m_ChatWindow.MainCharacter;
        SetChapterDialogue((eStageTag)UserInfo.Instance.CurrentGameData.CurrentChapterStage, "DL1");
    }

    IEnumerator Update_C()
    {
        var textData = m_CurrentChapterTextData.GetTextData(m_CurrentEventIdx);
        SetTextData(textData);
        while (true)
        {
            if (m_ChatWindow.MainTouch.gameObject.activeSelf && 
                (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            {
                OnMainTouch();
            }
            yield return null;
        }
    }

    private void OnMainTouch()
    {
        if (!m_IsChoice && m_CurrentChatter != null && m_CurrentChatter.CharacterActivate)
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

        var charData = DataManager.Instance.GetCharacterData(data.CharacterID);
        if (!m_CurrentCharacterDic.ContainsKey(data.CharacterID))
            SetActiveCharacter(data.CharacterID, true);
        m_CurrentChatter = m_CurrentCharacterDic[data.CharacterID];
        m_CurrentChatter.SetFocus(true, () =>
        {
            m_CurrentChatter.Bubble.SetTextData(data, TextEvent);
        });

        var logData = new BackLogTextData(data.GetCharacterName(), TextManager.GetStoryText(data.ID));
        LogTextList.Insert(0, logData);
    }

    public void CheckOverlapEvent(eOverlapType targetType)
    {
        var nextTextData = m_CurrentChapterTextData.GetTextData(m_CurrentEventIdx + 1);
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
        ++m_CurrentEventIdx;
        var textData = m_CurrentChapterTextData.GetTextData(m_CurrentEventIdx);

        if (textData != null)
            SetTextData(textData);
        else
        {
            SetMainTouch(false);
            return;
        }
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
                    
                    target.BubbleBG.enabled = true;
                    target.ParentObject.CharacterActivate = true;
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
                    sequence.SetDelay(Func.Msec2sec(Func.GetFloat(time)));
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
                        iter.Current.Value.Bubble.gameObject.SetActive_Check(false);
                    }
                }
                break;
            case eTextEventTag.APRITEM:
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
        }
    }

    public void SetMainTouch(bool active)
    {
        m_ChatWindow.MainTouch.gameObject.SetActive_Check(active);
    }

    private void ChoiceEvent(string flag)
    {
        m_IsChoice = false;
        SetChapterDialogue((eStageTag)UserInfo.Instance.CurrentGameData.CurrentChapterStage, flag);
        for (int i = 0; i < m_ChoiceList.Count; ++i)
            ObjectFactory.Instance.DeactivateObject(m_ChoiceList[i]);
        m_CurrentEventIdx = 1;
        var textData = m_CurrentChapterTextData.GetTextData(m_CurrentEventIdx);
        if (textData != null)
            SetTextData(textData);
        SetMainTouch(true);
    }

    private void SetChapterDialogue(eStageTag tag, string dialogueID)
    {
        UserInfo.Instance.CurrentGameData.CurrentChapterStage = (int)tag;
        m_CurrentChapterTextData = m_CurrentChapterData.GetChapterTextData(tag, dialogueID);
        if (m_ChatWindow.BackGroundImage.sprite.name != m_CurrentChapterTextData.BGResourceName)
            m_ChatWindow.BackGroundImage.sprite = m_CurrentChapterTextData.GetBackGroundSprite();
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
        m_CurrentChatter = null;
        if (m_ChatWindow != null)
            m_ChatWindow.ReleaseWindow();
        LogTextList.Clear();
    }
}
