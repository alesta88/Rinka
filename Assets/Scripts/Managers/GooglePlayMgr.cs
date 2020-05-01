#if UNITY_ANDROID
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

/// <summary>
/// GooglePlay機能を管理するクラス
/// </summary>
public class GooglePlayMgr : MonoSingleton<GooglePlayMgr> {
    /// <summary>
    /// GooglePlayにログイン
    /// </summary>
    public void Init() {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
#if !RELEASE || LOG_ENABLE
        PlayGamesPlatform.DebugLogEnabled = true;
#endif
        PlayGamesPlatform.InitializeInstance( config );
        PlayGamesPlatform.Activate();
    }

    public void SignIn( Action onSuccess, Action onFail ) {
        Social.localUser.Authenticate( ( isSuccess, msg ) => {
            if( isSuccess ) {
                Debug.Log( "GooglePlayログイン成功" );
                GameModel.IsGooglePlayActive.Value = true;
                AcquireRankingScores();
                onSuccess.NullSafe();
            } else {
                Debug.LogWarning( "GooglePlayログイン失敗: " + msg );
                GameModel.IsGooglePlayActive.Value = false;
                onFail.NullSafe();
            }
        } );
    }

    void AcquireRankingScores() {
        ILeaderboard board = PlayGamesPlatform.Instance.CreateLeaderboard();
        board.id = GPGSIds.leaderboard_leaderboard;
        PlayGamesPlatform.Instance.LoadScores( 
            GPGSIds.leaderboard_leaderboard, 
            LeaderboardStart.PlayerCentered,
            25, 
            LeaderboardCollection.Public, 
            LeaderboardTimeSpan.AllTime, 
            ( result ) => {
                Debug.Log( "ランキング取得結果: " + result.Status );
                if( result.Status == ResponseStatus.Success || result.Status == ResponseStatus.SuccessWithStale ) {
                    foreach(var score in result.Scores) {
                        Debug.Log( score.userID + " " + score.rank + " " + score.value );
                    }
                } 
        } );
    }

    /// <summary>
    /// スコアをGooglePlayのランキングに送信
    /// </summary>
    public void ReportScore() {
        if( !PlayGamesPlatform.Instance.IsAuthenticated() )
            return;

        Social.ReportScore( GameModel.Score.Value, Define.Ranking.RANKING_ID, ( isSuccess ) => {
            if( isSuccess ) {
                Debug.Log( "ランキングへスコア報告成功" );
            } else {
                Debug.LogWarning( "ランキングへスコア報告失敗" );
            }
        } );
    }

    /// <summary>
    /// ランキングUIを表示
    /// </summary>
    public void DisplayRankingUI() {
        if( !PlayGamesPlatform.Instance.IsAuthenticated() )
            return;

        PlayGamesPlatform.Instance.ShowLeaderboardUI( GPGSIds.leaderboard_leaderboard, LeaderboardTimeSpan.AllTime, ( sts ) => {
            Debug.Log( sts );
        } );
    }
}
#endif