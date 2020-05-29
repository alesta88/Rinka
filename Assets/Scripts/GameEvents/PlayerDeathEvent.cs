using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class PlayerDeathEvent : IGameEvent {
    public PlayerDeathEvent() {
        GameModel.PlayerLives.Value -= 1;
        Debug.Log("LIVES " + GameModel.PlayerLives.Value);

        StageMgr.Instance.isStageClear = false;
        StageMgr.Instance.oneclear = false;
        StageMgr.Instance.onedead = true;
        //CameraMgr.Instance.CameraToZero();

        //alestastages  
        //if(StageMgr.Instance.CurrentStage.Difficulty>5)
        {
            GameModel.StageWhenClear.Value = null;
            GameModel.StageWhenDied.Value = GameModel.Stage.Value.Chunks[0];
        }
        //else
        //{
        //    GameModel.StageWhenDied.Value = StageMgr.Instance.CurrentStage;//StageMgr.Instance.ClearNowStage; //
        //}

     //   GameModel.StageWhenClear.Value = StageMgr.Instance.CurrentStage; 
        // GameModel.StageC.Value = StageMgr.Instance.ClearMetaData;
        //alestasns    SnsMgr.Instance.ReportScore();
        AudioMgr.Instance.PlayDeath();
        GameModel.CumulativeDistance.Value += GameModel.CurrentLifeDistance.Value;

        Analytics.CustomEvent( Define.AnalyticsEvent.STAGE_OF_DEATH, new Dictionary<string, object>() {
            ["stage"] = GameModel.StageWhenDied.Value.Difficulty
        } );

        // HighScore判定と保存
        var storedHighScore = PlayerPrefs.GetInt( Define.PlayerPref.HIGH_SCORE );
        if( storedHighScore < GameModel.Score.Value) {
            PlayerPrefs.SetInt( Define.PlayerPref.HIGH_SCORE, GameModel.Score.Value );
        }
    }
}