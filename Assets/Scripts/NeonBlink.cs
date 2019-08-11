using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonBlink : MonoBehaviour
{
    #region Inspector
    public Material Outline1;
    public Material RedLight;
    #endregion
    private float m_DeltaTime = 0;
    private int m_RandomCount;
    private float m_RandomTime;
    private float m_BlinkTerm = 0.2f;
    private float m_OrgOutlineSize = 2;
    private float m_OffOutlineSize = 4;
    private Color m_OffColor = new Color(0.3f, 0.3f, 0.3f, 1);

    private float m_FixedTime = 2.0f;
    private float m_DeltaTime2;

    private void Start()
    {
        Outline1.SetFloat("_OutlineSize", m_OrgOutlineSize);
        m_RandomCount = Random.Range(0, 3);
        m_RandomTime = Random.Range(2.0f, 5.0f);
        m_BlinkTerm = Random.Range(0.1f, 0.15f);
    }

    void Update()
    {
        m_DeltaTime += Time.deltaTime;

        if (m_DeltaTime > m_RandomTime)
        {
            Outline1.EnableKeyword("SHUT_OFF");
            Outline1.SetFloat("_OutlineSize", m_OffOutlineSize);
        }

        if (m_DeltaTime >= m_RandomTime + m_BlinkTerm)
        {
            m_DeltaTime -= m_BlinkTerm * m_RandomCount;
            --m_RandomCount;
            Outline1.DisableKeyword("SHUT_OFF");
            Outline1.SetFloat("_OutlineSize", m_OrgOutlineSize);
            if (m_RandomCount < 0)
            {
                m_DeltaTime = 0;
                m_RandomTime = Random.Range(2.0f, 5.0f);
                m_RandomCount = Random.Range(0, 3);
                m_BlinkTerm = Random.Range(0.1f, 0.2f);
            }
        }

        m_DeltaTime2 += Time.deltaTime;
        if (m_DeltaTime2 > m_FixedTime)
        {
            RedLight.EnableKeyword("SHUT_OFF");
            RedLight.SetFloat("_OutlineSize", m_OffOutlineSize);
        }

        if (m_DeltaTime2 > m_FixedTime * 2)
        {
            m_DeltaTime2 = 0;
            RedLight.DisableKeyword("SHUT_OFF");
            RedLight.SetFloat("_OutlineSize", m_OrgOutlineSize);
        }
    }
}
