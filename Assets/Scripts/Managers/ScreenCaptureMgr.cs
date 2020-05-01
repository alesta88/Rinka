using System;
using UnityEngine;

/// <summary>
/// 画面キャプチャ
/// カメラに追加して使用する
/// </summary>
public class ScreenCaptureMgr : MonoSingleton<ScreenCaptureMgr> {
    Texture2D m_texture;
    Action<Texture2D> m_onCaptured;

    /// <summary>
    /// キャプチャ開始
    /// </summary>
    public void Take( Action<Texture2D> onCaptured ) {
        if( m_texture != null ) {
            DestroyImmediate( m_texture );
        }
        m_onCaptured = onCaptured;
    }

    void OnPostRender() {
        if( m_onCaptured != null ) {
            Debug.Log( "OnPostRender capture" );
            m_texture = new Texture2D( Screen.width, Screen.height, TextureFormat.ARGB32, mipChain: false );
            m_texture.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
            m_texture.Apply();

            m_onCaptured( m_texture );
            m_onCaptured = null;
        }
    }

    protected override void OnDestroy() {
        if( m_texture != null ) {
            DestroyImmediate( m_texture );
        }
        base.OnDestroy();
    }
}