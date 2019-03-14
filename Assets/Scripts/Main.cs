using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : Singleton<Main>
{
    public Transform CachedTransform { get; private set; }
    private void Awake()
    {
        CachedTransform = transform;
        Application.targetFrameRate = 60;
        Application.runInBackground = true;

        StartCoroutine(Main_C());
    }

    IEnumerator Main_C()
    {
        yield return null;
        MSSceneManager.Instance.EnterScene(SceneBase.eScene.INTRO);
    }
}
