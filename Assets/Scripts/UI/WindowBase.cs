using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBase : MonoBehaviour
{
    public enum eWINDOW
    {
        Title,
        ChatMain,
    }

    public class WindowData
    {
        public eWINDOW WindowType { get; private set; }
        public string Path { get; private set; }

        protected WindowBase m_Instance = null;
        public WindowBase Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = ResourcesManager.Instantiate<WindowBase>(Path);
                    m_Instance.WindowType = WindowType;
                }
                return m_Instance;
            }
        }

        public WindowData(eWINDOW eType, string path)
        {
            WindowType = eType;
            Path = path;
        }

        public bool CheckWindowExist()
        {
            if (m_Instance == null)
                return false;
            else
                return true;
        }
    }

    static Dictionary<eWINDOW, WindowData> m_WindowInfoDic = new Dictionary<eWINDOW, WindowData>()
    {
        { eWINDOW.ChatMain,  new WindowData(eWINDOW.ChatMain, "Prefab/UI/window_chat_main") },
        { eWINDOW.Title,  new WindowData(eWINDOW.ChatMain, "Prefab/UI/window_title") },
    };

    public eWINDOW WindowType { get; private set; }
    static Stack<WindowBase> m_CurrentWindowStack = new Stack<WindowBase>();
    protected bool Overlap = false;

    protected virtual void Open() { }
    protected virtual void AfterOpen() { }
    protected virtual void Close() { }
    protected virtual void Refresh() { }

    public static WindowBase OpenWindow(eWINDOW eWindow, Transform parent, bool bOverlap)
    {
#if UNITY_EDITOR
        if (!m_WindowInfoDic.ContainsKey(eWindow))
        {
            MSLog.LogError("Window is null - " + eWindow.ToString());
            return null;
        }
#endif

        WindowBase addObj = m_WindowInfoDic[eWindow].Instance;
        addObj.Overlap = bOverlap;
        if (!m_CurrentWindowStack.Contains(addObj))
        {
            if (m_CurrentWindowStack.Count > 0 && !m_CurrentWindowStack.Peek().Overlap)
            {
                m_CurrentWindowStack.Peek().gameObject.SetActive(false);
            }

            addObj.gameObject.SetActive(true);
            addObj.transform.SetParent(parent);
            addObj.transform.localScale = new Vector3(1f, 1f, 1f);
            addObj.transform.localPosition = new Vector3();
            addObj.gameObject.transform.SetAsLastSibling();
            m_CurrentWindowStack.Push(addObj);

            RectTransform rectTrans = addObj.gameObject.GetComponent<RectTransform>();
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.sizeDelta = Vector2.zero;

            addObj.Open();
        }
        return addObj;
    }

    public static WindowBase OpenWindowWithFade(eWINDOW eWindow, Transform parent, bool bOverlap)
    {
#if UNITY_EDITOR
        if (!m_WindowInfoDic.ContainsKey(eWindow))
        {
            MSLog.LogError("Window is null - " + eWindow.ToString());
            return null;
        }
#endif
        WindowBase addObj = m_WindowInfoDic[eWindow].Instance;
        AllwaysTopCanvas.Instance.SetFadeAnimation(0.5f, () =>
        {
            var window = OpenWindow(eWindow, parent, bOverlap);
            window.AfterOpen();
        });

        return addObj;
    }

    public static bool IsTopWindow(WindowBase window)
    {
        return m_CurrentWindowStack.Count > 0 && m_CurrentWindowStack.Peek() == window;
    }

    public bool IsTopWindow()
    {
        return IsTopWindow(this);
    }

    public static void CloseWindow(WindowBase window)
    {
        if (m_CurrentWindowStack.Peek() == window)
        {
            m_CurrentWindowStack.Pop();
        }
        window.gameObject.SetActive(false);
        window.Close();

        if (m_CurrentWindowStack.Count > 0 && !m_CurrentWindowStack.Peek().gameObject.activeSelf)
        {
            WindowBase tempWindow = m_CurrentWindowStack.Peek();
            tempWindow.gameObject.SetActive(true);
            tempWindow.Refresh();
        }
    }

    public static void CloseWindowIncludeChilds(WindowBase window)
    {
        WindowBase tempWindow;
        while (m_CurrentWindowStack.Count > 0)
        {
            tempWindow = m_CurrentWindowStack.Peek();
            if (tempWindow == window)
            {
                CloseWindow(window);
                break;
            }
            else
            {
                tempWindow = m_CurrentWindowStack.Pop();
                tempWindow.gameObject.SetActive(false);
                tempWindow.Close();
            }
        }
    }

    public static void CloseAll()
    {
        CloseWindowIncludeChilds(null);
    }

    public static void Release()
    {
        while (m_CurrentWindowStack.Count > 0)
        {
            WindowBase winBase = m_CurrentWindowStack.Pop();
            if (winBase != null)
            {
                winBase.Close();
            }
        }
        m_CurrentWindowStack.Clear();
    }

    public void CloseWindow()
    {
        CloseWindow(this);
    }

    public static T GetWindow<T>(bool getExistWindow = true) where T : WindowBase
    {
        T result = null;

        var iter = m_WindowInfoDic.GetEnumerator();
        while (iter.MoveNext())
        {
            var value = iter.Current.Value;

            if (getExistWindow)
            {
                if (value.CheckWindowExist())
                {
                    if (value.Instance is T)
                    {
                        result = value.Instance as T;
                        break;
                    }
                }
            }
            else
            {
                if (value.Instance is T)
                {
                    result = value.Instance as T;
                    break;
                }
            }
        }
        return result;
    }
}
