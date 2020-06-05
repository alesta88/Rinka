using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using MoonIsland;


/// <summary>
/// ステージの一部の表示
/// </summary>
public class StageChunkView : MonoBehaviour {
    [Header("Stage Views")]
    [SerializeField] SpriteRenderer m_orbSpawnSprite;
   // [SerializeField] MeshCollider m_orbSpawnSprite2;
    [SerializeField] SpriteRenderer m_background;
    [Header("Orb Spawning")]
    [SerializeField] float m_minWallDistance;
    [SerializeField] int m_orbSpawnTryCnt;
    [Space( 20 )]
    [SerializeField] OrbData[] m_orbData;

    /// <summary>舞台横方向の大きさ比率</summary>
    // ※ 149...は、計算後6.832になればちらつかない為、逆算。
    //      この精度を可能な限り高くすると、継ぎ目が現れ難くなる。
    const float STAGE_SIZE_X_RATE = 149.882903981f;

    public GameObject Boundary;
    GameObject m_wall;
    public StageChunkData Data { private set; get; }
    public Action<Collider2D> OnHitBoundary;
    public GameObject FinishMark;
    public GameObject BonusText;
    public GameObject CountdownBonusText;
    public event Action OnReturn = () => { };

    List<Vector3> orbPositions;
    PolygonCollider2D m_col;
    Collider2D[] m_colOverlaps = new Collider2D[5];
    int m_wallLayer;
    int m_wallLayer2;
    int m_spawnOrbLayer;

    float rot_y;

    void Awake() {
        m_wallLayer = 1 << LayerMask.NameToLayer( "StageWall" );
        m_wallLayer2 = 1 << LayerMask.NameToLayer( "GenerateWall" );
        m_spawnOrbLayer = 1 << LayerMask.NameToLayer( "OrbSpawn" );
    }

    /// <summary>
    /// ステージの一部を表示
    /// </summary>
    public void Display1( StageChunkView prevChunkView, StageChunkData chunkData, Vector2? playerSpawnPos ) {
        AssignData( chunkData );
        SetPosition( prevChunkView );
        CreateWall( chunkData );

        if( GameModel.GameState.Value == Define.GameState.Playing ) {
            CreateOrbs( chunkData, playerSpawnPos );
        }
        ///////////////
       // rot_y = Data.rotate_value;

    }

    void AssignData( StageChunkData chunkData ) {
        Data = chunkData;
        m_orbSpawnSprite.sprite = chunkData.ItemSpawnAreaSprite;
      //  m_orbSpawnSprite2.sharedMesh = chunkData.SpawnMesh;
        m_background.sprite = chunkData.BackgroundData.Sprite;
        m_background.gameObject.GetComponent<SpriteRenderer>().color = chunkData.stageColor;
    }

    void SetPosition( StageChunkView prevChunkView ) {
        // 位置の調整
        Vector2 pos = Define.Origin;
        if( prevChunkView != null ) {
            // ２番目以降のステージは１番のが終わったらところから配置
            var x = prevChunkView.transform.position.x + prevChunkView.Data.LowerExitPosition.x / STAGE_SIZE_X_RATE;
            pos += new Vector2( x, 0f );
        }
        transform.position = pos;
    }

    void CreateWall( StageChunkData chunkData ) {

        var i = GameModel.Stage.Value.StageNumber;
        if ( m_wall == null || m_wall.name != chunkData.StageGenerate.WallObject.name ) {
#if !RELEASE || ENABLE_LOG
            DebugMainInfo.Instance.StartMeasure();
#endif
            if ( m_wall != null ) {
                Destroy( m_wall );
            }
            m_wall = Instantiate( chunkData.StageGenerate.WallObject, transform );
            m_wall.name = chunkData.StageGenerate.WallObject.name;
#if !RELEASE || ENABLE_LOG
            var second = DebugMainInfo.Instance.StopMeasure();
           // Debug.LogError( $"壁生成秒 : {second}" );
            Debug.Log( $"壁生成秒 : {second}" );
#endif
        }
    }

    void CreateOrbs( StageChunkData data, Vector2? playerSpawnPos ) {
        // Colliderの再初期化
        Destroy( m_orbSpawnSprite.GetComponent<PolygonCollider2D>() );
        StartCoroutine( CreateOrbSequence( data, playerSpawnPos ) );
    }

    bool IsPointTouchingCollider( Vector3 point, int layer ) {
        int colCnt = Physics2D.OverlapCircleNonAlloc( point, m_minWallDistance, m_colOverlaps, layer );
        return colCnt >= 1;
    }

    IEnumerator CreateOrbSequence( StageChunkData data, Vector2? playerSpawnPos ) {
        // 配置エリアの初期化
        m_col = m_orbSpawnSprite.gameObject.AddComponent<PolygonCollider2D>();
        m_col.isTrigger = true;

        // 配置エリアのサイズ
        const float MYSTERY_NUMBER = 0.027f;
        float spawnAreaWidth = data.ItemSpawnAreaSprite.texture.width * MYSTERY_NUMBER;
        float height = data.ItemSpawnAreaSprite.texture.height * MYSTERY_NUMBER;

        orbPositions = new List<Vector3>();

        float left = ( -spawnAreaWidth / 2f ) + Data.MinOrbDistance;
        float right = ( -spawnAreaWidth / 2f ) + Data.MaxOrbDistance;

        // プレイヤー配置の直前に１つ玉を配置
        if( playerSpawnPos.HasValue ) {
            var playerPos = playerSpawnPos.Value;
            yield return StartCoroutine(TryPlaceOrb(playerPos.x + 0.06f, playerPos.x + 0.061f, playerPos.y - 0.06f, playerPos.y - 0.061f, isAsync: false));
            if( orbPositions.Count != 0 ) {
                // オーブの生成に成功していたら、次の生成位置をずらす.
                left = right + Data.MinOrbDistance;
                right = right + Data.MaxOrbDistance;
            }
        }

        int spawnOrbCnt = Data.MaxOrbCount;
        int orbCnt = orbPositions.Count;
        // Orb配置ループ
        while( orbCnt < spawnOrbCnt ) {
            // プレイヤーより左および付近のオーブは表示させない.
            if( playerSpawnPos.HasValue && (playerSpawnPos.Value.x + 0.2f) > ( left + 0.1f ) ) {
                spawnOrbCnt--;
            } else {
                // １フレームに１個オーブを生成.
                yield return StartCoroutine( TryPlaceOrb( left + 0.1f, right, -height / 2f + 0.1f, height / 2f - 0.1f, isAsync: false ) );
                yield return null;
            }
            left = right + Data.MinOrbDistance;
            right = right + Data.MaxOrbDistance;
            orbCnt++;
        }
    }

    IEnumerator TryPlaceOrb( float xMin, float xMax, float yMin, float yMax, bool isAsync ) {
        int i = 0;
        bool isSuccess = false;
        while( !isSuccess && i++ < m_orbSpawnTryCnt ) {
            float randX = UnityEngine.Random.Range( xMin, xMax );
            float randY = UnityEngine.Random.Range( yMin, yMax );
            Vector3 randPosition = transform.position + new Vector3( randX, randY, 0f );

            if( IsPointTouchingCollider( randPosition, m_spawnOrbLayer ) && !IsPointTouchingCollider( randPosition, m_wallLayer ) && !IsPointTouchingCollider(randPosition, m_wallLayer2)) {
                // Orb配置
                OrbMgr.Instance.Add( this, GetOrbData(), randPosition );
                orbPositions.Add( randPosition );
                // 成功
                isSuccess = true;
            }
            if( isAsync ) {
                yield return null;
            }
        }
    }

    /// <summary>
    /// Orb種類のデータを取得
    /// </summary>
    OrbData GetOrbData() { 
        int difficulty = GameModel.Stage.Value.Difficulty;
        var orbTypeList = new List<Define.OrbType>();
        
        if( difficulty >= 1 && difficulty <= 2 ) {
            orbTypeList.Add( Define.OrbType.Blue );
        }
        if( difficulty >= 2 && difficulty <= 3 ) {
            orbTypeList.Add( Define.OrbType.Green );
        }
        if( difficulty >= 3 && difficulty <= 4 ) {
            orbTypeList.Add( Define.OrbType.Orange );
        }
        if( difficulty >= 4 && difficulty <= 5) {
            orbTypeList.Add( Define.OrbType.Red );
        }
        if (difficulty >= 9)
        {
            orbTypeList.Add(Define.OrbType.Blue);
            orbTypeList.Add(Define.OrbType.Red);
            orbTypeList.Add(Define.OrbType.Orange);
            orbTypeList.Add(Define.OrbType.Green);
        }
        // var size = orbTypeList.Count;
        Define.OrbType randColor = orbTypeList.Random();

        return m_orbData.Where( orb => orb.Type == randColor ).First();
    }

    public void OnBeforeReturn() {
        OnReturn();
    }
}
