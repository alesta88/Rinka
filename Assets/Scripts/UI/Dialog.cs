using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class Dialog : MonoBehaviour {
    [SerializeField] Text m_titleText;
    [SerializeField] Text m_messageText;
    [SerializeField] Text m_confirmButtonText;
    [SerializeField] CanvasGroup m_dialogImageGroup;
    [SerializeField] GameObject[] m_closeButtonColliders;

    Action m_onTapButton;
    int m_callbackDelayMs;


    void Awake() {
        // 閉じるボタンが押された時
        foreach(var btn in m_closeButtonColliders) {
            btn.AddComponent<ObservablePointerClickTrigger>()
               .OnPointerClickAsObservable()
               .TakeUntilDestroy( this ).Subscribe( _ => {
                   m_dialogImageGroup.blocksRaycasts = false;
                   m_dialogImageGroup.alpha = 0f;
                   Observable.Timer( TimeSpan.FromMilliseconds( m_callbackDelayMs ) ).Subscribe( __ => {
                       gameObject.SetActive( false );
                       m_onTapButton?.Invoke();
                   } );
               } );
        }
    }

    public void Display( string title, string msg, Action onTapButton, int callbackDelayMs ) {
        // テクスト設定
        m_titleText.text = title;
        m_messageText.text = msg;
        m_confirmButtonText.text = Application.systemLanguage == SystemLanguage.Japanese ? "はい" : "OK";
        m_dialogImageGroup.blocksRaycasts = true;
        m_dialogImageGroup.alpha = 1f;
        // コールバック設定
        m_onTapButton = onTapButton;
        m_callbackDelayMs = callbackDelayMs;
        // 表示
        gameObject.SetActive( true );
    }
}
