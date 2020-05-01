using System;
using UnityEngine;
using UnityEngine.Purchasing;

///====================================================================================================
/// <summary>
/// ■ 商品情報のクラス。
///     プロジェクトごとに継承して、継承先で色々設定して使う。
/// </summary>
///====================================================================================================
[Serializable]
public abstract class ProductInfo {
    ///------------------------------------------------------------------------------------------------
    /// ● 要素
    ///------------------------------------------------------------------------------------------------
    /// <summary>製品タイプ(消費型、非消費型、定期購入型)</summary>
    public ProductType type { get; private set; }
    /// <summary>Android用のProductID</summary>
    public string playStoreId { get; private set; }
    /// <summary>iOS用のProductID</summary>
    public string appStoreId { get; private set; }
    /// <summary>名前（鍵に使用）</summary>
    public string name { get; private set; }
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● コンストラクタ
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    protected ProductInfo( ProductType type, string playStoreId, string appStoreId, string name ) {
        this.type = type;
        this.playStoreId = playStoreId;
        this.appStoreId = appStoreId;
        this.name = name;
    }
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 商品番号を追加
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    public void SetProductId( ConfigurationBuilder builder ) {
        if ( builder == null ) {
            Debug.LogErrorFormat( "ConfigurationBuilder が無" );
            return;
        }

        var ids = new IDs();

        // iOS : AppleAppStore
        if ( !string.IsNullOrEmpty( appStoreId ) ) {
            ids.Add( appStoreId, new string[] { AppleAppStore.Name } );
        }
        // Android : GooglePlay
        if ( !string.IsNullOrEmpty( playStoreId ) ) {
            ids.Add( playStoreId, new string[] { GooglePlay.Name } );
        }

        // ConfigurationBuilderに追加
        builder.AddProduct( name, type, ids );
    }
}