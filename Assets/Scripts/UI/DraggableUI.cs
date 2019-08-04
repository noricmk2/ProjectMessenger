using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MSUtil;

public class DraggableUI : MonoBehaviour, IPointerExitHandler
{
    #region Inspector
    public float HoldTime = 0.5f;
    #endregion
    private float m_Timer;

    private bool m_IsHolding;
    private bool m_CanDrag;
    private bool m_IsOverlap;

    [HideInInspector]
    public bool IsActivate = false;

    void Start()
    {
        m_Timer = HoldTime;
    }

    private Coroutine m_HoldingCoroutine;

    void Update()
    {
        if (!IsActivate)
            return;
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            if(RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition, UICamera.Instance.Camera))
            {
                m_IsOverlap = true;
                m_IsHolding = true;
                if (m_HoldingCoroutine != null)
                    StopCoroutine(m_HoldingCoroutine);
                m_HoldingCoroutine = StartCoroutine(Holding());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (m_HoldingCoroutine != null)
            {
                StopCoroutine(m_HoldingCoroutine);
                m_HoldingCoroutine = null;
            }
            
            if (m_DragObjectTrans != null)
            {
                m_IsHolding = false;

                if (m_CanDrag)
                {
                    m_CanDrag = false;
                    m_Timer = HoldTime;

                    var data = new PointerEventData(EventSystem.current);
                    data.position = Input.mousePosition;
                    OnEndDrag(data);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (m_DragObjectTrans != null)
            {
                if (m_CanDrag)
                {
                    var data = new PointerEventData(EventSystem.current);
                    data.position = Input.mousePosition;
                    OnDrag(data);
                }
                else
                {
                    if (!m_IsOverlap)
                    {
                        m_IsHolding = false;
                    }
                }
            }

            if (m_HoldingCoroutine != null && !RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition, UICamera.Instance.Camera))
            {
                if (m_HoldingCoroutine != null)
                {
                    StopCoroutine(m_HoldingCoroutine);
                    m_HoldingCoroutine = null;
                }
            }
        }

#else
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, touch.position, UICamera.Instance.Camera))
                {
                    m_IsOverlap = true;
                    m_IsHolding = true;
                    if (m_HoldingCoroutine != null)
                      StopCoroutine(m_HoldingCoroutine);
                    m_HoldingCoroutine = StartCoroutine(Holding());
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if (m_HoldingCoroutine != null)
                {
                    StopCoroutine(m_HoldingCoroutine);
                    m_HoldingCoroutine = null;
                }
            
                if (m_DragObjectTrans != null)
                {
                    m_IsHolding = false;

                    if (m_CanDrag)
                    {
                        m_CanDrag = false;
                        m_Timer = HoldTime;

                        var data = new PointerEventData(EventSystem.current);
                        data.position = touch.position;
                        OnEndDrag(data);
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (m_DragObjectTrans != null)
                {
                    if (m_CanDrag)
                    {
                        var data = new PointerEventData(EventSystem.current);
                        data.position = touch.position;
                        OnDrag(data);
                    }
                    else
                    {
                        if (!m_IsOverlap)
                        {
                            m_IsHolding = false;
                        }
                    }
                }

                if (m_HoldingCoroutine != null && !RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, touch.position, UICamera.Instance.Camera))
                {
                    if (m_HoldingCoroutine != null)
                    {
                        StopCoroutine(m_HoldingCoroutine);
                        m_HoldingCoroutine = null;
                    }
                }
            }
        }
#endif
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_IsOverlap = false;
    }

    IEnumerator Holding()
    {
        while (m_Timer > 0)
        {
            if (!m_IsHolding)
            {
                m_Timer = HoldTime;
                yield break;
            }

            m_Timer -= Time.deltaTime;
            yield return null;
        }

        m_CanDrag = true;
        var data = new PointerEventData(EventSystem.current);
#if UNITY_EDITOR || UNITY_STANDALONE
        data.position = Input.mousePosition;
#else
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            data.position = touch.position;
        }
#endif
        OnBeginDrag(data);
    }

    public void Reset()
    {
        m_IsHolding = false;
        m_CanDrag = false;
        m_IsOverlap = false;
    }

    public delegate void BeginDragDelegate(DraggableUI target, PointerEventData eventData);
    public delegate void OnDragDelegate(DraggableUI target, PointerEventData eventData);
    public delegate void EndDragDelegate(DraggableUI target, PointerEventData eventData);

    private BeginDragDelegate m_BeginDragEvent;
    private OnDragDelegate m_OnDragEvent;
    private EndDragDelegate m_EndDragEvent;
    private RectTransform m_DragObjectTrans;
    private RecycleScroll m_RecycleScroll;

    public void Init(BeginDragDelegate beginEvent = null, OnDragDelegate onEvent = null, EndDragDelegate endEvent = null, RecycleScroll loopScroll = null)
    {
        m_BeginDragEvent = beginEvent;
        m_OnDragEvent = onEvent;
        m_EndDragEvent = endEvent;
        m_RecycleScroll = loopScroll;
        if (onEvent == null)
            IsActivate = false;
        else
            IsActivate = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_BeginDragEvent != null)
            m_BeginDragEvent(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_OnDragEvent != null)
            m_OnDragEvent(this, eventData);

        if (m_DragObjectTrans != null)
        {
            var pos = m_DragObjectTrans.position;
            var touchPos = UICamera.Instance.Camera.ScreenToWorldPoint(eventData.position);
            pos.x = touchPos.x;
            pos.y = touchPos.y;
            m_DragObjectTrans.position = pos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        MSLog.Log(gameObject.GetInstanceID());
        if (m_EndDragEvent != null)
            m_EndDragEvent(this, eventData);

        m_DragObjectTrans = null;
    }

    public void SetDragObjectTrans(RectTransform trans)
    {
        m_DragObjectTrans = trans;
    }

    public RectTransform GetDragObjectTrans()
    {
        return m_DragObjectTrans;
    }
}
