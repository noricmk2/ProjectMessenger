using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MSUtil;

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
        if (Camera == null)
            Camera = Camera.main;
        if (Camera == null)
        {
            MSLog.LogError("No camera available");
            return;
        }
        m_FlashMaterial = new Material(Shader.Find("Custom/CameraFlash"));
        SetCamera();
    }

    #region CameraRatio
    private Camera backgroundCam;

    public int ScreenHeight
    {
        get
        {
            return (int)(Screen.height * Camera.rect.height);
        }
    }
    public int ScreenWidth
    {
        get
        {
            return (int)(Screen.width * Camera.rect.width);
        }
    }
    public int XOffset
    {
        get
        {
            return (int)(Screen.width * Camera.rect.x);
        }
    }
    public int YOffset
    {
        get
        {
            return (int)(Screen.height * Camera.rect.y);
        }
    }
    public Rect ScreenRect
    {
        get
        {
            return new Rect(Camera.rect.x * Screen.width, Camera.rect.y * Screen.height, Camera.rect.width * Screen.width, Camera.rect.height * Screen.height);
        }
    }
    public Vector3 MousePosition
    {
        get
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.y -= (int)(Camera.rect.y * Screen.height);
            mousePos.x -= (int)(Camera.rect.x * Screen.width);
            return mousePos;
        }
    }
    public Vector2 GUIMousePosition
    {
        get
        {
            Vector2 mousePos = Event.current.mousePosition;
            mousePos.y = Mathf.Clamp(mousePos.y, Camera.rect.y * Screen.height, Camera.rect.y * Screen.height + Camera.rect.height * Screen.height);
            mousePos.x = Mathf.Clamp(mousePos.x, Camera.rect.x * Screen.width, Camera.rect.x * Screen.width + Camera.rect.width * Screen.width);
            return mousePos;
        }
    }
    public bool HasPillarBox { get; private set; }

    public void SetCamera()
    {
        var targetAspectRatio = ConstValue.DEFULT_SCREEN_SIZE.x / ConstValue.DEFULT_SCREEN_SIZE.y;
        float currentAspectRatio = (float)Screen.width / Screen.height;

        if (currentAspectRatio.Equals(targetAspectRatio))
        {
            Camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            if (backgroundCam != null)
                Destroy(backgroundCam.gameObject);
            return;
        }
        // Pillarbox
        if (currentAspectRatio > targetAspectRatio)
        {
            HasPillarBox = true;
            float inset = 1.0f - targetAspectRatio / currentAspectRatio;
            Camera.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);
        }
        // Letterbox
        else
        {
            HasPillarBox = false;
            float inset = 1.0f - currentAspectRatio / targetAspectRatio;
            Camera.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);
        }

        if (!backgroundCam)
        {
            backgroundCam = new GameObject("BackgroundCam", typeof(Camera)).GetComponent<Camera>();
            backgroundCam.depth = int.MinValue;
            backgroundCam.clearFlags = CameraClearFlags.SolidColor;
            backgroundCam.backgroundColor = Color.black;
            backgroundCam.cullingMask = 0;
        }
    }
    #endregion

    #region CameraEffect
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
    #endregion
}
