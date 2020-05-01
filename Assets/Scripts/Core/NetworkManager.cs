using UniRx;
using UnityEngine;

///====================================================================================================
/// <summary>
/// ■ 接続状態の管理クラス。
/// </summary>
///====================================================================================================
public class NetworkManager : MonoSingleton<NetworkManager> {
    ///------------------------------------------------------------------------------------------------
    /// ● 要素
    ///------------------------------------------------------------------------------------------------
    const float CHECK_CONECT_SECOND = 2;

    public ReactiveProperty<bool> isConnection { get; private set; } = new ReactiveProperty<bool>();
    float m_connectionSecond;
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 初期化
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    void Awake() {
        // 接続状態に遷移した場合、各管理クラスをリフレッシュ
        isConnection.DistinctUntilChanged().Subscribe( isConect => {
            if ( isConect ) {
                Debug.Log( "接続管理 : 接続時リフレッシュ" );
                RinkaAdvertisementManager.Instance.Refresh();
                // ここで課金クラスをリフレッシュすると
                // 登録されているeventが削除されて
                // 課金後のeventが呼び出されない可能性がある
                //RinkaPurchaseManager.Instance.Refresh();
            }
        } );
    }
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● 更新
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    void Update() {
        // 接続状態が一定秒無い場合、非接続とする
        m_connectionSecond =
            Application.internetReachability != NetworkReachability.NotReachable ?
            m_connectionSecond + Time.deltaTime : 0;
        m_connectionSecond = Mathf.Clamp( m_connectionSecond, 0, 100 );
        isConnection.Value = m_connectionSecond > CHECK_CONECT_SECOND;
    }
}