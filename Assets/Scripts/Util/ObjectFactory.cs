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

    public Transform ChatPoolParent { get; private set; }
    public Transform IngamePoolParent { get; private set; }
    public Material SpriteOutlineMaterial { get; private set; }
    public Material GrayScaleMaterial { get; private set; }

    public void CreateAllPool()
    {
        UIAtlas = ResourcesManager.LoadObject<SpriteAtlas>("SpriteAtlas/atlas_ui");
        CharacterAtlas_Nika = ResourcesManager.LoadObject<SpriteAtlas>("SpriteAtlas/atlas_character_nika");
        CharacterAtlas_Less = ResourcesManager.LoadObject<SpriteAtlas>("SpriteAtlas/atlas_character_less");
        SpriteOutlineMaterial = new Material(Shader.Find("Sprites/Outline"));
        SpriteOutlineMaterial.SetColor("_OutlineColor", Color.red);
        SpriteOutlineMaterial.SetFloat("_IsOutlineEnabled", 1);
        GrayScaleMaterial = new Material(Shader.Find("Custom/GrayScale"));
    }

    public void CreateChatObjectPool(Transform parent)
    {
        ChatPoolParent = parent;
        Release();
        CreatePool<CharacterObject>(3, "Prefab/UI/CharacterObject", ChatPoolParent);
        CreatePool<ChoiceObject>(3, "Prefab/UI/ChoiceObject", ChatPoolParent);
        CreatePool<ItemObject>(1, "Prefab/UI/ItemObject", ChatPoolParent);
        CreatePool<BackLogText>(10, "Prefab/UI/BackLogText", ChatPoolParent);
        CreatePool<BagSlot>(10, "Prefab/UI/BagSlot", ChatPoolParent);
        CreatePool<LetterBundle>(1, "Prefab/UI/LetterBundle", ChatPoolParent);
        CreatePool<LetterSlot>(10, "Prefab/UI/LetterSlot", ChatPoolParent);
    }

    public void CreateIngameObjectPool(Transform parent)
    {
        IngamePoolParent = parent;
        Release();
        CreatePool<LetterListObject>(1, "Prefab/UI/LetterObject", IngamePoolParent);
        CreatePool<MapObject>(1, "Prefab/Map/MapObject", IngamePoolParent);
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
        }

        if (CharacterAtlas_Nika != null)
            return atlas.GetSprite(spriteName);
        return null;
    }

    public Sprite GetBackGroundSprite(string spriteName)
    {
        return ResourcesManager.LoadObject<Sprite>("BG/" + spriteName);
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

    public void DeactivateObject<T>(T obj, bool findType = false) where T : MonoBehaviour, IPoolObjectBase
    {
        var poolName = typeof(T).Name;
        if (m_TotalPoolDic.ContainsKey(poolName))
            m_TotalPoolDic[poolName].Push(obj);

        if (findType)
        {
            var iter = m_TotalPoolDic.GetEnumerator();
            while (iter.MoveNext())
            {
                var pObj = iter.Current.Value.Peek();
                if (obj.GetType() == pObj.GetType())
                    iter.Current.Value.Push(obj);
            }
        }
    }

    public void CreatePool<T>(int count, string path, Transform parent = null) where T : MonoBehaviour, IPoolObjectBase
    {
        var pool = new ObjectPool<IPoolObjectBase>(count, () =>
        {
            T poolObj = ResourcesManager.Instantiate<T>(path, parent);
            return poolObj;
        }
        , (IPoolObjectBase pushObj) => { pushObj.PushAction(); }, (IPoolObjectBase popObj) => { popObj.PopAction(); });

        m_TotalPoolDic[typeof(T).Name] = pool;
    }

    public void Release()
    {
        var iter = m_TotalPoolDic.GetEnumerator();
        while (iter.MoveNext())
            iter.Current.Value.Release();
        m_TotalPoolDic.Clear();
    }
}
