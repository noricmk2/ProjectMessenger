using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class LetterGirlEditor : MonoBehaviour
{
    [MenuItem("LetterGirl/ClearPlayerPrefs")]
    static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("GameObject/UI/MSTextMeshPro", false)]
    static void CreateMSTextMeshPro(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("MSTextMeshPro");
        go.AddComponent<TextMeshProUGUI>();
        go.AddComponent<LanguageSubscriber>();
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
