using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(RecycleScroll))]
public class RecycleScrollEditor : Editor
{
    private RecycleScroll m_Scroll;

    private void OnEnable()
    {
        m_Scroll = target as RecycleScroll;
    }

    public override void OnInspectorGUI()
    {
        m_Scroll.content = EditorGUILayout.ObjectField("Content", m_Scroll.content, typeof(RectTransform), true) as RectTransform;
        m_Scroll.viewport = EditorGUILayout.ObjectField("Viewport", m_Scroll.viewport, typeof(RectTransform), true) as RectTransform;
        m_Scroll.horizontal = EditorGUILayout.Toggle("Horizontal", m_Scroll.horizontal);
        m_Scroll.vertical = EditorGUILayout.Toggle("Vertical", m_Scroll.vertical);
        m_Scroll.movementType = (ScrollRect.MovementType)EditorGUILayout.EnumPopup("MovementType", m_Scroll.movementType);
        m_Scroll.LayoutGroup = EditorGUILayout.ObjectField("LayoutGroup", m_Scroll.LayoutGroup, typeof(LayoutGroup), true) as LayoutGroup;
    }
}
