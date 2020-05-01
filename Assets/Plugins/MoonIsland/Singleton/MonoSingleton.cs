using UnityEngine;

/// <summary>
/// MonoBehaviourを利用するSingletonクラス
/// Scene切替でも存在し続ける
/// </summary>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
    static public bool IsInstantiated => m_instance != null;

    static bool m_isBeingDestroyed = false;
    static T m_instance;
    static public T Instance {
        get {
            if( m_isBeingDestroyed ) {
                return null;
            }
            if( m_instance == null ) {
                Create();
            }
            return m_instance;
        }
    }

    static public void Create() {
        if( m_instance == null ) {
            m_instance = GameObject.FindObjectOfType<T>();
            if( m_instance == null ) {
                const string SINGLETON_GO = "Singletons";
                var mgrInstance = GameObject.Find( SINGLETON_GO );
                if( mgrInstance == null ) {
                    mgrInstance = new GameObject( SINGLETON_GO );
                    DontDestroyOnLoad( mgrInstance );
                }
                m_instance = mgrInstance.AddComponent<T>();
            }
        }
    }

    virtual protected void OnDestroy() {
        m_isBeingDestroyed = true;
    }
}
