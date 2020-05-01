#if !RELEASE || ENABLE_LOG
using System;
using System.Diagnostics;
using UnityEngine;

namespace MoonIsland {
    ///====================================================================================================
    /// <summary>
    /// ■ DebugMainInfo
    ///     デバッグの肝心情報のクラス。
    /// </summary>
    ///====================================================================================================
    public class DebugMainInfo : MonoSingleton<DebugMainInfo> {
        ///------------------------------------------------------------------------------------------------
        /// ● メンバ変数
        ///------------------------------------------------------------------------------------------------
        int m_updateCount;
        int m_fps;
        float m_nextUpdateCountSecond;
        Stopwatch stopwatch;
        ///------------------------------------------------------------------------------------------------
        /// <summary>
        /// ● 初期化（早）
        /// </summary>
        ///------------------------------------------------------------------------------------------------
        void Awake() {
            stopwatch = new Stopwatch();
        }
        ///------------------------------------------------------------------------------------------------
        /// ● 処理時間を計測
        ///------------------------------------------------------------------------------------------------
        /// <summary>
        /// ● 処理計測開始
        /// </summary>
        public void StartMeasure() {
            stopwatch.Start();
        }
        /// <summary>
        /// ● 処理計測終了
        ///     計測秒数が戻り値
        /// </summary>
        public float StopMeasure() {
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds / 1000f;
        }
        ///------------------------------------------------------------------------------------------------
        /// <summary>
        /// ● 更新
        /// </summary>
        ///------------------------------------------------------------------------------------------------
        void Update() {
            UpdateFps();


//            DebugDisplay.Instance.Add( Color.blue );

            DebugDisplay.Instance.Add( $"FPS : {m_fps}" );
            DebugDisplay.Instance.Add( $"GC : {GC.CollectionCount( 0 )}" );
//            DebugDisplay.Instance.Add( $"Memory System : {SystemInfo.systemMemorySize} MB" );
//            DebugDisplay.Instance.Add( $"Memory Graphics : {SystemInfo.graphicsMemorySize} MB" );

//            DebugDisplay.Instance.Add( Color.white );
        }
        ///------------------------------------------------------------------------------------------------
        /// <summary>
        /// ● FPS更新
        /// </summary>
        ///------------------------------------------------------------------------------------------------
        void UpdateFps() {
            m_updateCount++;

            if ( m_nextUpdateCountSecond <= Time.unscaledTime ) {
                m_nextUpdateCountSecond = Time.unscaledTime + 1;
                m_fps = m_updateCount;
                m_updateCount = 0;
            }
        }
    }
}
#endif