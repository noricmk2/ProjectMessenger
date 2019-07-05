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
        var count = (int)DataManager.eSheetType.Length;
#if UNITY_EDITOR

        for (int i = 0; i < count; ++i)
            yield return DataManager.Instance.LoadFromGoogleSheet((DataManager.eSheetType)i);
#else
        for (int i = 0; i < count; ++i)
            yield return DataManager.Instance.LoadFromGoogleSheet((DataManager.eSheetType)i);
#endif
        UserInfo.Instance.InitUserInfo();
        ObjectFactory.Instance.CreateAllPool();
        MSSceneManager.Instance.EnterScene(eScene.INGAME);
    }

    public override IEnumerator Exit_C()
    {
        yield break;
    }
}
