using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using TMPro;
using System;

/// <summary>
/// プレイ―シーン画面
/// </summary>
public class PlaySceneController : MonoBehaviour, ISceneController {
    [Header( "Playing UI" )]
    [SerializeField] TMP_Text m_highScoreText;
    [SerializeField] TMP_Text m_distanceText;
    [SerializeField] TMP_Text m_scoreText;
    [SerializeField] TMP_Text m_pauseText;
    [SerializeField] TMP_Text m_countdownText;
    [SerializeField] Image m_pauseFilterImage;
    [SerializeField] Image m_pauseIcon;
    [SerializeField] Image m_playIcon;
    [SerializeField] GameObject m_tapScreenInput;
    [Header( "Game Speed" )]
    [SerializeField] float m_gameSpeedIncreaseRate;

    ReactiveProperty<int> m_timeSpeedDistance = new ReactiveProperty<int>();

    private Vector2 beginPosition;
    private DateTime beginTime;
    public float ThresholdSenconds = 1.0f;
    public float ThresholdDistance = 100.0f;

    private Subject<Unit> onSwipeLeft = new Subject<Unit>();
    //alesta unirx
    public IObservable<Unit> OnSwipeLeft
    {
        get { return onSwipeLeft; }
    }

    private Subject<Unit> onSwipeRight = new Subject<Unit>();
    public IObservable<Unit> OnSwipeRight
    {
        get { return onSwipeRight; }
    }

    //***********************************************************
    // 初期化
    //***********************************************************
    void OnEnable() {
        BindModelToUI();
        BindInput();
        Init();
    }

    void BindModelToUI() {
        // スコアのモデルとUIのバインディング
        GameModel.Score.TakeUntilDisable( this ).DistinctUntilChanged().Subscribe( score => {
            m_scoreText.text = score.ToString();
        } );

        // Reactiveではなくてスタート時だけに更新
        m_highScoreText.text = "Best " + GameModel.HighScore.Value;

        // 距離のモデルとUIのバインディング 
        MessageBroker.Default.Receive<DistanceTravelEvent>().Subscribe( _ => {
            m_distanceText.text = $"{ GameModel.TotalDistance.Value.ToString( "0." )}m";
            //m_distanceText.DOFadeAtSpeed( 1f, 1f ).SetUpdate( true ).OnComplete( () => {
            //    m_distanceText.DOFadeAtSpeed( 0f, 1f );
            //} );
        } );
    }

    void BindInput() {
        // ポーズボタンが押された
        m_pauseIcon.ObservableButton( this ).Subscribe( _ => { PauseGame(); } );
        // 再生ボタンが押された
        m_playIcon.ObservableButton( this ).Subscribe( _ => GameModel.GameState.Value = Define.GameState.UnpauseCountdown );
    }

    /// <summary>
    /// ゲームを一時停止する.
    /// </summary>
    void PauseGame()
    {
        // プレイヤーが死んでいない限りポーズ可能
        var player = PlayerMgr.Instance.PlayerInstance;
        if (player?.Move.Value != Player.MoveState.Die)
        {
            GameModel.GameState.Value = Define.GameState.Paused;
        }
    }

    void InitPlayer( Player player ) {
        // 飛び始める
        player.Move.Value = Player.MoveState.Glide;

        m_tapScreenInput.AddComponent<ObservablePointerDownTrigger>()
            .OnPointerDownAsObservable()
            .Where( _ => player.CanInteract )
            .TakeUntilDestroy( this )
            .Subscribe( _ => player.Move.Value = Player.MoveState.Fly );

        // 飛びやめる
        m_tapScreenInput.AddComponent<ObservablePointerUpTrigger>()
            .OnPointerUpAsObservable()
            .Where( _ => player.CanInteract )
            .TakeUntilDestroy( this )
            .Subscribe( _ => player.Move.Value = Player.MoveState.Glide );

        // 飛びやめる
        m_tapScreenInput.AddComponent<ObservableEventTrigger>()
            .OnBeginDragAsObservable()
            .Where(_ => player.CanInteract)
            .TakeUntilDestroy(this)
            .Select(eventData => eventData.position)
            .Subscribe(//_ => Debug.Log("DRAGGGGGGGGGGG"), 
            position =>
            {
                this.beginPosition = position;
                this.beginTime = DateTime.Now;
            });
        m_tapScreenInput.GetComponent<ObservableEventTrigger>()
            .OnEndDragAsObservable()
            .TakeUntilDisable(this)
            .Where(eventData => (DateTime.Now - this.beginTime).TotalSeconds < ThresholdSenconds) 
            .Select(eventData => eventData.position)
            .Where(position => beginPosition.x > position.x)
            .Where(position => Mathf.Abs(beginPosition.x - position.x) >= this.ThresholdDistance)
            .Subscribe(_ => player.Move.Value = Player.MoveState.Left);//Debug.Log("LEFTTTTTTTTTTT")); 

        m_tapScreenInput.GetComponent<ObservableEventTrigger>()
            .OnEndDragAsObservable()
            .TakeUntilDisable(this)
            .Where(eventData => (DateTime.Now - this.beginTime).TotalSeconds < ThresholdSenconds) 
            .Select(eventData => eventData.position)
            .Where(position => position.x > beginPosition.x)
            .Where(position => Mathf.Abs(position.x - beginPosition.x) >= this.ThresholdDistance)
            .Subscribe(_ => player.Move.Value = Player.MoveState.Right); //Debug.Log("RIGHTTTTTTTTTT"));  






    }

    void Init() {
        Time.timeScale = Define.INIT_TIME_SCALE;
        AudioMgr.Instance.PlayGameBgm();

        GameModel.Score.Value = 0;
        GameModel.GameState.Value = Define.GameState.Playing;

        GameModel.GameState
            .DistinctUntilChanged()
            .TakeUntilDisable( this )
            .Subscribe( state => OnGameStateChanged( state ) );

        GameModel.TotalDistance
            .DistinctUntilChanged()
            .TakeUntilDestroy( this )
            .Subscribe( _ => {
                if( GameModel.IsLastStage.Value ) {
                    m_timeSpeedDistance.Value++;
                    float newGameSpeed = Define.INIT_TIME_SCALE + m_timeSpeedDistance.Value * m_gameSpeedIncreaseRate;
                    Time.timeScale = Mathf.Min( Define.MAX_GAME_SPEED, newGameSpeed );
                } else {
                    Time.timeScale = Define.INIT_TIME_SCALE;
                }
            } );

        GameModel.IsLastStage
            .DistinctUntilChanged()
            .TakeUntilDestroy( this )
            .Where( isLastStage => isLastStage )
            .Subscribe( _ => m_timeSpeedDistance.Value = 0 );

        MessageBroker.Default.Receive<PlayerDeathEvent>()
            .TakeUntilDisable( this )
            .Delay( System.TimeSpan.FromSeconds( 1 ) )
            .Subscribe( _ => {
                if( GameModel.CanContinue ) {
                    GameModel.GameState.Value = Define.GameState.Continue;
                } else {
                    GameModel.GameState.Value = Define.GameState.GameOver;
                }
            } );
        ////////////////////////////////////////////
        MessageBroker.Default.Receive<ClearStageEvent>()
           .TakeUntilDisable(this)
           .Delay(System.TimeSpan.FromSeconds(1))
           .Subscribe(_ => {
               if (GameModel.CanContinue)
               {
                   GameModel.GameState.Value = Define.GameState.Playing ;
                   Debug.Log("GOTOBONUS!!"+ GameModel.GameState.Value);
               }
               else
               {
                   GameModel.GameState.Value = Define.GameState.GameOver;
               }
           });
    }
    
    void OnGameStateChanged( Define.GameState state ) {
        m_countdownText.enabled = ( state == Define.GameState.UnpauseCountdown );
        m_pauseFilterImage.enabled = ( state == Define.GameState.Paused || state == Define.GameState.UnpauseCountdown );
        m_pauseIcon.enabled = ( state == Define.GameState.Playing );
        m_playIcon.enabled = ( state == Define.GameState.Paused );
        m_pauseText.enabled = ( state == Define.GameState.Paused );
        m_highScoreText.enabled = ( state != Define.GameState.GameOver );
        m_scoreText.enabled = ( state != Define.GameState.GameOver );

        if( state == Define.GameState.UnpauseCountdown ) {
            PlayCountdown();
        } 
    }

    void PlayCountdown() {
        SetCountdownText( 0f, "3" );
        SetCountdownText( 1f, "2" );
        SetCountdownText( 2f, "1" );
        Observable.Timer( System.TimeSpan.FromSeconds( 3f ), Scheduler.MainThreadIgnoreTimeScale ).Subscribe( _ => 
            GameModel.GameState.Value = Define.GameState.Playing 
        );
    }

    void SetCountdownText( float waitTime, string text ) {
        var duration = System.TimeSpan.FromSeconds( waitTime );
        Observable.Timer( duration, Scheduler.MainThreadIgnoreTimeScale ).Subscribe( _ => m_countdownText.text = text );
    }


    void OnApplicationFocus(bool focus)
    {
#if !UNITY_EDITOR
        if( focus ) {
            // 画面が切り替わったらポーズする.
            PauseGame();
        }
#endif
    }

    //***********************************************************
    // シーン切替コールバック
    //***********************************************************
    public void OnSceneActive() {
        // ステージ選択画面→プレイ画面
        StageMgr.Instance.InitSpawnStage( GameModel.Stage.Value, onFinish: (player) => InitPlayer( player ) );
    }

    public void OnSceneInactive() {
    }

}
