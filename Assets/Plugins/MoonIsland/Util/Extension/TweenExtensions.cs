using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static class TweenExtensions {
    public static Tweener DOFadeAtSpeed( this Graphic target, float endValue, float speedSec ) {
        float duration = Mathf.Abs( target.color.a - endValue ) * speedSec;
        return target.DOFade( endValue, duration );
    }

    public static Tweener DOFadeAtSpeed( this SpriteRenderer target, float endValue, float speedSec ) {
        float duration = Mathf.Abs( target.color.a - endValue ) * speedSec;
        return target.DOFade( endValue, duration );
    }

    public static Tweener DOScaleAtSpeed( this Transform target, float endValue, float speedSec ) {
        float duration = Mathf.Abs( target.localScale.x - endValue ) * speedSec;
        return target.DOScale( endValue, duration );
    }
}
