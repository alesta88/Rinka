using System;
using UnityEngine;
using UniRx;

/// <summary>
/// プレイヤーを管理するクラス
/// </summary>
public class PlayerMgr : MonoSingleton<PlayerMgr> {
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] bool IsInvulnerable;

    public Player PlayerInstance { private set; get; }


    public void InstantiatePlayer( Action<Player> onFinish ) {
        if( PlayerInstance == null ) {
            var playerObj = GameObject.Instantiate<GameObject>( PlayerPrefab );
            PlayerInstance = playerObj.GetComponent<Player>();
            PlayerInstance.IsInvulnerable = IsInvulnerable;
        }

        MovePlayerToSpawnPosition( onFinish );
    }

    void MovePlayerToSpawnPosition( Action<Player> onFinish ) {
        PlayerInstance.Move.Value = Player.MoveState.Init;
        var new_position = PlayerInstance.transform;
        new_position.SetPositionAndRotation(new Vector3(0,0,0), Quaternion.Euler(new Vector3(0, 0, 0)));
        CameraMgr.Instance.Follow(new_position);// PlayerInstance.transform );
        
        Observable.NextFrame().Subscribe( _ => AfterCameraInit( onFinish ) );
      //  CameraMgr.Instance.
    }

    void AfterCameraInit( Action<Player> onFinish ) {
        PlayerInstance.transform.position = Define.Origin + GameModel.SpawnStageChunk.Value.PlayerSpawnPosition;
        SceneMgr.Instance.FadeOut( onFinish: () => onFinish?.Invoke( PlayerInstance ) );
    }
}
