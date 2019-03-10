using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{
    private static Dictionary<string, Object> m_OriginObjDic = new Dictionary<string, Object>();

    public static void Release()
    {
        m_OriginObjDic.Clear();
    }

    public static T LoadObject<T>(string originalObjName) where T : Object
    {
        T retObj;
        if (m_OriginObjDic.ContainsKey(originalObjName))
        {
            retObj = m_OriginObjDic[originalObjName] as T;
        }
        else
        {
            retObj = Resources.Load<T>(originalObjName);
            m_OriginObjDic.Add(originalObjName, retObj);
        }
        return retObj;
    }

    public static GameObject CreateGameObject(string objName, Transform parent)
    {
        GameObject retObj = new GameObject(objName);
        if (parent == null)
        {
            //retObj.transform.SetParent(Main.Instance.Transform);
        }
        else
        {
            retObj.transform.SetParent(parent);
        }
        return retObj;
    }

    public static T CreateGameObject<T>(string objName, Transform parent) where T : MonoBehaviour
    {
        GameObject retObj = CreateGameObject(objName, parent);
        return retObj.AddComponent<T>();
    }

    public static GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject createdObj = Instantiate(LoadObject<GameObject>(path));
        if (parent == null)
        {
            //createdObj.transform.SetParent(Main.Instance.Transform);
        }
        else
        {
            createdObj.transform.SetParent(parent);
        }
        return createdObj;
    }

    public static T Instantiate<T>(string path, Transform parent = null)
    {
        GameObject createdObj = Instantiate(path, parent);
        return createdObj.GetComponent<T>();
    }
}
