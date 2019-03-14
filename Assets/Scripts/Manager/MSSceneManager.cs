using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSSceneManager : Singleton<MSSceneManager>
{
    private Dictionary<SceneBase.eScene, SceneBase> m_SceneDic = new Dictionary<SceneBase.eScene, SceneBase>();
    private SceneBase m_CurrentScene;

    private void Awake()
    {
        var count = (int)SceneBase.eScene.LENGTH;
        for (int i = 0; i < count; ++i)
            m_SceneDic[(SceneBase.eScene)i] = SceneBase.CreateScene((SceneBase.eScene)i);
    }

    public void EnterScene(SceneBase.eScene scene)
    {
        if (m_CurrentScene == null || scene != m_CurrentScene.SceneType)
            StartCoroutine(LoadScene_C(scene));
    }

    IEnumerator LoadScene_C(SceneBase.eScene scene)
    {
        var isFirstScene = false;
        WindowBase.Release();

        if (m_CurrentScene != null)
            yield return StartCoroutine(m_CurrentScene.Exit_C());
        else
            isFirstScene = true;

        ResourcesManager.Release();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        var nextScene = m_SceneDic[scene];
        if (!isFirstScene)
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene.SceneName);

        yield return null;
        m_CurrentScene = nextScene;
        yield return StartCoroutine(m_CurrentScene.Enter_C());
    }
}
