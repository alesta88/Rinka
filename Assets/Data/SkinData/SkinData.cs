using UnityEngine;

[CreateAssetMenu(fileName = "New Skin Type", menuName = "Create Skin Type")]
public class SkinData : ScriptableObject
{
    public Define.SkinType Type;
    public Sprite SkinSprite;
}
