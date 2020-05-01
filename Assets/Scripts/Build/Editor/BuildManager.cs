using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
//using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
using System.Globalization;
using UnityEditor.iOS.Xcode;

///====================================================================================================
/// <summary>
/// ■ 建築の管理クラス。
///     建築開始後、自動的に様々な設定を行うが、
///     developmentBuildだけ、設定後反映までに遅延が起き、自動設定されない。
///     現状、建設前に自分で押す必要がある。
/// </summary>
///====================================================================================================
public class BuildManager : IPreprocessBuildWithReport, IPostprocessBuildWithReport {
    ///------------------------------------------------------------------------------------------------
    /// ● 要素
    ///------------------------------------------------------------------------------------------------
    /// <summary>実行準</summary>
    public int callbackOrder { get { return 0; } }
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● ビルド前処理（呼戻）
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    public void OnPreprocessBuild( BuildReport report ) {
        // 番号適用
        var version = float.Parse( PlayerSettings.bundleVersion, CultureInfo.InvariantCulture);
        var versionCode = (int)Mathf.Round( version * 100 );
        PlayerSettings.bundleVersion = version.ToString();
        PlayerSettings.Android.bundleVersionCode = versionCode; 
        PlayerSettings.iOS.buildNumber = versionCode.ToString();

        // ログ出力設定
#if RELEASE && !ENABLE_LOG
        PlayerSettings.usePlayerLog = false;
        PlayerSettings.SetStackTraceLogType( LogType.Error,     StackTraceLogType.None );
        PlayerSettings.SetStackTraceLogType( LogType.Assert,    StackTraceLogType.None );
        PlayerSettings.SetStackTraceLogType( LogType.Warning,   StackTraceLogType.None );
        PlayerSettings.SetStackTraceLogType( LogType.Log,       StackTraceLogType.None );
        PlayerSettings.SetStackTraceLogType( LogType.Exception, StackTraceLogType.None );
#endif

        Debug.Log( "ビルド前処理完了" );
    }
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● ビルド後処理（呼戻）
    /// </summary>
    ///------------------------------------------------------------------------------------------------
    public void OnPostprocessBuild( BuildReport report ) {
        // ログ出力設定を復元
#if RELEASE && !ENABLE_LOG
        PlayerSettings.usePlayerLog = true;
        PlayerSettings.SetStackTraceLogType( LogType.Error,     StackTraceLogType.ScriptOnly );
        PlayerSettings.SetStackTraceLogType( LogType.Assert,    StackTraceLogType.ScriptOnly );
        PlayerSettings.SetStackTraceLogType( LogType.Warning,   StackTraceLogType.ScriptOnly );
        PlayerSettings.SetStackTraceLogType( LogType.Log,       StackTraceLogType.ScriptOnly );
        PlayerSettings.SetStackTraceLogType( LogType.Exception, StackTraceLogType.ScriptOnly );
#endif

        Debug.Log( "ビルド後処理完了" );
    }
    ///------------------------------------------------------------------------------------------------
    /// <summary>
    /// ● ビルド後処理（呼戻）
    /// </summary>
    ///------------------------------------------------------------------------------------------------
#if UNITY_IOS
    [PostProcessBuild]
    public static void OnPostProcessBuild( BuildTarget buildTarget, string path ) {
        var projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
        var proj = new PBXProject();
        proj.ReadFromString( File.ReadAllText( projPath ) );
        var target = proj.TargetGuidByName( "Unity-iPhone" );

        // IAP機能を追加
        proj.AddCapability( target, PBXCapabilityType.InAppPurchase );

        File.WriteAllText( projPath, proj.WriteToString() );
        Debug.Log( "iOSビルド後処理完了" ); 
    }
#endif
}