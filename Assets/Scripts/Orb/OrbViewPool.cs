using UnityEngine;
using UniRx.Toolkit;

/// <summary>
/// ステージの表示プール
/// </summary>
public class OrbViewPool : ObjectPool<OrbView> {
    readonly GameObject m_orbViewPrefab;

    public OrbViewPool( GameObject prefab ) {
        m_orbViewPrefab = prefab;
    }

    protected override OrbView CreateInstance() {
        return GameObject.Instantiate<GameObject>( m_orbViewPrefab ).GetComponent<OrbView>();
    }
}
