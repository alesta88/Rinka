using UnityEngine;

[CreateAssetMenu(fileName = "New Stage Flow", menuName = "Create Stage Flow")]
public class StageFlowData : ScriptableObject {
    public StageChunkData FirstStageChunk;
    public StageMetaData[] Stages;
}
