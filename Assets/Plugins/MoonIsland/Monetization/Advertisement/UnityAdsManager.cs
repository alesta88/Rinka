using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using MoonIsland;

///====================================================================================================
/// <summary>
/// ■ Unityの動画広告の管理クラス。
///     全広告管理クラスから参照する。
/// </summary>
///====================================================================================================
public class UnityAdsManager {
    ///------------------------------------------------------------------------------------------------
    /// ● 要素
    ///------------------------------------------------------------------------------------------------
    /// <summary>飛ばせる動画一覧</summary>
    readonly Dictionary<bool, string> m_skipMovies = new Dictionary<bool, string>() {
        { true,     "video" },          // 飛ばせる動画
        { false,    "rewardedVideo" },  // 最後まで見させる動画（報酬）
    };
    /// <summary>失敗時の呼戻変数</summary>
    Action<AdvertisementError> m_onError;

    /// <summary>初期化済か？</summary>
    public bool isInitialized {
        get { return Advertisement.isInitialized; }
    }
    /// <summary>準備完了か？</summary>
    public bool isReady {
        get {
            return (
                Advertisement.IsReady( m_skipMovies[true] ) &&
                Advertisement.IsReady( m_skipMovies[false] )
            );
        }
    }
    /// <summary>再生中か？</summary>
    public bool isPlaying {
        get { return Advertisement.isShowing; }
    }
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 初期化
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    public void Initialize( Action<AdvertisementError> onError ) {
        m_onError = onError;    // 呼戻登録

        Debug.Log( "UnityAds初期化開始" );
        Refresh( m_onError );
    }
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● リフレッシュ
    ///     場面切り替えごとに呼ぶと、初期化失敗の可能性が下がる
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    public void Refresh( Action<AdvertisementError> onError ) {
        m_onError = onError;    // 呼戻登録

        // デバッグ表示を設定
        Advertisement.debugMode =
#if RELEASE
            false
#else
            true
#endif
        ;

        // 通信接続していない場合、失敗
        if ( Application.internetReachability == NetworkReachability.NotReachable ) {
            Debug.LogError( "UnityAds初期化失敗 : 通信接続失敗" );
            m_onError?.Invoke( AdvertisementError.NoNetwork );

            // 対応していない場合、失敗
        } else if ( !Advertisement.isSupported ) {
            Debug.LogError( "UnityAds初期化失敗 : 非対応" );
            m_onError?.Invoke( AdvertisementError.NoInitialize );

            // 未初期化の場合、成功
        } else if ( !isInitialized ) {
            // 初期化失敗防止の為、初期化（通常は既に行われている）
            Advertisement.Initialize(
#if UNITY_ANDROID
                "3598378",
#elif UNITY_IOS
                "2684009",
#endif
#if RELEASE
                false
#else 
               true
#endif
            );
            Debug.Log( "UnityAds初期化（リフレッシュ）完了" );
        }

        m_onError = null;   // 呼戻リセット
    }
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 動画再生
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    public void PlayMovie( bool isSkip, Action onFinish, Action onSkip, Action<AdvertisementError> onError ) {
        m_onError = onError;

        var id = m_skipMovies[isSkip];

        // 動画準備が未完了の場合、未再生
        if ( !Advertisement.IsReady( id ) ) {
            Debug.LogError( "UnityAds動画広告 : 準備不足" );
            m_onError?.Invoke( AdvertisementError.Downloading );
            return;
        }

        // 再生
        //alestaads Advertisement.Show(
        //    id,
        //    new ShowOptions { resultCallback = result => {
        //        // 最後まで見ても、途中で飛ばしても、失敗でも、等しくonFinishされる
        //        switch ( result ) {
        //            case ShowResult.Finished:
        //                Debug.Log( "UnityAds動画広告 : 正常終了" );
        //                onFinish?.Invoke();
        //                break;
        //            case ShowResult.Skipped:
        //                Debug.Log( "UnityAds動画広告 : 飛越" );
        //                onSkip?.Invoke();
        //                break;
        //            case ShowResult.Failed:
        //                Debug.LogError( "UnityAds動画広告 : 失敗" );
        //                Refresh( null );
        //                onFinish?.Invoke();
        //                break;
        //        }
        //    } }
        //);
    }
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 更新（デバッグ）
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    public void UpdateDebug() {
        // 軽量化の為、stringを可能な限り分離
        const string text1 = "広告状態 : 飛, 報 : {0}, {1}";
        DebugDisplay.Instance.Add(
            string.Format(
                text1,
                Advertisement.GetPlacementState( m_skipMovies[true] ),
                Advertisement.GetPlacementState( m_skipMovies[false] )
            )
        );
        const string text2 = "広告準備完了 : 飛, 報 : {0}, {1}";
        DebugDisplay.Instance.Add(
            string.Format(
                text2,
                Advertisement.IsReady( m_skipMovies[true] ),
                Advertisement.IsReady( m_skipMovies[false] )
            )
        );
    }
}