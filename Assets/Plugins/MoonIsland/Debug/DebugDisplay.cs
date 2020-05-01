using System.Collections;
using UnityEngine;

namespace MoonIsland {
    ///====================================================================================================
    /// <summary>
    /// ■ 画面表示のデバッグクラス。
    ///     アプリ画面にデバック文字を表示する。
    /// </summary>
    ///====================================================================================================
    public class DebugDisplay : MonoSingleton<DebugDisplay> {
        ///------------------------------------------------------------------------------------------------
        /// ● メンバ変数
        ///------------------------------------------------------------------------------------------------
        /// <summary>描画するか？</summary>
        public bool isDraw;
        /// <summary>背景描画するか？</summary>
        public bool isDrawBackground;
        /// <summary>文字の大きさ</summary>
        public int fontSize = 13;
        /// <summary>縦開始位置</summary>
        public int y = 13;
        /// <summary>描画スタイル</summary>
        GUIStyle m_guiStyle = new GUIStyle();
        /// <summary>登録用、文章配列</summary>
        ArrayList m_texts = new ArrayList();
        /// <summary>描画用、文章配列</summary>
        ArrayList m_drawTexts = new ArrayList();
        ///------------------------------------------------------------------------------------------------
        /// <summary>
        /// ● 初期化
        /// </summary>
        ///------------------------------------------------------------------------------------------------
        void Awake() {
#if !RELEASE || ENABLE_LOG
            isDraw = true;

            // 描画スタイルを設定
            m_guiStyle.normal.background = Texture2D.whiteTexture;
            m_guiStyle.normal.textColor = Color.white;
#endif
        }
        ///------------------------------------------------------------------------------------------------
        /// <summary>
        /// ● 追加（文章）
        /// </summary>
        ///------------------------------------------------------------------------------------------------
        public void Add( string text ) {
#if !RELEASE || ENABLE_LOG
            m_texts.Add( text );
#endif
        }
        ///------------------------------------------------------------------------------------------------
        /// <summary>
        /// ● 追加（色）
        /// </summary>
        ///------------------------------------------------------------------------------------------------
        public void Add( Color color ) {
#if !RELEASE || ENABLE_LOG
            m_texts.Add( color );
#endif
        }
        ///------------------------------------------------------------------------------------------------
        /// <summary>
        /// ● 追加（改行）
        /// </summary>
        ///------------------------------------------------------------------------------------------------
        public void AddLine() {
#if !RELEASE || ENABLE_LOG
            m_texts.Add( "\n" );
#endif
        }
        ///------------------------------------------------------------------------------------------------
        /// <summary>
        /// ● 更新
        /// </summary>
        ///------------------------------------------------------------------------------------------------
        void Update() {
#if !RELEASE || ENABLE_LOG
/*
InputManager内のnGUIを入れていない為、コメント
            // スワイプした場合、描画切り替え
            if ( InputManager.instance.isSwipeUp ) {
                isDraw = !isDraw;
            }
*/
#endif
        }
        ///------------------------------------------------------------------------------------------------
        /// <summary>
        /// ● 更新（遅）
        /// 毎回、文章登録を初期化する為、全プログラムの一番最後に呼ぶべき。
        /// </summary>
        ///------------------------------------------------------------------------------------------------
        void LateUpdate() {
#if !RELEASE || ENABLE_LOG
            // 毎フレーム登録を更新する
            m_drawTexts = new ArrayList( m_texts );
            m_texts.Clear();
#endif
        }
        ///------------------------------------------------------------------------------------------------
        /// <summary>
        /// ● アプリ描画
        /// </summary>
        ///------------------------------------------------------------------------------------------------
#if !RELEASE || ENABLE_LOG
        void OnGUI() {
            // 描画する場合
            if ( isDraw ) {
                // 背景描画開始
                GUI.backgroundColor = new Color( 0, 0, 0, (isDrawBackground ? 0.3f : 0) );
                var size = new Vector2( Screen.width, Screen.height );
                fontSize = (int)Mathf.Round( size.y / 30 );

                m_guiStyle.fontSize = fontSize;
                var marginSize = fontSize + fontSize / 8;
                var rect = new Rect( 10, y, size.x - 20, marginSize );

                // 描画文章配列を走査し、画面に描画
                foreach ( var text in m_drawTexts ) {
                    // 色の場合
                    if ( text is Color ) {
                        m_guiStyle.normal.textColor = (Color)text;

                        // 文章の場合
                    } else if ( text is string ) {
                        GUI.Label( rect, (string)text, m_guiStyle );
                        rect.y += marginSize;
                    }
                }
                // 背景描画終了
                GUI.backgroundColor = new Color( 0, 0, 0, 0 );
            }
        }
#endif
    }
}