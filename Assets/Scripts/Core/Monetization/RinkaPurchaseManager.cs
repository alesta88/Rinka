//using System.Linq;
//using UnityEngine;

/////====================================================================================================
///// <summary>
///// ■ リンカの購入の管理クラス。
/////     購入管理クラスを継承する。
///// </summary>
/////====================================================================================================
//public class RinkaPurchaseManager : PurchaseManager<RinkaPurchaseManager> {
//    ///------------------------------------------------------------------------------------------------
//    /// ● 要素
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>全広告削除名</summary>
//    const string DELETE_ADVERTISEMENT_NAME = "全広告削除";

//    /// <summary>Androidの積載物</summary>
//    // ※ 非消費アイテムは、領収書が永遠に残る為、絶対に変えてはならない（前回購入の検証が通らなくなる）
//    protected override string androidPayload { get; } = "Please_play_Rinka_and_give_me_some_money.";
//    /// <summary>購入済か？</summary>
//    bool isPurchased { get { return GameModel.IsAdSkipPurchased.Value; } }
//    /// <summary>初期化した事にする？</summary>
//    public bool isPassInitialize { get; private set; }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 初期化済か？
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    public new bool IsInitialized() {
//        return base.IsInitialized() || isPassInitialize;
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 初期化
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    void Awake() {
//        // 情報管理クラスの商品情報で、初期化
//        Initialize(
//            ProductModel.Instance.ProductInfos.ToList<ProductInfo>(),
//            (error) => {
//                switch ( error ) {
//                    // 成功
//                    case PurchaseError.None:
//                        break;

//                    // 未接続失敗
//                    case PurchaseError.NetworkUnavailable:
//                        // 未購入の場合のみ、失敗通知
//                        if ( !isPurchased ) {
//                            DialogMgr.Instance.DisplayNoConnectionError();
//                        }
//                        isPassInitialize = true;    // 初期化した事にする
//                        break;

//                    // 購入未許可失敗
//                    case PurchaseError.PurchasingUnavailable:
//                        // 未購入の場合のみ、失敗通知
//                        if ( !isPurchased ) {
//                            DialogMgr.Instance.DisplayPurchaseSettingsError();
//                        }
//                        isPassInitialize = true;    // 初期化した事にする
//                        break;

//                    // 初期化失敗（謎）
//                    default:
//                        // 未購入の場合のみ、失敗通知
//                        if ( !isPurchased ) {
//                            DialogMgr.Instance.DisplayInitError();
//                        }
//                        isPassInitialize = true;    // 初期化した事にする
//                        break;
//                }
//            }
//        );
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● リフレッシュ
//    ///     場面切り替え時に毎回読むと、購入漏れを軽減できる。
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    public void Refresh() {
//        Refresh( (error) => {} );
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 再インストール処理
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    public void Restore() {
//        DialogMgr.Instance.SetScreenFilterActive( true );
//        Restore(
//            (error) => {
//                switch ( error ) {
//                    // 成功
//                    case PurchaseError.None:
//                        DialogMgr.Instance.DisplayRestoreSuccess();
//                        break;

//                    // 未接続失敗
//                    case PurchaseError.NetworkUnavailable:
//                        DialogMgr.Instance.DisplayNoConnectionError();
//                        break;

//                    // 初期化失敗
//                    case PurchaseError.NotInitialization:
//                        DialogMgr.Instance.DisplayInitError( () => Refresh() );
//                        break;

//                    // リストア失敗（謎）
//                    default:
//                        DialogMgr.Instance.DisplayRestoreError();
//                        break;
//                }
//                DialogMgr.Instance.SetScreenFilterActive( false );
//            }
//        );
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 購入
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    public void Buy( RinkaProductInfo info ) {
//        DialogMgr.Instance.SetScreenFilterActive( true );
//        Buy(
//            info,
//            (error) => {
//                switch ( error ) {
//                    // 成功
//                    case PurchaseError.None:
//                    case PurchaseError.UserCancelled:
//                        break;

//                    // 未接続失敗
//                    case PurchaseError.NetworkUnavailable:
//                        DialogMgr.Instance.DisplayNoConnectionError();
//                        break;

//                    // 購入未許可失敗
//                    case PurchaseError.PurchasingUnavailable:
//                        DialogMgr.Instance.DisplayPurchaseSettingsError();
//                        break;

//                    // 初期化失敗
//                    case PurchaseError.NotInitialization:
//                    case PurchaseError.Initializing:
//                        DialogMgr.Instance.DisplayInitError( () => Refresh() );
//                        break;

//                    // 購入失敗（謎）
//                    default:
//                        DialogMgr.Instance.DisplayUnknownPurchaseError();
//                        break;
//                }
//                DialogMgr.Instance.SetScreenFilterActive( false );
//            }
//        );
//    }
//    ///------------------------------------------------------------------------------------------------
//    /// <summary>
//    /// ● 購入済（呼戻）
//    ///     Buy()のコールバック以外にも、Initialize()の保留判定、Restore()からも呼ばれる。
//    /// </summary>
//    ///------------------------------------------------------------------------------------------------
//    public override void OnBought( ProductInfo info ) {
//        var rinkaInfo = (RinkaProductInfo)info;

//        if ( rinkaInfo.name == DELETE_ADVERTISEMENT_NAME ) {
//            GameModel.IsAdSkipPurchased.Value = true;
//        }

//        if ( BootLoader.IsDebugScene() ) {
//            OnBoughtDebug( rinkaInfo );
//        }
//    }
//    /// <summary>
//    /// ● 購入済（デバッグ）
//    /// </summary>
//    void OnBoughtDebug( RinkaProductInfo info ) {
//        // 永久購入商品対応ボタンの状態、動画状態を変更
//        if ( !info.name.Contains( "舞台" ) ) {
//            var b = GameObject.Find( info.name ).GetComponentInChildren<UnityEngine.UI.Button>();
//            b.interactable = false;

//            switch ( info.name ) {
//                case DELETE_ADVERTISEMENT_NAME:
//                    b = GameObject.Find( "ButtonSkip" ).GetComponentInChildren<UnityEngine.UI.Button>();
//                    b.interactable = false;
//                    b = GameObject.Find( "ButtonForced" ).GetComponentInChildren<UnityEngine.UI.Button>();
//                    b.interactable = false;
//                    break;
//            }
//        }
//    }
//}