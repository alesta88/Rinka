//using System;
//using UniRx;
//using MoonIsland;

/////====================================================================================================
///// <summary>
///// ■ Rinka広告の管理クラス。
///// </summary>
/////====================================================================================================
//public class RinkaAdvertisementManager : AdvertisementManager<RinkaAdvertisementManager> {
//    ///------------------------------------------------------------------------------------------------
//    /// ● 要素
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>初期化済か？</summary>
//    public new bool isInitialized {
//        get { return base.isInitialized || isPassInitialize; }
//    }
//    /// <summary>初期化した事にする？</summary>
//    public bool isPassInitialize { get; private set; }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 初期化
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    void Awake() {
//        // 失敗通知のゲーム情報伝搬を設定
//        this
//            .ObserveEveryValueChanged( this_ => this_.isError )
//            .DistinctUntilChanged()
//            .Subscribe( is_ => {
//                GameModel.IsAdError.Value = is_;
//            } );

//        // 初期化
//        Initialize(
//            ( error ) => {
//                switch ( error ) {
//                    // 広告無効の場合、初期化した事にする
//                    case AdvertisementError.Dissable:
//                        isPassInitialize = true;
//                        break;

//                    // 通信未接続失敗
//                    case AdvertisementError.NoNetwork:
//                        DialogMgr.Instance.DisplayNoConnectionError();
//                        isPassInitialize = true;
//                        break;

//                    // 未初期化失敗
//                    case AdvertisementError.NoInitialize:
//                        DialogMgr.Instance.DisplayInitError();
//                        isPassInitialize = true;
//                        break;
//                }
//            }
//        );
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● リフレッシュ
//    ///     場面切り替えごとに呼ぶと、初期化失敗の可能性が下がる
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    public void Refresh() {
//        Refresh( ( error ) => {} );
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 動画再生
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    public void PlayMovie( bool isSkip, Action onFinish = null, Action onSkip = null, Action onError = null ) {
//        PlayMovie(
//            isSkip,
//            onFinish,
//            onSkip,
//            ( error ) => {
//                switch ( error ) {
//                    // 広告無効は成功とする
//                    case AdvertisementError.Dissable:
//                        onFinish?.Invoke();
//                        break;

//                    // 通信未接続失敗
//                    case AdvertisementError.NoNetwork:
//                        DialogMgr.Instance.DisplayNoConnectionError();
//                        onError?.Invoke();
//                        break;

//                    // 未初期化失敗
//                    case AdvertisementError.NoInitialize:
//                        DialogMgr.Instance.DisplayInitError(
//                            () => {
//                                Refresh();
//                                onError?.Invoke();
//                            }
//                        );
//                        break;
//                    case AdvertisementError.Downloading:
//                        DialogMgr.Instance.DisplayAdvertisementDownloadingError();
//                        onError?.Invoke();
//                        break;
//                }
//            }
//        );
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 更新
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//#if !RELEASE || ENABLE_LOG
//    protected override void Update() {
//        base.Update();

//        // 軽量化の為、stringを可能な限り分離
//        const string text = "広告失敗中 : {0}";
//        DebugDisplay.Instance.Add( string.Format( text, GameModel.IsAdError.Value ) );
//    }
//#endif
//}