using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

/// <summary>
/// Continue画面
/// </summary>
public class ContinueSceneController : MonoBehaviour, ISceneController {
    [SerializeField] GameObject m_playAdBtn;
    [SerializeField] GameObject m_purchaseAdSkipBtn;
    [SerializeField] GameObject m_continueBtn;
    [SerializeField] GameObject m_closeContinueWindowArea;
    [SerializeField] Sprite m_playWithAdImage;
    [SerializeField] Sprite m_errorAdImage;
    Image m_playAdImage;



    [SerializeField] TMP_Text m_scoreText;

    //***********************************************************
    // 初期化
    //***********************************************************
    void Awake() {
        m_playAdImage = m_playAdBtn.GetComponent<Image>();
    }

    void OnEnable() {
        m_scoreText.text = GameModel.Score.Value.ToString();
        BindInput();
        BindModelToUI();
        UpdateUI();
    }

    //***********************************************************
    // Input設定
    //***********************************************************
    void BindInput() {
        // 戻るボタンが押された
        m_closeContinueWindowArea.ObservableButton( this ).Subscribe( _ => GameModel.GameState.Value = Define.GameState.GameOver );
        //alestaads2  m_purchaseAdSkipBtn.ObservableButton( this ).Subscribe( _ => RinkaPurchaseManager.Instance.Buy( ProductModel.Instance.ProductInfos[0] ) );
        m_playAdBtn.ObservableButton( this ).Subscribe( _ => GameModel.GameState.Value = Define.GameState.PlayContinueAd );
        m_continueBtn.ObservableButton( this ).Subscribe( _ => GameModel.GameState.Value = Define.GameState.Playing );
    }

    //***********************************************************
    // Model変更によるコールバック
    //***********************************************************
    void BindModelToUI() {
        GameModel.MustWatchContinueAd.TakeUntilDisable( this ).Subscribe( _ => UpdateUI() );
        GameModel.IsAdSkipPurchased.TakeUntilDisable( this ).Subscribe( _ => UpdateUI() );
        GameModel.IsAdError.TakeUntilDisable( this ).Subscribe( _ => UpdateUI() );
    }

    void UpdateUI() {
        bool isShowPlayAdButton = !GameModel.IsAdSkipPurchased.Value && GameModel.MustWatchContinueAd.Value;
        m_purchaseAdSkipBtn.SetActive( isShowPlayAdButton );
        m_playAdBtn.SetActive( isShowPlayAdButton );
        m_continueBtn.SetActive( !isShowPlayAdButton );
        // 広告再生ボタン画像を変更
        m_playAdImage.sprite = GameModel.IsAdError.Value ? m_errorAdImage : m_playWithAdImage;
    }

    //***********************************************************
    // シーン切替コールバック
    //***********************************************************
    public void OnSceneActive() {
    }

    public void OnSceneInactive() {
    }
}
