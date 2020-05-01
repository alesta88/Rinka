using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UX改善のために、プレイヤーがボタンをちゃんと押さなくても反応の範囲をより広くできるクラス
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ButtonInputArea : MonoBehaviour {
    public Image Area;

    /// <summary>
    /// コンポーネント追加で実行
    /// </summary>
    void Awake() {
        bool isInit = transform.childCount != 0;
        if( !isInit ) {
            // 表示画像で入力を受け付けない
            var img = GetComponent<Image>();
            img.raycastTarget = false;

            // 入力用のGameObjectとImageを生成
            var inputAreaObj = new GameObject( "InputArea", typeof( Image ), typeof( RectTransform ) );
            Area = inputAreaObj.GetComponent<Image>();
            Area.color = Color.clear;
            // ParentのRect設定と同じようにしておく
            var rect = inputAreaObj.GetComponent<RectTransform>();
            rect.anchoredPosition = this.transform.position;
            rect.anchorMin = new Vector2( 0, 0 );
            rect.anchorMax = new Vector2( 1, 1 );
            rect.pivot = new Vector2( 0.5f, 0.5f );
            rect.localScale = Vector3.one;
            rect.sizeDelta = GetComponent<RectTransform>().rect.size;
            rect.transform.SetParent( this.transform );
        }
    }
}
