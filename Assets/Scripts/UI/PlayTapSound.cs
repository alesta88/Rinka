using UnityEngine;
using UnityEngine.EventSystems;

public class PlayTapSound : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {
    public void OnPointerClick( PointerEventData eventData ) {
        if( AudioMgr.IsInstantiated ) {
            AudioMgr.Instance.PlayTapButton();
        }
    }

    public void OnPointerDown( PointerEventData eventData ) {
    }

    public void OnPointerUp( PointerEventData eventData ) {
    }
}
