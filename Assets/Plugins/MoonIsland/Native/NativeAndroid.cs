using System;
using UnityEngine;
using UniRx;

public class NativeAndroid {
    static AndroidJavaObject m_activity;
    static bool m_isInit;

    static void InitActivity() {
        if( !m_isInit ) {
            AndroidJavaClass unity = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
            m_activity = unity.GetStatic<AndroidJavaObject>( "currentActivity" );
            m_isInit = true;
        }
    }

    public static void DisplayMessage( string title, string message, string okString, Action onTapButton = null ) {
        InitActivity();

        m_activity.Call( "runOnUiThread", new AndroidJavaRunnable( () => {
            AndroidJavaObject alertDialogBuilder = new AndroidJavaObject( "android.app.AlertDialog$Builder", m_activity );
            alertDialogBuilder.Call<AndroidJavaObject>( "setTitle", title );
            alertDialogBuilder.Call<AndroidJavaObject>( "setMessage", message );
            alertDialogBuilder.Call<AndroidJavaObject>( "setCancelable", false );
            alertDialogBuilder.Call<AndroidJavaObject>( "setPositiveButton", okString, onTapButton );
            AndroidJavaObject dialog = alertDialogBuilder.Call<AndroidJavaObject>( "create" );
            dialog.Call( "show" );
        } ) );
    }

    class ButtonListener : AndroidJavaProxy {
        MainThreadCallbackBehaviour m_mainThreadCbBehaviour;

        public ButtonListener( Action callback ) : base( "android.content.DialogInterface$OnClickListener" ) {
            var obj = new GameObject( "CallbackGameObject" );
            m_mainThreadCbBehaviour = obj.AddComponent<MainThreadCallbackBehaviour>();
            m_mainThreadCbBehaviour.Init( callback );
        }

        public void onClick( AndroidJavaObject obj, int value ) {
            m_mainThreadCbBehaviour?.Invoke();
        }
    }
}



