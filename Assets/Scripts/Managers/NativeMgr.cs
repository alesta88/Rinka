using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class NativeMgr : MoonIsland.Singleton<NativeMgr> {
#if UNITY_IOS
    public static bool IsAppInstalled( string scheme ) {
        return SA.IOSNative.System.SharedApplication.CheckUrl( scheme );
    }
#endif

#if UNITY_ANDROID
    public static string ANDROID_PLUGIN_BUNDLEID = "com.moonisland.rinka.NativeBasics";
    public static AndroidJavaClass Context => new AndroidJavaClass( ANDROID_PLUGIN_BUNDLEID );

    public static AndroidJavaObject GetApplicationContext() => Context?.GetStatic<AndroidJavaObject>( "getContext" );

    public static void DisplayLog() => Context?.CallStatic<int>( "displayLog" );

    public static void ShowSimpleAlert( string msg ) => Context?.CallStatic<int>( "showSimpleAlert", msg );

    public static void ShowAlert( string title, string msg ) => Context?.CallStatic<int>( "showSimpleAlert", title, msg );

    public static void GetInt() {
        string str = "" + Context?.CallStatic<int>( "getInt" );
        Debug.Log( "native getInt " + str );
    }

    public static bool IsAppInstalled( string appName ) {
        string packageName = null;
        if(string.Equals( appName, Define.SocialPlatform.FACEBOOK ) ) {
            packageName = "com.facebook.katana";
        } else if(string.Equals( appName, Define.SocialPlatform.TWITTER )) {
            packageName = "com.twitter.android";
        } else if(string.Equals( appName, Define.SocialPlatform.LINE )) {
            packageName = "jp.naver.line.android";
        } else {
            packageName = appName;
        }

        bool isInstalled = false;
        AndroidJavaClass context = new AndroidJavaClass( "com.moonisland.rinka.NativeBasics" );
        if(context != null && packageName != null) {
            isInstalled = context.CallStatic<bool>( "isPackageExist", packageName );
            Debug.Log( "appChecker returned " + isInstalled );
        }

        return isInstalled;
    }
#endif

    public void DisplayMsg( string title, string msg, Action onTapButton = null ) {
#if UNITY_IOS
        var popup = IOSMessage.Create( title, msg );
        if( onTapButton != null ) {
            popup.OnComplete += onTapButton;
        }
#else
        NativeAndroid.DisplayMessage( title, msg, DialogMgr.Text.NativeConfirmDiagText, onTapButton );
#endif
    }
}
