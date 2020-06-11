using UnityEngine;
using UniRx;

public static class GameModel {
    public static ReactiveProperty<bool> IsTutorial { get; } = new ReactiveProperty<bool>( true );
    public static ReactiveProperty<bool> IsAudioOn { get; } = new ReactiveProperty<bool>( true );
    public static ReactiveProperty<bool> IsAdSkipPurchased { get; } = new ReactiveProperty<bool>( false );
    public static ReactiveProperty<bool> MustWatchContinueAd { get; } = new ReactiveProperty<bool>( false );
    public static ReactiveProperty<bool> IsAppReviewed { get; } = new ReactiveProperty<bool>( false );
    public static ReactiveProperty<bool> IsLastStage { get; } = new ReactiveProperty<bool>();
    public static ReactiveProperty<bool> IsGooglePlayActive { get; } = new ReactiveProperty<bool>();
    public static ReactiveProperty<bool> IsGameCenterActive { get; } = new ReactiveProperty<bool>();
    public static ReactiveProperty<bool> IsAdError { get; } = new ReactiveProperty<bool>();
    public static ReactiveProperty<Define.Ranking.MsgType> ScoreMsgType { get; } = new ReactiveProperty<Define.Ranking.MsgType>();
    public static ReactiveProperty<int> CumulativeDistance { get; } = new ReactiveProperty<int>();
    public static ReactiveProperty<int> CurrentLifeDistance { get; } = new ReactiveProperty<int>();
    public static ReactiveProperty<int> TotalDistance { get; } = new ReactiveProperty<int>();
    public static ReactiveProperty<int> Score { get; } = new ReactiveProperty<int>();
    public static ReactiveProperty<int> HighScore { get; } = new ReactiveProperty<int>();
    public static ReactiveProperty<int> PlayerLives { get; } = new ReactiveProperty<int>();
    public static ReactiveProperty<Define.GameState> GameState { get; } = new ReactiveProperty<Define.GameState>();
    public static ReactiveProperty<Define.GameSubState> GameSubState { get; } = new ReactiveProperty<Define.GameSubState>();
    public static ReactiveProperty<StageMetaData> Stage { get; } = new ReactiveProperty<StageMetaData>();
    public static ReactiveProperty<StageMetaData> StageC { get; } = new ReactiveProperty<StageMetaData>();
    public static ReactiveProperty<StageChunkData> SpawnStageChunk { get; } = new ReactiveProperty<StageChunkData>();
    public static ReactiveProperty<StageChunkData> StageWhenDied { get; } = new ReactiveProperty<StageChunkData>();
    public static ReactiveProperty<StageChunkData> StageWhenClear { get; } = new ReactiveProperty<StageChunkData>(); ///my
    public static Define.Platform Platform { private set; get; }
    public static bool IsGameHighScore;
    public static bool CanContinue => PlayerLives.Value >= 1;

    static float m_playStartTime = 0f;
    static bool m_isStageSelectionAd = false; // ステージ選択時に広告を見たか.

    public static void Init() {
#if UNITY_EDITOR
        Platform = Define.Platform.Editor;
#elif UNITY_IOS
        Platform = Define.Platform.iOS;
#else
        Platform = Define.Platform.Android;
#endif
        InitSkipAdsPurchased();
        InitContinueAds();
        InitTutorialStatus();
        InitAudio();
        InitScoring();
        InitDistance();
        InitLives();
        InitAppReviewedStatus();
    }

    /// <summary>
    /// 課金したかどうかの初期化
    /// </summary>
    static void InitSkipAdsPurchased() {
        // 課金の状態が変わったら、PlayerPrefsに保存
        IsAdSkipPurchased
            .SkipLatestValueOnSubscribe()
            .DistinctUntilChanged()
            .Subscribe(
                isPurchased => {
                    PlayerPrefsUtil.SetBool( Define.PlayerPref.IS_AD_SKIP_PURCHASED, isPurchased );
                     //aleetaads1 RinkaAdvertisementManager.isEnable = !isPurchased;
                }
            );
        IsAdSkipPurchased.Value = PlayerPrefsUtil.GetBool( Define.PlayerPref.IS_AD_SKIP_PURCHASED );
    }

    /// <summary>
    /// ゲームスタートから死ぬまでの指定期間の間、広告を表示しないように設定
    /// </summary>
    static void InitContinueAds() {//
        // ゲームスタート時にContinue広告を表示しない
        GameState.Buffer( 2, 1 ).Subscribe( states => {
            var prevState = states[0];
            var nextState = states[1];

            bool isPrevStateAd = ( prevState == Define.GameState.PlayStageSelectionAd || prevState == Define.GameState.PlayContinueAd || prevState == Define.GameState.StageSelection );
            if( isPrevStateAd && nextState == Define.GameState.Playing ) {
                MustWatchContinueAd.Value = false;
            }
        } );

        // ゲームプレイシーンに遷移した時点の時間を保存.
        GameState.Buffer( 2, 1 )
            .Where( states => ( states[0] ==Define.GameState.PlayStageSelectionAd || Define.GameState.StageSelection == states[0] ) && states[1] == Define.GameState.Playing )
            .Subscribe( states => {
                m_playStartTime = Time.time;
                m_isStageSelectionAd = states[0] == Define.GameState.PlayStageSelectionAd;
            } );

        // プレイシーンからコンテニュー画面に遷移したときに広告を出す必要があるかのフラグを更新.
        GameState.Buffer( 2, 1 )
            .Where( states => states[0] == Define.GameState.Playing && states[1] == Define.GameState.Continue )
            .Subscribe( states => {
                if( !m_isStageSelectionAd )
                    return;

                float playingTime = Time.time - m_playStartTime;
                if( playingTime >= Define.IGNORE_ADS_AFTER_PLAY_SEC ) {
                    MustWatchContinueAd.Value = true;
                }
            } );
    }

    /// <summary>
    /// チュートリアル関係の初期化
    /// </summary>
    static void InitTutorialStatus() {
        IsTutorial.Value = PlayerPrefsUtil.GetBool( Define.PlayerPref.TUTORIAL, defaultVal: true );

        // タイトル画面へ遷移したら、チュートリアルをもう表示しない
        GameState
            .SkipLatestValueOnSubscribe()
            .Where( state => state == Define.GameState.Title )
            .Take( 1 )
            .Subscribe( _ => PlayerPrefsUtil.SetBool( Define.PlayerPref.TUTORIAL, false ) );
    }

    /// <summary>
    /// Audio関係の初期化
    /// </summary>
    static void InitAudio() {
        IsAudioOn.Value = PlayerPrefsUtil.GetBool( Define.PlayerPref.AUDIO_VOLUME, defaultVal: true );

        IsAudioOn.DistinctUntilChanged().Subscribe( isOn => {
            PlayerPrefsUtil.SetBool( Define.PlayerPref.AUDIO_VOLUME, isOn );
        } );
    }

    /// <summary>
    /// スコア関係の初期化
    /// </summary>
    static void InitScoring() {
        // スコアが変わったら、ベストスコア判定
        Score.Subscribe( score => {
            var msgType = ( score > HighScore.Value ) ? Define.Ranking.MsgType.HighScore : Define.Ranking.MsgType.LastScore;
            ScoreMsgType.SetValueAndForceNotify( msgType );
        } );

        // ベストスコアができたら、HighScore更新
        ScoreMsgType
            .Where( msgType => msgType == Define.Ranking.MsgType.HighScore )
            .Subscribe( _ => {
                HighScore.Value = Score.Value;
                IsGameHighScore = true;
                PlayerPrefs.SetInt( Define.PlayerPref.HIGH_SCORE, HighScore.Value );
            } );

        HighScore.Value = UnityEngine.PlayerPrefs.GetInt( Define.PlayerPref.HIGH_SCORE );
    }

    /// <summary>
    /// 距離関係の初期化
    /// </summary>
    static void InitDistance() {
        // 指定した距離で通知イベントを発生
        TotalDistance
            .Where( dis => dis % Define.DISTANCE_NOTIFICATION == 0 )
            .Subscribe( dis => MessageBroker.Default.Publish( new DistanceTravelEvent() ) );

        // プレイ中の距離を全ての距離の計算に加算
        CurrentLifeDistance.Subscribe( currentDis => TotalDistance.Value = CumulativeDistance.Value + currentDis );
    }

    /// <summary>
    /// ゲームごとのプレイヤーの残りライフ数
    /// </summary>
    static void InitLives() {
        PlayerLives.Value = Define.PLAYER_LIVES_PER_GAME;
    }

    /// <summary>
    /// ユーザーがアプリをレビューしたかどうか
    /// </summary>
    static void InitAppReviewedStatus() {
        IsAppReviewed.Value = PlayerPrefsUtil.GetBool( Define.PlayerPref.APP_REVIEW, defaultVal: false );
    }

    public static void ResetScore() {
        CumulativeDistance.Value = 0;
        IsGameHighScore = false;
    }
}
