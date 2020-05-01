using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MoonIsland;

public static class BootLoader {

    static readonly string[] m_debugScenes = new string[] {
        "TestAdvertisement"
    };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init() {
        DG.Tweening.DOTween.Init( recycleAllByDefault: false, useSafeMode: false, logBehaviour: DG.Tweening.LogBehaviour.ErrorsOnly );
#if !RELEASE || ENABLE_LOG
        var lunarConsole = Resources.Load<GameObject>( "LunarConsole" );
        GameObject.Instantiate( lunarConsole );
        var debug = DebugMainInfo.Instance;
#endif
        // ビルド場面のみ、初期化
        if ( !IsDebugScene() ) {
            GameMgr.Instance.InitValues();
        }
    }

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    public static void OnSceneLoaded() {
        // ビルド場面のみ、初期化
        if ( !IsDebugScene() ) {
            GameMgr.Instance.InitScene();
        }
    }

    /// <summary>
    /// ● 現在場面が、ビルド場面に含まれているか？
    /// </summary>
    public static bool IsDebugScene() {
        return m_debugScenes.Any( scene => scene == SceneManager.GetActiveScene().name );
    }
}
