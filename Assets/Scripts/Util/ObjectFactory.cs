using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public interface IPoolObjectBase
{
    void PushAction();
    void PopAction();
}

public class ObjectFactory : Singleton<ObjectFactory>
{
    private Dictionary<string, ObjectPool<IPoolObjectBase>> m_TotalPoolDic = new Dictionary<string, ObjectPool<IPoolObjectBase>>();
    private SpriteAtlas UIAtlas;
    private SpriteAtlas CharacterAtlas;

    public void CreateAllPool()
    {
        UIAtlas = ResourcesManager.LoadObject<SpriteAtlas>("SpriteAtlas/atlas_ui");
        CharacterAtlas = ResourcesManager.LoadObject<SpriteAtlas>("SpriteAtlas/atlas_character");
    }

    public void CreateChatObjectPool()
    {
        Release();
        CreatePool<SpeechBubble>(3, "Prefab/UI/SpeechBubble");
        CreatePool<CharacterObject>(3, "Prefab/UI/CharacterObject");
    }

    public Sprite GetUISprite(string spriteName)
    {
        if (UIAtlas != null)
            return UIAtlas.GetSprite(spriteName);
        return null;
    }

    public Sprite GetCharacterSprite(string spriteName)
    {
        if (CharacterAtlas != null)
            return CharacterAtlas.GetSprite(spriteName);
        return null;
    }

    public T ActivateObject<T>(Transform parent = null) where T : class, IPoolObjectBase
    {
        var poolName = typeof(T).Name;
        if (m_TotalPoolDic.ContainsKey(poolName))
        {
            var objectPool = m_TotalPoolDic[poolName];
            return objectPool.Pop() as T;
        }
        return default(T);
    }

    public void DeactivateObject<T>(T obj) where T : class, IPoolObjectBase
    {
        var poolName = typeof(T).Name;
        if (m_TotalPoolDic.ContainsKey(poolName))
        {
            m_TotalPoolDic[poolName].Push(obj);
        }
    }

    public void CreatePool<T>(int count, string path) where T : IPoolObjectBase
    {
        var pool = new ObjectPool<IPoolObjectBase>(count, () =>
        {
            T poolObj = ResourcesManager.Instantiate<T>(path);
            return poolObj;
        }
        , (IPoolObjectBase pushObj) => { pushObj.PushAction(); }, (IPoolObjectBase popObj) => { popObj.PopAction(); });

        m_TotalPoolDic[typeof(T).Name] = pool;
    }

    public void Release()
    {
        m_TotalPoolDic.Clear();
    }
}
