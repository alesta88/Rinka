using UnityEngine;

/// <summary>
/// GameCenter管理クラス
/// </summary>
public class GameCenterMgr : MonoSingleton<GameCenterMgr> {
    [SerializeField] string LeaderBoardId;
    public bool IsInitSuccess { private set; get; }

#if UNITY_IOS
    public void Init() {
        GameCenterManager.OnAuthFinished += OnAuthFinished;
        GameCenterManager.Init();
    }

    public void ShowLeaderboard() {
        if( IsInitSuccess && GameCenterManager.IsPlayerAuthenticated ) {
            GameCenterManager.ShowLeaderboard( LeaderBoardId );
        } else {
            var title = Application.systemLanguage == SystemLanguage.Japanese ? "お知らせ" : "Notice";
            var msg = Application.systemLanguage == SystemLanguage.Japanese ? "ログインしていません。" : "Not Logged In";
            IOSNativePopUpManager.showMessage( title, msg );
        }
    }

    public void ReportScore() {
        if( IsInitSuccess && GameCenterManager.IsPlayerAuthenticated ) {
            GameCenterManager.ReportScore( GameModel.Score.Value, LeaderBoardId );
        }
    }

    void OnAuthFinished( SA.Common.Models.Result result ) {
        IsInitSuccess = result.IsSucceeded;
        if( IsInitSuccess ) {
            GameCenterManager.OnLeadrboardInfoLoaded += OnLoadLeaderboardInfo;
            GameCenterManager.LoadLeaderboardInfo( LeaderBoardId );
            GameModel.IsGameCenterActive.Value = true;
            Debug.Log( "GameCenter承認成功 " );
        } else {
            Debug.LogWarning( "GameCenter承認失敗, CODE: " + result.Error.Code + ", " + result.Error.Message );
            GameModel.IsGameCenterActive.Value = false;
        }
    }

    void OnLoadLeaderboardInfo( GK_LeaderboardResult obj ) {
    }
#endif
}
