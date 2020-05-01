using UnityEngine;
using UniRx;

public class OrbView : MonoBehaviour {
    [SerializeField] SpriteRenderer m_orbSprite;

    public OrbData Data { private set; get; }
    public int Point { private set; get; }

    public void Init( OrbData data, int point ) {
        Data = data;
        Point = point;
        m_orbSprite.sprite = data.OrbSprite;
    } 
}
