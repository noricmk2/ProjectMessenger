using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSLog
{
    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(bool bCondition, object msg)
    {
        if (bCondition)
        {
            Debug.Log(msg);
        }
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarning(bool bCondition, object msg)
    {
        if (bCondition)
        {
            Debug.LogWarning(msg);
        }
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogError(bool bCondition, object msg)
    {
        if (bCondition)
        {
            Debug.LogError(msg);
        }
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object msg)
    {
        Debug.Log(msg);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarning(object msg)
    {
        Debug.LogWarning(msg);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogError(object msg)
    {
        Debug.LogError(msg);
    }
}
