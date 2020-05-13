using UnityEngine;
using UniRx;
using System.Collections;

/// <summary>
/// ゲーム管理クラス
/// </summary>
public class GameMgr : MonoSingleton<GameMgr> {
    [SerializeField] StageMetaData m_defaultStartStage;

    public void InitValues() {
        GameModel.Init();
        GameModel.Stage.Value = StageMgr.Instance.TitleStage;
        GameModel.SpawnStageChunk.Value = StageMgr.Instance.TitleStage.Chunks[0];
    }

    public void InitScene() {
        StartCoroutine( InitSceneSequence() );
    }

    IEnumerator InitSceneSequence() {
        // マルチタップ無効設定.
        Input.multiTouchEnabled = false;
        // FPS設定
        Application.targetFrameRate = 30;

        DialogMgr.Instance.Init( Application.systemLanguage );

        while ( !RinkaPurchaseManager.Instance.IsInitialized() )
            yield return null;

       //alestaads1 //while ( !RinkaAdvertisementManager.Instance.isInitialized )
        //    yield return null;

        SceneMgr.Instance.FadeIn();
        // TODO: Facebook統合
        //FacebookMgr.Instance.Init();
#if UNITY_ANDROID
        GooglePlayMgr.Instance.Init();
        GooglePlayMgr.Instance.SignIn( null, null );
#elif UNITY_IOS
        GameCenterMgr.Instance.Init();
#endif
        var net = NetworkManager.Instance;
        GameModel.GameState.Value = GameModel.IsTutorial.Value ? Define.GameState.Tutorial : Define.GameState.Title;
        SceneMgr.Instance.LoadScene( Define.Scene.TITLE );
        StageMgr.Instance.InitSpawnStage();
    }
}
