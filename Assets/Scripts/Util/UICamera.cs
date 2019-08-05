using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : Singleton<UICamera>
{
    #region Inspector
    public TransitionEffect TransEffect;
    #endregion

    public Camera Camera { get; private set; }

    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }
}
