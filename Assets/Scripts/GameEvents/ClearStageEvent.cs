using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class ClearStageEvent : IGameEvent
{
    public ClearStageEvent()
    {
      //GameModel.PlayerLives.Value += 10;
      //    GameModel.GameState.Value = Define.GameState.GoToBonus; ;/// 
        Debug.Log("STAGECLEAREVENT!!"+ GameModel.GameState.Value);
      //GameModel.StageWhenDied.Value = StageMgr.Instance.ClearNowStage;
        GameModel.StageWhenClear.Value = StageMgr.Instance.ClearNowStage;
      //  GameModel.StageC.Value = StageMgr.Instance.ClearMetaData;
        var i = GameModel.Stage.Value.StageNumber;
    　//   GameModel.Stage.Value = StageMgr.Instance.m_stageFlow.Stages[i]; 
  　　//alestasns     SnsMgr.Instance.ReportScore();
      //  AudioMgr.Instance.PlayDeath();　
        GameModel.CumulativeDistance.Value += GameModel.CurrentLifeDistance.Value;

        // HighScore判定と保存
        var storedHighScore = PlayerPrefs.GetInt(Define.PlayerPref.HIGH_SCORE);
        if (storedHighScore < GameModel.Score.Value)
        {
            PlayerPrefs.SetInt(Define.PlayerPref.HIGH_SCORE, GameModel.Score.Value);
        }
    }
}