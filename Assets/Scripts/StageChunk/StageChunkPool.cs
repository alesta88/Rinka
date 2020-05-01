using UnityEngine;
using UniRx.Toolkit;
using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージの表示プール
/// </summary>
public class StageChunkPool : ObjectPool<StageChunkView> {
    readonly GameObject m_stageChunkPrefab;

    public StageChunkPool( GameObject prefab ) {
        m_stageChunkPrefab = prefab;
    }

    protected override StageChunkView CreateInstance() {
        var chunkObj = GameObject.Instantiate<GameObject>( m_stageChunkPrefab );
        SceneManager.MoveGameObjectToScene( chunkObj, SceneMgr.Instance.Persistent );
        return chunkObj.GetComponent<StageChunkView>();
    }

    protected override void OnBeforeRent( StageChunkView instance ) {
        base.OnBeforeRent( instance );
        // プレイヤーが境界にぶつけると次のステージ部分を生成
        instance.Boundary
            .OnTriggerEnter2DAsObservable()
            .TakeUntilDisable( instance.gameObject )
            .Subscribe( other => {
                instance.OnHitBoundary?.Invoke( other );
            } );
    }

    protected override void OnBeforeReturn( StageChunkView instance ) {
        base.OnBeforeReturn( instance );
        instance.OnBeforeReturn();
    }
}
