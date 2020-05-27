using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Analytics;

/// <summary>
/// ステージを管理するクラス
/// </summary>
public class StageMgr : MonoSingleton<StageMgr> {
    [Header("Stage Chunks")]
    [SerializeField] public StageFlowData m_stageFlow;
    [SerializeField] GameObject m_chunkViewPrefab;

    public int ConsumedOrbsInStage;
    public StageMetaData TitleStage => m_stageFlow.Stages[0];
    public StageChunkData CurrentStage => m_viewQueue.Count == 2 ? m_viewQueue.ElementAt( 0 ).Data : m_viewQueue.ElementAt( 1 ).Data;
    public StageChunkData ClearNowStage;
    public GameObject Finmrk;
    public StageMetaData ClearMetaData;

    public Queue<StageChunkView> m_viewQueue = new Queue<StageChunkView>();
    const int MAX_CHUNKS = 3;//alesta
    bool m_isSpawnPlayer => m_viewQueue.Count == 1;
    StageChunkPool m_pool;

    public int StagesCounter;//chunks in stage
    public int StagesCount;//stages in level
    public StageChunkView currentchunk_bonustext;

    public bool isStageClear;
    public bool oneclear=false;

    void Awake() {
        m_pool = new StageChunkPool( m_chunkViewPrefab );

    }

    public void InitSpawnStage( Action<Player> onFinish = null ) {
        InitSpawnStage( TitleStage, onFinish );
    }

    public void InitSpawnStage( StageMetaData stageMetaData, Action<Player> onFinish = null ) {
        SceneMgr.Instance.FadeIn( 0f );
      ///  GameModel.StageC.Value = null;/////////
        ReturnAllChunks();
        DisplayNextChunk( stageMetaData );
        DisplayNextChunk( stageMetaData );
        PlayerMgr.Instance.InstantiatePlayer( onFinish );
        ///////////////////////Rotate camera 45f
         
        Debug.Log("ROTATE" + CurrentStage.rotate_value);
       // if(CurrentStage.rotate_value)
            CameraMgr.Instance.RotateCamera(CurrentStage.rotate_value);
        
       // CameraMgr.Instance.RotateCamera(CurrentStage.rotate_value);
    }

    /// <summary>
    /// 次のステージの一部を表示
    /// </summary>
    public void DisplayNextChunk( StageMetaData stage ) {
        var currChunk = m_viewQueue.LastOrDefault();
        var nextChunk = m_pool.Rent();
        m_viewQueue.Enqueue( nextChunk );
        Debug.Log("STAGE " + GameModel.Stage.Value);
        Debug.Log("STAGEC " + GameModel.StageC.Value);
        if (m_viewQueue.Count > MAX_CHUNKS) 
        {
            currChunk.FinishMark.gameObject.SetActive(true);
            currentchunk_bonustext = currChunk;
            //currentchunk_bonustext.gameObject.SetActive(true);
            //    currChunk.BonusText.gameObject.SetActive(true);
            Debug.Log("hgsogho"+m_viewQueue.Count);
            var prevChunk = m_viewQueue.Dequeue();
            m_pool.Return(prevChunk);
        }
        else
        {
            Debug.Log("hgsogho2" + m_viewQueue.Count);
            if(currChunk!=null) 
            currChunk.FinishMark.gameObject.SetActive(false);
           //  currChunk.BonusText.gameObject.SetActive(false);
        }

        StageChunkData nextChunkData = GetNextStageChunk();
        Vector2? playerSpawnPosition = m_isSpawnPlayer ? nextChunkData.PlayerSpawnPosition : default( Vector2? );
        nextChunk.Display1( currChunk, nextChunkData, playerSpawnPosition );
        nextChunk.OnHitBoundary = ( col ) => {
            if( col.tag == Define.Tag.PLAYER && m_viewQueue.Count >= 2 ) { 
                DisplayNextChunk( stage );
                nextChunk.OnHitBoundary = null;
            }
        };
    }
     
    /// <summary>
    /// 次のステージ部分を取得
    /// </summary>
    StageChunkData GetNextStageChunk() {
        UpdateCurrentStage();

        StageChunkData nextStageChunk = null;
        
        if (isStageClear == false)
        {// 死んだ場合、同じステージ部分からスタート
            if (GameModel.StageWhenDied.Value != null)
            {
                nextStageChunk = GameModel.StageWhenDied.Value;
                Debug.Log("DIEDSTAGE");
                GameModel.SpawnStageChunk.Value = nextStageChunk;
                GameModel.StageWhenDied.Value = null;

                StagesCounter = 0;

                // 死んでいなくて最初プレイの場合、、最初のステージ部分スタート
            }
            else if (GameModel.StageWhenClear.Value != null)
            {
                //   nextStageChunk = GameModel.StageWhenClear.Value;
                //var i = CurrentStage.stageNumber;
                // nextStageChunk = ClearMetaData.Chunks[GameModel.Stage.Value.Difficulty - 1];
                Debug.Log("ONECLEAR" + oneclear);
               if(!oneclear)
                {
                    nextStageChunk = ClearMetaData.Chunks[0];
                    oneclear = true;
                }
               else
                {
                    nextStageChunk = ClearMetaData.Chunks[1];
                   // oneclear = false;
                }
                
                GameModel.SpawnStageChunk.Value = nextStageChunk;
                //  GameModel.StageWhenClear.Value = null; 

                StagesCounter = 0;

                // 死んでいなくて最初プレイの場合、、最初のステージ部分スタート
            }
            else if (m_isSpawnPlayer)
            {
                nextStageChunk = GameModel.Stage.Value.Chunks[0];//////////////////////////////////////////////////////////////////
                GameModel.SpawnStageChunk.Value = nextStageChunk;//////////////////////////////////////////////////////////////////
            Debug.Log("MYNEWSTAGE");                                       //   nextStageChunk = GameModel.Stage.Value.Chunks.Random();
                StagesCounter = 0;
                // それ以外は、ランダムステージ部分へ遷移
            }
            else
            {
               // var i = CurrentStage.stageNumber;
                nextStageChunk = GameModel.Stage.Value.Chunks[0];
                GameModel.SpawnStageChunk.Value = nextStageChunk;
                //  nextStageChunk = GameModel.Stage.Value.Chunks.Random();
            }
        }
        else
        {
            if (GameModel.StageWhenDied.Value != null)
            {
                nextStageChunk = GameModel.StageWhenDied.Value;
                GameModel.SpawnStageChunk.Value = nextStageChunk;
                GameModel.StageWhenDied.Value = null;

                StagesCounter = 0;

                // 死んでいなくて最初プレイの場合、、最初のステージ部分スタート
            }
            else

            {
                //  var i = CurrentStage.stageNumber;
                var i = StageMgr.Instance.StagesCount;
                nextStageChunk = GameModel.Stage.Value.Chunks[i];
                //  nextStageChunk = GameModel.Stage.Value.Chunks.Random();

                //   nextStageChunk = GameModel.Stage.Value.Chunks[0];//////////////////////////////////////////////////////////////////
                GameModel.SpawnStageChunk.Value = nextStageChunk;//////////////////////////////////////////////////////////////////
                Debug.Log("MYNEWSTAGE2");                                       //   nextStageChunk = GameModel.Stage.Value.Chunks.Random();
                StagesCounter = 0;
                isStageClear = false;
            }
           
        }

        return nextStageChunk;
    }

    void UpdateCurrentStage() {
        // 最後のステージが生成後の次のUpdate時の設定する
        GameModel.IsLastStage.Value = ( GameModel.Stage.Value.Difficulty == 5 );
        StagesCounter++;
        Debug.Log("stages counter " + StagesCounter);
        // 決まった玉の数以上を消費した場合、次のステージへ遷移する 
        for (int i = 0; i < m_stageFlow.Stages.Length; i++)
        {
            var stage = m_stageFlow.Stages[i];///////////////////// var stage = m_stageFlow.Stages[i];
            Debug.Log("MyStages " + stage);
            Debug.Log(ClearMetaData.Chunks[1]);
        //if (/*stage == GameModel.Stage.Value &&*/ ConsumedOrbsInStage >= stage.OrbClearCount)
        //    {
        //        //int nextStageIndex = Mathf.Min(i + 1, m_stageFlow.Stages.Length - 1);
        //        //GameModel.Stage.Value = m_stageFlow.Stages[nextStageIndex+1];
        //        ConsumedOrbsInStage = 0;
        //        Debug.Log("Clear!" + ConsumedOrbsInStage + "b" + stage.OrbClearCount);
        //        MessageBroker.Default.Publish(new ClearStageEvent());
        //          return;
         

        //        //GameModel.GameState.Value = Define.GameState.Paused;
        //        //Analytics.CustomEvent(Define.AnalyticsEvent.STAGE_SELECTION, new Dictionary<string, object>()
        //        //{
        //        //    ["time"] = 0f
        //        //});
        //    }
        }
    }

    public void ReturnAllChunks() {
        foreach( var chunk in m_viewQueue ){
            m_pool.Return( chunk );
        }
        m_viewQueue.Clear();
    }
}
