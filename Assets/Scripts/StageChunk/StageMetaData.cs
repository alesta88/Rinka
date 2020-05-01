using UnityEngine;
using Malee;

[CreateAssetMenu( fileName = "New Stage Meta Data", menuName = "Create Stage Meta Data" )]
public class StageMetaData : ScriptableObject {
    [Header( "Stage Selection" )]
    public Sprite StageSelectionImage;
    public Sprite DifficultyIndicatorImage;
    public bool IsShowAd;
    [Range(1, 10)] public int Difficulty = 1;
    [Header(" Stage Level Data")]
    public int PointsPerOrb;
    public int OrbClearCount;
    public int StageNumber;
    [Reorderable] public StageChunkDataList Chunks;
}
