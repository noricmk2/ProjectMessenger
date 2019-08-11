using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LetterGirlEditor : MonoBehaviour
{
    [MenuItem("LetterGirl/ClearPlayerPrefs")]
    static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
