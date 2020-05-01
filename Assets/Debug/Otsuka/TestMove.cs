using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TestMove : MonoBehaviour {

    void Awake() {
        var t = GetComponentInChildren<RectTransform>();
        var s = DOTween.Sequence()
            .SetLoops( -1, LoopType.Yoyo )
            .SetEase( Ease.Linear )
            .Append( t.DOLocalMoveY( -400, 2 ) );
        s.Play();

        var i = GetComponent<Image>();
        var h = 0f;
        var s2 = DOTween.Sequence()
            .SetLoops( -1, LoopType.Yoyo )
            .SetEase( Ease.Linear )
            .Append( DOTween.To(
                () => h,
                (time) => {
                    h = time;
                    i.color = Color.HSVToRGB(h, 1, 1);
                },
                1,
                2
                ) );
        s2.Play();
    }
}