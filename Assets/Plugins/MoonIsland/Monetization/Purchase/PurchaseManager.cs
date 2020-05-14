using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Purchasing.MiniJSON;

///====================================================================================================
/// <summary>
/// ■ 購入管理のクラス。
///     プロジェクトごとに継承して、継承先で色々設定して使う。
///     UnityIAPのラッパー。
/// 
///     パクリ元
/// 　  https://gist.github.com/YoshihideSogawa/f7c118127ce50e593a5b4a12e8426d6e
///     パクリ元のパクリ元（Unite2016TokyoWS01）
/// 　  https://github.com/unity3d-jp/Unite2016TokyoWS01/blob/master/Assets/Example/Scripts/Workshop/Purchaser.cs
/// 　  
///     -- 元のソースコードより --
///     非消費、サブスクリプション型は動作未検証です。
///     Android、iOSのみ動作検証しています。
///     INFO:   iOSでは、初期化後直ぐに、未購入商品の認証窓が表示される為、Initialize()呼び出しを調整する。
///     INFO:   初期化時に商品情報を設定する。
///     INFO:   Buy()、Restore()は、連打不可。（UI側で調整する。）
///     TODO:   エディタ上では、購入しない選択で、謎のエラーが出る。（実機ではキャンセルエラーが出る。）
/// </summary>
///====================================================================================================
public abstract class PurchaseManager<T> : MonoSingleton<T>, IStoreListener where T : MonoBehaviour {

    // 要素関連
#region Member
    ///------------------------------------------------------------------------------------------------
    /// ● 要素
    ///------------------------------------------------------------------------------------------------
    /// <summary>初期化状態</summary>
    enum InitializeState {
        NotInitialization,  // 初期化をまだ行っていない
        Initializing,       // 初期化中
        Successful,         // 初期化成功
        Failure,            // 初期化失敗
    }
    /// <summary>購入失敗状態</summary>
    protected enum PurchaseError {
        None,                       // エラー無
        NotInitialization,          // 課金システムが初期化されていない
        Initializing,               // 課金システムが初期化中
        UnknownItem,                // 販売されていないアイテムを指定した
        InvalidReceipt,             // 無効な領収書
        NotReceiveMessage,          // 課金メッセージを受け取れない場合（主にコールバックを設定していないミス）
        NetworkUnavailable,         // 通信不可状態（課金システムの初期化は完了）
        NotSupported,               // 非サポート（リストアの場合）
        Unknown,                    // 不明

        // UnityIAPのPurchaseFailureReasonをラッピング
        PurchasingUnavailable,      // 購入不可
        ExistingPurchasePending,    // 既存購入保留中
        ProductUnavailable,         // 商品利用不可
        SignatureInvalid,           // 署名無効
        UserCancelled,              // 利用者取消
        PaymentDeclined,            // 支払い拒否
        DuplicateTransaction,       // 重複取引

        // UnityIAPのInitializationFailureReasonをラッピング

        Available,        // 使用可能商品が無
        AppNotKnown,                // 知らないアプリ
        NoProductsAvailable,
    }
    /// <summary>ストア購入直前の失敗文一覧</summary>
    readonly Dictionary<PurchaseError, string> m_purchaseErrors = new Dictionary<PurchaseError, string>() {
        { PurchaseError.NotInitialization,  "課金システムが初期化されていない" },
        { PurchaseError.Initializing,       "課金システムが初期化中" },
        { PurchaseError.UnknownItem,        "販売されていないアイテムを指定した" },
        { PurchaseError.InvalidReceipt,     "領収書が無効" },
        { PurchaseError.NotReceiveMessage,  "課金メッセージを受け取れない場合\n" +
                                            "主にコールバックを設定していないミス" },
        { PurchaseError.NetworkUnavailable, "通信不可状態" },
        { PurchaseError.NotSupported,       "非サポート（リストアの場合）" },
        { PurchaseError.Unknown,            "不明なエラー" },

        // UnityIAPのPurchaseFailureReasonをラッピング（Unity公式資料情報）
        {
            PurchaseError.PurchasingUnavailable,
            "システムの購入機能が利用できません。\n" +
            "デバイス設定でアプリ内購入が無効になっています。" +
            "Unity IAP では、セキュリティ設定で無効であったり、" +
            "課金システムのライブラリが古くなっている可能性があります。"
        }, {
            PurchaseError.ExistingPurchasePending,
            "新たに購入をリクエストしましたが、すでに購入処理中でした。\n" +
            "これは現在 Google Play のみです。IStoreListener.ProcessPurchase や" +
            " IStoreListener.OnPurchaseFailed からのコールバックを受け取ったあとに" +
            "新しい購入をリクエストしてください。"
        }, {
            PurchaseError.ProductUnavailable,
            "ストアで購入できる商品ではありません。\n" +
            "ストア内の商品の定義を確認してください。"
        }, {
            PurchaseError.SignatureInvalid,
            "課金領収書のシグネチャ検証に失敗しました。"
        }, {
            PurchaseError.UserCancelled,
            "ユーザは購入の続行よりキャンセルを選びました。\n" +
            "これはキャンセルを識別できないプラットフォーム（Amazon、Microsoft）では指定されません。"
        }, {
            PurchaseError.PaymentDeclined,
            "支払いに問題がありました。\n" +
            "これはAppleプラットフォーム固有です。"
        }, {
            PurchaseError.DuplicateTransaction,
            "取引がすでに正常に完了した場合、二重取引失敗。\n" +
            "このエラーは、ユーザーがログインしている間に生成された領収書を使用して" +
            "App Storeからログアウトしている間に、トランザクションが正常に終了すると" +
            "Appleプラットフォームで発生する可能性があります。"

        // UnityIAPのInitializationFailureReasonをラッピング
        }, {
            PurchaseError.NoProductsAvailable,
            "購入可能な商品はありません。"
        }, {
            PurchaseError.AppNotKnown,
            "この店はこのアプリを未知数と報告した。"
        }
    };
    /// <summary>Unity購入失敗から共通失敗への変換辞書</summary>
    readonly Dictionary<PurchaseFailureReason, PurchaseError> m_unityPurchaseErrorToPurchaseError =
        new Dictionary<PurchaseFailureReason, PurchaseError>()
    {
        { PurchaseFailureReason.PurchasingUnavailable,      PurchaseError.PurchasingUnavailable },
        { PurchaseFailureReason.ExistingPurchasePending,    PurchaseError.ExistingPurchasePending },
        { PurchaseFailureReason.ProductUnavailable,         PurchaseError.ProductUnavailable },
        { PurchaseFailureReason.SignatureInvalid,           PurchaseError.SignatureInvalid },
        { PurchaseFailureReason.UserCancelled,              PurchaseError.UserCancelled },
        { PurchaseFailureReason.PaymentDeclined,            PurchaseError.PaymentDeclined },
        { PurchaseFailureReason.DuplicateTransaction,       PurchaseError.DuplicateTransaction },
        { PurchaseFailureReason.Unknown,                    PurchaseError.Unknown },
    };
    /// <summary>Unity初期化失敗から共通失敗への変換辞書</summary>
    readonly Dictionary<InitializationFailureReason, PurchaseError> m_unityInitializeErrorToPurchaseError =
        new Dictionary<InitializationFailureReason, PurchaseError>()
    {
        { InitializationFailureReason.PurchasingUnavailable,    PurchaseError.PurchasingUnavailable },
        { InitializationFailureReason.NoProductsAvailable,      PurchaseError.NoProductsAvailable },
        { InitializationFailureReason.AppNotKnown,              PurchaseError.AppNotKnown },
    };
    /// <summary>店舗制御の参照</summary>
    IStoreController m_storeController;
    /// <summary>拡張時の店舗提供者の参照</summary>
	IExtensionProvider m_storeExtensionProvider;
    /// <summary>初期化状態</summary>
    InitializeState m_initialize = InitializeState.NotInitialization;
    /// <summary>購入保留中商品一覧（鍵は何でも可）</summary>
    readonly Dictionary<string, Product> m_pendingProducts = new Dictionary<string, Product>();
    /// <summary>商品情報</summary>
    List<ProductInfo> m_productInfos = new List<ProductInfo>();
    /// <summary>Androidの積載物</summary>
    protected abstract string androidPayload { get; }
    /// <summary>生領収書への参照</summary>
    IAppleConfiguration m_appleConfig;
    /// <summary>失敗時の呼戻変数</summary>
    Action<PurchaseError> m_onError;
#endregion

    // 初期化関連
#region Initialize
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 初期化済か？
    /// </summary>
    /// <returns><c>true</c> なら初期化済み、<c>false</c>なら未初期化</returns>
    ///------------------------------------------------------------------------------------------------
    public bool IsInitialized() {
        // 両方の購買参照が設定されている場合、初期化済
        return m_storeController != null && m_storeExtensionProvider != null;
    }
    ///------------------------------------------------------------------------------------------------
    /// ● 初期化
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 初期化
    ///     継承先で必ず呼びなさい。
    ///     
    ///     UnityIAPの初期化処理と商品リストの登録を行う。
    ///     処理が完了したら onInitialized もしくは onInitializeFailed がコールされる。
    ///     ただし、すでにIAPの初期化が完了している場合はコールされない。
    /// </summary>
    /// <param name="productInfos">登録する課金アイテムのリスト。Key名はフロント側で識別可能であれば何でもOK</param>
    protected void Initialize( List<ProductInfo> productInfos, Action<PurchaseError> onError ) {
        m_onError = null;   // 前回呼戻リセット

        // 未初期化の場合のみ、初期化
        if ( !IsInitialized() ) {
            m_onError = onError;            // 呼戻登録
            m_productInfos = productInfos;  // 商品情報を保存

            // 通信不可の場合、未処理
            if ( !IsNetworkConnection() ) {
                ErrorLog( PurchaseError.NetworkUnavailable );
                return;
            }

            // 初期化状態を変更
            m_initialize = InitializeState.Initializing;

            // Unityの課金システム構築
            var module = StandardPurchasingModule.Instance();
            
//#if UNITY_EDITOR
//            module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
//#endif //alestaios
            var builder = ConfigurationBuilder.Instance( module );
            // 商品リストを ConfigurationBuilder に登録
            foreach ( var info in m_productInfos ) {
                info.SetProductId( builder );
            }
            // IAppleConfigurationへの参照を取得
            m_appleConfig = builder.Configure<IAppleConfiguration>();

            // 非同期の課金処理の初期化を開始
            UnityPurchasing.Initialize( this, builder );
            Debug.Log( "課金 : 初期化挑戦中" );
        }
    }
    /// <summary>
    /// ● 初期化完了（呼戻）
    ///     初期化終了後、UnityEngineから呼ばれます。
    /// </summary>
    public void OnInitialized( IStoreController controller, IExtensionProvider extensions ) {
        Debug.Log( "課金 : 初期化成功" );
        m_storeController = controller;
        m_storeExtensionProvider = extensions;
        m_initialize = InitializeState.Successful;

        // Pending状態のままの商品があればPendingリストに登録
        foreach ( var product in GetProducts() ) {
#if false
            Debug.Log(
                $"receipt {product.definition.id} :" +
                    $"{ (string.IsNullOrEmpty( product.receipt ) ? "無" : "存在") }\n" +
                $"transactionID {product.definition.id} : " +
                    $"{ (string.IsNullOrEmpty( product.transactionID ) ? "無" : "存在") }\n"
            );
#endif
            // 領収書が残ったままの商品はPending状態
            if ( product.hasReceipt || product.receipt != null ) {
                UpdatePendingProduct( product.transactionID, product, PurchaseProcessingResult.Pending );
            }

            // デバッグ用
            if ( product.hasReceipt == false && product.receipt != null ) {
                Debug.LogWarning(
                    "product.hasReceipt == false && " +
                    "product.receipt != null\nhasReceiptがfalseなのに領収書が残ってる……？"
                );
            }
        }

        // リフレッシュ
        Refresh( m_onError );
    }
    /// <summary>
    /// ● 初期化失敗（呼戻）
    ///     初期化失敗後、UnityEngineから呼ばれます。
    /// </summary>
    public void OnInitializeFailed( InitializationFailureReason error ) {
        Debug.LogError( "課金 : 初期化失敗" );
        m_initialize = InitializeState.Failure;
        ErrorLog(error);
    }
#endregion

    // リフレッシュ関連
#region Refresh
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● リフレッシュ
    ///     場面切り替え時に、毎回読むと良い。
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    protected void Refresh( Action<PurchaseError> onError ) {
        // 初期化中は何もしない
        if ( m_initialize == InitializeState.Initializing ) {
            return;
        }

        m_onError = onError;    // 呼戻登録

        // 通信不可の場合、未処理
        if ( !IsNetworkConnection() ) {
            ErrorLog( PurchaseError.NetworkUnavailable );
            return;
        }
        // 初期化されていない場合、初期化のみ行う
        if ( !IsInitialized() ) {
            Initialize( m_productInfos, onError );
            ErrorLog( PurchaseError.NotInitialization );
            return;
        }

        // 支払い途中の商品をチェックする
        var pendings = GetPendingProducts();
        if ( pendings.Length == 0 ) {
            Debug.Log( "保留中商品なし" );
            return;
        }
        // Pending状態の商品をすべて処理する
        foreach ( var product in pendings ) {
            Debug.Log(
                "保留中商品\n" +
                $"{DataDumpUtil.GetDumpStr( product, DataDumpOption.DeepDump )}"
            );
            ValidateReceipt( product );
        }
    }
#endregion

    // 商品情報関連
#region ProductInfo
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 購入保留中商品を更新
    /// </summary>
    /// <param name="transactionId">IAPのトランザクションID</param>
    /// <param name="product">アイテム.</param>
    /// <param name="result">PurchaseProcessingResult.</param>
    ///------------------------------------------------------------------------------------------------
    void UpdatePendingProduct( string transactionId, Product product, PurchaseProcessingResult result ) {
        // 領収書を持っていない場合は何もしない
        if ( !product.hasReceipt ) {
            return;
        }
        // transactionIdがnull
        if ( string.IsNullOrEmpty( transactionId ) ) {
            Debug.LogError(
                "UpdatePendingProduct() で tranzactionId が無\n" +
                $"{DataDumpUtil.GetDumpStr( product, "Product", DataDumpOption.DeepDump )}"
            );
            return;
        }

        // Pendingの場合は最新のものに更新
        if ( result == PurchaseProcessingResult.Pending ) {
            if ( m_pendingProducts.ContainsKey( transactionId ) ) {
                m_pendingProducts.Remove( transactionId );
            }
            m_pendingProducts.Add( transactionId, product );
        } else if ( result == PurchaseProcessingResult.Complete ) {
            // 完了した場合は削除
            if ( m_pendingProducts.ContainsKey( transactionId ) ) {
                m_pendingProducts.Remove( transactionId );
            }
        }
    }
    ///------------------------------------------------------------------------------------------------
    /// ● 商品情報を取得
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 全購入保留中商品を取得
    /// </summary>
    protected Product[] GetPendingProducts() {
        var products = new Product[0];

        // 初期化済で、商品が存在する場合、商品一覧を複製
        if ( IsInitialized() && m_pendingProducts.Count > 0 ) {
            products = new Product[m_pendingProducts.Values.Count];
            m_pendingProducts.Values.CopyTo( products, 0 );
        }
        return products;
    }
    /// <summary>
    /// ● 商品情報を取得
    /// </summary>
    /// <param name="unityProductId">Unityで扱うアイテムID.</param>
    Product GetProduct( string unityProductId ) {
        // 初期化済の場合のみ、商品を返す
        if ( IsInitialized() ) {
            return m_storeController.products.WithID( unityProductId );
        }
        return null;
    }
    /// <summary>
    /// ● 全商品情報を取得
    /// </summary>
    protected Product[] GetProducts() {
        // 初期化済の場合のみ、商品一覧を返す
        if ( IsInitialized() ) {
            return m_storeController.products.all;
        }
        return new Product[0];
    }
#endregion

    // 購入関連
#region Purchase
    ///------------------------------------------------------------------------------------------------
    /// ● 購入
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 購入
    /// </summary>
    protected void Buy( ProductInfo info, Action<PurchaseError> onError ) {
        m_onError = onError;    // 呼戻登録

        // UnityIAP購入処理
        var result = BuySub( info.name );
        // 無失敗以外の場合、ログ表示（無の場合は、購入後の呼戻関数に任せる）
        if ( result != PurchaseError.None ) {
            ErrorLog( result );
        }
    }
    /// <summary>
    /// ● 購入（補助）
    /// </summary>
    PurchaseError BuySub( string productKey ) {
        // アプリが強制終了しても処理を続行する
        try {
            // 通信不可の場合は何もしない（初期化は必ず終了している）
            if ( !IsNetworkConnection() ) {
                return PurchaseError.NetworkUnavailable;
            }

            // 課金システムが未初期化の場合はなにもしない
            if ( !IsInitialized() ) {
                switch ( m_initialize ) {
                    case InitializeState.NotInitialization:
                    case InitializeState.Failure:
                                                            return PurchaseError.NotInitialization;
                    case InitializeState.Initializing:      return PurchaseError.Initializing;
                    default:                                return PurchaseError.Unknown;
                }
            }

            var product = m_storeController.products.WithID( productKey );

            // 購入できないアイテムの場合
            if ( product == null || !product.availableToPurchase ) {
                Debug.LogError(
                    "アイテムが販売していません。\n" +
                    $"product : {product}\n" +
                    $"availableToPurchase : {product.availableToPurchase}\n"
                );
                return PurchaseError.UnknownItem;
            }

            // Androidの場合は必ず積載物を送る
#if UNITY_ANDROID && !UNITY_EDITOR
            if ( androidPayload == null ) {
                Debug.LogError( "androidPayload が無" );
                return PurchaseError.NotReceiveMessage;
            }
            // 実機で必ず警告が出るが、UnityEngineのバグなので、問題ないらしい
            // https://forum.unity.com/threads/solved-initiatepurchase-with-developerpayload-on-iap-1-17-0.523945/
            m_storeController.InitiatePurchase( product, androidPayload );
#else
            m_storeController.InitiatePurchase( product );
#endif
            // 成功
            return PurchaseError.None;


        // 何らかのエラーが発生（課金処理は未発生）
        } catch ( Exception ) {
            return PurchaseError.Unknown;
        }
    }
    /// <summary>
    /// ● 購入手続（呼戻）
    ///     購入成功後、UnityEngineから呼ばれます。
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase( PurchaseEventArgs args ) {
        var unityProductId = args.purchasedProduct.definition.id;
        var product = args.purchasedProduct;

        // 通信前提のため一度Pendingに追加する
        UpdatePendingProduct( product.transactionID, product, PurchaseProcessingResult.Pending );

        // 初期化時に登録されていないアイテムの場合（アプリの不具合・サーバの設定ミス等）
        if ( unityProductId == null ) {
            ErrorLog( PurchaseFailureReason.ProductUnavailable );
            return PurchaseProcessingResult.Pending;
        }

        // アプリの強制終了にも耐えうるようにここで処理
        try {
            // アイテムの購入完了処理
            // 未登録のアイテムの除外はしていない->
            // 過去に購入した現在は販売していないアイテムが未消費の可能性があるため
            ValidateReceipt( product );  // 領収書検証

        } catch ( Exception ) {
            // 不明なエラーが発生（成功のコールバックで強制終了している場合もここで通知されるので、領収書の有無で判断する）
            ErrorLog( PurchaseFailureReason.Unknown );
        }

        return PurchaseProcessingResult.Pending;
    }
    /// <summary>
    /// ● 購入失敗（呼戻）
    ///     購入失敗後、UnityEngineから呼ばれます。
    /// </summary>
    public void OnPurchaseFailed( Product product, PurchaseFailureReason failureReason ) {
        Debug.LogError( "購入失敗 : " + product.definition.id );
        ErrorLog( failureReason );
    }
    ///------------------------------------------------------------------------------------------------
    /// ● 購入完了
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 購入完了
    /// </summary>
    void CompletePurchase( Product product, List<string> ids ) {
        // 通信不可の場合は消費しない
        if ( !IsNetworkConnection() ) {
            ErrorLog( PurchaseError.NetworkUnavailable );
            return;
        }
        // 未初期化の場合は、未処理
        if ( !IsInitialized() ) {
            ErrorLog( PurchaseError.NotInitialization );
            Debug.Log("Not ini"); //alesta
            return;
        }

        // 完了の通知とPendingアイテム情報の更新
        // たとえUpdatePendingProductで消費できなくとも、初期化後にまたアイテム情報がやってくる
        m_storeController.ConfirmPendingPurchase( product );
        UpdatePendingProduct( product.transactionID, product, PurchaseProcessingResult.Complete );

        foreach ( var info in m_productInfos ) {
            if ( ids.Contains( info.playStoreId ) || ids.Contains( info.appStoreId ) ) {
                Debug.Log( $"購入成功 : {info.name}" );
                OnBought( info );
            }
        }

        ErrorLog( PurchaseError.None );
    }
    /// <summary>
    /// ● 購入済（呼戻）
    /// </summary>
    public abstract void OnBought( ProductInfo info );
#endregion

    // 領収書関連
#region Receipt
    ///------------------------------------------------------------------------------------------------
    /// ● 領収書検証
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 領収書検証
    /// </summary>
    void ValidateReceipt( Product product ) {
        // エディタ上は、処理を飛ばす
#if UNITY_EDITOR
        // 購入完了処理
        var id = m_productInfos
            .Where( info => info.name == product.definition.id )
            .Select( info => info.playStoreId )
            .ToArray()[0];
        CompletePurchase( product, new List<string> { id } );
        return;
#endif

        // UnityIAP検証は、Android、iOS、Macのみ対応
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        var ids = new List<string>();

        // エディタ上の検証ウィンドウ設定を準備
        var validator = new CrossPlatformValidator(
            GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier );

        try {
#if UNITY_ANDROID
            // GooglePlayStoreの場合、積荷を取得
            var payload = product.receipt;
            var path = "Payload/json/developerPayload/developerPayload";    // 領収書payload階層が深い
            var keys = path.Split( '/' );
            foreach ( var key in keys ) {
                var json = (Dictionary<string, object>)Json.Deserialize( payload );
                payload = (string)json[key];
            }
            // Base64形式から変換
            var bytes = Convert.FromBase64String( payload );
            payload = System.Text.Encoding.UTF8.GetString( bytes );
            Debug.Log( $"androidPayload : {payload}" );
#endif

            // GooglePlayは1つの商品が、AppStoreは複数の商品が含まれている為、走査
            var result = validator.Validate( product.receipt );
            foreach ( var receipt in result ) {
//                DebugViewReceipt( receipt );    // 領収書をログ表示

                // GooglePlayStore領収書判定
                if ( receipt is GooglePlayReceipt ) {
                    var google = (GooglePlayReceipt)receipt;
                    // 識別番号、パッケージ名、購入状態を比較し、整合性を判定
                    if (    m_productInfos.Any( info => info.playStoreId == google.productID ) &&
                            google.packageName == Application.identifier &&
                            google.purchaseState == GooglePurchaseState.Purchased
#if UNITY_ANDROID
                            && payload == androidPayload
#endif
                    ) {
                        ids.Add( receipt.productID );   // 購入番号を追加
                        Debug.Log( $"領収書検証成功 : {receipt.productID}" );
                    }

                // AppStore領収書判定
                } else if ( receipt is AppleInAppPurchaseReceipt ) {
                    var apple = (AppleInAppPurchaseReceipt)receipt;
                    // 生領収書の抽出
                    var receiptData = Convert.FromBase64String( m_appleConfig.appReceipt );
                    var appleRaw = new AppleValidator( AppleTangle.Data() ).Validate( receiptData );

                    // 識別番号、パッケージ名、購入状態を比較し、整合性を判定
                    if (    m_productInfos.Any( info => info.appStoreId == apple.productID ) &&
                            appleRaw.bundleID == Application.identifier ) {
                        ids.Add( receipt.productID );   // 購入番号を追加
                        Debug.Log( $"領収書検証成功 : {receipt.productID}" );
                    }
                }
            }
            // 購入完了処理
            CompletePurchase( product, ids );

        } catch ( IAPSecurityException ) {
            Debug.LogError(
                "無効な領収書で、コンテンツのロックを解除していません。\n" +
                "領収書検証に失敗\n" +
                $"{DataDumpUtil.GetDumpStr( product, DataDumpOption.DeepDump )}"
            );
            ErrorLog( PurchaseError.InvalidReceipt );
        }
#endif
    }

    /// <summary>
    /// ● 領収書を表示（デバッグ）
    /// </summary>
    void DebugViewReceipt( IPurchaseReceipt receipt ) {
        if ( receipt is GooglePlayReceipt ) {
            var google = (GooglePlayReceipt)receipt;
            Debug.Log(
                $"GooglePlayStore領収書 : \n" +

                // GooglePlayStoreでテスト時、sandboxは注文番号を発行しない為、無になる
                $"transactionID : {google.transactionID}\n" +

                $"productID : {google.productID}\n" +
                $"purchaseDate : {google.purchaseDate}\n" +
                $"\n" +
                $"packageName : {google.packageName}\n" +
                $"purchaseToken : {google.purchaseToken}\n" +
                $"purchaseState : {google.purchaseState}\n"
            );

        } else if ( receipt is AppleInAppPurchaseReceipt ) {
            var apple = (AppleInAppPurchaseReceipt)receipt;
            // 生領収書
            var receiptData = Convert.FromBase64String( m_appleConfig.appReceipt );
            var appleRaw = new AppleValidator( null).Validate(receiptData);//AppleTangle.Data() ).Validate( receiptData );

            Debug.Log(
                $"AppStore領収書 : \n" +
                $"transactionID : {apple.transactionID}\n" +
                $"productID : {apple.productID}\n" +
                $"purchaseDate : {apple.purchaseDate}\n" +
                $"\n" +
                $"quantity : {apple.quantity}\n" +
                $"originalTransactionIdentifier : {apple.originalTransactionIdentifier}\n" +
                $"originalPurchaseDate : {apple.originalPurchaseDate}\n" +
                $"subscriptionExpirationDate : {apple.subscriptionExpirationDate}\n" +
                $"cancellationDate : {apple.cancellationDate}\n" +
                $"isFreeTrial : {apple.isFreeTrial}\n" +
                $"productType : {apple.productType}\n" +
                $"isIntroductoryPricePeriod : {apple.isIntroductoryPricePeriod}\n" +
                $"\n" +
                $"AppStore生領収書 : \n" +
                $"bundleID : {appleRaw.bundleID}\n" +
                $"appVersion : {appleRaw.appVersion}\n" +
                $"expirationDate : {appleRaw.expirationDate}\n" +
                $"opaque : {string.Join( "", appleRaw.opaque.Select( b => b.ToString() ) )}\n" +
                $"hash : {string.Join( "", appleRaw.hash.Select( b => b.ToString() ) )}\n" +
                $"originalApplicationVersion : {appleRaw.originalApplicationVersion}\n" +
                $"receiptCreationDate : {appleRaw.receiptCreationDate}\n"
            );
        }
    }
    #endregion

    // 再インストール関連
    #region Restore
    ///------------------------------------------------------------------------------------------------
    /// ● 再インストール
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 再インストール
    ///     iOSXのみ、リストア処理を行う。
    /// </summary>
    protected virtual void Restore( Action<PurchaseError> onError ) {
        Debug.Log( "リストア開始" );

        m_onError = onError;    // 呼戻登録

#if UNITY_IOS || UNITY_STANDALONE_OSX
        // 通信不可の場合は何もしない
        if ( !IsNetworkConnection() ) {
            ErrorLog( PurchaseError.NetworkUnavailable );
            return;
        }
        // 初期化されていない場合は何もしない
        if ( !IsInitialized() ) {
            ErrorLog( PurchaseError.NotInitialization );
            return;
        }

        // リストア処理
        var apple = m_storeExtensionProvider.GetExtension<IAppleExtensions>();
        apple.RestoreTransactions( ( result ) => {
            // 復元失敗の場合、謎の失敗で終了
            if ( !result ) {
                ErrorLog( PurchaseError.Unknown );
                return;
            }
            // 消費型アイテムの場合は自動で購入処理が呼ばれないので、呼び出し側から購入処理を行ってもらう
            var pendingProducts = GetPendingProducts();
            // 商品が無の場合、無失敗で終了
            if ( pendingProducts.Length == 0 ) {
                ErrorLog( PurchaseError.None );
                return;
            }
            // 商品が存在する場合
            foreach ( var p in pendingProducts ) {
                if ( p.hasReceipt ) {
                    ValidateReceipt( p );   // 領収書検証
                }
            }
        } );
#else
        // iPhone/OSXでない場合、サポートしない
        ErrorLog( PurchaseError.NotSupported );
#endif
    }
    #endregion

    // 失敗関連
    #region Error
    ///------------------------------------------------------------------------------------------------
    /// ● 購入失敗文を表示
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 購入失敗文を表示（内部初期化失敗）
    /// </summary>
    void ErrorLog( InitializationFailureReason error ) {
        ErrorLog( m_unityInitializeErrorToPurchaseError[error] );
    }
    /// <summary>
    /// ● 購入失敗文を表示（内部購入失敗）
    /// </summary>
    void ErrorLog( PurchaseFailureReason error ) {
        ErrorLog( m_unityPurchaseErrorToPurchaseError[error] );
    }
    /// <summary>
    /// ● 購入失敗文を表示
    /// </summary>
    void ErrorLog( PurchaseError error ) {
        // 失敗文が存在する場合、ログ表示
        if ( m_purchaseErrors.ContainsKey( error ) ) {
            Debug.LogError( "課金失敗 : " + error + "\n" + m_purchaseErrors[error] );
        }
        // 失敗呼戻関数を実行
        m_onError?.Invoke( error );
        m_onError = null;
    }
    #endregion

    // その他関連
#region Other
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 通信接続中か？
    /// </summary>
    /// <returns><c>true</c>の場合は通信接続がある</returns>
    ///------------------------------------------------------------------------------------------------
    bool IsNetworkConnection() {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
#endregion
}