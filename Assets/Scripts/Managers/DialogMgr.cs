using System;
using UnityEngine;

/// <summary>
/// ダイアログとテクストを管理するクラス
/// </summary>
public class DialogMgr : MonoSingleton<DialogMgr> {
    public static Dialog Dialog { private set; get; }
    public static GameObject ScreenFilter { private set; get; }
    public static TextData Text { private set; get; }

    const string TEXTDATA_PATH = "TextData/";

    //***********************************************************
    // 初期化
    //***********************************************************
    /// <summary>
    /// システムの言語で初期化
    /// </summary>
    public void Init( SystemLanguage language ) {
        var jpnLang = Resources.Load<TextData>( TEXTDATA_PATH + "jpn_text_data" );
        var engLang = Resources.Load<TextData>( TEXTDATA_PATH + "eng_text_data" );
        Text = ( language == SystemLanguage.Japanese ) ? jpnLang : engLang;

        // ダイアログ管理の付箋設定
        var canvas = GameObject.FindObjectOfType<Canvas>();
        if( canvas != null) {
            // 画面フィルター.
            var filterPref = Resources.Load<GameObject>( "UI/ScreenFilter" );
            ScreenFilter = Instantiate<GameObject>( filterPref, canvas.transform );
            SetScreenFilterActive( false );

            var dialogPref = Resources.Load<GameObject>( "UI/Dialog" );
            var dialogObj = Instantiate<GameObject>( dialogPref, canvas.transform );
            Dialog = dialogObj.GetComponent<Dialog>();
        } else {
            Debug.LogError( "ダイアログPrefabが見つかれない" );
        }
    }

    //***********************************************************
    // ランキング関連テクスト
    //***********************************************************
    public string GetSnsRankingMsg() {
        int score = GameModel.Score.Value;
        int highScore = GameModel.HighScore.Value;
        bool isHighScore = GameModel.ScoreMsgType.Value == Define.Ranking.MsgType.HighScore;

        // 設定画面からのメッセージ
        if( GameModel.GameState.Value == Define.GameState.Settings ) {
            return ( highScore == 0 ) ? 
                string.Format( Text.PromoteSocialMsg, score, highScore ) :
                string.Format( Text.HighScoreSocialMsg, score, highScore );
        }

        // ゲームオバー画面からのメッセージ
        if( score == 0 ) {
            return string.Format( Text.PromoteSocialMsg, score, highScore );
        } else if( isHighScore ) {
            return string.Format( Text.HighScoreSocialMsg, score, highScore );
        } else {
            return string.Format( Text.LastScoreSocialMsg, score, highScore );
        }
    }

    public string GetTwitterRankingMsg() {
        int score = GameModel.Score.Value;
        int highScore = GameModel.HighScore.Value;
        bool isHighScore = GameModel.ScoreMsgType.Value == Define.Ranking.MsgType.HighScore;

        // 設定画面からのメッセージ
        if( GameModel.GameState.Value == Define.GameState.Settings ) {
            return ( highScore == 0 ) ?
                string.Format( Text.PromoteSocialMsgWithHashtag, score, highScore ) :
                string.Format( Text.HighScoreSocialMsgWithHashtag, score, highScore );
        }

        // ゲームオバー画面からのメッセージ
        if( score == 0 ) {
            return string.Format( Text.PromoteSocialMsgWithHashtag, score, highScore );
        } else if( isHighScore ) {
            return string.Format( Text.HighScoreSocialMsgWithHashtag, score, highScore );
        } else {
            return string.Format( Text.LastScoreSocialMsgWithHashtag, score, highScore );
        }
    }

    //***********************************************************
    // エラーダイアログ
    //***********************************************************

    /// <summary>
    /// 表示
    /// </summary>
    void Display( string title, string message, Action onTapButton, int callbackDelayMs ) {
        Dialog.Display( title, message, onTapButton, callbackDelayMs );
    }
    /// <summary>
    /// 表示（初期化失敗）
    /// </summary>
    public void DisplayInitError( Action onTapButton = null, int callbackDelayMs = 500 ) {
        Display( Text.InitErrorTitle, Text.InitErrorMsg, onTapButton, callbackDelayMs );
    }
    /// <summary>
    /// 表示（ネットワーク接続失敗）
    /// </summary>
    public void DisplayNoConnectionError( Action onTapButton = null, int callbackDelayMs = 500 ) {
        Display( Text.NoConnectionErrorTitle, Text.NoConnectionErrorMsg, onTapButton, callbackDelayMs );
    }
    /// <summary>
    /// 表示（購入商品復元成功）
    /// </summary>
    public void DisplayRestoreSuccess( Action onTapButton = null, int callbackDelayMs = 500 ) {
        Display( Text.RestoreSuccessTitle, Text.RestoreSuccessMsg, onTapButton, callbackDelayMs );
    }
    /// <summary>
    /// 表示（購入商品復元失敗）
    /// </summary>
    public void DisplayRestoreError( Action onTapButton = null, int callbackDelayMs = 500 ) {
        Display( Text.RestoreErrorTitle, Text.RestoreErrorMsg, onTapButton, callbackDelayMs );
    }
    /// <summary>
    /// 表示（購入未許可失敗）
    /// </summary>
    public void DisplayPurchaseSettingsError( Action onTapButton = null, int callbackDelayMs = 500 ) {
        Display( Text.PurchaseSettingsErrorTitle, Text.PurchaseSettingsErrorMsg, onTapButton, callbackDelayMs );
    }
    /// <summary>
    /// 表示（謎の購入失敗）
    /// </summary>
    public void DisplayUnknownPurchaseError( Action onTapButton = null, int callbackDelayMs = 500 ) {
        Display( Text.UnknownPurchaseErrorTitle, Text.UnknownPurchaseErrorMsg, onTapButton, callbackDelayMs );
    }
    /// <summary>
    /// 表示（広告再生失敗）
    /// </summary>
    public void DisplayAdvertisementDownloadingError( Action onTapButton = null, int callbackDelayMs = 500 ) {
        Display( Text.AdvertisementDownloadingErrorTitle, Text.AdvertisementDownloadingErrorMsg, onTapButton, callbackDelayMs );
    }
    /// <summary>
    /// 表示（アプリ未導入失敗）
    /// </summary>
    public void DisplayNativeUnavailableApp( Action onTapButton = null, int callbackDelayMs = 500 ) {
        Display( Text.NoAppInstalledTitle, Text.NoAppInstalledMsg, onTapButton, callbackDelayMs );
    }

    //***********************************************************
    // 画面フィルター
    //***********************************************************
    /// <summary>
    /// フィルターの有効、無効を切り替え.
    /// </summary>
    /// <param name="isActive"></param>
    public void SetScreenFilterActive( bool isActive ) {
        if( ScreenFilter != null ) {
            ScreenFilter.SetActive( isActive );
        }
        Debug.Log( "SetScreenFilterActive: " + isActive );
    }
}