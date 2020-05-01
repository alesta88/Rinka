using UnityEngine;
using UnityEngine.UI;

public class TestPurchase : MonoBehaviour {

    [SerializeField] Button m_buttonPrefab;

    void Awake() {
        DialogMgr.Instance.Init( SystemLanguage.Japanese );
        GameModel.Init();
        var i = NetworkManager.Instance;
    }

    void Start() {
        // 広告初期化
        var a = RinkaAdvertisementManager.Instance;

        var buttons = GetComponentsInChildren<Button>();
        foreach ( var b in buttons ) {
            b.onClick.AddListener( () => {
                switch ( b.name ) {
                    case "ButtonSkip":
                        RinkaAdvertisementManager.Instance.PlayMovie( true );
                        break;
                    case "ButtonForced":
                        RinkaAdvertisementManager.Instance.PlayMovie( false );
                        break;
                }
            } );
        }


        // 課金初期化
        var p = RinkaPurchaseManager.Instance;

        foreach ( var info in ProductModel.Instance.ProductInfos ) {
            var go = Instantiate( m_buttonPrefab.gameObject, transform );
            go.SetActive( true );
            go.name = info.name;
            go.GetComponentInChildren<Text>().text = info.name;
            go.GetComponentInChildren<Button>().onClick
                .AddListener( () => RinkaPurchaseManager.Instance.Buy( info ) );
        }
        // リストアボタン作成
        {
            var go = Instantiate( m_buttonPrefab.gameObject, transform );
            go.SetActive( true );
            go.name = "リストア";
            go.GetComponentInChildren<Text>().text = go.name;
            go.GetComponentInChildren<Button>().onClick
                .AddListener( () => RinkaPurchaseManager.Instance.Restore() );
        }
    }

    void Update() {
        if ( Input.GetKeyDown( KeyCode.Return ) ) {
            PlayerPrefs.DeleteAll();
            var i = PlayerPrefsUtil.GetBool( Define.PlayerPref.IS_AD_SKIP_PURCHASED );
            Debug.LogError( $"保存データ削除 : 広告 : {i}" );
        }
    }
}