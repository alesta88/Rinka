using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Analytics;

/// <summary>
/// ステージ選択画面のステージ
/// </summary>
public class StageButton : MonoBehaviour {
    /// <summary>広告ボタン状態</summary>
    enum ButtonAdState {
        PLAY,   // 再生
        NONE,   // 無
        ERROR   // 失敗
    }
    [SerializeField] Image m_selectionInput;
    [SerializeField] Image m_stageImage;
    [SerializeField] Image m_playWithAdImage;
    [SerializeField] Image m_playWithoutAdImage;
    [SerializeField] Image m_errorAdImage;
    [SerializeField] HorizontalLayoutGroup m_difficultLayoutGroup;
    ButtonAdState m_adState;
    bool m_doesStageShowAd;

    void OnEnable() {
        GameModel.IsAdSkipPurchased.TakeUntilDisable( this ).Subscribe( _ => UpdateUI() );
        GameModel.IsAdError.TakeUntilDisable( this ).Subscribe( _ => UpdateUI() );
    }

    void UpdateUI() {
        // 広告状態を設定
        // 広告非表示の場合、無状態
        //if ( GameModel.IsAdSkipPurchased.Value || !m_doesStageShowAd )  { m_adState = ButtonAdState.NONE; }
        //// 広告失敗中の場合、失敗状態
        //else if ( GameModel.IsAdError.Value )                           { m_adState = ButtonAdState.ERROR; }
        //// それ以外の場合、再生中状態
        //else                                                           
        { m_adState = ButtonAdState.PLAY; }

        // 画像状態を設定
        m_playWithAdImage.enabled = ( m_adState == ButtonAdState.PLAY );
        m_playWithoutAdImage.enabled = ( m_adState == ButtonAdState.NONE );
        m_errorAdImage.enabled = ( m_adState == ButtonAdState.ERROR );
    }

    public void Init( StageMetaData metaData ) {
        m_doesStageShowAd = metaData.IsShowAd;

        // ステージバナー画像
        m_stageImage.sprite = metaData.StageSelectionImage;

        // ステージ難度の画像
        int i = 0;
        foreach(var img in m_difficultLayoutGroup.GetComponentsInChildren<Image>()) {
            img.sprite = metaData.DifficultyIndicatorImage;
            img.gameObject.SetActive( i++ < metaData.Difficulty );
        }

        // 入力設定
        m_selectionInput.gameObject.AddComponent<ObservablePointerClickTrigger>()
            .OnPointerClickAsObservable()
            .TakeUntilDisable( this )
            .Subscribe( _ => {
                GameModel.Stage.Value = metaData;
                GameModel.SpawnStageChunk.Value = metaData.Chunks[0];
                GameModel.GameState.Value = m_adState == ButtonAdState.NONE ?
                    Define.GameState.Playing :  Define.GameState.PlayStageSelectionAd;
                Analytics.CustomEvent( Define.AnalyticsEvent.STAGE_SELECTION, new Dictionary<string, object>() {
                    ["stage"] = metaData.Difficulty
                } );
            });

        UpdateUI();
    }
}
