using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using TMPro;

/// <summary>
/// タイトル画面
/// </summary>
public class TitleSceneController : MonoBehaviour, ISceneController {
    [Header( "UI Windows" )]
    [SerializeField] GameObject m_titleWindow;
    [SerializeField] GameObject m_tutorialWindow;
    [SerializeField] GameObject m_settingsWindow;
    [Header( "Title UI" )]
    [SerializeField] GameObject m_settingsIcon;
    [SerializeField] TMP_Text m_versionText;
    [SerializeField] Image m_tapToBeginImage;
    [SerializeField] GameObject m_tapToBeginInput;
    [Header( "Tutorial UI" )]
    [SerializeField] GameObject m_completeTutorialInput;
    [Header( "Settings UI" )]
    [SerializeField] GameObject m_googlePlayRankingIcon;
    [SerializeField] GameObject m_gameCenterRankingIcon;
    [SerializeField] GameObject m_reviewIcon;
    [SerializeField] GameObject m_restoreImage;
    [SerializeField] Image m_audioOnIcon;
    [SerializeField] Image m_audioOffIcon;
    [SerializeField] GameObject m_twitterIcon;
    [SerializeField] GameObject m_lineIcon;
    [SerializeField] GameObject m_facebookIcon;
    [SerializeField] GameObject m_quitSettingsInput;
    [SerializeField] GameObject m_closeSettingsBtnInput;

    Tweener m_tapToBeginImgTween;

    //***********************************************************
    // 初期化
    //***********************************************************
    void OnEnable() {
        BindTitleInput();
        BindTutorialInput();
        BindSettingsInput();
        Init();
    }

    void OnDisable() {
        m_tapToBeginImgTween.Kill();
    }

    //***********************************************************
    // タイトルウィンドウ初期化
    //***********************************************************
    void BindTitleInput() {
        // 設定ボタンが押された
        m_settingsIcon.AddComponent<ObservablePointerClickTrigger>()
            .OnPointerClickAsObservable()
            .TakeUntilDestroy( this )
            .Subscribe( _ => GameModel.GameState.Value = Define.GameState.Settings );

        // タイトルが押された
        m_tapToBeginInput.AddComponent<ObservablePointerClickTrigger>()
            .OnPointerClickAsObservable()
            .TakeUntilDestroy( this )
            .Subscribe( _ => GameModel.GameState.Value = Define.GameState.StageSelection );
    }

    //***********************************************************
    // チュートリアルウィンドウ初期化
    //***********************************************************
    void BindTutorialInput() {
        // チュートリアル画面が押された
        m_completeTutorialInput.ObservableButton( this ).Subscribe( _ => GameModel.GameState.Value = Define.GameState.Title );
        m_closeSettingsBtnInput.ObservableButton( this ).Subscribe( _ => GameModel.GameState.Value = Define.GameState.Title );
    }

    //***********************************************************
    // 設定ウィンドウ初期化
    //***********************************************************
    void BindSettingsInput() {
        m_audioOffIcon.ObservableButton( this ).Subscribe( _ => GameModel.IsAudioOn.Value = true );
        m_audioOnIcon.ObservableButton( this ).Subscribe( _ => GameModel.IsAudioOn.Value = false );
        m_reviewIcon.ObservableButton( this ).Subscribe( _ => SnsMgr.Instance.RequestAppReview() );
        m_gameCenterRankingIcon.ObservableButton( this ).Subscribe( _ => SnsMgr.Instance.DisplayRanking() );
        m_googlePlayRankingIcon.ObservableButton( this ).Subscribe( _ => SnsMgr.Instance.DisplayRanking() );
        m_twitterIcon.ObservableButton( this ).Subscribe( _ => SnsMgr.Instance.PostTwitterMessage() );
        m_facebookIcon.ObservableButton( this ).Subscribe( _ => SnsMgr.Instance.PostFacebookMessage() );
        m_lineIcon.ObservableButton( this ).Subscribe( _ => SnsMgr.Instance.PostLineMessage() );
        m_quitSettingsInput.ObservableButton( this ).Subscribe( _ => GameModel.GameState.Value = Define.GameState.Title );
        //alestaads2 m_restoreImage.ObservableButton( this ).Subscribe( _ => RinkaPurchaseManager.Instance.Restore() );
    }

    //***********************************************************
    // 他の初期化
    //***********************************************************
    void Init() {
        SceneMgr.Instance.FadeIn( 0f );

        GameModel.IsGameCenterActive.TakeUntilDestroy( this ).Subscribe( isActive => m_gameCenterRankingIcon.SetActive( isActive ) );
        GameModel.IsGooglePlayActive.TakeUntilDestroy( this ).Subscribe( isActive => m_googlePlayRankingIcon.SetActive( isActive ) );
        m_gameCenterRankingIcon.SetActive( GameModel.IsGameCenterActive.Value );
        m_googlePlayRankingIcon.SetActive( GameModel.IsGooglePlayActive.Value );
        m_restoreImage.SetActive( GameModel.Platform == Define.Platform.iOS );
        // TODO: Facebook統合
        m_facebookIcon.SetActive( false );

#if RELEASE
        m_versionText.text = "";
#else
        m_versionText.text = Application.version;
#endif
        // ゲームステート
        GameModel.GameState.TakeUntilDestroy( this ).Subscribe( state => OnGameStateChanged( state ) );

        // Audio
        UpdateAudioUI( GameModel.IsAudioOn.Value );
        GameModel.IsAudioOn
            .TakeUntilDestroy( this )
            .Skip( 1 )
            .Subscribe( isOn => {
                UpdateAudioUI( isOn );
                HandleAudioChanges( isOn );
            } );

        m_tapToBeginImgTween = m_tapToBeginImage
            .DOFade( 0f, 1f )
            .OnStart( () => m_tapToBeginImage.color = Color.white )
            .SetLoops( -1, LoopType.Yoyo );

        AudioMgr.Instance.PlayTitleBgm();
    }

    void OnGameStateChanged( Define.GameState state ) {
        m_tutorialWindow.SetActive( state == Define.GameState.Tutorial );
        m_settingsWindow.SetActive( state == Define.GameState.Settings );
        m_titleWindow.SetActive( state == Define.GameState.Title );
    }

    void UpdateAudioUI( bool isOn ) {
        m_audioOffIcon.gameObject.SetActive( !isOn );
        m_audioOnIcon.gameObject.SetActive( isOn );
    }

    void HandleAudioChanges( bool isOn ) {
        if( isOn ) {
            AudioMgr.Instance.PlayTapButton();
            AudioMgr.Instance.PlayTitleBgm();
        } else {
            AudioMgr.Instance.StopBgm();
        }
    }

    //***********************************************************
    // シーン切替コールバック
    //***********************************************************
    public void OnSceneActive() {
    }

    public void OnSceneInactive() {
    }
}
