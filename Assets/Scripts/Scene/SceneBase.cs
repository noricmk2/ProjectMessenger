using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase
{
    public enum eScene
    {
        INTRO,
        INGAME,
        LENGTH
    }

    public eScene SceneType { get; private set; }
    public string SceneName { get; private set; }

    public SceneBase(eScene scene)
    {
        SceneType = scene;
        SceneName = scene.ToString().ToLower();
    }

    public static SceneBase CreateScene(eScene scene)
    {
        SceneBase ret = null;
        switch (scene)
        {
            case eScene.INTRO:
                ret = new IntroScene();
                break;
            case eScene.INGAME:
                ret = new IngameScene();
                break;
        }
        return ret;
    }

    public abstract IEnumerator Enter_C();
    public abstract IEnumerator Exit_C();
}
