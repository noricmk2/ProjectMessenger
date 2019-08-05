using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

public class TransitionEffect : MonoBehaviour
{
    private Material m_CircleTransition;
    private float m_DeltaTime;
    private float m_TargetTime;
    private bool m_IsEffectActive;
    private eTransitionType m_CurrentType;
    private Dictionary<eTransitionType, Material> m_MaterialDic = new Dictionary<eTransitionType, Material>();
    private System.Action m_EndAction;
    private bool m_IsReset;

    private void Awake()
    {
        m_IsEffectActive = false;
        if (m_CircleTransition == null)
        {
            m_CircleTransition = new Material(Shader.Find("Custom/CircleTransition"));
        }
        m_MaterialDic[eTransitionType.CIRCLE] = m_CircleTransition;
    }

    public void SetTransitionEffect(bool active, bool reset, eTransitionType type = eTransitionType.NONE, float effectTime = 0, System.Action endAction = null)
    {
        m_CurrentType = type;
        m_IsEffectActive = active;
        m_TargetTime = effectTime;
        m_DeltaTime = 0;
        m_EndAction = endAction;
        m_IsReset = reset;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_IsEffectActive)
        {
            var curMaterial = m_MaterialDic[m_CurrentType];
            switch (m_CurrentType)
            {
                case eTransitionType.CIRCLE:
                    {
                        if (m_DeltaTime > m_TargetTime && !m_IsReset)
                        {
                            m_IsEffectActive = false;
                            m_DeltaTime = 0;
                            curMaterial.DisableKeyword("REVERSE_ON");
                            if(m_EndAction != null)
                            {
                                m_EndAction();
                                m_EndAction = null;
                            }
                            return;
                        }
                        else if (m_DeltaTime > m_TargetTime && m_IsReset)
                        {
                            m_IsEffectActive = true;
                            m_DeltaTime = 0;
                            curMaterial.EnableKeyword("REVERSE_ON");
                            m_IsReset = false;
                            if (m_EndAction != null)
                            {
                                m_EndAction();
                                m_EndAction = null;
                            }
                            return;
                        }
                        else
                        {
                            var angle = m_DeltaTime / m_TargetTime;
                            curMaterial.SetFloat("_CurAngle", Mathf.Lerp(0, 360, angle));
                            m_DeltaTime += Time.deltaTime;
                        }
                    }
                    break;
            }
            Graphics.Blit(source, destination, curMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
