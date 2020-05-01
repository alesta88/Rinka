using UnityEngine;

public class ConsumeOrbEvent : IGameEvent {
    public readonly OrbView Orb;

    public ConsumeOrbEvent( OrbView orb ) {
        Orb = orb;
        GameModel.Score.Value += Orb.Point;
        AudioMgr.Instance.PlayGetOrb();
    }
}
