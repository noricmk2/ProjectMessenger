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
    private float m_FlashCount;
    private Transform m_ShakeTarget;
    private Coroutine m_ShakeCoroutine;
    private Action m_ShakeEndAction;
    private Material m_FlashMaterial;

    private void Awake()
    {
        Camera = GetComponent<Camera>();
        m_FlashMaterial = new Material(Shader.Find("Custom/CameraFlash"));
    }

    public void Flash(float count)
    {
        m_FlashCount = count;
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

    IEnumerator Shake_C()
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

    float deltaTime;
    float flashTime = 0.2f;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_FlashCount > 0)
        {
            deltaTime += Time.deltaTime;
            if (deltaTime < flashTime * 0.5f)
                m_FlashMaterial.SetFloat("_Threshold", Mathf.Lerp(0, 1, deltaTime));
            else if (deltaTime >= flashTime * 0.5f && deltaTime < flashTime)
                m_FlashMaterial.SetFloat("_Threshold", Mathf.Lerp(1, 0, deltaTime));
            else
            {
                --m_FlashCount;
                deltaTime = 0;
            }

            Graphics.Blit(source, destination, m_FlashMaterial);
        }
        else
            Graphics.Blit(source, destination);
    }
}
