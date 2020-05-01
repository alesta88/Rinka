using UnityEngine;
using System.Linq;
using UniRx;

/// <summary>
/// Orb管理クラス
/// </summary>
public class OrbMgr : MonoSingleton<OrbMgr> {
    public OrbData DefaultOrb;
    [SerializeField] GameObject m_orbPrefab;

    OrbViewPool m_orbPool;


    void Awake() {
        m_orbPool = new OrbViewPool( m_orbPrefab );
    }

    /// <summary>
    /// ステージにOrbを生成して配置する
    /// </summary>
    public void Add( StageChunkView stageView, OrbData orbData, Vector3 orbPos ) {
        // Orb初期化
        var orb = m_orbPool.Rent();
        orb.transform.position = orbPos;
        orb.Init( orbData, GameModel.Stage.Value.PointsPerOrb );
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene( orb.gameObject, SceneMgr.Instance.Persistent );
        
        // Orb消費処理
        MessageBroker.Default.Receive<ConsumeOrbEvent>()
            .TakeUntilDisable( orb.gameObject )
            .Where( orbEvent => orbEvent.Orb == orb )
            .Subscribe( _ => OnConsumed( orb ) );

        // Orb削除処理
        Observable.FromEvent( addHandler: h => stageView.OnReturn += h, removeHandler: h => stageView.OnReturn -= h )
            .TakeUntilDisable( orb.gameObject )
            .Take( 1 )
            .Subscribe( _ => OnDiscarded( orb ) );
    }

    void OnConsumed( OrbView orb ) {
        m_orbPool.Return( orb );
    }

    void OnDiscarded( OrbView orb ) {
        m_orbPool.Return( orb );
    }
}
