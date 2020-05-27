using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public static class UniRxExtensions {
    /// <summary>
    /// Observableボタンを作成
    /// </summary>
   static public IObservable<PointerEventData> ObservableButton( this GameObject btnObject, Component controller ) {
   //static public UniRx.IObservable<PointerEventData> ObservableButton( this GameObject btnObject, Component controller ) {
        var btnInputArea = btnObject.GetComponent<ButtonInputArea>();
        GameObject btnObj = btnInputArea != null ? btnInputArea.Area.gameObject : btnObject.gameObject;

        return btnObj
            .AddComponent<ObservablePointerClickTrigger>()
            .OnPointerClickAsObservable()
            .TakeUntilDisable( controller );
    }

    /// <summary>
    /// Observableボタンを作成
    /// </summary>//alesta unirx
    static public IObservable<PointerEventData> ObservableButton( this Image btnImage, Component controller ) {
    //static public UniRx.IObservable<PointerEventData> ObservableButton( this Image btnImage, Component controller ) {
        return ObservableButton( btnImage.gameObject, controller );
    }
}
