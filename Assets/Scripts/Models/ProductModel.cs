using System.Collections.Generic;
using UnityEngine.Purchasing;

///====================================================================================================
/// <summary>
/// ■ 商品の管理クラス。
/// </summary>
///====================================================================================================
public class ProductModel : MoonIsland.Singleton<ProductModel> {
    ///------------------------------------------------------------------------------------------------
    /// ● 要素
    ///------------------------------------------------------------------------------------------------
    /// <summary>商品情報一覧</summary>
    public readonly List<RinkaProductInfo> ProductInfos = new List<RinkaProductInfo>();
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 初期化
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    public ProductModel() {
        SetPurchaseInfos(); // 商品情報一覧を設定
    }
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 商品情報一覧を設定
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    void SetPurchaseInfos() {
        // 偽情報設定
        ProductInfos.Add(
            new RinkaProductInfo(
                type:           ProductType.NonConsumable,
                playStoreId:    "delete_advertisement",
                appStoreId:     "ios_delete_advertisement",
                name:           "全広告削除"
            )
        );
#if !RELEASE
        ProductInfos.Add(
            new RinkaProductInfo(
                type:           ProductType.NonConsumable,
                playStoreId:    "delete_advertisement_2",
                appStoreId:     "delete_advertisement_2",
                name:           "全広告削除_2"
            )
        );
        ProductInfos.Add(
            new RinkaProductInfo(
                type: ProductType.NonConsumable,
                playStoreId:    "delete_advertisement_3",
                appStoreId:     "delete_advertisement_3",
                name:           "全広告削除_3"
            )
        );
        ProductInfos.Add(
            new RinkaProductInfo(
                type: ProductType.NonConsumable,
                playStoreId:    "delete_advertisement_4",
                appStoreId:     "delete_advertisement_4",
                name:           "全広告削除_4"
            )
        );
        ProductInfos.Add(
            new RinkaProductInfo(
                type: ProductType.NonConsumable,
                playStoreId:    "delete_advertisement_5",
                appStoreId:     "delete_advertisement_5",
                name:           "全広告削除_5"
            )
        );
        ProductInfos.Add(
            new RinkaProductInfo(
                type:           ProductType.Consumable,
                playStoreId:    "release_stage_1",
                appStoreId:     "release_stage_1",
                name:           "舞台1解放"
            )
        );
        ProductInfos.Add(
            new RinkaProductInfo(
                type:           ProductType.Consumable,
                playStoreId:    "release_stage_2",
                appStoreId:     "release_stage_2",
                name:           "舞台2解放"
            )
        );
#endif
    }
}