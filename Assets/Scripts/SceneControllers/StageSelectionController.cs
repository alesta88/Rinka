using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// シーン選択画面
/// </summary>
public class StageSelectionController : MonoBehaviour, ISceneController {
    [SerializeField] Image m_backIcon;
    [SerializeField] Image m_skipAdsImage;
    [SerializeField] Image m_skinBtn;
    [Header("Stages")]
    [SerializeField] Transform m_stageButtonParent;
    [SerializeField] StageMetaData[] m_stageData;

    //***********************************************************
    // 初期化
    //***********************************************************
    void Start() {
      //alestaads  BindModelToUI();
        BindInput();
        BindInput2();
        Init();
    }

    void BindModelToUI() {
        GameModel.IsAdSkipPurchased
            .TakeUntilDestroy( this )
            .Subscribe( isAdSkipPurchased => m_skipAdsImage.gameObject.SetActive( !isAdSkipPurchased ) );
        m_skipAdsImage.gameObject.SetActive( !GameModel.IsAdSkipPurchased.Value );
    }

    void BindInput() {
        // 戻るボタンが押された
        m_backIcon.ObservableButton( this ).Subscribe( _ => GameModel.GameState.Value = Define.GameState.Title );
     //   m_skinBtn.ObservableButton(this).Subscribe(_ => GameModel.GameState.Value = Define.GameState.Skin);
     //alestaads   m_skipAdsImage.ObservableButton( this ).Subscribe( _ => RinkaPurchaseManager.Instance.Buy( ProductModel.Instance.ProductInfos[0] ) );
     
    }
    void BindInput2()
    {
        // Skinボタンが押された
        m_skinBtn.ObservableButton(this).Subscribe(_ => GameModel.GameState.Value = Define.GameState.Skin);
       // m_skipAdsImage.ObservableButton(this).Subscribe(_ => RinkaPurchaseManager.Instance.Buy(ProductModel.Instance.ProductInfos[0]));

    }
    void Init() {
        var buttons = m_stageButtonParent.GetComponentsInChildren<StageButton>();
        for( int i=0; i< buttons.Length; i++ ) {
            buttons[i].Init( m_stageData[i] );
        }

        AudioMgr.Instance.PlayTitleBgm();
    }

    //***********************************************************
    // シーン切替コールバック
    //***********************************************************
    public void OnSceneActive() {
    }

    public void OnSceneInactive() {
    }
}
