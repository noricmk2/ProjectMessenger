using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MSUtil;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    #region Inspector
    public bool Interactable = true;
    public Image ButtonImage;
    public UnityEvent OnClickEvent;
    public UnityEvent LongClickEvent;
    public UnityEvent OnDownEvent;
    public UnityEvent OnUpEvent;
    #endregion
    [System.NonSerialized]
    public bool IsColorHilight;

    private bool m_IsLongTouch;
    private bool m_LongTouchStart;
    private bool m_IsDown;
    private float m_CurTime;
    private IEnumerator CountEnumerator;
    private Color m_OriginColor;

    private void Awake()
    {
        if (ButtonImage == null)
            ButtonImage = gameObject.GetComponent<Image>();
        m_OriginColor = ButtonImage.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Interactable)
        {
            if (OnClickEvent != null && !m_LongTouchStart)
                OnClickEvent.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_LongTouchStart = false;
        if (Interactable)
        {
            m_IsDown = true;
            if (OnDownEvent != null)
                OnDownEvent.Invoke();

            if (CountEnumerator == null)
            {
                CountEnumerator = PointerDownCount_C();
                StartCoroutine(CountEnumerator);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_CurTime = 0;
        m_IsLongTouch = false;
        m_IsDown = false;
        if (CountEnumerator != null)
        {
            StopCoroutine(CountEnumerator);
            CountEnumerator = null;
        }

        if (Interactable)
        {
            if (OnUpEvent != null)
                OnUpEvent.Invoke();
        }
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if(IsColorHilight)
            ButtonImage.color = ColorPalette.BUTTON_HILIGHT_COLOR;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsColorHilight)
            ButtonImage.color = m_OriginColor;
        m_CurTime = 0;
        m_IsLongTouch = false;
        m_IsDown = false;
        if (CountEnumerator != null)
        {
            StopCoroutine(CountEnumerator);
            CountEnumerator = null;
        }
    }

    IEnumerator PointerDownCount_C()
    {
        while (!m_IsLongTouch && m_CurTime < ConstValue.LONG_CLICK_TIME)
        {
            m_CurTime += Time.deltaTime;
            yield return null;
        }
        m_LongTouchStart = true;
        m_IsLongTouch = true;
        m_CurTime = 0;
    }

    private void Update()
    {
        if (m_IsDown)
        {
            if (LongClickEvent != null && m_IsLongTouch)
                LongClickEvent.Invoke();
        }
    }
}
