using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

public class TransitionEffect : MonoBehaviour
{
    private Material m_ImageTransition;
    private float m_DeltaTime;
    private float m_TargetTime;
    private bool m_IsEffectActive;
    private eTransitionType m_CurrentType;
    private System.Action m_EndAction;
    private bool m_IsRepeat;
    private Texture2D m_TargetMask;
    private float m_MaskValue;
    private Color m_MaskColor = Color.black;

    private void Awake()
    {
        m_IsEffectActive = false;
        if (m_ImageTransition == null)
            m_ImageTransition = new Material(Shader.Find("Custom/ScreenTransitionImageEffect"));
    }

    public void SetTransitionEffect(bool repeat, eTransitionType type = eTransitionType.NONE, float effectTime = 0, System.Action endAction = null)
    {
        m_TargetMask = ObjectFactory.Instance.GetTransitonMask(type);
        m_ImageTransition.SetTexture("_MaskTex", m_TargetMask);
        m_CurrentType = type;
        m_IsEffectActive = true;
        m_TargetTime = effectTime;
        m_DeltaTime = 0;
        m_EndAction = endAction;
        m_IsRepeat = repeat;
        m_MaskValue = 0;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_IsEffectActive)
        {
            if (m_DeltaTime >= m_TargetTime && !m_IsRepeat)
            {
                m_IsEffectActive = false;
                m_DeltaTime = 0;
                m_MaskValue = 0;
                m_ImageTransition.DisableKeyword("INVERT_MASK");
                if (m_EndAction != null)
                {
                    m_EndAction();
                    m_EndAction = null;
                }
                else
                    AlwaysTopCanvas.Instance.IsOnFade = false;
            }
            else if (m_DeltaTime >= m_TargetTime && m_IsRepeat)
            {
                m_IsEffectActive = true;
                m_DeltaTime = 0;
                m_MaskValue = 0;
                m_IsRepeat = false;
                m_ImageTransition.EnableKeyword("INVERT_MASK");
                if (m_EndAction != null)
                {
                    m_EndAction();
                    m_EndAction = null;
                }
                return;
            }
            else
            {
                m_MaskValue = m_DeltaTime / m_TargetTime;
                m_DeltaTime += Time.deltaTime;
                m_ImageTransition.SetColor("_MaskColor", m_MaskColor);
                m_ImageTransition.SetFloat("_MaskValue", m_MaskValue);
                Graphics.Blit(source, destination, m_ImageTransition);
            }
            #region temp
            //switch (m_CurrentType)
            //{
            //    case eTransitionType.CIRCLE:
            //        {
            //            if (m_DeltaTime > m_TargetTime && !m_IsReset)
            //            {
            //                m_IsEffectActive = false;
            //                m_DeltaTime = 0;
            //                curMaterial.DisableKeyword("REVERSE_ON");
            //                if (m_EndAction != null)
            //                {
            //                    m_EndAction();
            //                    m_EndAction = null;
            //                }
            //                else
            //                    AlwaysTopCanvas.Instance.IsOnFade = false;
            //                return;
            //            }
            //            else if (m_DeltaTime > m_TargetTime && m_IsReset)
            //            {
            //                m_IsEffectActive = true;
            //                m_DeltaTime = 0;
            //                curMaterial.EnableKeyword("REVERSE_ON");
            //                m_IsReset = false;
            //                if (m_EndAction != null)
            //                {
            //                    m_EndAction();
            //                    m_EndAction = null;
            //                }
            //                return;
            //            }
            //            else
            //            {
            //                var angle = m_DeltaTime / m_TargetTime;
            //                curMaterial.SetFloat("_CurAngle", Mathf.Lerp(0, 360, angle));
            //                m_DeltaTime += Time.deltaTime;
            //            }
            //        }
            //        break;
            //}
            #endregion 
        }
        else
            Graphics.Blit(source, destination);
    }
}
