//using System;
//using UnityEngine;

/////====================================================================================================
///// <summary>
///// ■ 全広告の管理クラス。
///// </summary>
/////====================================================================================================
//public abstract class AdvertisementManager<T> : MonoSingleton<T> where T : MonoBehaviour {
//    ///------------------------------------------------------------------------------------------------
//    /// ● 要素
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>有効か？</summary>
//    // ※コールバック設定を考慮して、staticにした
//    public static bool isEnable = true;
//    /// <summary>Unityの動画広告</summary>
//    readonly UnityAdsManager m_unityAds = new UnityAdsManager();

//    /// <summary>表示中か？</summary>
//    public bool isShowing {
//        get { return m_unityAds.isPlaying; }
//    }
//    /// <summary>初期化済か？</summary> 
//    public bool isInitialized {
//        get { return m_unityAds.isInitialized; }
//    }
//    /// <summary>問題発生中か？</summary>
//    public bool isError {
//        get {
//            // ネット未接続か、未初期化か、配信未完了の場合
//            return (
//                Application.internetReachability == NetworkReachability.NotReachable ||
//                !isInitialized ||
//                !m_unityAds.isReady
//            );
//        }
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 成功判定
//    ///     同一処理を共通化。
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    void CheckSuccess( Action onFinish, Action<AdvertisementError> onError, bool isIgnoreInitialize = false ) {
//        // 無効の場合、未処理
//        if ( !isEnable ) {
//            Debug.Log( $"動画失敗 : {AdvertisementError.Dissable}" );
//            onError?.Invoke( AdvertisementError.Dissable );

//        // 通信未接続失敗
//        } else if ( Application.internetReachability == NetworkReachability.NotReachable ) {
//            Debug.LogError( $"動画失敗 : {AdvertisementError.NoNetwork}" );
//            onError?.Invoke( AdvertisementError.NoNetwork );

//        // 初期化失敗
//        } else if ( !isIgnoreInitialize && !isInitialized ) {
//            Debug.LogError( $"動画失敗 : {AdvertisementError.NoInitialize}" );
//            onError?.Invoke( AdvertisementError.NoInitialize );

//        // それ以外は、成功
//        } else {
//            onFinish?.Invoke();
//        }
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 初期化
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    protected virtual void Initialize( Action<AdvertisementError> onError ) {
//        // 成功判定
//        CheckSuccess(
//            () => m_unityAds.Initialize( onError ), // 成功
//            onError,                                // 失敗
//            true                                    // 初期化無視する
//        );
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● リフレッシュ
//    ///     場面切り替えごとに呼ぶと、初期化失敗の可能性が下がる
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    protected virtual void Refresh( Action<AdvertisementError> onError ) {
//        // 成功判定
//        CheckSuccess(
//            () => m_unityAds.Refresh( onError ),    // 成功
//            onError,                                // 失敗
//            true                                    // 初期化無視する
//        );
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 動画再生
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    protected virtual void PlayMovie( bool isSkip, Action onFinish, Action onSkip, Action<AdvertisementError> onError ) {
//        // 成功判定
//        CheckSuccess(
//            () => m_unityAds.PlayMovie( isSkip, onFinish, onSkip, onError ),    // 成功
//            onError                                                     // 失敗
//        ); 
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 更新
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//#if !RELEASE || ENABLE_LOG
//    protected virtual void Update() {
//        m_unityAds.UpdateDebug();
//    }
//#endif
//}



///// <summary>広告失敗型（ジェネリックで取得できない為分離）</summary>
//public enum AdvertisementError {
//    Dissable,       // 広告が無効
//    NoInitialize,   // 未初期化
//    NoNetwork,      // 通信未接続
//    Downloading,    // 配信中
//}