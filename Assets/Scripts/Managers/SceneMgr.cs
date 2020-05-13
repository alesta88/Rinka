using System;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UniRx;

/// <summary>
/// シーン切替の管理クラス
/// </summary>
public class SceneMgr : MonoSingleton<SceneMgr> {
    [SerializeField] [Range( 0f, 1f )] float m_defaultFadeInDuration;
    [SerializeField] [Range( 0f, 1f )] float m_defaultFadeOutDuration;
    [SerializeField] Image m_fadeImage;
    public Scene Persistent { private set; get; }
    [SerializeField] StageFlowData m_stageFlow;
    StageMgr StageMgr;

    void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        InitSceneChangeSettings();
        FindAndSetPermenantScene();
    }

    void FindAndSetPermenantScene() {
        var scene = SceneManager.GetSceneByName( Define.Scene.PERMANENT );
        Persistent = scene;
    }

    // ゲームステート変更時のシーン切替などの処理を初期化
    void InitSceneChangeSettings() {
        GameModel.GameState
            .Buffer( 2, 1 )
            .Subscribe( states => OnGameStateChanged( states[0], states[1] ) );
    }

    // ゲームステート変更時のコールバック
    void OnGameStateChanged( Define.GameState prevState, Define.GameState nextState ) {
        // タイトル画面から遷移
        if (prevState == Define.GameState.Title && nextState == Define.GameState.StageSelection)
        {
            //alestaads RinkaAdvertisementManager.Instance.Refresh();
            //alestaads RinkaPurchaseManager.Instance.Refresh();
            LoadScene(Define.Scene.STAGESELECTION);
        }
        // ステージ選択画面から遷移
        else if (prevState == Define.GameState.StageSelection)
        {
            if (nextState == Define.GameState.Title)
            {
                UnloadScene(Define.Scene.STAGESELECTION);
            }
            else if (nextState == Define.GameState.Playing)
            {
                StageSelectionToPlayGame();
            }
            else if (nextState == Define.GameState.PlayStageSelectionAd)
            { 
                //FadeIn();
                //Analytics.CustomEvent(Define.AnalyticsEvent.FULL_AD_START);
                ////alestaads RinkaAdvertisementManager.Instance.PlayMovie(
                //    isSkip: false,
                //    onFinish: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //        {
                //            GameModel.GameState.Value = Define.GameState.Playing;
                //            FadeOut();
                //        });
                //    }, onSkip: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //        {
                //            GameModel.GameState.Value = Define.GameState.StageSelection;
                //            FadeOut();
                //        });
                //    }, onError: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //        {
                //            GameModel.GameState.Value = Define.GameState.StageSelection;
                //            FadeOut();
                //        });
                //    });
                //StageSelectionToPlayGame(); 
            }
            else if (nextState == Define.GameState.Skin)
            {
                StageSelectionToSkin();
            }
            // ステージ選択広告から遷移
        }
        else if (prevState == Define.GameState.PlayStageSelectionAd)
        {
            if (nextState == Define.GameState.Playing)
            {
                Analytics.CustomEvent(Define.AnalyticsEvent.FULL_AD_FINISH);
                StageSelectionToPlayGame();
            }
            // プレイ画面から遷移
        }
        else if (prevState == Define.GameState.Playing)
        {
            if (nextState == Define.GameState.GameOver)
            {
                RevealGameOverScreen();
                LoadScene(Define.Scene.GAMEOVER);
            }
            else if (nextState == Define.GameState.Continue)
            {
                LoadScene(Define.Scene.GAMEOVER);
                LoadScene(Define.Scene.CONTINUE);
            }
            else if (nextState == Define.GameState.GoToBonus)///clear stage
            {
                // LoadScene(Define.Scene.GAMEOVER);
                // LoadScene(Define.Scene.GOTOBONUS);
                Debug.Log("Going to bonus");
                // ContinueToPlayGame();
            }
            else if (nextState == Define.GameState.Clear)///clear stage
            {
                // LoadScene(Define.Scene.GAMEOVER);
                LoadScene(Define.Scene.CONTINUE1);
            }
            // Continue画面から遷移
        }
        else if (prevState == Define.GameState.Continue)
        {
            if (nextState == Define.GameState.GameOver)
            {
                RevealGameOverScreen();
                UnloadScene(Define.Scene.CONTINUE);
            }
            else if (nextState == Define.GameState.Playing)
            {
                ContinueToPlayGame();
            }
            else if (nextState == Define.GameState.PlayContinueAd)
            {
                FadeIn(0f);
                //alestaads Analytics.CustomEvent(Define.AnalyticsEvent.FULL_AD_START);
                ////alestaads RinkaAdvertisementManager.Instance.PlayMovie(
                //    isSkip: false,
                //    onFinish: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //       {
                //           GameModel.GameState.Value = Define.GameState.Playing;
                //           FadeOut();
                //       });
                //    }, onSkip: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //       {
                //           GameModel.GameState.Value = Define.GameState.Continue;
                //           FadeOut();
                //       });
                //    }, onError: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //       {
                //           GameModel.GameState.Value = Define.GameState.Continue;
                //           FadeOut();
                //       });
                //    });
                GameModel.GameState.Value = Define.GameState.Playing;
            }
        }
        else if (prevState == Define.GameState.Clear)///////// 
        {
            if (nextState == Define.GameState.GameOver)
            {
                RevealGameOverScreen();
                UnloadScene(Define.Scene.CONTINUE);
                UnloadScene(Define.Scene.GOTOBONUS);
                UnloadScene(Define.Scene.GAMEOVER);
                UnloadScene(Define.Scene.CONTINUE1);
            }
            else if (nextState == Define.GameState.Playing)
            {
                Debug.Log("nextState == Define.GameState.Playing");
                ContinueToPlayGame1();
            }
            else if (nextState == Define.GameState.PlayContinueAd)
            {
                FadeIn(0f);
                ////alestaads Analytics.CustomEvent(Define.AnalyticsEvent.FULL_AD_START);
                //RinkaAdvertisementManager.Instance.PlayMovie(
                //    isSkip: false,
                //    onFinish: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //        {
                //            GameModel.GameState.Value = Define.GameState.Playing;
                //            FadeOut();
                //        });
                //    }, onSkip: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //        {
                //            GameModel.GameState.Value = Define.GameState.Continue;
                //            FadeOut();
                //        });
                //    }, onError: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //        {
                //            GameModel.GameState.Value = Define.GameState.Continue;
                //            FadeOut();
                //        });
                //    });
                GameModel.GameState.Value = Define.GameState.Playing;
            }
            // Continue画面の広告から遷移
        }
        else if (prevState == Define.GameState.GoToBonus)
        {
            if (nextState == Define.GameState.GameOver)
            {
                RevealGameOverScreen();
                UnloadScene(Define.Scene.CONTINUE);
                UnloadScene(Define.Scene.GOTOBONUS);
                UnloadScene(Define.Scene.GAMEOVER);
                UnloadScene(Define.Scene.CONTINUE1);
            }
            else if (nextState == Define.GameState.Playing)
            {
                ContinueToPlayGame();
            }
            else if (nextState == Define.GameState.PlayContinueAd)
            {
                FadeIn(0f);
                ////alestaads Analytics.CustomEvent(Define.AnalyticsEvent.FULL_AD_START);
                //RinkaAdvertisementManager.Instance.PlayMovie(
                //    isSkip: false,
                //    onFinish: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //        {
                //            GameModel.GameState.Value = Define.GameState.Playing;
                //            FadeOut();
                //        });
                //    }, onSkip: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //        {
                //            GameModel.GameState.Value = Define.GameState.Continue;
                //            FadeOut();
                //        });
                //    }, onError: () =>
                //    {
                //        Observable.NextFrame().Subscribe(_ =>
                //        {
                //            GameModel.GameState.Value = Define.GameState.Continue;
                //            FadeOut();
                //        });
                //    });
                GameModel.GameState.Value = Define.GameState.Playing;
            }




            // Continue画面の広告から遷移
        }
        else if (prevState == Define.GameState.PlayContinueAd)
        {
            if (nextState == Define.GameState.Playing)
            {
                Analytics.CustomEvent(Define.AnalyticsEvent.FULL_AD_FINISH);
                ContinueToPlayGame();
            }
            // ゲームオバー画面から遷移
        }
        else if (prevState == Define.GameState.GameOver && nextState == Define.GameState.StageSelection)
        {
            //alestaads1  RinkaAdvertisementManager.Instance.Refresh();
            //alestaads2 RinkaPurchaseManager.Instance.Refresh();
            UnloadScene(Define.Scene.PLAY);
            UnloadScene(Define.Scene.GAMEOVER);
            StageMgr.Instance.ConsumedOrbsInStage = 0;
            GameModel.StageWhenDied.Value = null;
            GameModel.Stage.Value = StageMgr.Instance.TitleStage;
            GameModel.SpawnStageChunk.Value = StageMgr.Instance.TitleStage.Chunks[0];
            StageMgr.Instance.InitSpawnStage();
            PlayerMgr.Instance.PlayerInstance.SetWispSprite(OrbMgr.Instance.DefaultOrb);
            LoadScene(Define.Scene.TITLE);
            LoadScene(Define.Scene.STAGESELECTION);
            GameModel.PlayerLives.Value = Define.PLAYER_LIVES_PER_GAME;
        }

        else if (prevState == Define.GameState.Skin)
        {
            if (nextState == Define.GameState.StageSelection)
            {
                UnloadScene(Define.Scene.SKIN);
              //  LoadScene(Define.Scene.TITLE);
                LoadScene(Define.Scene.STAGESELECTION);
            }
        }

    }

    /// <summary>
    /// ステージ選択画面→プレイ画面の切替
    /// </summary>
    void StageSelectionToPlayGame() {
        GameModel.ResetScore();
        UnloadScene( Define.Scene.TITLE );
        UnloadScene( Define.Scene.STAGESELECTION );
        LoadScene( Define.Scene.PLAY );
    }

    /// <summary>
    /// ステージ選択画面→Skin画面の切替
    /// </summary>
    void StageSelectionToSkin()
    {
        GameModel.ResetScore();
        UnloadScene(Define.Scene.TITLE);
        UnloadScene(Define.Scene.STAGESELECTION);
        LoadScene(Define.Scene.SKIN);
    }

    /// <summary>
    /// Continue画面→プレイ画面の切替
    /// </summary>
    void ContinueToPlayGame() {
        UnloadScene( Define.Scene.CONTINUE );
        UnloadScene( Define.Scene.GAMEOVER );
        UnloadScene( Define.Scene.CONTINUE1 );
        UnloadScene(Define.Scene.GOTOBONUS);/// GameModel.StageWhenDied.Value = null;

        Debug.Log("abir");
        StageMgr.Instance.InitSpawnStage( GameModel.StageC.Value //GameModel.Stage.Value
            , onFinish: ( player ) => {
            player.Move.Value = Player.MoveState.Glide;
            GameModel.GameState.Value = Define.GameState.UnpauseCountdown;
        } );
    }

    /// <summary>
    /// Continue1画面→プレイ画面の切替
    /// </summary>
    void ContinueToPlayGame1()
    {
        var i  = GameModel.Stage.Value.StageNumber;
       // metadata = m_stageFlow.Stages.GetValue();
        UnloadScene(Define.Scene.CONTINUE);
        UnloadScene(Define.Scene.GAMEOVER);
        UnloadScene(Define.Scene.CONTINUE1);
        UnloadScene(Define.Scene.GOTOBONUS);
        Debug.Log("abir1"+i );
        
        GameModel.Stage.Value = StageMgr.Instance.m_stageFlow.Stages[i]; 
        StageMgr.Instance.InitSpawnStage(GameModel.Stage.Value //StageMgr.Instance.m_stageFlow.Stages[i + 1] //m_stageFlow.Stages[Mathf.Min(i + 1, 5)]   //m_stageFlow.Stages[i+1] //
            , onFinish: (player) => {
                player.Move.Value = Player.MoveState.Glide;
                GameModel.GameState.Value = Define.GameState.UnpauseCountdown;
            }); 
    }


    /// <summary>
    /// プレイ画面かContinue画面→ゲームオバーの切替
    /// </summary>
    void RevealGameOverScreen() {
        if( GameModel.IsGameHighScore ) {
            AudioMgr.Instance.PlayNewHighScore();
        }
    }

    //***********************************************************
    // シーンロード・アンロード
    //***********************************************************
    public void LoadScene( string sceneName ) {
        for(int i=0; i<SceneManager.sceneCount; i++) {
            var loadedScene = SceneManager.GetSceneAt( i );
            if( loadedScene.name == sceneName && loadedScene.IsValid() && loadedScene.isLoaded ) {
                OnSceneLoaded( loadedScene, LoadSceneMode.Additive );
                return;
            }
        }
        SceneManager.LoadScene( sceneName, LoadSceneMode.Additive );
    }

    public void UnloadScene( string sceneName ) {
        var scene = SceneManager.GetSceneByName( sceneName );
        if( scene.isLoaded ) {
            SceneManager.UnloadSceneAsync( sceneName );
        }
    }

    void OnSceneLoaded( Scene loadedScene, LoadSceneMode sceneMode ) {
        SceneManager.SetActiveScene( loadedScene );
    }

    //***********************************************************
    // 有効シーン切り替え
    //***********************************************************
    private void OnActiveSceneChanged( Scene inactiveScene, Scene activeScene ) {
        ISceneController inactiveController = GetSceneController( inactiveScene );
        ISceneController activeController = GetSceneController( activeScene );

        inactiveController?.OnSceneInactive();
        activeController?.OnSceneActive();
    }

    ISceneController GetSceneController( Scene scene ) {
        switch(scene.name) {
            case Define.Scene.TITLE:
                return GameObject.FindObjectOfType<TitleSceneController>() as ISceneController;
            case Define.Scene.PLAY:
                return GameObject.FindObjectOfType<PlaySceneController>() as ISceneController;
            case Define.Scene.STAGESELECTION:
                return GameObject.FindObjectOfType<StageSelectionController>() as ISceneController;
            case Define.Scene.GAMEOVER:
                return GameObject.FindObjectOfType<GameOverController>() as ISceneController;
            case Define.Scene.CONTINUE:
                return GameObject.FindObjectOfType<ContinueSceneController>() as ISceneController;
            default:
                return null;
        }
    }


    //***********************************************************
    // フェードイン・アウト
    //***********************************************************
    public void FadeIn( Action onFinish = null ) {
        FadeIn( m_defaultFadeInDuration, onFinish );
    }

    public void FadeIn( float duration, Action onFinish = null ) {
        m_fadeImage.DOFade( 1f, duration )
            .OnStart( () => m_fadeImage.raycastTarget = true )
            .OnComplete( () => onFinish?.Invoke() );
    }

    public void FadeOut( Action onFinish = null ) {
        FadeOut( m_defaultFadeOutDuration, onFinish );
    }

    public void FadeOut( float duration, Action onFinish = null ) {
        if( duration == 0f) {
            m_fadeImage.color = Color.black;
            m_fadeImage.raycastTarget = false;
            onFinish?.Invoke();
        } else {
            m_fadeImage.DOFade( 0f, duration )
                .OnComplete( () => {
                    m_fadeImage.raycastTarget = false;
                    onFinish?.Invoke();
                } );
        }
    }

    //***********************************************************
    // Destroy
    //***********************************************************
    protected override void OnDestroy() {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
} 
