using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UICamera : Singleton<UICamera>
{
    #region Inspector
    public TransitionEffect TransEffect;
    #endregion
    public Camera Camera { get; private set; }
    private Vector3 m_OriginPos;
    private float m_Intensity;
    private float m_Duration;
    private Transform m_ShakeTarget;
    private Coroutine m_ShakeCoroutine;
    private Action m_ShakeEndAction;

    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }

    public void CameraShake(Transform target, float intentsity, float duration, Action endAction = null)
    {
        m_OriginPos = target.localPosition;
        m_Intensity = intentsity;
        m_Duration = duration;
        m_ShakeTarget = target;
        m_ShakeEndAction = endAction;

        if (m_ShakeCoroutine != null)
            StopCoroutine(m_ShakeCoroutine);
        m_ShakeCoroutine = StartCoroutine(Shake_C());
    }

    public IEnumerator Shake_C()
    {
        float timer = 0;
        while (timer <= m_Duration)
        {
            m_ShakeTarget.localPosition = (Vector3)UnityEngine.Random.insideUnitCircle * m_Intensity + m_OriginPos;

            timer += Time.deltaTime;
            yield return null;
        }
        m_ShakeTarget.localPosition = m_OriginPos;
        if (m_ShakeEndAction != null)
            m_ShakeEndAction();
    }
}
