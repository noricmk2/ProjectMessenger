using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : SceneBase
{
    public IntroScene() : base(eScene.INTRO)
    {

    }

    public override IEnumerator Enter_C()
    {
        //TODO: 시트 로딩 & 데이터 처리

        yield return new WaitForSeconds(2.0f);

        MSSceneManager.Instance.EnterScene(eScene.INGAME);
    }

    public override IEnumerator Exit_C()
    {
        yield break;
    }
}
