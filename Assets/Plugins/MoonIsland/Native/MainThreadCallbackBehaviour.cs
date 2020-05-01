using System;
using UnityEngine;

public class MainThreadCallbackBehaviour : MonoBehaviour {
    Action m_callback;
    bool m_isInvoke;


    public void Init( Action callback ) => m_callback = callback;

    public void Invoke() => m_isInvoke = true;

    void Update() {
        if( m_isInvoke ) {
            m_callback?.Invoke();
            m_isInvoke = false;
            Destroy( this.gameObject );
        }
    }
}