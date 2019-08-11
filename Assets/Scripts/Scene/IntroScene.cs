using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSUtil;

public class IntroScene : SceneBase
{
    public IntroScene() : base(eScene.INTRO)
    {

    }

    public override IEnumerator Enter_C()
    {
        //TODO: 시트 로딩 & 데이터 처리

#if UNITY_EDITOR
        var count = (int)DataManager.eSheetType.Length;
        int sheetLoaded = PlayerPrefs.GetInt(ConstValue.SHEET_SAVE, 0);
        if (sheetLoaded == 1)
        {
            for (int i = 0; i < count; ++i)
                yield return DataManager.Instance.LoadOffline((DataManager.eSheetType)i);
            MSLog.Log("Load by savedata");
        }
        else
        {
            for (int i = 0; i < count; ++i)
                yield return DataManager.Instance.LoadFromGoogleSheet((DataManager.eSheetType)i);
            MSLog.Log("Load by online sheet");
        }
#else
        for (int i = 0; i < count; ++i)
            yield return DataManager.Instance.LoadOffline((DataManager.eSheetType)i);
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
