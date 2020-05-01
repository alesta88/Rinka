using UnityEngine;
using UniRx;
using TMPro;

/// <summary>
/// ゲームオバー画面
/// </summary>
public class GameOverController : MonoBehaviour, ISceneController {
    [Header( "Game Over UI" )]
    [SerializeField] GameObject m_restartBtn;
    [SerializeField] TMP_Text m_gameOverText;
    [SerializeField] TMP_Text m_distanceText;
    [SerializeField] TMP_Text m_scoreText;
    [SerializeField] TMP_Text m_highScoreText;
    [Header( "Social Buttons" )]
    [SerializeField] GameObject m_gameCenterRankingBtn;
    [SerializeField] GameObject m_googlePlayRankingBtn;
    [SerializeField] GameObject m_facebookBtn;
    [SerializeField] GameObject m_twitterBtn;
    [SerializeField] GameObject m_lineBtn;

    //***********************************************************
    // 初期化
    //***********************************************************
    void Start() {
        DisplayScore();
        BindInput();
        Init();
    }

    void DisplayScore() {
        m_scoreText.text = GameModel.Score.Value.ToString();
        m_highScoreText.text = $"Best {GameModel.HighScore.Value.ToString()}";
        m_distanceText.text = $"{GameModel.TotalDistance.Value.ToString( "0." )}m traveled";
        m_gameOverText.text = GameModel.IsGameHighScore ? "NEW BEST!" : "GAME OVER";
    }

    void BindInput() {
        // ポーズボタンが押された
        m_restartBtn.ObservableButton( this ).Subscribe( _ => GameModel.GameState.Value = Define.GameState.StageSelection );
        m_gameCenterRankingBtn.ObservableButton( this ).Subscribe( _ => SnsMgr.Instance.DisplayRanking() );
        m_googlePlayRankingBtn.ObservableButton( this ).Subscribe( _ => SnsMgr.Instance.DisplayRanking() );
        m_lineBtn.ObservableButton( this ).Subscribe( _ => SnsMgr.Instance.PostLineMessage() );
        m_facebookBtn.ObservableButton( this ).Subscribe( _ => SnsMgr.Instance.PostFacebookMessage() );
        m_twitterBtn.ObservableButton( this ).Subscribe( _ => SnsMgr.Instance.PostTwitterMessage() );
    }

    void Init() {
        GameModel.IsGameCenterActive.TakeUntilDestroy( this ).Subscribe( isActive => m_gameCenterRankingBtn.SetActive( isActive ) );
        GameModel.IsGooglePlayActive.TakeUntilDestroy( this ).Subscribe( isActive => m_googlePlayRankingBtn.SetActive( isActive ) );
        m_gameCenterRankingBtn.SetActive( GameModel.IsGameCenterActive.Value );
        m_googlePlayRankingBtn.SetActive( GameModel.IsGooglePlayActive.Value );
        // TODO: Facebook統合
        m_facebookBtn.SetActive( false );
    }

    //***********************************************************
    // シーン切替コールバック
    //***********************************************************
    public void OnSceneActive() {
    }

    public void OnSceneInactive() {
    }
}
