using UnityEngine;

[CreateAssetMenu(fileName = "New Orb Type", menuName = "Create Orb Type")]
public class OrbData : ScriptableObject {
    public Define.OrbType Type;
    public Sprite WispSprite;
    public Sprite WispIlluminationSprite;
    public Sprite OrbSprite;
}
