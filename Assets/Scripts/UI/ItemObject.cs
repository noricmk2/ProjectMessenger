using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MSUtil;

public class ItemObject : MonoBehaviour, IPoolObjectBase
{
    #region Inspector
    public Image Icon;
    #endregion

    public void Init(Transform parent, string resourceName)
    {
        transform.Init(parent);
        Icon.sprite = ObjectFactory.Instance.GetUISprite(resourceName);
        Icon.SetNativeSize();
    }

    public void PushAction()
    {
        gameObject.SetActive(false);
        transform.SetParent(null);
    }

    public void PopAction()
    {
        gameObject.SetActive(true);
    }
}
