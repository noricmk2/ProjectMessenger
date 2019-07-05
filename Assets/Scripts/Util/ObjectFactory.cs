using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using MSUtil;

public interface IPoolObjectBase
{
    void PushAction();
    void PopAction();
}

public class ObjectFactory : Singleton<ObjectFactory>
{
    private Dictionary<string, ObjectPool<IPoolObjectBase>> m_TotalPoolDic = new Dictionary<string, ObjectPool<IPoolObjectBase>>();
    private SpriteAtlas UIAtlas;
    private SpriteAtlas CharacterAtlas_Nika;
    private SpriteAtlas CharacterAtlas_Less;

    public void CreateAllPool()
    {
        UIAtlas = ResourcesManager.LoadObject<SpriteAtlas>("SpriteAtlas/atlas_ui");
        CharacterAtlas_Nika = ResourcesManager.LoadObject<SpriteAtlas>("SpriteAtlas/atlas_character_nika");
        CharacterAtlas_Less = ResourcesManager.LoadObject<SpriteAtlas>("SpriteAtlas/atlas_character_less");
    }

    public void CreateChatObjectPool()
    {
        Release();
        CreatePool<CharacterObject>(3, "Prefab/UI/CharacterObject");
        CreatePool<ChoiceObject>(3, "Prefab/UI/ChoiceObject");
        CreatePool<ItemObject>(1, "Prefab/UI/ItemObject");
    }

    public Sprite GetUISprite(string spriteName)
    {
        if (UIAtlas != null)
            return UIAtlas.GetSprite(spriteName);
        return null;
    }

    public Sprite GetCharacterSprite(eCharacter character, string spriteName)
    {
        SpriteAtlas atlas = null;
        switch (character)
        {
            case eCharacter.NIKA:
                atlas = CharacterAtlas_Nika;
                break;
            case eCharacter.LUCIA:
                break;
            case eCharacter.RINTA:
                break;
            case eCharacter.LESS:
                atlas = CharacterAtlas_Less;
                break;
            case eCharacter.JACQUES:
                break;
            case eCharacter.ARUE:
                break;
            case eCharacter.LENGTH:
                break;
        }

        if (CharacterAtlas_Nika != null)
            return atlas.GetSprite(spriteName);
        return null;
    }

    public T ActivateObject<T>() where T : class, IPoolObjectBase
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
